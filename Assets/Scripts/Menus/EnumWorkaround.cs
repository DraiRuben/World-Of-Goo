using UnityEngine;

[CreateAssetMenu]
public class EnumWorkaround : ScriptableObject
{
    //since onclick event can't take a function with an enum as parameter for some fucking reason
    public Difficulty Difficulty;
}
