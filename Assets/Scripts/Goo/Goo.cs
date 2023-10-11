using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        for (int i = 0; i < AllowAnchorsAmount; i++) ValidAnchors.Add(null);
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
            if(ValidAnchors != null)
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
        foreach(var a in ValidAnchors)
        {
            //attach this object to each anchorpoint
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
                    ValidAnchors[i] = null;
                }
            }
            var  A = Physics.OverlapSphere(transform.position, MaxAttachDistance);
            if(A != null)
                foreach(var coll in A)
                { 
                    //checks for min distance and if it's a goo
                    if (coll.CompareTag("Goo"))
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
                            }
                            else
                            {
                                int HighestDistanceAnchorIndex = -1;
                                for(int i =0;i<ValidAnchors.Count;i++)
                                {
                                    if (Vector2.Distance(transform.position, ValidAnchors[i].transform.position) > Distance)
                                        HighestDistanceAnchorIndex = i;
                                }
                                if(HighestDistanceAnchorIndex!=-1)
                                    ValidAnchors[HighestDistanceAnchorIndex] = coll.gameObject;
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
