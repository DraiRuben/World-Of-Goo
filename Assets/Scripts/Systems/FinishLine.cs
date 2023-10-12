using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{

    private DistanceJoint2D m_vaccum;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Goo") && !Goo.GoToFinishLine && collision.GetComponent<Goo>().m_isUsed)
        {
            m_vaccum.enabled = true;
            m_vaccum.connectedBody = collision.GetComponent<Rigidbody2D>();
            Goo.GoToFinishLine = true;
            StartCoroutine(Suck());
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Goo") && !Goo.GoToFinishLine && collision.GetComponent<Goo>().m_isUsed)
        {
            m_vaccum.enabled = true;
            m_vaccum.connectedBody = collision.GetComponent<Rigidbody2D>();
            Goo.GoToFinishLine = true;
            StartCoroutine(Suck());
        }
    }
    private void OnJointBreak2D(Joint2D joint)
    {
        Goo.GoToFinishLine = false;
        m_vaccum.connectedBody = null;
    }
    void Start()
    {
        m_vaccum = GetComponent<DistanceJoint2D>();
    }

    private IEnumerator Suck()
    {
        yield return null;
        m_vaccum.autoConfigureDistance = false;
        while(Goo.GoToFinishLine)
        {
            //m_vaccum
            if(m_vaccum.distance>1.2f)
                m_vaccum.distance -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }
        m_vaccum.autoConfigureDistance = true;
    }
    void Update()
    {
        
    }
}
