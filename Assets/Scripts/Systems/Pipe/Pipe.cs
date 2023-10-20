using System.Collections;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

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
        if (collision.CompareTag("Goo") && collision.GetComponent<Goo>().m_isUsed && collision.GetComponent<Goo_Balloon>() == null)
        {
            if (!Goo.s_goToFinishLine)
            {
                m_vaccum.ActivateVaccuum();
                m_vaccum.m_finishGoo = collision.gameObject;
                m_coroutine ??= StartCoroutine(TryEndLevel());
            }
            else //if we detected a new target
            {
                PathFinder.Instance.SetClosenessToExit(collision.GetComponent<Goo>(), 0);
            }
        }
        //suck goos that aren't on the structure and adds them to the score
        else if (collision.CompareTag("Goo") && Goo.s_goToFinishLine && !collision.GetComponent<Goo>().m_isUsed && !collision.GetComponent<Goo>().m_isSelected)
        {
            collision.GetComponent<Goo>().Absorb();
        }
    }
    //same thing as previous one, useful only because trigger enter doesn't detect a goo that spawns directly in the collider
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Goo") && collision.GetComponent<Goo>().m_isUsed && collision.GetComponent<Goo_Balloon>() == null)
        {
            if (!Goo.s_goToFinishLine)
            {
                m_vaccum.ActivateVaccuum();
                m_vaccum.m_finishGoo = collision.gameObject;
                m_coroutine ??= StartCoroutine(TryEndLevel());
            }
            else //if we detected a new target
            {
                PathFinder.Instance.SetClosenessToExit(collision.GetComponent<Goo>(), 0);
            }


        }
    }
    //disable vaccuum if the goo in range disappears
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Goo")
            && Goo.s_goToFinishLine
            && collision.GetComponent<Goo>().m_isUsed
            && collision.GetComponent<Goo_Balloon>() == null
            && collision.gameObject == m_vaccum.m_finishGoo)
        {
            m_vaccum.m_finishGoo = null;
            Goo.s_goToFinishLine = false;
            m_vaccum.DisableVaccuum();

        }
    }
    private IEnumerator TryEndLevel()
    {
        //if the structure stays in range for at least 0.2 sec, then it's fixed and we can trigger the level's end
        yield return new WaitForSeconds(0.2f);
        if (m_vaccum.m_magnet.enabled)
        {
            NextLevel.instance.Appear();
            Goo comp = m_vaccum.m_finishGoo.GetComponent<Goo>();
            PathFinder.Instance.SetClosenessToExit(comp, 0);
            //comp.m_rb.constraints = RigidbodyConstraints2D.FreezeAll;
            Goo.s_goToFinishLine = true;
        }
        m_coroutine = null;
    }


}
