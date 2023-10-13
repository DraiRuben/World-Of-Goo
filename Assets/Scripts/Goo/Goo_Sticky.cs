using System.Collections;
using UnityEngine;

public class Goo_Sticky : Goo
{
    private bool m_canStick = false;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (m_canStick) m_rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }
    public override IEnumerator DoThingIfUsed()
    {
        yield return null;
        m_canStick = true;
        s_isThereAGooSelected = false;
    }
}
