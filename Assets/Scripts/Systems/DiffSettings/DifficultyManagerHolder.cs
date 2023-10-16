using UnityEngine;

public class DifficultyManagerHolder : MonoBehaviour
{
    public static DifficultyManagerHolder instance;
    public DifficultyManager manager;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
