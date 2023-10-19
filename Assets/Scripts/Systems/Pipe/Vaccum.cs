using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Vaccum : MonoBehaviour
{
    [HideInInspector]
    public PointEffector2D m_magnet;
    [HideInInspector]
    public GameObject m_finishGoo;
    private SpriteRenderer m_spriteRenderer;

    private AudioSource m_audioSource;

    [SerializeField]
    private AudioClip m_vaccuumBegin;
    [SerializeField]
    private AudioClip m_vaccuumEnd;
    [SerializeField]
    private AudioClip m_vaccuum;
    private void Awake()
    {
        m_magnet = GetComponent<PointEffector2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == m_finishGoo)
        {
            Goo.s_goToFinishLine = false;
            m_finishGoo = null;
            m_magnet.enabled = false;
        }
    }
    private void Update()
    {
        if(NextLevel.instance.m_isInWaitScreen && m_magnet.enabled)
            DisableVaccuum();
        else if (!NextLevel.instance.m_isInWaitScreen && Time.timeScale==0)
            m_audioSource.Pause();
        else if(m_magnet.enabled && Time.timeScale==1 && !m_audioSource.isPlaying)
            m_audioSource.UnPause();
    }
    public void ActivateVaccuum()
    {
        m_magnet.enabled = true;
        m_spriteRenderer.enabled = true;
        StartCoroutine(Suck());
    }
    public void DisableVaccuum()
    {
        m_magnet.enabled = false;
        m_spriteRenderer.enabled = false;
        m_audioSource.clip = m_vaccuumEnd;
        m_audioSource.loop = false;
        m_audioSource.Play();
    }
    private IEnumerator Suck()
    {
        m_audioSource.clip = m_vaccuumBegin;
        m_audioSource.loop = false;
        m_audioSource.Play();
        yield return new WaitWhile(isPlaying);
        if (m_magnet.enabled)
        {
            m_audioSource.clip = m_vaccuum;
            m_audioSource.loop = true;
            m_audioSource.Play();
        }
    }
    private bool isPlaying() { return m_magnet.enabled&&m_audioSource.isPlaying; }
}
