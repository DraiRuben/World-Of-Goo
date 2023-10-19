using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Goo_Electric : Goo
{
    [SerializeField]
    private AudioSource m_electricity;
    [SerializeField]
    private GameObject m_electricityZone;


    public override IEnumerator DoThingIfUsed()
    {
        m_electricityZone.SetActive(true);
        StartCoroutine(SetSelectableLate());
        yield return null;

    }
    private protected override void DisablePreviewers()
    {
        List<Goo> FilteredAnchors = m_validAnchors.ToList();
        FilteredAnchors.RemoveAll(x => x == null);
        for (int i = 0; i < FilteredAnchors.Count; i++)
        {
            //the electric goo always has at least 1 child (electric field)
            if (transform.childCount <= 1) break;
            if (transform.GetChild(1).TryGetComponent<Connection>(out Connection comp))
            {
                comp.IsInUse = false;
            }
        }
    }

}
