using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class GOArrayStorage : SerializableDictionary.Storage<List<GameObject>> { }

[Serializable]
public class GOGOArrayDictionary : SerializableDictionary<GameObject, List<GameObject>, GOArrayStorage>
{
    public GOGOArrayDictionary() { }

    public GOGOArrayDictionary(IDictionary<GameObject, List<GameObject>> dict) : base(dict)
    {
    }
}


[Serializable]
public class IndexDiffDictionary : SerializableDictionary<int, DifficultySettings> { }
