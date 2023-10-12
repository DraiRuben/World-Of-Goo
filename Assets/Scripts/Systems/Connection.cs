using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    public GameObject m_target;
    public bool m_isInUse = false;
    public float ScaleY = 0.3f;
    //origin is parent
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (m_isInUse)
        {
            transform.localScale = new(Vector2.Distance(transform.position, m_target.transform.position),ScaleY, 1);
            float angle =Mathf.Atan2(m_target.transform.position.y- transform.position.y , m_target.transform.position.x- transform.position.x ) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0,0,angle);
        }
        else if (transform.parent != Pooling.Instance.transform)
        {
            m_target = null;
            transform.parent = Pooling.Instance.transform;
            transform.localPosition = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = new(1, ScaleY, 1);
        }

    }
}
