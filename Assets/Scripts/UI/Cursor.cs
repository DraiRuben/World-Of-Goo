using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    public static Cursor instance;
    private RectTransform m_cursorTransform;
    private Goo m_selectedGoo;

    [SerializeField]
    private float m_rotationSpeed = 5f;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        m_cursorTransform = GetComponent<RectTransform>();
    }
    private void Start()
    {
        StartCoroutine(FollowMouse());
    }
    private void Update()
    {
        TryGetSelectable();
    }
    public void UpdateSizeWithZoom(float orthoSize)
    {
        float newScale = 10 / orthoSize;
        m_cursorTransform.localScale = new Vector3(newScale, newScale, newScale);
    }
    private void TryGetSelectable()
    {
        RaycastHit2D Hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), 5000, LayerMask.GetMask("Goo"));
        if (Hit.collider != null)
        {
            var comp = Hit.collider.GetComponent<Goo>();
            if(comp != null && (!comp.m_isUsed || comp.m_isReusable)) 
            {
                m_selectedGoo = comp;
            }
        }
        else if (!Goo.s_isThereAGooSelected)
        {
            m_selectedGoo = null;

        }
    }
    private IEnumerator FollowMouse()
    {
        while(true)
        {
            if (m_selectedGoo == null)
            {
                m_cursorTransform.anchoredPosition = Input.mousePosition;
                yield return null;
            }
            else
            {
                m_cursorTransform.anchoredPosition = Camera.main.WorldToScreenPoint(m_selectedGoo.m_rb.position);
                yield return null;
            }

        }
    }
    private IEnumerator RotateAnimation()
    {
        while (true)
        {
            m_cursorTransform.rotation =  Quaternion.Euler(0, 0, 1*m_rotationSpeed);
            yield return null;
        }
    }
}
