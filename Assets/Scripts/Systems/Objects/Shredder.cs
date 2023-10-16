using UnityEngine;

public class Shredder : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Goo"))
        {
            var comp = collision.collider.GetComponent<Goo>();
            if (comp.m_isUsed)
            {
                comp.RemovePointFromStructure(comp);
                if (comp != null)
                    Destroy(comp.gameObject);
            }
            else if (!comp.m_isSelected)
            {
                Destroy(collision.collider.gameObject);
            }
        }
        else if (collision.collider.CompareTag("GooConnection"))
        {
            var comp = collision.collider.GetComponentInParent<Connection>();
            if (comp != null && comp.m_isInUse && !comp.m_isPreviewer)
            {
                comp.transform.parent.GetComponent<Goo>().RemoveConnectionFromStructure(comp);
                if (comp != null)
                    Destroy(comp.gameObject);
            }
        }
    }
}
