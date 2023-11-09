using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//저장하는 방법
//1. 저장할 데이터 존재
//2. 데이터를 제이슨으로 변환
//3. 제이슨을 외부로 저장

//불러오는 방법
//1. 외부에 저장된 제이슨을 가져옴
//2. 제이슨을 데이터형태로 변환
//3. 불러온 데이터를 사용

[System.Serializable]
public class StellaData
{
    public int StellaCoin;
    public string Something;
    public int Something2;
}

public class DataJson : MonoBehaviour
{
    public static DataJson instance;
    StellaData stellaData = new StellaData();

    string path;
    string filename = "save";

    private void Awake() //싱글톤
    {
        if(instance == null)
        {
            instance = this;
        }    
        else if(instance != null)
        {
            Destroy(instance.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);

        path = Application.persistentDataPath + "/";
    }

    private void Start() {
        string data  = JsonUtility.ToJson(stellaData);

        File.WriteAllText(path + filename, data);
    }
}

    

