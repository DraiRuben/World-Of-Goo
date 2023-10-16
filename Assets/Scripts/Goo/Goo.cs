using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Goo : MonoBehaviour
{
    public static bool s_isThereAGooSelected = false;
    public static bool s_goToFinishLine = false;
    public static GameObject s_finishLineGoo;

    public bool m_isUsed = false;
    [HideInInspector]
    public bool m_isSelected = false;
    [SerializeField]
    private protected bool m_isBuildableOn = true;
    [SerializeField]
    private protected int m_maxAllowedAnchorsAmount = 3;
    public int m_minAllowedAnchorsAmount = 2;
    private protected List<GameObject> m_validAnchors = new();
    [SerializeField]
    private protected float m_minAttachDistance = 1f;
    [SerializeField]
    private protected float m_maxAttachDistance = 4f;
    [SerializeField]
    private protected float m_attachStrength = 13f;

    [SerializeField]
    private protected GameObject m_connectionPrefab;

    private GameObject m_pathOrigin;
    private GameObject m_pathTarget;

    private protected List<SpringJoint2D> m_springJoints;
    private protected List<DistanceJoint2D> m_distanceJoints;
    private protected List<GameObject> pathToEnd = null;
    private protected Rigidbody2D m_rb;
    private protected float m_movementTimer = 0;
    private protected Coroutine m_behaviour;
    private void Start()
    {
        for (int i = 0; i < m_maxAllowedAnchorsAmount; i++) m_validAnchors.Add(null);
        for (int i = GetComponents<SpringJoint2D>().Length; i < m_maxAllowedAnchorsAmount; i++)
        {
            var temp = gameObject.AddComponent<SpringJoint2D>();
            temp.enabled = false;
            temp.frequency = m_attachStrength;
        }
        for (int i = GetComponents<DistanceJoint2D>().Length; i < m_maxAllowedAnchorsAmount; i++)
        {
            var temp = gameObject.AddComponent<DistanceJoint2D>();
            temp.enabled = false;
        }
        m_distanceJoints = GetComponents<DistanceJoint2D>().ToList();
        m_springJoints = GetComponents<SpringJoint2D>().ToList();
        m_rb = GetComponent<Rigidbody2D>();

        if (!m_isUsed)
        {
            if (TryGetPath(1.5f))
            {
                m_behaviour ??= StartCoroutine(Behaviour());
            }
        }
    }
    private void OnJointBreak2D(Joint2D joint)
    {
        //not sure this works, since if the joint break,s maybe the connected body is set to null, I didn't test it
        PathFinder.Instance.Structure.RemoveConnection(gameObject, joint.connectedBody.gameObject);
        PathFinder.Instance.Structure.vertices--;
    }


    public virtual void TryInteract()
    {
        if (m_isUsed || (s_isThereAGooSelected && !m_isSelected)) return;

        if (m_isSelected)
        {
            //checks if the click was on top of a link between 2 goos, if yes, put the selected goo back there, otherwise just build
            if (TryGetPath())
            {
                m_behaviour ??= StartCoroutine(Behaviour());
                return;

            }
            //Try to attach it to the structure
            if (m_maxAllowedAnchorsAmount - m_validAnchors.Count(x => x == null) >= m_minAllowedAnchorsAmount)
            {
                Use();
                DisablePreviewers();
            }
            else
            {
                //drop the goo in the air
                m_behaviour ??= StartCoroutine(Behaviour());
                m_isSelected = false;
                m_rb.isKinematic = false;
                s_isThereAGooSelected = false;
                DisablePreviewers();

            }

        }
        else
        {
            //make it follow the mouse
            s_isThereAGooSelected = true;
            m_isSelected = true;
            StartCoroutine(Select());
            StartCoroutine(AnchorTesting());
        }
    }
    private void DisablePreviewers()
    {
        var FilteredAnchors = m_validAnchors.ToList();
        FilteredAnchors.RemoveAll(x => x == null);
        for (int i = 0; i < FilteredAnchors.Count; i++)
        {
            if (i >= transform.childCount) break;

            transform.GetChild(i).GetComponent<Connection>().m_isInUse = false;
        }
        EmptyAnchors();
    }
    private void EmptyAnchors()
    {
        for (int i = 0; i < m_validAnchors.Count; i++) m_validAnchors[i] = null;
    }
    public virtual void Use()
    {
        //stops select and anchorpoint testing routines
        //manages flags
        //Places goo on structure or puts it back if close enough
        //starts coroutine that does whatever I want when it's placed on a structure like a balloon lifing up

        //remove all null refs
        var filteredAnchors = m_validAnchors.ToList();
        filteredAnchors.RemoveAll(x => x == null);
        for (int i = 0; i < filteredAnchors.Count; i++)
        {
            PlaceConnection(filteredAnchors, i);
        }
        //creates a copy of the anchors and pass it onto the structure
        PathFinder.Instance.Structure.Connections[gameObject] = filteredAnchors.ToList();
        foreach (var connector in filteredAnchors)
        {
            PathFinder.Instance.Structure.Connections[connector].Add(gameObject);
        }
        m_isUsed = true;
        m_isSelected = false;
        StartCoroutine(DoThingIfUsed());
    }
    public IEnumerator AnchorTesting()
    {
        while (m_isSelected)
        {
            var temp = m_validAnchors.ToList();
            temp.RemoveAll(x => x == null);
            TryResetPreviewers(temp.Count);
            var A = Physics2D.OverlapCircleAll(transform.position, m_maxAttachDistance, LayerMask.GetMask("Goo"));
            A = A.Where(x => x.CompareTag("Goo") && x.GetComponent<Goo>().m_isUsed && x.GetComponent<Goo>().m_isBuildableOn).ToArray();
            if (!s_goToFinishLine && A != null && A.Length >= m_minAllowedAnchorsAmount)
                foreach (var coll in A)
                {
                    //checks for min distance and if it's a goo and fixed on a structure
                    float Distance = Vector2.Distance(coll.transform.position, transform.position);
                    Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    if (Distance >= m_minAttachDistance
                        && !Physics2D.Raycast(coll.transform.position, MousePos - coll.transform.position, Vector2.Distance(MousePos, coll.transform.position), LayerMask.GetMask("Default")))
                    {
                        //display phantom connection and adds anchor in list if the distance
                        //between this and the anchor is smaller than one of the things in the list
                        //or if the list isn't full yet, just add it to the anchor points instead of replacing one
                        var index = m_validAnchors.IndexOf(null);
                        //returns -1 if it doesn't find an element that's null => if the list is full already
                        if (index != -1 && !m_validAnchors.Contains(coll.gameObject))
                        {
                            m_validAnchors[index] = coll.gameObject;
                            SetupPreviewer(coll);
                        }
                        else if (!m_validAnchors.Contains(coll.gameObject))
                        {
                            int HighestDistanceAnchorIndex = -1;
                            for (int i = 0; i < m_validAnchors.Count; i++)
                            {
                                if (Vector2.Distance(transform.position, m_validAnchors[i].transform.position) > Distance)
                                    HighestDistanceAnchorIndex = i;
                            }
                            //replace an existing previewer
                            if (HighestDistanceAnchorIndex != -1)
                            {
                                UpdatePreviewer(coll, HighestDistanceAnchorIndex);
                                m_validAnchors[HighestDistanceAnchorIndex] = coll.gameObject;
                            }
                        }
                    }
                }
            yield return null;
        }
    }
    private bool TryGetPath(float searchRadius = 0.5f)
    {
        var overlapping = Physics2D.OverlapCircleAll(transform.position, searchRadius, LayerMask.GetMask("GooConnection"));

        foreach (var p in overlapping)
        {
            if (p.name.Contains("Bar"))
            {
                m_pathTarget = p.transform.parent.parent.gameObject;
                m_pathOrigin = p.transform.parent.GetComponent<Connection>().m_target;
                //don't want to be able to place a goo back on a balloon's string
                if (m_pathTarget.GetComponent<Goo_Balloon>() != null || m_pathOrigin.GetComponent<Goo_Balloon>() != null) return false;

                DisablePreviewers();

                //swaps if the origin is further from the goo, so that the target is the further one
                if (m_isSelected && Vector2.Distance(transform.position, m_pathTarget.transform.position) < Vector2.Distance(transform.position, m_pathOrigin.transform.position))
                {
                    var temp = m_pathTarget;
                    m_pathTarget = m_pathOrigin;
                    m_pathOrigin = temp;
                }
                m_movementTimer = Vector2.Distance(transform.position, m_pathOrigin.transform.position) / Vector2.Distance(m_pathOrigin.transform.position, m_pathTarget.transform.position);

                m_isSelected = false;
                s_isThereAGooSelected = false;
                m_rb.isKinematic = true;
                return true;
            }
        }
        return false;
    }
    public IEnumerator Select()
    {
        transform.parent = null;
        m_rb.isKinematic = false;
        while (m_isSelected)
        {
            m_rb.MovePosition((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));
            yield return new WaitForFixedUpdate();
        }
    }
    //clears the selected goo flag only 1 frame later so that if the player places a goo on another one, it doesn't select that previous goo as well during the same frame
    public virtual IEnumerator DoThingIfUsed() { yield return null; s_isThereAGooSelected = false; }

    public IEnumerator Behaviour()
    {
        yield return null;
        if (m_rb.isKinematic)
        {
            m_rb.gravityScale = 0f;
        }
        else
        {
            //gets first child of the structure => bottom left goo of the starter structure
            m_pathTarget = PathFinder.Instance.transform.parent.GetChild(0).gameObject;
        }

        m_isSelected = false;
        //fix since when I put rigidbody to kinematic, the velocity is frozen at its values before being kine
        m_rb.velocity = new(m_rb.velocity.x, 0);
        while (!m_isSelected)
        {
            //if it's on the structure it's kinematic
            if (m_rb.isKinematic)
            {
                //normalizes so that speed doesn't change depending on the length of the connection
                //TODO: fix null ref to either origin or target, don't know which, when the structure gets destroyed
                m_movementTimer += 2 * Time.fixedDeltaTime / Vector2.Distance(m_pathOrigin.transform.position, m_pathTarget.transform.position);
                if (Vector2.Distance(transform.position, m_pathTarget.transform.position) < 0.1f)
                {
                    m_movementTimer = 0f;
                    if (!FindNextTarget()) break;
                }
                else
                {
                    m_rb.MovePosition(Vector3.Lerp(m_pathOrigin.transform.position, m_pathTarget.transform.position, m_movementTimer));
                }
            }//otherwise it's on the ground
            else
            {
                m_rb.velocity = new Vector2(Mathf.Sign(m_pathTarget.transform.position.x - transform.position.x) * 3, m_rb.velocity.y);
                TryGetPath();
            }

            yield return new WaitForFixedUpdate();
        }
        m_rb.gravityScale = 1f;
        m_behaviour = null;
    }
    private bool FindNextTarget()
    {
        //for some reason, when dropping the goo above the structure, velocity y fucks with it and the goo has some weird clipping because of it
        m_rb.velocity = new(m_rb.velocity.x, 0);

        //finds next target, either random if the structure isn't connected to the exit,
        //or the next target in the shortest path towards the exit if there is one
        m_pathOrigin = m_pathTarget;
        if (s_goToFinishLine && pathToEnd == null)
        {
            pathToEnd = PathFinder.Instance.Structure.GetShortestPathBetween(m_pathOrigin, s_finishLineGoo);
        }
        else if (s_goToFinishLine && pathToEnd.Count > 0)
        {
            pathToEnd.RemoveAt(0);
            if (pathToEnd.Count <= 0) return false;

            if (pathToEnd[0] != null)
            {
                m_pathTarget = pathToEnd[0];
            }
            else
            {
                //regens the path to the end if one step gets destroyed/if a connection breaks
                pathToEnd = PathFinder.Instance.Structure.GetShortestPathBetween(m_pathOrigin, s_finishLineGoo);
                m_pathTarget = pathToEnd[0];
            }
        }
        else
        {
            m_pathTarget = PathFinder.Instance.Structure.GetRandomDestination(m_pathTarget);
            //if the target got destroyed
            if (m_pathTarget == null)
            {
                m_isSelected = false;
                m_rb.isKinematic = false;
                m_pathTarget = PathFinder.Instance.transform.parent.GetChild(0).gameObject;

            }

        }
        return true;
    }
    private void SetupPreviewer(Collider2D coll)
    {
        //gets a connection previewer that's usable and sets it up
        Connection availableConnection = (Connection)Pooling.Instance.pools["Previewers"].Find(x => !((Connection)x).m_isInUse);
        availableConnection.transform.position = transform.position;
        availableConnection.transform.parent = transform;
        availableConnection.m_target = coll.gameObject;
        availableConnection.m_isInUse = true;
        availableConnection.enabled = true;
    }
    private void TryResetPreviewers(int ValidAnchorCount)
    {
        //clears connections for goos that were in connection preview but went too far
        for (int i = 0; i < m_validAnchors.Count; i++)
        {
            if (m_validAnchors[i] == null) continue;
            float Distance = Vector2.Distance(m_validAnchors[i].transform.position, transform.position);
            if (!(Distance >= m_minAttachDistance && Distance <= m_maxAttachDistance) || ValidAnchorCount <= m_minAllowedAnchorsAmount)
            {
                //disable preview connection thingy, then remove from list, cool since we're not changing the collection's size
                var allChildren = transform.Cast<Transform>().Select(t => t.GetComponent<Connection>()).ToList();
                //replaces the target of the previewer to replace, instead of going through the pool system which would be longer
                var previewer = allChildren.Find(x => x.m_target == m_validAnchors[i]);
                previewer.m_target = null;
                previewer.m_isInUse = false;
                previewer.transform.parent = Pooling.Instance.transform;
                previewer.transform.localPosition = Vector3.zero;
                previewer.enabled = false;
                m_validAnchors[i] = null;
            }
        }
    }
    private void UpdatePreviewer(Collider2D coll, int HighestDistanceAnchorIndex)
    {
        var allChildren = transform.Cast<Transform>().Select(t => t.GetComponent<Connection>()).ToList();
        //replaces the target of the previewer to replace, instead of going through the pool system which would be longer
        var Previewer = allChildren.Find(x => x.m_target == m_validAnchors[HighestDistanceAnchorIndex]);
        Previewer.m_target = coll.gameObject;
        Previewer.m_isInUse = true;
        Previewer.enabled = true;
    }
    private void PlaceConnection(List<GameObject> filteredAnchors, int i)
    {
        PathFinder.Instance.Structure.vertices++;
        m_springJoints[i].connectedBody = filteredAnchors[i].GetComponent<Rigidbody2D>();
        m_springJoints[i].enabled = true;
        m_springJoints[i].autoConfigureDistance = false;
        var connection = Instantiate(m_connectionPrefab, transform.position, Quaternion.identity, transform);
        connection.GetComponent<Connection>().m_target = filteredAnchors[i];
        connection.GetComponent<Connection>().m_isInUse = true;
    }
    public IEnumerator PlanDestruction()
    {
        while (transform.localScale.magnitude > 0.4f)
        {
            transform.localScale -= transform.localScale * Time.fixedDeltaTime*2;
            yield return new WaitForFixedUpdate();
        }
        gameObject.SetActive(false);
        Score.Instance.m_Score++;

    }
}
