using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public static class TransformExtensions //플레이어 자식 오브젝트 탐색 클래스
{
    
    public static Transform FindDeepChild(this Transform parent, string name)
    {
        Transform result = parent.Find(name);
        if (result != null)
            return result;
        foreach (Transform child in parent)
        {
            result = child.FindDeepChild(name);
            if (result != null)
                return result;
        }
        return null;
    }
}

public class GameManager : MonoBehaviour
{

    //상태 변수
    public static bool isPause = false; // 메뉴가 호출되면 true; //변경
    public bool isBattle;
    public static bool isNight = false;
    public static bool isWater = false;
    private bool flag = false;
    bool isStop;

    //인게임 UI***********************
    public int stage = 20; //난이도 설정

    public TextMeshProUGUI stageTxtTMP;
    public TextMeshProUGUI scoreTxtTMP;
    public TextMeshProUGUI playTimeTxtTMP;
    public TextMeshProUGUI killcountTxtTMP;
    public float playTime;
    public int wavecount = 1; //현재 스테이지

    private int bosscount = 0;
    float timer;
    //클리어 캔버스 UI*****************

    public TextMeshProUGUI LastscoreTxt;
    public TextMeshProUGUI LastplayTimeTxt;
    public TextMeshProUGUI LastkillcountTxt;
    public float LastplayTime;

    //일시정지 캔버스 UI****************

    public TextMeshProUGUI pauseStageTxt;
    public TextMeshProUGUI pausescoreTxt;
    public TextMeshProUGUI pauseplayTimeTxt;
    public TextMeshProUGUI pausekillcountTxt;
    public float pauseplayTime;

    //실패 캔버스 UI****************

    public TextMeshProUGUI failStageTxt;
    public TextMeshProUGUI failscoreTxt;
    public TextMeshProUGUI failplayTimeTxt;
    public TextMeshProUGUI failkillcountTxt;
    public float failplayTime;

    //키 설명 UI****************************

    public GameObject waponui;

    public GameObject wapontxt;

    //***********스테이지 정보 관리*********************

    private int stageNumber = 1;
    public TextMeshProUGUI Cointext;
    private bool isCoin = false;
   
   //********************************
    public GameObject StartZone; //스테이지 게임 시작존 관리

    public GameObject Bosszone; //보스존 관리

    //몬스터 관리***********************
    public Transform[] enemyZone; 
    public GameObject[] enemies;
    public List<int> enemyList;

    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;
    public int enemyCntD;

    //배터리 스폰 관리*******************

    public GameObject Battery_prefab;
    public Transform[] battery_trans;
    private List<GameObject> Battery_prefab_Ins = new List<GameObject>();
    private List<int> emptySpawnPoints = new List<int>();

    //*********************************
    
    //필요한 컴포넌트
    public PlayerController player;
    private WeaponManager theWM;
    public BaseCamp baseCamp;

    public Transform baseCamps;

    public GunController gunController;

    public WeaponChanger weaponchanger; //무기 변경 스크립트

    public ButtonController buttonController;
    public StatusController statusController;
    private DataJson dataJson;
    private StageDataJson stageDataJson;
    

    [SerializeField]
    private string bgm;

    [SerializeField]
    private string StageStartSound;

    void Awake()
    {
        Application.targetFrameRate = 60;
        enemyList = new List<int>();
    }


    void Start()
    {
        theWM = FindObjectOfType<WeaponManager>();
        baseCamp = GetComponent<BaseCamp>();
        BatteryRespawner(); // 게임 시작시 배터리 스폰시킴
        SoundManager.instance.PlayBGM(bgm); 
        dataJson = FindObjectOfType<DataJson>();
        stageDataJson = FindObjectOfType<StageDataJson>();
        isCoin = false;
        //weaponchanger.GunA(); //플레이어 무기 타입 결정 임시
        
    }

    public void StageStart() //스테이지 시작시 원하는 오브젝트 비활성화 
    {
        StartZone.SetActive(false);
        SoundManager.instance.PlaySE(StageStartSound);
        foreach(Transform zone in enemyZone) //게임 시작시 스폰 활성
        zone.gameObject.SetActive(true);
        isBattle = true;
        StartCoroutine(InBattle());

    }

    public void StageEnd()  //스테이지 끝난후 활성화
    {
        //플레이어 .transform.position = Vector3.up * 0.8f; 를 통해서 스테이지 끝난후 위치 바꾸는것도 가능

        StartZone.SetActive(true);

        foreach(Transform zone in enemyZone) //게임 시작시 스폰 비활성
        zone.gameObject.SetActive(false);
        BatteryRespawner();
        isBattle = false;
        //baseCamp.EndDecreaseBaseHP();
        wavecount++;
        stage += 1;

    }

