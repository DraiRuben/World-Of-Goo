using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class ButtonClickSound : MonoBehaviour
{
    [SerializeField]
    private AudioClip m_clickSound;

    private AudioSource m_audioSource;
    // Start is called before the first frame update
    private void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_audioSource.clip = m_clickSound;
    }
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(m_audioSource.Play);
    }
}
