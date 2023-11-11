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
public class CoinData
{
    public int Coin;
    public int Medal;
}

public class DataJson : MonoBehaviour
{
    public static DataJson instance;
    public CoinData coinData;
    private const string coinDataPath = "Asset/coinData.json";

    [ContextMenu("To Json Data")]
    void SaveCoinDataToJson()
    {
        string jsonData = JsonUtility.ToJson(coinData,true);
        //string path = Path.Combine(Application.dataPath, "coinData.json");
        File.WriteAllText(coinDataPath, jsonData);
    }

    [ContextMenu("From Json Data")]
    void LoadCoinDataFromJson()
    {
        //string path = Path.Combine(Application.dataPath, "coinData.json");
        string jsonData = File.ReadAllText(coinDataPath);
        coinData = JsonUtility.FromJson<CoinData>(jsonData);
    }

    private void Awake() 
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(instance.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        //LoadData();
        print(coinDataPath);
    }

    public void LoadData()
    {
        if(File.Exists(coinDataPath))
        {
            string jsonData = File.ReadAllText(coinDataPath);
            coinData = JsonUtility.FromJson<CoinData>(jsonData);
        }
        else //파일이 없을 경우 초기화
        {   
            coinData = new CoinData();
            SaveData();
        }
    }

    public void SaveData()
    {
        string jsonData = JsonUtility.ToJson(coinData,true);
        File.WriteAllText(coinDataPath, jsonData);
    }

    public void ClearStage()
    {
        coinData.Coin += 200;
        SaveData();
        Debug.Log("스테이지 클리어! 코인 : " + coinData.Coin);
    }
}

    

