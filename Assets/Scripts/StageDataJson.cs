using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class StageDataJson : MonoBehaviour
{
    private const string stageClearDataPath = "Assets/stageClearData.json";
    private List<StageClearData> stageClearDataList;

    private void Start() 
    {
        LoadStageClearData();                                       // 1.게임 시작 시 클리어 데이터 불러옴
    }

    private void LoadStageClearData()
    {
        if (File.Exists(stageClearDataPath))
        {
            string jsonData = File.ReadAllText(stageClearDataPath); // 2. 파일에서 JSON 데이터 읽어옴
            stageClearDataList = JsonUtility.FromJson<List<StageClearData>>(jsonData); // 3. JSON을 List<StageClearData> 로 변환
        }
        else
        {
            InitializeStageClearData();                             //4. 클리어 데이터 파일이 없는 경우 초기화 수행
        }
    }

    private void SaveStageClearData()
    {
        string jsonData = JsonUtility.ToJson(stageClearDataList, true); // 5. List<StageClearData>를 JSON으로 변환
        File.WriteAllText(stageClearDataPath, jsonData);            // 6. JSON 데이터를 파일에 저장
    }

    private void InitializeStageClearData()
    {
        stageClearDataList = new List<StageClearData>();            // 7. 클리어 데이터 리스트 초기화

        // 초기화: 3개의 스테이지에 대한 클리어 정보 생성
        for (int i = 1; i <= 3; i++)
        {
            StageClearData newData = new StageClearData(i, false);  // 8. 초기에는 클리어되지 않은 상태로 설정
            stageClearDataList.Add(newData);                        // 9. 클리어 데이터를 리스트에 추가
        }

        SaveStageClearData();                                       // 10. 초기화한 데이터를 파일에 저장
    }

    public void ClearStage(int stageNumber)
    {
                                                                    // 11. 해당 스테이지의 클리어 정보 찾기
        StageClearData existingData = stageClearDataList.Find(data => data.stageNumber == stageNumber);

        if (existingData != null)
        {
            if (!existingData.isCleared)
            {
                existingData.isCleared = true;                      // 12. 클리어되지 않은 경우 클리어 상태를 업데이트
                SaveStageClearData();                               // 13. 변경된 클리어 정보를 파일에 저장

                Debug.Log($"Stage {stageNumber} cleared! Data saved.");
            }
            else
            {
                Debug.Log($"Stage {stageNumber} is already cleared.");
            }
        }
        else
        {
            Debug.LogError($"Stage {stageNumber} not found in data.");
        }
    }
    
}
