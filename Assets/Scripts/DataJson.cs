using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1. 데이터(코드 = 클래스)를 만들어야함 => 저장할 데이터 생성
// 2. 그 데이터를 Json으로 변환

public class DataJson : MonoBehaviour
{
    public StellaData stellaData;
}
[System.Serializable]
public class StellaData
{
    public int StellaCoin;
    public string Something;
    public int Something2;
}
