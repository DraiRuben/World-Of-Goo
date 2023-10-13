using UnityEngine;
using UnityEngine.UI;

public class SongPlayer : MonoBehaviour
{
    [SerializeField]
    private Slider VolumeSlider;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        if (PlayerPrefs.HasKey("MainVolume"))
        {
            float Volume = PlayerPrefs.GetFloat("MainVolume");
            GetComponent<AudioSource>().volume = Volume;
            VolumeSlider.value = Volume;
        }
    }
    public void ChangeVolume()
    {
        GetComponent<AudioSource>().volume = VolumeSlider.value;
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("MainVolume", VolumeSlider.value);
    }
}
