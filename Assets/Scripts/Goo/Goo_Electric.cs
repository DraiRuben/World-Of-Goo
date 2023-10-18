using System.Collections;
using UnityEngine;

public class Goo_Electric : Goo
{
    [SerializeField]
    private GameObject m_electricityZone;
    public override IEnumerator DoThingIfUsed()
    {
        m_electricityZone.SetActive(true);
        StartCoroutine(SetSelectableLate());
        yield return null;

    }
}
