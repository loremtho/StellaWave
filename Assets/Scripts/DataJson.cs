using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

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
}

public class DataJson : MonoBehaviour
{
    public static DataJson instance;
    public CoinData coinData;
    private const string coinDataPath = "Assets/coinData.json";

    [ContextMenu("To Json Data")]
    void SaveCoinDataToJson()
    {
        string jsonData = JsonUtility.ToJson(coinData,true);
        File.WriteAllText(coinDataPath, jsonData);
    }

    [ContextMenu("From Json Data")]
    void LoadCoinDataFromJson()
    {
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
        LoadData();
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
        Debug.Log("데이터가 저장되었습니다.");
    }

    public void ClearStageGiveCoin()
    {
        coinData.Coin += 200;
        SaveData();
        Debug.Log("스테이지 클리어! 코인 : " + coinData.Coin);
    }

    public void ClearBossCoin()
    {
        coinData.Coin += 500;
        SaveData();
        Debug.Log("보스스테이지 클리어! 코인 : " + coinData.Coin);
    }

    public void ClearBossRushCoin()
    {
        coinData.Coin += 1000;
        SaveData();
        Debug.Log("보스러쉬 스테이지 클리어! 코인 : " + coinData.Coin);
    }
}

    

