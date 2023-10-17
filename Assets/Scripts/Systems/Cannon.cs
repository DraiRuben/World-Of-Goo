using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField]
    private float m_intialWaitTime = 2f;
    [SerializeField]
    private float m_shootCooldown = 2f;
    [SerializeField]
    private float m_shootStrength = 20f;
    [SerializeField]
    private List<Spawnable> m_toLaunch;

    private void Start()
    {
        StartCoroutine(Shoot());
    }
    private IEnumerator Shoot()
    {
        yield return new WaitForSeconds(m_intialWaitTime);
        while (m_toLaunch != null && m_toLaunch.Count > 0)
        {
            Spawnable toSpawn = m_toLaunch[0];

            GameObject goo = Instantiate(m_toLaunch[0].Object, transform.position, Quaternion.identity);
            Goo comp = goo.GetComponent<Goo>();
            comp.MoveOutOfStructure();
            Vector3 rotatedVector = (Quaternion.AngleAxis(45, Vector3.forward) * transform.right);
            comp.m_rb.velocity = rotatedVector.normalized * m_shootStrength;
            toSpawn.Amount--;
            m_toLaunch[0] = toSpawn;
            if (toSpawn.Amount <= 0)
                m_toLaunch.RemoveAt(0);
            yield return new WaitForSeconds(m_shootCooldown);
        }
    }
}
