using System.Collections;
using System.Linq;
using UnityEngine;

public class Goo_Balloon : Goo
{
    [SerializeField]
    private float LiftStrength = 1.0f;


    public override IEnumerator DoThingIfUsed()
    {
        yield return null;
        s_isThereAGooSelected = false;
        while (m_isUsed)
        {
            m_rb.velocity += new Vector2(0, LiftStrength);
            //clamps x so the balloon doesn't go apeshit
            m_rb.velocity = new(Mathf.Clamp(m_rb.velocity.x, 0, 1), m_rb.velocity.y);
            yield return new WaitForFixedUpdate();
        }
    }
    public override void Use()
    {
        //stops select and anchorpoint testing routines
        //manages flags
        //Places goo on structure
        //starts coroutine that does whatever I want when it's placed on a structure like a balloon lifing up

        //remove all null refs
        var filteredAnchors = m_validAnchors.ToList();
        filteredAnchors.RemoveAll(x => x == null);
        for (int i = 0; i < filteredAnchors.Count; i++)
        {
            PlaceConnection(filteredAnchors, i);
        }
        m_isUsed = true;
        m_isSelected = false;
        StartCoroutine(AdaptConnectionLength());
        StartCoroutine(DoThingIfUsed());
    }
    private IEnumerator AdaptConnectionLength()
    {
        while (Mathf.Abs(m_distanceJoints[0].distance - m_maxAttachDistance) > 0.4f)
        {
            m_distanceJoints[0].distance += 2 * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
}
