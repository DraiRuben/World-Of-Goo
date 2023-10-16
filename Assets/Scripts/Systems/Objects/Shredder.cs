using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shredder : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Goo"))
        {
            if (collision.collider.GetComponent<Goo>().m_isUsed)
            {
                PathFinder.Instance.Structure.RemovePoint(collision.collider.gameObject);

            }
            else
            {
                Destroy(collision.collider.gameObject);
            }
        }
        else if (collision.collider.CompareTag("GooConnection"))
        {
            if(collision.collider.GetComponentInParent<Connection>().m_isInUse)
            {

            }
        }
    }
}
