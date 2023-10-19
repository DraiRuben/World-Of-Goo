using UnityEngine;

public class DifficultyManagerHolder : MonoBehaviour
{
    //kind of a useless script now
    public static DifficultyManagerHolder instance;
    public DifficultyManager manager;
    
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
