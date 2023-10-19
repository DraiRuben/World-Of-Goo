using UnityEngine;
using UnityEngine.UI;

public class SongPlayer : MonoBehaviour
{
    [SerializeField]
    private Slider m_volumeSlider;

    public static SongPlayer instance;
    private AudioSource m_audioSource;

    private float m_volume;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        m_audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        if (PlayerPrefs.HasKey("MainVolume"))
        {
            m_volume = PlayerPrefs.GetFloat("MainVolume");
            m_audioSource.volume = m_volume;
            m_volumeSlider.value = m_volume*5;
        }
    }
    public void ChangeVolume()
    {
        m_volume = m_volumeSlider.value*0.25f;
        m_audioSource.volume = m_volume;
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("MainVolume", m_volume);
    }
}
