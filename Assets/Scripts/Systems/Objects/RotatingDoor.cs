using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingDoor : Activable
{
    [SerializeField]
    private Quaternion targetRotation;
    public override void Interact()
    {
        if(!m_activated)
        {
            StartCoroutine(DoThingIfActivated());
            m_activated = true;
        }
    }
    private IEnumerator DoThingIfActivated()
    {
        Quaternion originalRotation = transform.rotation;
        float timer = 0f;
        while(Quaternion.Angle(transform.rotation, targetRotation) > 2f)
        {
            transform.rotation = Quaternion.Slerp(originalRotation, targetRotation, timer);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
}
