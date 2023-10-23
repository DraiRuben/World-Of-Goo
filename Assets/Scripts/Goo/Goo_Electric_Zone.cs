using UnityEngine;

public class Goo_Electric_Zone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.GetComponent<Activable>() != null)
        {
            collider.GetComponent<Activable>().Interact();
        }
    }
}
