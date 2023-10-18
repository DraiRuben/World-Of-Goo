using UnityEngine;
using UnityEngine.Events;

public class Connection : MonoBehaviour
{
    public Goo m_target;
    private UnityEvent returnToPoolingSystem = new();
    public bool m_isInUse = false;
    public bool m_IsInUse { get { return m_isInUse; } set { if(!value && m_isPreviewer && m_isInUse) returnToPoolingSystem.Invoke(); m_isInUse = value; } }
    public bool m_isPreviewer = false;
    public float ScaleY = 0.3f;
    //origin is parent
    // Start is called before the first frame update
    private void Awake()
    {
        returnToPoolingSystem.AddListener(ReturnToPool);
    }
    // Update is called once per frame
    void Update()
    {
        if (m_isInUse)
        {
            transform.localScale = new(Vector2.Distance(transform.position, m_target.transform.position)/transform.parent.localScale.x, ScaleY, 1);
            float angle = Mathf.Atan2(m_target.transform.position.y - transform.position.y, m_target.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    private void ReturnToPool()
    {
        m_target = null;
        transform.parent = Pooling.Instance.transform;
        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = new(1, ScaleY, 1);
    }
}
