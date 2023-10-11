using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    public GameObject Target;
    public bool IsInUse = false;
    //origin is parent
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (IsInUse)
        {
            transform.localScale = new(0.3f, Vector2.Distance(transform.position, Target.transform.position), 1);
            float angle = Mathf.Atan2(transform.position.z, transform.position.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else if (transform.parent != Pooling.Instance.transform) 
            transform.parent = Pooling.Instance.transform;
        
    }
}
