using UnityEngine;
using UnityEngine.UI;

public class SongPlayer : MonoBehaviour
{
    [SerializeField]
    private Slider m_volumeSlider;

    private float m_volume;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        if (PlayerPrefs.HasKey("MainVolume"))
        {
            m_volume = PlayerPrefs.GetFloat("MainVolume");
            GetComponent<AudioSource>().volume = m_volume;
            m_volumeSlider.value = m_volume*5;
        }
    }
    public void ChangeVolume()
    {
        m_volume = m_volumeSlider.value*0.25f;
        GetComponent<AudioSource>().volume = m_volume;
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("MainVolume", m_volume);
    }
}
