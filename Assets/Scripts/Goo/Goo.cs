using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Goo : MonoBehaviour
{
    private protected bool IsUsed = false;
    private protected bool IsSelected = false;
    private protected static bool IsThereAGooSelected = false;
    private protected int AllowAnchorsAmount = 2;
    private protected List<GameObject> ValidAnchors = new ();
    [SerializeField]
    private protected float MinAttachDistance = 1f;
    [SerializeField]
    private protected float MaxAttachDistance = 5f;
    [SerializeField]
    private protected GameObject ConnectionPrefab;

    private protected List<SpringJoint2D> SpringJoints;
    
    private void Start()
    {
        for (int i = 0; i < AllowAnchorsAmount; i++) ValidAnchors.Add(null);
        for(int i = 0; i < AllowAnchorsAmount; i++)
        {
            var temp = gameObject.AddComponent<SpringJoint2D>();
            temp.enableCollision = true;
        }
        SpringJoints = GetComponents<SpringJoint2D>().ToList();
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
        if(IsUsed || (IsThereAGooSelected && !IsSelected)) return;

        if (IsSelected)
        {
            //Try to attach it to the structure
            if(!ValidAnchors.Contains(null))
            Use();

        }
        else
        {
            //make it follow the mouse
            IsThereAGooSelected = true;
            StartCoroutine(Select());
            StartCoroutine(AnchorTesting());
        }
    }
    //=null is only for compilation for now, remove it when tryinteract it done
    public void Use()
    {
        //stops select and anchorpoint testing routines
        //manages flags
        //Places goo on structure
        //starts coroutine that does whatever I want when it's placed on a structure like a balloon lifing up
        StopCoroutine(Select());
        StopCoroutine(AnchorTesting());
        for(int i = 0; i < ValidAnchors.Count; i++)
        {
            SpringJoints[i].connectedBody = ValidAnchors[i].GetComponent<Rigidbody2D>();
            var connection = Instantiate(ConnectionPrefab, transform.position, Quaternion.AngleAxis(Mathf.Atan2(transform.position.z, transform.position.y) * Mathf.Rad2Deg, Vector3.forward), transform);
            connection.GetComponent<Connection>().Target = ValidAnchors[i];
        }
        IsUsed = true;
        IsSelected = false;
        StartCoroutine(DoThingIfUsed());
    }
    public IEnumerator AnchorTesting()
    {
        while (true)
        {
            //clears connections for goos that were in connection preview but went too far
            for(int i=0; i<ValidAnchors.Count;i++)
            {
                if (ValidAnchors[i] == null) continue;
                float Distance = Vector2.Distance(ValidAnchors[i].transform.position, transform.position);
                if (!(Distance >= MinAttachDistance && Distance <= MaxAttachDistance))
                {
                    //disable preview connection thingy, then remove from list, cool since we're not changing the collection's size
                    var allChildren = transform.Cast<Transform>().Select(t => t.GetComponent<Connection>()).ToList();
                    //replaces the target of the previewer to replace, instead of going through the pool system which would be longer
                    var previewer = allChildren.Find(x => x.Target == ValidAnchors[i]);
                    previewer.Target = null;
                    previewer.IsInUse = false;
                    previewer.transform.parent = Pooling.Instance.transform;
                    previewer.transform.localPosition = Vector3.zero;
                    previewer.enabled = false;
                    ValidAnchors[i] = null;
                }
            }
            var  A = Physics.OverlapSphere(transform.position, MaxAttachDistance);
            if(A != null)
                foreach(var coll in A)
                { 
                    //checks for min distance and if it's a goo and fixed on a structure
                    if (coll.CompareTag("Goo") && coll.GetComponent<Goo>().IsUsed)
                    {
                        float Distance = Vector2.Distance(coll.transform.position, transform.position);
                        if (Distance >= MinAttachDistance)
                        {
                            //display phantom connection and adds anchor in list if the distance
                            //between this and the anchor is smaller than one of the things in the list
                            //or if the list isn't full yet, just add it to the anchor points instead of replacing one
                            var index = ValidAnchors.IndexOf(null);
                            //returns -1 if it doesn't find an element that's null/if the list is full already
                            if (index != -1)
                            {
                                ValidAnchors[index] = coll.gameObject;
                                //gets a connection previewer that's usable and sets it up
                                Connection availableConnection = (Connection)Pooling.Instance.pools["Previewers"].Find(x => !((Connection)x).IsInUse);
                                availableConnection.transform.position = transform.position;
                                availableConnection.transform.parent = transform;
                                availableConnection.Target = coll.gameObject;
                                availableConnection.IsInUse = true;
                                availableConnection.enabled = true;
                            }
                            else
                            {
                                int HighestDistanceAnchorIndex = -1;
                                for(int i =0;i<ValidAnchors.Count;i++)
                                {
                                    if (Vector2.Distance(transform.position, ValidAnchors[i].transform.position) > Distance)
                                        HighestDistanceAnchorIndex = i;
                                }
                                //replace an existing previewer
                                if (HighestDistanceAnchorIndex != -1)
                                {
                                    var allChildren = transform.Cast<Transform>().Select(t => t.GetComponent<Connection>()).ToList();
                                    //replaces the target of the previewer to replace, instead of going through the pool system which would be longer
                                    var connection = allChildren.Find(x=>x.Target == ValidAnchors[HighestDistanceAnchorIndex]);
                                    connection.Target = coll.gameObject;
                                    connection.IsInUse = true;
                                    connection.enabled = true;
                                    ValidAnchors[HighestDistanceAnchorIndex] = coll.gameObject;
                                }
                            }
                            
                        }
                    }
                }
            yield return null;
            //TODO: Remove all connection previews on routine exit
        }
    }
    public IEnumerator Select()
    {
        while (true)
        {
            transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            yield return null;
        }
    }
    //clears the selected goo flag only 1 frame later so that if the player places a goo on another one, it doesn't select that previous goo as well during the same frame
    public virtual IEnumerator DoThingIfUsed() { yield return null; IsThereAGooSelected = false; }
}
