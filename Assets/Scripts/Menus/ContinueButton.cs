using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ContinueButton : MonoBehaviour
{

    void Start()
    {
        if (!PlayerPrefs.HasKey("LastPlayedLevel"))
        {
            GetComponent<Button>().interactable = false;
        }
        else
        {
            GetComponent<Button>().onClick.AddListener(
                () => SceneChanger.instance.ChangeSceneWithIndex(PlayerPrefs.GetInt("LastPlayedLevel")));
        }
    }

}
