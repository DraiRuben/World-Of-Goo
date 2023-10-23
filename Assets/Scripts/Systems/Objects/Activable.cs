using UnityEngine;

public abstract class Activable : MonoBehaviour
{
    private protected bool m_activated = false;
    public abstract void Interact();
}
