using UnityEngine;
using UnityEngine.InputSystem;

public class MouseController : MonoBehaviour
{
    public void Click(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            var Hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), 5000, LayerMask.GetMask("Goo"));
            if (Hit.collider != null && Hit.collider.GetComponent<Goo>() != null)
            {
                Hit.collider.GetComponent<Goo>().TryInteract();
            }
        }
    }
}
