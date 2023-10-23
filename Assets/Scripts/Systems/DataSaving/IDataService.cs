using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//followed a tutorial for this
public interface IDataService
{
    bool SaveData<T>(string RelativePath, T Data);
    T LoadData<T>(string RelativePath);
}
