using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Goo : MonoBehaviour
{
    public bool move = true;
    [SerializeField]
    private protected bool m_IsUsed = false;
    private protected bool m_isSelected = false;
    private protected static bool IsThereAGooSelected = false;
    private protected int m_allowedAnchorsAmount = 2;
    private protected List<GameObject> m_validAnchors = new ();
    [SerializeField]
    private protected float m_minAttachDistance = 1f;
    [SerializeField]
    private protected float m_maxAttachDistance = 5f;
    [SerializeField]
    private protected float m_movementSpeed = 5f;
    [SerializeField]
    private protected GameObject m_connectionPrefab;

    public GameObject target;

    private protected List<SpringJoint2D> m_springJoints;
    private Rigidbody2D m_rb;
    
    private void Start()
    {
        for (int i = 0; i < m_allowedAnchorsAmount; i++) m_validAnchors.Add(null);
        for(int i = GetComponents<SpringJoint2D>().Length; i < m_allowedAnchorsAmount; i++)
        {
            var temp = gameObject.AddComponent<SpringJoint2D>();
            temp.enabled = false;
            temp.frequency = 500f;
        }
        m_springJoints = GetComponents<SpringJoint2D>().ToList();
        m_rb = GetComponent<Rigidbody2D>();

        if (!m_IsUsed) StartCoroutine(Behaviour());
    }
    private void OnJointBreak2D(Joint2D joint)
    {
        //not sure this works, since if the joint break,s maybe the connected body is set to null, I didn't test it
        PathFinder.Instance.Structure.RemoveConnection(gameObject, joint.connectedBody.gameObject);
    }
    public IEnumerator GoToPipe()
    {
        //algo that allows goo to go to the pipe when it's active by using a navmesh through all connections of the structure built by the player
        yield return null;
    }
    //thing done when selected and then placed
    public virtual void TryInteract()
    {
        if(m_IsUsed || (IsThereAGooSelected && !m_isSelected)) return;

        if (m_isSelected)
        {
            //checks if the click was on top of a link between 2 goos, if yes, put the selected goo back there, otherwise just build
            var overlapping = Physics2D.OverlapCircleAll(transform.position, 0.5f);
            foreach(var p in overlapping)
            {
                if (p.name.Contains("Bar"))
                {
                    //sets the origin of the connection as the initial target to go to
                    DisablePreviewers();
                    m_rb.isKinematic = true;
                    IsThereAGooSelected = false;
                    m_isSelected = false;
                    target = p.transform.parent.parent.gameObject;
                    StartCoroutine(Behaviour());
                    return;
                }
            }
            //Try to attach it to the structure
            if (!m_validAnchors.Contains(null))
            {
                Use();
                DisablePreviewers();

            }

        }
        else
        {
            //make it follow the mouse
            IsThereAGooSelected = true;
            m_isSelected = true;
            StartCoroutine(Select());
            StartCoroutine(AnchorTesting());
        }
    }
    private void DisablePreviewers()
    {
        for (int i = 0; i < m_validAnchors.Count; i++)
        {
            transform.GetChild(i).GetComponent<Connection>().m_isInUse = false;
        }
        EmptyAnchors();
    }
    private void EmptyAnchors()
    {
        for (int i = 0; i < m_validAnchors.Count; i++) m_validAnchors[i] = null;
    }
    public void Use()
    {
        //stops select and anchorpoint testing routines
        //manages flags
        //Places goo on structure
        //starts coroutine that does whatever I want when it's placed on a structure like a balloon lifing up
        for(int i = 0; i < m_validAnchors.Count; i++)
        {
            m_springJoints[i].connectedBody = m_validAnchors[i].GetComponent<Rigidbody2D>();
            m_springJoints[i].enabled = true;
            m_springJoints[i].autoConfigureDistance = false;
            var connection = Instantiate(m_connectionPrefab, transform.position, Quaternion.identity, transform);
            connection.GetComponent<Connection>().m_target = m_validAnchors[i];
            connection.GetComponent<Connection>().m_isInUse = true;
        }
        //creates a copy of the anchors and pass it onto the structure
        PathFinder.Instance.Structure.Connections[gameObject] = m_validAnchors.ToList();
        foreach(var connector in m_validAnchors)
        {
            PathFinder.Instance.Structure.Connections[connector].Add(gameObject);
        }
        m_IsUsed = true;
        m_isSelected = false;
        StartCoroutine(DoThingIfUsed());
    }
    public IEnumerator AnchorTesting()
    {
        while (m_isSelected)
        {
            //clears connections for goos that were in connection preview but went too far
            for(int i=0; i<m_validAnchors.Count;i++)
            {
                if (m_validAnchors[i] == null) continue;
                float Distance = Vector2.Distance(m_validAnchors[i].transform.position, transform.position);
                if (!(Distance >= m_minAttachDistance && Distance <= m_maxAttachDistance))
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
            var  A = Physics2D.OverlapCircleAll(transform.position, m_maxAttachDistance);
            if(A != null)
                foreach(var coll in A)
                { 
                    //checks for min distance and if it's a goo and fixed on a structure
                    if (coll.CompareTag("Goo") && coll.GetComponent<Goo>().m_IsUsed)
                    {
                        float Distance = Vector2.Distance(coll.transform.position, transform.position);
                        if (Distance >= m_minAttachDistance)
                        {
                            //display phantom connection and adds anchor in list if the distance
                            //between this and the anchor is smaller than one of the things in the list
                            //or if the list isn't full yet, just add it to the anchor points instead of replacing one
                            var index = m_validAnchors.IndexOf(null);
                            //returns -1 if it doesn't find an element that's null/if the list is full already
                            if (index != -1 && !m_validAnchors.Contains(coll.gameObject))
                            {
                                m_validAnchors[index] = coll.gameObject;
                                //gets a connection previewer that's usable and sets it up
                                Connection availableConnection = (Connection)Pooling.Instance.pools["Previewers"].Find(x => !((Connection)x).m_isInUse);
                                availableConnection.transform.position = transform.position;
                                availableConnection.transform.parent = transform;
                                availableConnection.m_target = coll.gameObject;
                                availableConnection.m_isInUse = true;
                                availableConnection.enabled = true;
                            }
                            else if (!m_validAnchors.Contains(coll.gameObject))
                            {
                                int HighestDistanceAnchorIndex = -1;
                                for(int i =0;i<m_validAnchors.Count;i++)
                                {
                                    if (Vector2.Distance(transform.position, m_validAnchors[i].transform.position) > Distance)
                                        HighestDistanceAnchorIndex = i;
                                }
                                //replace an existing previewer
                                if (HighestDistanceAnchorIndex != -1)
                                {
                                    var allChildren = transform.Cast<Transform>().Select(t => t.GetComponent<Connection>()).ToList();
                                    //replaces the target of the previewer to replace, instead of going through the pool system which would be longer
                                    var connection = allChildren.Find(x=>x.m_target == m_validAnchors[HighestDistanceAnchorIndex]);
                                    connection.m_target = coll.gameObject;
                                    connection.m_isInUse = true;
                                    connection.enabled = true;
                                    m_validAnchors[HighestDistanceAnchorIndex] = coll.gameObject;
                                }
                            }
                            
                        }
                    }
                }
            yield return null;
            //TODO: Remove all connection previews on routine exit
        }
    }
    public IEnumerator Behaviour()
    {
        yield return null;
        m_rb.gravityScale = 0f;
        m_isSelected = false;
        //in case the structure falls somehow
        while (!m_isSelected)
        {
            if(Vector2.Distance(transform.position, target.transform.position) < 0.2f)
            {
                target = PathFinder.Instance.Structure.GetRandomDestination(target);
            }
            else
            {
                m_rb.MovePosition(Vector3.Lerp(transform.position,target.transform.position, m_movementSpeed*Time.fixedDeltaTime/10));
            }
            yield return new WaitForFixedUpdate();
        }
        m_rb.gravityScale = 1f;
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
    public virtual IEnumerator DoThingIfUsed() { yield return null; IsThereAGooSelected = false; }
}
