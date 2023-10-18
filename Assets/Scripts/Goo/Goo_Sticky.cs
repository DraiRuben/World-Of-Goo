using System.Collections;
using UnityEngine;

public class Goo_Sticky : Goo
{
    private bool m_canStick = false;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (m_canStick) m_rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }
    //not much to say, this just stops moving when it first collides with something by freezing itself
    public override IEnumerator DoThingIfUsed()
    {
        StartCoroutine(SetSelectableLate());
        yield return null;
        m_canStick = true;
    }
}
