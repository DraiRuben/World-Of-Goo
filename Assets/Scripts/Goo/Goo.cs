using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class Goo : MonoBehaviour
{
    //This script is the fattest one in the entire game, however everything is commented and properly arranged,
    //I shall make multiple scripts for each function when I'll have up to the 7th level and the reusable goos since this would take a lot of time with the special goos inheriting from this
    public static bool s_isThereAGooSelected;
    public static bool s_goToFinishLine = false;
    public static int s_moves = 0;

    public int m_exitCloseness = -1;
    public bool m_stayIdle = false;
    public bool m_isUsed = false;
    public bool m_isReusable = false;

    public List<Goo> m_connections;

    [HideInInspector]
    public bool m_isSelected = false;
    [HideInInspector]
    public Rigidbody2D m_rb;
    [HideInInspector]
    public bool isDying = false;
    [SerializeField]
    private protected bool m_isBuildableOn = true;
    [SerializeField]
    private protected int m_maxAllowedAnchorsAmount = 3;
    [SerializeField]
    private protected int m_minAllowedAnchorsAmount = 2;
    [SerializeField]
    private protected float m_minAttachDistance = 1f;
    [SerializeField]
    private protected float m_maxAttachDistance = 4f;
    [SerializeField]
    private protected float m_attachStrength = 13f;
    [SerializeField]
    private protected float m_gravity = 1f;
    [SerializeField]
    private protected float m_movementSpeed = 1f;
    [SerializeField]
    private protected float m_movementSpeedOnLevelEnd = 2f;
    [SerializeField]
    private protected GameObject m_connectionPrefab;
    [SerializeField]
    private protected AudioSource m_deathAudio;
    [SerializeField]
    private protected AudioSource m_buildAudio;

    public Goo ClosestToExit
    {
        get
        {
            int minDist = m_connections.Select(x => x.m_exitCloseness).Min();
            return m_connections.FirstOrDefault(x => x.m_exitCloseness == minDist);
        }
    }


    private Goo m_pathOrigin;
    private Goo m_pathTarget;

    private protected List<Goo> m_validAnchors;
    private protected List<SpringJoint2D> m_springJoints;
    private protected List<DistanceJoint2D> m_distanceJoints;
    private protected Coroutine m_behaviour = null;
    private protected float m_movementTimer = 0;
    private protected bool m_arrivedAtEnd = false;
    private protected Animator m_animator;
    private protected SpriteRenderer m_spriteRenderer;

    //Initializes all useful values
    private void Awake()
    {
        m_validAnchors = new List<Goo>();
        for (int i = 0; i < m_maxAllowedAnchorsAmount; i++) m_validAnchors.Add(null);
        for (int i = GetComponents<SpringJoint2D>().Length; i < m_maxAllowedAnchorsAmount; i++)
        {
            SpringJoint2D temp = gameObject.AddComponent<SpringJoint2D>();
            temp.enabled = false;
            temp.frequency = m_attachStrength;
        }
        for (int i = GetComponents<DistanceJoint2D>().Length; i < m_maxAllowedAnchorsAmount; i++)
        {
            DistanceJoint2D temp = gameObject.AddComponent<DistanceJoint2D>();
            temp.maxDistanceOnly = true;

            temp.enabled = false;
        }
        m_distanceJoints = GetComponents<DistanceJoint2D>().ToList();
        m_springJoints = GetComponents<SpringJoint2D>().ToList();
        m_rb = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }
    //starts behaviour
    private void Start()
    {

        if (!m_isUsed)
        {
            if (TryGetPath(1.5f))
            {
                m_behaviour ??= StartCoroutine(Behaviour());
            }
        }
    }

    //thing called when you click on a goo, virtual cuz I may make goos that have special conditions for interaction
    public virtual void TryInteract()
    {
        if (m_isUsed && !m_isReusable|| (s_isThereAGooSelected && !m_isSelected)) return;

        if (m_isSelected)
        {
            //checks if the click was on top of a link between 2 goos, if yes, put the selected goo back there, otherwise just build
            if (TryGetPath())
            {
                m_behaviour ??= StartCoroutine(Behaviour());
                return;

            }
            //Try to attach it to the structure
            else if (m_maxAllowedAnchorsAmount - m_validAnchors.Count(x => x == null) >= m_minAllowedAnchorsAmount)
            {
                DisablePreviewers();
                Use();
                EmptyAnchors();
            }
            else
            {
                //drop the goo in the air
                MoveOutOfStructure(true);
            }

        }
        else
        {
            //make it follow the mouse
            s_isThereAGooSelected = true;
            m_isSelected = true;
            m_animator.enabled = true;

            if (m_isUsed && m_isReusable)
            {
                RemovePointFromStructure(this, false);
                EmptyAnchors();
                s_moves++;


            }

            m_isUsed = false;
            StartCoroutine(Select());
            StartCoroutine(AnchorTesting());
        }
    }

    //used to make a goo move out of the structure
    public void MoveOutOfStructure(bool wasSelected = false)
    {
        m_stayIdle = true;
        m_behaviour ??= StartCoroutine(Behaviour());
        m_rb.isKinematic = false;
        m_rb.gravityScale = m_gravity;
        if (wasSelected)
        {
            m_animator.enabled = true;
            StartCoroutine(SetSelectableLate());
            DisablePreviewers();
            EmptyAnchors();
        }
        m_isSelected = false;
    }
    //Used when connections get placed
    private protected virtual void DisablePreviewers()
    {
        List<Goo> FilteredAnchors = m_validAnchors.ToList();
        FilteredAnchors.RemoveAll(x => x == null);
        for (int i = 0; i < FilteredAnchors.Count; i++)
        {
            if (transform.childCount <= 0) break;
            if (transform.GetChild(0).TryGetComponent(out Connection comp))
            {
                comp.IsInUse = false;
            }
        }
    }



    //this function does:
    //-get all the valid anchors that are assigned a value,
    //-connect each anchors to this goo,
    //-lift useful flags,
    //-could do something special when placed afterwards
    public virtual void Use()
    {

        List<Goo> filteredAnchors = m_validAnchors.ToList();
        filteredAnchors.RemoveAll(x => x == null|| x.isDying);
        for (int i = 0; i < filteredAnchors.Count; i++)
        {
            PlaceConnection(filteredAnchors, i);
        }
        s_moves++;
        m_animator.enabled = false;
        m_isUsed = true;
        m_isSelected = false;
        m_spriteRenderer.renderingLayerMask = 1;
        StartCoroutine(DoThingIfUsed());
    }
    //-Used to find points to attach the previewers and the definitive connectors when used
    public IEnumerator AnchorTesting()
    {
        while (m_isSelected)
        {
            //resets previewers that are too far out
            TryResetPreviewers(m_validAnchors.Count(x => x != null));
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, m_maxAttachDistance, LayerMask.GetMask("Goo"));

            //only keeps goos that can be built upon, are close enough, and not blocked by any obstacles
            colliders = colliders.Where(
                x => x.CompareTag("Goo")
                && x.GetComponent<Goo>().m_isUsed
                && x.GetComponent<Goo>().m_isBuildableOn
                && Vector2.Distance(x.transform.position, transform.position) >= m_minAttachDistance
                && Vector2.Distance(x.transform.position, transform.position) <= m_maxAttachDistance
                && !Physics2D.Raycast(x.transform.position, transform.position - x.transform.position, Vector2.Distance(transform.position, x.transform.position), LayerMask.GetMask("Default"))).ToArray();
            //basically just gets all the goo components of each element in the list
            Goo[] Goos = colliders.Select(x => x.GetComponent<Goo>()).ToArray();
            if (!s_goToFinishLine && Goos != null && Goos.Length >= m_minAllowedAnchorsAmount)
                for (int i = 0; i < Goos.Length; i++)
                {
                    float Distance = Vector2.Distance(Goos[i].transform.position, transform.position);
                    if (Distance > m_maxAttachDistance || Distance < m_minAttachDistance) continue;
                    int index = m_validAnchors.IndexOf(null);
                    //returns -1 if it doesn't find an element that's null => if the anchor list is maxed out/full
                    //this allows us to either add a new anchor to the list if it's not full yet, or replace the furthest anchor if the new one is closer
                    if (index != -1 && !m_validAnchors.Contains(Goos[i]))
                    {
                        m_validAnchors[index] = Goos[i];

                        SetupPreviewer(Goos[i]);

                    }
                    else if (!m_validAnchors.Contains(Goos[i]))
                    {
                        int HighestDistanceAnchorIndex = -1;
                        for (int u = 0; u < m_validAnchors.Count; u++)
                        {
                            if (Vector2.Distance(transform.position, m_validAnchors[u].transform.position) > Distance)
                                HighestDistanceAnchorIndex = u;
                        }
                        //replace an existing previewer
                        if (HighestDistanceAnchorIndex != -1)
                        {
                            UpdatePreviewer(Goos[i], HighestDistanceAnchorIndex);

                            m_validAnchors[HighestDistanceAnchorIndex] = Goos[i];

                        }
                    }

                }

            yield return null;
        }
    }
    //Used for:
    //-Getting a path to follow when placed back on the structure
    //-Getting a path to follow when close enough to the structure when on the ground
    //Notes: goo cannot be placed back on structure if there are too many goos on the connection, despite the layer counteracting this and all
    private protected bool TryGetPath(float searchRadius = 0.5f)
    {
        Collider2D[] overlapping = Physics2D.OverlapCircleAll(transform.position, searchRadius, LayerMask.GetMask("GooConnection"));

        foreach (Collider2D ovlp in overlapping)
        {
            if (ovlp.name.Contains("Bar"))
            {
                m_pathTarget = ovlp.transform.parent.parent.GetComponent<Goo>();
                m_pathOrigin = ovlp.transform.parent.GetComponent<Connection>().m_target;
                //don't want to be able to place a goo back on a balloon's string
                if (m_pathTarget.GetComponent<Goo_Balloon>() != null || m_pathOrigin.GetComponent<Goo_Balloon>() != null) continue;

                //for when placed back onto the structure
                if (m_isSelected)
                {
                    DisablePreviewers();
                    EmptyAnchors();
                    StartCoroutine(SetSelectableLate());
                }


                //swaps if the origin is further from the goo, so that the target is the furthest one
                if (m_isSelected && Vector2.Distance(transform.position, m_pathTarget.transform.position) < Vector2.Distance(transform.position, m_pathOrigin.transform.position))
                {
                    (m_pathOrigin, m_pathTarget) = (m_pathTarget, m_pathOrigin);
                }
                m_movementTimer = Vector2.Distance(transform.position, m_pathOrigin.transform.position) / Vector2.Distance(m_pathOrigin.transform.position, m_pathTarget.transform.position);


                m_isSelected = false;
                m_rb.isKinematic = true;
                m_stayIdle = false;
                return true;
            }
        }
        return false;
    }
    //mouse follow behaviour
    public IEnumerator Select()
    {
        transform.parent = null;
        m_rb.isKinematic = false;
        m_rb.velocity = Vector2.zero;
        while (m_isSelected)
        {
            m_rb.MovePosition((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));
            yield return new WaitForFixedUpdate();
        }
    }


    public IEnumerator Behaviour()
    {
        yield return null;
        if (m_rb.isKinematic)
        {
            //don't remember why I did this but I believe it's because of some issues I had with getting out of kinematic state with some velocity that was conserved
            m_rb.gravityScale = 0f;
        }
        else
        {
            //gets first child of the structure => bottom left goo of the starter structure
            m_pathTarget = PathFinder.Instance.transform.parent.GetChild(0).GetComponent<Goo>();
        }

        m_isSelected = false;
        //weird thing with kinematic is that it keeps in memory the velocity the rb had before entering kine state,
        //and gives it back when gettings out of kinematic state, and we don't want a random impulse when falling from the structure
        if (!m_stayIdle)
            m_rb.velocity = Vector2.zero;

        while (!m_isSelected)
        {
            //if it's on the structure it's kinematic
            if (m_rb.isKinematic)
            {
                //find a new target if we don't have one, don't have a valid one anymore, if the origin point was destroyed
                //or if we arrived at destination
                if (m_pathTarget == null || m_pathTarget.isDying ||m_pathTarget.m_isSelected
                    || m_pathOrigin == null || m_pathOrigin.isDying || m_pathOrigin.m_isSelected
                    || Vector2.Distance(transform.position, m_pathTarget.transform.position) < 0.1f)
                {
                    m_movementTimer = 0f;
                    if (!FindNextTarget()) break;
                }
                else if (!m_pathOrigin.isDying && !m_pathTarget.isDying)
                {
                    m_rb.MovePosition(Vector3.Lerp(m_pathOrigin.transform.position, m_pathTarget.transform.position, m_movementTimer));
                }
                //because for some fucking reason it can happen despite the null reference check a few lines above
                if (m_pathOrigin != null && m_pathTarget != null)
                    m_movementTimer += 2 * Time.fixedDeltaTime
                        / Vector2.Distance(m_pathOrigin.transform.position, m_pathTarget.transform.position)
                        * (s_goToFinishLine ? m_movementSpeedOnLevelEnd : m_movementSpeed);

            }
            //if it's not kinematic then it's on the ground
            else
            {
                if (!m_stayIdle && m_pathTarget != null)
                    m_rb.velocity = new Vector2(Mathf.Sign(m_pathTarget.transform.position.x - transform.position.x) * 3 * m_movementSpeed, m_rb.velocity.y);
                TryGetPath();
            }

            yield return new WaitForFixedUpdate();
        }
        m_rb.gravityScale = m_gravity;
        m_behaviour = null;
    }
    //I don't remember why I wanted it to return a bool, it might be useless, but it works still, so might as well not touch it
    private protected bool FindNextTarget()
    {
        //for some reason, when dropping the goo above the structure, velocity y fucks with it and the goo has some weird clipping because of it
        //yeah, this bug again, with kinematic keeping the velocity in memory, for fuck's sake
        if (!m_stayIdle)
            m_rb.velocity = Vector2.zero;

        //finds next target, either random if the structure isn't connected to the exit,
        //or the next target in the shortest path towards the exit if there is one
        m_pathOrigin = m_pathTarget;

        if (s_goToFinishLine)
        {
            //stop moving if we reached the end of the path
            if (!m_arrivedAtEnd)
                m_pathTarget = m_pathOrigin.ClosestToExit;

            if (m_pathTarget.m_exitCloseness <= 0f)
                m_arrivedAtEnd = true;

        }
        else
        {
            if (m_pathOrigin != null)
            {
                List<Goo> valid = m_pathOrigin.GetFilteredConnections();
                if (valid.Count > 0)
                    m_pathTarget = valid[Random.Range(0, valid.Count - 1)];
            }
            

            //if there's still no valid target, just reset to bottom left of the starting structure, otherwise idk, just grab the goo and put it back yourself
            if (m_pathTarget == null)
            {
                m_isSelected = false;
                m_rb.isKinematic = false;
                m_pathTarget = PathFinder.Instance.transform.parent.GetChild(0).GetComponent<Goo>();

            }
        }
        //if the target got destroyed or selected, this means the connection we're on is invalid, so we need to fall
        if (m_pathTarget == null || m_pathTarget.isDying || m_pathTarget.m_isSelected
            || m_pathOrigin == null||m_pathOrigin.isDying||m_pathOrigin.m_isSelected)
        {
            m_pathTarget = PathFinder.Instance.transform.parent.GetChild(0).GetComponent<Goo>();
            MoveOutOfStructure();

        }

        return true;
    }
    private protected void SetupPreviewer(Goo anchorPoint)
    {
        //gets a connection previewer that's usable and sets it up
        Connection availableConnection = (Connection)Pooling.Instance.pools["Previewers"].Find(x => !((Connection)x).IsInUse);
        if (availableConnection != null)
        {
            availableConnection.transform.position = transform.position;
            availableConnection.transform.parent = transform;
            availableConnection.m_target = anchorPoint;
            availableConnection.IsInUse = true;
            availableConnection.enabled = true;
        }
        else
        {
            Debug.Log($"You only have {Pooling.Instance.pools["Previewers"].Count} previewers in the pooling system and asked to use more than that you retard");
        }
    }
    private protected void TryResetPreviewers(int ValidAnchorCount)
    {
        //clears connections for goos that were in connection preview but went too far or got blocked by an obstacle
        for (int i = 0; i < m_validAnchors.Count; i++)
        {
            if (m_validAnchors[i] == null) continue;
            float Distance = Vector2.Distance(m_validAnchors[i].transform.position, transform.position);
            if (!(Distance >= m_minAttachDistance && Distance <= m_maxAttachDistance)
                || ValidAnchorCount < m_minAllowedAnchorsAmount
                || Physics2D.Raycast(m_validAnchors[i].transform.position, transform.position - m_validAnchors[i].transform.position, Vector2.Distance(transform.position, m_validAnchors[i].transform.position), LayerMask.GetMask("Default")))
            {
                //disable preview connection thingy, then remove from list, cool since we're not changing the collection's size,
                List<Connection> allChildren = transform.Cast<Transform>().Select(t => t.GetComponent<Connection>()).ToList();
                allChildren.RemoveAll(x => x == null);
                //replaces the target of the previewer to replace, instead of going through the pool system which would be longer
                Connection previewer = allChildren.Find(x => x.m_target == m_validAnchors[i] ||x.m_target.isDying);
                if (previewer != null)
                {
                    previewer.m_target = null;
                    previewer.IsInUse = false;
                    previewer.enabled = false;
                    m_validAnchors[i] = null;
                }//if the list is empty and there's still somehow a connection
                else if (m_validAnchors.Count == 0)
                {
                    foreach(var child in allChildren)
                    {
                        child.m_target = null;
                        child.IsInUse = false;
                        child.enabled = false;
                    }
                }

            }
        }
    }
    //replaces the target of one of the Previewers currently used
    private protected void UpdatePreviewer(Goo anchorPoint, int HighestDistanceAnchorIndex)
    {
        List<Connection> allChildren = transform.Cast<Transform>().Select(t => t.GetComponent<Connection>()).ToList();
        allChildren.RemoveAll(x => x == null);
        //replaces the target of the previewer to replace, instead of going through the pool system which would be longer
        Connection Previewer = allChildren.Find(x => x.m_target == m_validAnchors[HighestDistanceAnchorIndex]);
        if (Previewer != null)
        {
            Previewer.m_target = anchorPoint;
            Previewer.IsInUse = true;
            Previewer.enabled = true;
        }

    }
    //places both the visual line and attributes the connected rb to one of the unused spring joints
    private protected virtual void PlaceConnection(List<Goo> filteredAnchors, int i)
    {
        m_springJoints[i].connectedBody = filteredAnchors[i].m_rb;
        m_springJoints[i].autoConfigureDistance = false;
        m_springJoints[i].distance = Vector2.Distance(transform.position, filteredAnchors[i].transform.position);
        m_springJoints[i].enabled = true;

        if (m_connections.Count < i + 1)
        {
            m_connections.Add(filteredAnchors[i]);
            filteredAnchors[i].m_connections.Add(this);
        }
        else
        {
            m_connections[i] = filteredAnchors[i];
            filteredAnchors[i].m_connections.Add(this);
        }
        GameObject connection = Instantiate(m_connectionPrefab, transform.position, Quaternion.identity);
        connection.GetComponent<Connection>().IsInUse = true;
        connection.transform.parent = transform;
        connection.GetComponent<Connection>().m_target = filteredAnchors[i];
        m_buildAudio.Play();
    }

    //explicit enough
    public void RemovePointFromStructure(Goo _toRemove,bool destroyAfter = true)
    {
        if (this == _toRemove)
        {
            foreach (Goo connected in m_connections)
            {
                connected.RemovePointFromStructure(this);
            }
            foreach (SpringJoint2D spr in m_springJoints)
                spr.enabled = false;
            foreach (DistanceJoint2D dist in m_distanceJoints)
                dist.enabled = false;

            if (destroyAfter)
            {
                Die();
            }
            else
            {
                List<Connection> allChildren = transform.Cast<Transform>().Select(t => t.GetComponent<Connection>()).ToList();
                allChildren.RemoveAll(x => x == null);

                foreach(var connector in allChildren)
                    Destroy(connector.gameObject);

                if(s_goToFinishLine)
                PathFinder.Instance.SetClosenessToExit(Vaccuum.instance.m_finishGoo.GetComponent<Goo>(), 0);
            }


        }
        else
        {
            //find the joint that's connected to the thing to remove, might need to do the check for the balloon's joint as well
            SpringJoint2D result = m_springJoints.Find(x => x.connectedBody == _toRemove.m_rb);
            if (result != null)
            {
                result.connectedBody = null;
                result.enabled = false;
            }
            m_connections.Remove(_toRemove);

            foreach (Transform child in transform)
            {
                Connection comp = child.GetComponent<Connection>();
                if (comp != null && comp.m_target == _toRemove)
                {
                    //O B L I T E R A T E   T H E  C H I L D
                    Destroy(child.gameObject);
                }
            }
        }
        //if all connections to this point were destroyed, we want it to switch to being like the unplaced goos
        if (m_connections == null || m_connections.Count <= 0)
        {
            m_isUsed = false;
            m_animator.enabled = true;
            MoveOutOfStructure();
        }
    }
    //same principle as the previous one but by using a line as reference

    public void RemoveConnectionFromStructure(Connection connection, bool IsParent = true)
    {

        if (IsParent)
        {
            SpringJoint2D spring = m_springJoints.Find(x => x.connectedBody == connection.m_target.m_rb);
            if (spring != null)
            {
                spring.connectedBody = null;
                spring.autoConfigureDistance = true;
                spring.enabled = false;
                m_connections.Remove(connection.m_target);
            }
            connection.m_target.RemoveConnectionFromStructure(connection, false);
            Destroy(connection.gameObject);
        }
        else
        {
            m_connections.Remove(connection.transform.parent.GetComponent<Goo>());
        }
        if (m_connections == null || m_connections.Count <= 0)
        {
            m_isUsed = false;
            m_animator.enabled = true;
            MoveOutOfStructure();
        }
    }
    public List<Goo> GetFilteredConnections()
    {
        return m_connections.Where(x => x!=null && x.GetComponent<Goo_Balloon>() == null).ToList();
    }
    private protected IEnumerator PlanDestruction(bool giveScore = true)
    {
        isDying = true;
        if (giveScore)
        {
            m_animator.SetBool("Die", true);

            yield return new WaitWhile(IsAlive);

            gameObject.SetActive(false);
            Score.Instance.m_Score++;
        }
        else
        {
            //destroys all connections
            foreach (Transform child in transform)
                Destroy(child.gameObject);

            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<CircleCollider2D>().enabled = false;
            m_deathAudio.Play();
            yield return new WaitWhile(FinishedDying);
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            m_stayIdle = false;
        }
    }
    //Some extension like methods
    //used for end of level
    public void Absorb()
    {
        StartCoroutine(PlanDestruction());
    }
    //used for death
    public void Die()
    {
        StartCoroutine(PlanDestruction(false));
    }
    //To avoid constantly reassigning memory to this list multiple times per frame per goo
    private protected void EmptyAnchors()
    {
        for (int i = 0; i < m_validAnchors.Count; i++) m_validAnchors[i] = null;
    }
    private bool FinishedDying()
    {
        return m_deathAudio.isPlaying;
    }
    private protected bool IsAlive() { return transform.localScale.x > 0f; }
    public virtual IEnumerator DoThingIfUsed() { StartCoroutine(SetSelectableLate()); yield return null; }

    //clears the selected goo flag only 1 frame later so that if the player places a goo on another one, it doesn't select that previous goo as well during the same frame
    private protected IEnumerator SetSelectableLate()
    {
        yield return new WaitForFixedUpdate();
        s_isThereAGooSelected = false;
    }

}