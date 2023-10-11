using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


[Serializable]
public class GOArrayStorage : SerializableDictionary.Storage<List<GameObject>> {}

[Serializable]
public class GOGOArrayDictionary : SerializableDictionary<GameObject, List<GameObject>, GOArrayStorage> {}
