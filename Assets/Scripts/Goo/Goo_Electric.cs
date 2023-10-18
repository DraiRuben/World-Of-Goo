using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private protected override void DisablePreviewers()
    {
        List<Goo> FilteredAnchors = m_validAnchors.ToList();
        FilteredAnchors.RemoveAll(x => x == null);
        for (int i = 0; i < FilteredAnchors.Count; i++)
        {
            if (transform.childCount <= 1) break;
            var comp = transform.GetChild(1).GetComponent<Connection>();
            if (comp != null)
            {
                comp.m_IsInUse = false;
            }
        }
    }
    public override void TryInteract()
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
            else if (m_maxAllowedAnchorsAmount - m_validAnchors.Count(x => x == null) >= m_minAllowedAnchorsAmount)
            {
                DisablePreviewers();
                Use();
                EmptyAnchors();
            }
            else
            {
                //drop the goo in the air
                MoveOutOfStructure();

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
}
