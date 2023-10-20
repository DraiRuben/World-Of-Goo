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
    private SpawnSettings m_toLaunchSettings;
    private List<Spawnable> m_toLaunch;
    [SerializeField]
    private Transform m_cannonTransform;
    [SerializeField]
    private bool m_flip = false;
    private void Awake()
    {
        m_toLaunch = new List<Spawnable>(m_toLaunchSettings.Spawn);
    }
    private void Start()
    {
        StartCoroutine(Shoot());
        Score.Instance.m_totalSpawnedGoos += m_toLaunchSettings.GetTotalGooAmount();
    }
    //basically just instantiate next goo in list and launches it at the angle the cannon is oriented, I won't be doing a line per line comment on this one
    private IEnumerator Shoot()
    {
        yield return new WaitForSeconds(m_intialWaitTime);
        while (m_toLaunch != null && m_toLaunch.Count > 0)
        {
            Spawnable toSpawn = m_toLaunch[0];

            GameObject goo = Instantiate(m_toLaunch[0].Object, transform.position, Quaternion.identity);
            Goo comp = goo.GetComponent<Goo>();
            comp.MoveOutOfStructure();

            Vector3 rotatedVector = (Quaternion.AngleAxis(m_flip ? -45 : 45, Vector3.forward) * m_cannonTransform.right);
            comp.m_rb.velocity = rotatedVector.normalized * m_shootStrength;
            toSpawn.Amount--;
            m_toLaunch[0] = toSpawn;
            if (toSpawn.Amount <= 0)
                m_toLaunch.RemoveAt(0);
            yield return new WaitForSeconds(m_shootCooldown);
        }
    }
}
