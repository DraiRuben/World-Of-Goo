using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Cursor : MonoBehaviour
{
    public static Cursor instance;
    private RectTransform m_cursorTransform;
    private Goo m_selectedGoo;
    private Image m_image;
    private Coroutine m_rotateRoutine;
    [SerializeField]
    private float m_rotationSpeed = 1f;
    [SerializeField]
    private Sprite m_dragging;
    [SerializeField]
    private Sprite m_idle;

    private bool m_animate = true;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        m_cursorTransform = GetComponent<RectTransform>();
        m_image = GetComponent<Image>();
    }
    private void Start()
    {
        StartCoroutine(FollowMouse());
        StartCoroutine(Animation());
    }
    private void Update()
    {
        if (Time.timeScale != 0)
        {
            TryGetSelectable();

        }
        else
        {
            m_selectedGoo = null;
        }
    }
    public void UpdateSizeWithZoom(float orthoSize)
    {
        float newScale = 10 / orthoSize;
        m_cursorTransform.localScale = new Vector3(newScale, newScale, newScale);
    }
    private void TryGetSelectable()
    {

        RaycastHit2D Hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), 5000, LayerMask.GetMask("Goo", "UI"));
        if (Hit.collider != null)
        {
            var comp = Hit.collider.GetComponent<Goo>();

            if (!Goo.s_isThereAGooSelected)
            {
                if (comp != null && (!comp.m_isUsed || comp.m_isReusable))
                {
                    m_selectedGoo = comp;
                }
            }
            else
            {
                if (comp.m_isSelected)
                {
                    m_selectedGoo = comp;
                }
            }
        }//reset target only if we don't have anything selected, since when moving the mouse,
         //the selected goo takes a bit of time to follow so the raycast may not hit sometimes
        else if (!Goo.s_isThereAGooSelected)
        {
            m_selectedGoo = null;

        }


    }
    private IEnumerator FollowMouse()
    {
        while (true)
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
    private IEnumerator Animation()
    {
        float scaleOffset = 0;
        while (m_animate)
        {
            
            if (m_selectedGoo == null || !m_selectedGoo.m_isSelected)
            {
                m_cursorTransform.rotation = Quaternion.identity;
                scaleOffset = Mathf.PingPong(Time.time*0.5f, 0.3f);
                m_image.pixelsPerUnitMultiplier = 0.9f + scaleOffset;
            }
            else
            {
                m_cursorTransform.rotation = Quaternion.Euler(0, 0, m_cursorTransform.rotation.eulerAngles.z+ m_rotationSpeed);
                m_image.pixelsPerUnitMultiplier = 1f;

            }
            yield return null;

        }
        m_cursorTransform.rotation = Quaternion.identity;
        m_image.pixelsPerUnitMultiplier = 1f;

    }
}
