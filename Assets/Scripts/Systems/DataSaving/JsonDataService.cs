using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonDataService : IDataService
{
    public T LoadData<T>(string RelativePath)
    {
        string path = Application.persistentDataPath + "/" + RelativePath + ".json";
        if (!File.Exists(path))
        {
            Debug.LogError($"Cannot load file at {path}. File does not exist !");
            throw new FileNotFoundException($"{path} does not exist");
        }
        try
        {
            T data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
            throw e;
        }
    }

    public bool SaveData<T>(string RelativePath, T Data)
    {
        string path = Application.persistentDataPath +"/"+ RelativePath + ".json";

        try
        {
            if (File.Exists(path))
            {
                Debug.Log("Data Exists. Deleting old file and writing a new one !");
                File.Delete(path);
            }
            else
            {
                Debug.Log("Writing file for the first time");
            }
            using FileStream stream = File.Create(path);
            stream.Close();
            File.WriteAllText(path, JsonConvert.SerializeObject(Data,Formatting.Indented));
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Unable to save data due to : {e.Message} {e.StackTrace}");
            return false;
        }
    }
}
