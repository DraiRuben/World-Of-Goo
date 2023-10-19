using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeButton : MonoBehaviour
{
    [SerializeField]
    private Sprite m_full;
    [SerializeField]
    private Sprite m_mute;
    private bool m_isMuted;

    private Image m_currentSprite;
    [SerializeField]
    private Slider m_slider;

    private float m_volumeBeforeMute;
    private void Awake()
    {
        m_currentSprite = GetComponent<Image>();
    }
    private void Start()
    {
        m_isMuted = m_slider.value == 0;
        if (m_isMuted)
            m_currentSprite.sprite = m_mute;

        m_slider.onValueChanged.AddListener(delegate { UpdateSprite(); });
    }
    public void Click()
    {
        m_isMuted = !m_isMuted;
        m_currentSprite.sprite = !m_isMuted && m_volumeBeforeMute!=0 ? m_full:m_mute;
        if(m_isMuted)
            m_volumeBeforeMute = m_slider.value;
        m_slider.value = m_isMuted ?0:m_volumeBeforeMute;
        m_slider.onValueChanged.Invoke(0);
       
    }
    public void UpdateSprite()
    {
        if (m_slider.value == 0)
        {
            m_currentSprite.sprite = m_mute;
        }
        else
        {
            m_currentSprite.sprite = m_full;
            m_isMuted = false;
        }
    }
}
