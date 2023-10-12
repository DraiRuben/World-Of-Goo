using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{

    private DistanceJoint2D m_vaccum;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //suck closest goo from structure if close enough and tell remaining goos to come to the exit
        if (collision.CompareTag("Goo") && !Goo.s_goToFinishLine && collision.GetComponent<Goo>().m_isUsed && collision.GetComponent<Goo_Balloon>() == null)
        {
            m_vaccum.enabled = true;
            m_vaccum.connectedBody = collision.GetComponent<Rigidbody2D>();
            Goo.s_finishLineGoo = collision.gameObject;
            Goo.s_goToFinishLine = true;
            StartCoroutine(Suck());
        }
        //suck goos that aren't on the structure and adds them to the score
        else if (collision.CompareTag("Goo") && Goo.s_goToFinishLine && !collision.GetComponent<Goo>().m_isUsed)
        {
            collision.gameObject.SetActive(false);
            Score.Instance.m_score++;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Goo") && !Goo.s_goToFinishLine && collision.GetComponent<Goo>().m_isUsed && collision.GetComponent<Goo_Balloon>()==null)
        {
            m_vaccum.enabled = true;
            m_vaccum.connectedBody = collision.GetComponent<Rigidbody2D>();
            Goo.s_finishLineGoo = collision.gameObject;
            Goo.s_goToFinishLine = true;
            StartCoroutine(Suck());
        }
        else if(collision.CompareTag("Goo") && Goo.s_goToFinishLine && !collision.GetComponent<Goo>().m_isUsed)
        {
            collision.gameObject.SetActive(false);
            Score.Instance.m_score++;
        }
    }
    private void OnJointBreak2D(Joint2D joint)
    {
        Goo.s_goToFinishLine = false;
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
        while(Goo.s_goToFinishLine)
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
