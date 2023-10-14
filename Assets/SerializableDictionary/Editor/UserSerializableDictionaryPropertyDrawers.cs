using UnityEditor;
[CustomPropertyDrawer(typeof(IndexDiffDictionary))]
[CustomPropertyDrawer(typeof(GOGOArrayDictionary))]
public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }



[CustomPropertyDrawer(typeof(GOArrayStorage))]
public class AnySerializableDictionaryStoragePropertyDrawer : SerializableDictionaryStoragePropertyDrawer { }
