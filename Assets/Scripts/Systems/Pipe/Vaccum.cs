using UnityEngine;

public class Vaccum : MonoBehaviour
{
    public PointEffector2D m_magnet;
    public GameObject m_finishGoo;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == m_finishGoo)
        {
            Goo.s_goToFinishLine = false;
            m_finishGoo = null;
            m_magnet.enabled = false;
        }
    }
}