    IEnumerator InBattle()
    {
    
        if(wavecount % 1 == 0) //보스
        {
            bosscount++;

            for(int i =0; i< bosscount; i++)
            {

                /*
                enemyCntD++;
                GameObject instantEnemy = Instantiate(enemies[3], enemyZone[0].position, enemyZone[0].rotation);
                Enemy enemy = instantEnemy.GetComponent<Enemy>();
                enemy.target = baseCamps.transform;
                enemy.gameManager = this;
                enemy.statusController = statusController;
                enemy.gunController = gunController;
                */
           

            }
        
        }
        for(int index = 0; index < stage; index++)
        {
            int ran = Random.Range(0 ,3); //존 개수 몬스터 늘릴시 갯수 수정
            enemyList.Add(ran);

            switch (ran) {
                case 0:
                enemyCntA++;
                break;
                case 1:
                enemyCntB++;
                break;
                case 2:
                enemyCntC++;
                break;
     
            }


        }
        while(enemyList.Count > 0)
        {
            int ranZone = Random.Range(0, 4); //몬스터 늘릴시 갯수 수정
            GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZone[ranZone].position, enemyZone[ranZone].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = baseCamps.transform;
            enemy.gameManager = this;
            enemy.statusController = statusController;
            enemy.gunController = gunController;
          
          
            enemyList.RemoveAt(0);
            
            yield return new WaitForSeconds(0.7f);
         } 

         while(enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0)
         {
            yield return null;
         }

        yield return new WaitForSeconds(1f);

        StageEnd();


    }

   
    void Update()
    {
        if(waponui.activeSelf)
        {
            wapontxt.SetActive(false);
        }

        if(isWater)
        {
            if(!flag)
            {
                StopAllCoroutines();
                StartCoroutine(theWM.WeaponInCoroutine());
                flag = true;
            }
        }
        else
        {
            if(flag)
            {
                flag = false;
                theWM.WeaponOut();
            }
        }
        if(isBattle)
        {
            playTime += Time.deltaTime;
        }
        WaveCheck();
        return;
    }


    void LateUpdate() 
    {
        stageTxtTMP.text = "Wave " + wavecount + " / 5";

        scoreTxtTMP.text = string.Format("Score : {0:n0}",player.score);

        killcountTxtTMP.text = string.Format("Kill : {0:n0}",player.killcount);

        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) /60);
        int second = (int)(playTime % 60);
        playTimeTxtTMP.text = string.Format("{0 : 00}", hour) + ":" + string.Format("{0 : 00}", min) + ":" + string.Format("{0 : 00}", second);
   
    }

    public void LastUi() //클리어시 출력 클리어 UI
    {
        LastscoreTxt.text = string.Format("Score : {0:n0}",player.score);

        LastkillcountTxt.text = string.Format("Kill : {0:n0}",player.killcount);

        Cointext.text = string.Format("Coin : {0:n0}", dataJson.coinData.Coin);

        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) /60);
        int second = (int)(playTime % 60);
        LastplayTimeTxt.text = string.Format("{0 : 00}", hour) + ":" + string.Format("{0 : 00}", min) + ":" + string.Format("{0 : 00}", second);

    }
    public void PauseUI()
    {
        pauseStageTxt.text = "Wave " + wavecount + " / 5";
        
        pausescoreTxt.text = string.Format("Score : {0:n0}",player.score);

        pausekillcountTxt.text = string.Format("Kill : {0:n0}",player.killcount);

        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) /60);
        int second = (int)(playTime % 60);
        pauseplayTimeTxt.text = string.Format("{0 : 00}", hour) + ":" + string.Format("{0 : 00}", min) + ":" + string.Format("{0 : 00}", second);
    }

    public void FailUI()
    {
        failStageTxt.text = "Wave " + wavecount + " / 5";
        
        failscoreTxt.text = string.Format("Score : {0:n0}",player.score);

        failkillcountTxt.text = string.Format("Kill : {0:n0}",player.killcount);

        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) /60);
        int second = (int)(playTime % 60);
        failplayTimeTxt.text = string.Format("{0 : 00}", hour) + ":" + string.Format("{0 : 00}", min) + ":" + string.Format("{0 : 00}", second);
    }

    public void WaveCheck()// 웨이브 상태 체크 함수
    {
        // 아니면 웨이브가 6에 진입되면 바로 클리어 시켜버려도 됨
        if(wavecount == 6 && !isCoin) 
        {
            isCoin = true;
            buttonController.inEnding();
            StartZone.SetActive(false);
            Bosszone.SetActive(true);
            dataJson.ClearStageGiveCoin();
            stageNumber++;
            stageDataJson.ClearStage(stageNumber);
        }
    }

    private void BatteryRespawner() //배터리 프리팹을 스폰시킴
    {
        int allSpawnPoint = battery_trans.Length;

        if(emptySpawnPoints.Count == 0 || emptySpawnPoints.Count == battery_trans.Length)
        {
            emptySpawnPoints.Clear();
            for (int i = 0; i < battery_trans.Length; i++)
            {
                emptySpawnPoints.Add(i);
            }
        }

        int BeSpawned = 0;

        for(int index = 0; index < stage; index++) //스테이지 마다 스폰시킴
        {
            foreach (Transform spawnPoint in battery_trans) //배열로 선언된 위치에 스폰함
            {
                var prefabInstance = Instantiate(Battery_prefab, spawnPoint.position, spawnPoint.rotation);
                Battery_prefab_Ins.Add(prefabInstance);
                BeSpawned++;
                if(BeSpawned >= emptySpawnPoints.Count)
                {   
                    emptySpawnPoints.Clear();
                    return;
                }
            }
        }
    }
}
