using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkingDoor : Activable
{
    [SerializeField]
    private SpriteRenderer m_sprite;
    private BoxCollider2D m_collider;

    [SerializeField]
    private bool m_isLeftShrinkPoint = false;
    private void Awake()
    {
        m_collider = GetComponent<BoxCollider2D>();
    }
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
        while(m_sprite.size.x > 1f)
        {
            m_sprite.size -= new Vector2(Time.fixedDeltaTime,0);
            m_collider.size -= new Vector2(Time.fixedDeltaTime,0);
            
            transform.position += new Vector3((m_isLeftShrinkPoint?-1:1) * Time.fixedDeltaTime/2, 0, 0);
            yield return new WaitForFixedUpdate();
        }
    }
}
