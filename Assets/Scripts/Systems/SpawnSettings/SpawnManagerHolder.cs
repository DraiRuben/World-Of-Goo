using UnityEngine;

public class SpawnManagerHolder : MonoBehaviour
{
    //kind of a useless script now
    public static SpawnManagerHolder instance;
    public SpawnManager manager;
    
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
