using UnityEngine;

public class Shredder : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Goo"))
        {
            Goo comp = collision.collider.GetComponent<Goo>();
            if (comp.m_isUsed)
            {
                comp.RemovePointFromStructure(comp);

            }
            else if (!comp.m_isSelected)
            {
                comp.Die();
            }
        }
        else if (collision.collider.CompareTag("GooConnection"))
        {
            Connection comp = collision.collider.GetComponentInParent<Connection>();
            if (comp != null && comp.IsInUse && !comp.m_isPreviewer)
            {
                comp.transform.parent.GetComponent<Goo>().RemoveConnectionFromStructure(comp);

            }
        }
    }
}
