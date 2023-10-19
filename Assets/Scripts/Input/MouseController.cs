using UnityEngine;
using UnityEngine.InputSystem;

public class MouseController : MonoBehaviour
{
    //tries to find a goo under mouse and interact with it
    public void Click(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            RaycastHit2D Hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), 5000, LayerMask.GetMask("Goo"));
            if (Hit.collider != null && Hit.collider.GetComponent<Goo>() != null)
            {
                Hit.collider.GetComponent<Goo>().TryInteract();
            }
        }
    }
}
