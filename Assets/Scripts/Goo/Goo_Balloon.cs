using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Goo_Balloon : Goo
{
    [SerializeField]
    private float LiftStrength = 1.0f;


    public override IEnumerator DoThingIfUsed()
    {
        StartCoroutine(SetSelectableLate());
        while (m_isUsed)
        {
            m_rb.velocity += new Vector2(0, LiftStrength);
            //clamps x so the balloon doesn't go apeshit
            m_rb.velocity = new(Mathf.Clamp(m_rb.velocity.x, 0, 1), m_rb.velocity.y);
            yield return new WaitForFixedUpdate();
        }
    }
    public override void Use()
    {

        List<Goo> filteredAnchors = m_validAnchors.ToList();
        filteredAnchors.RemoveAll(x => x == null);
        for (int i = 0; i < filteredAnchors.Count; i++)
        {
            PlaceConnection(filteredAnchors, i);
        }
        m_animator.enabled = false;
        m_isUsed = true;
        m_isSelected = false;
        StartCoroutine(AdaptConnectionLength());
        StartCoroutine(DoThingIfUsed());
        m_buildAudio.Play();
    }

    private protected override void PlaceConnection(List<Goo> filteredAnchors, int i)
    {
        m_distanceJoints[i].connectedBody = filteredAnchors[i].m_rb;
        m_distanceJoints[i].enabled = true;
        if (m_connections.Count < i + 1)
        {
            m_connections.Add(filteredAnchors[i]);
            filteredAnchors[i].m_connections.Add(this);
        }
        else
        {
            m_connections[i] = filteredAnchors[i];
            filteredAnchors[i].m_connections.Add(this);
        }
        GameObject connection = Instantiate(m_connectionPrefab, transform.position, Quaternion.identity, transform);
        connection.GetComponent<Connection>().IsInUse = true;
        connection.transform.parent = transform;
        connection.GetComponent<Connection>().m_target = filteredAnchors[i];
    }
    //already explicit enough, this just lengthens the string until the desired value
    private IEnumerator AdaptConnectionLength()
    {
        while (Mathf.Abs(m_distanceJoints[0].distance - m_maxAttachDistance) > 0.4f)
        {
            m_distanceJoints[0].distance += 2 * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
}
