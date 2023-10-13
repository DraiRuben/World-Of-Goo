using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    [SerializeField]
    private Vaccum m_vaccum;
    [SerializeField]
    private BoxCollider2D m_detectionCollider;

    private Coroutine m_coroutine;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //suck closest goo from structure if close enough and tell remaining goos to come to the exit
        if (collision.CompareTag("Goo") && !Goo.s_goToFinishLine && collision.GetComponent<Goo>().m_isUsed && collision.GetComponent<Goo_Balloon>() == null)
        {
            m_vaccum.m_magnet.enabled = true;
            m_vaccum.m_finishGoo = collision.gameObject;
            m_coroutine ??= StartCoroutine(TryEndLevel());
        }
        //suck goos that aren't on the structure and adds them to the score
        else if (collision.CompareTag("Goo") && Goo.s_goToFinishLine && !collision.GetComponent<Goo>().m_isUsed && !collision.GetComponent<Goo>().m_isSelected)
        {
            collision.GetComponent<Goo>().StartCoroutine(collision.GetComponent<Goo>().PlanDestruction());
        }
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Goo") && !Goo.s_goToFinishLine && collision.GetComponent<Goo>().m_isUsed && collision.GetComponent<Goo_Balloon>()==null)
        {
            m_vaccum.m_magnet.enabled = true;
            m_vaccum.m_finishGoo = collision.gameObject;
            m_coroutine ??= StartCoroutine(TryEndLevel());

        }
    }
    private IEnumerator TryEndLevel()
    {
        yield return new WaitForSeconds(2f); //if the structure stays in range for at least 2 sec, then it's fixed and we can trigger the level's end
        if (m_vaccum.m_magnet.enabled)
        {
            Goo.s_finishLineGoo = m_vaccum.m_finishGoo;
            Goo.s_goToFinishLine = true;
        }
        else
        {
            m_coroutine = null;
        }
    }


}
