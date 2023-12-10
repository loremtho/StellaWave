using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class ButtonController : MonoBehaviour
{

    public GameObject ShopUI;

    private WeaponChanger weaponchanger; //무기 변경 스크립트

    public GameObject WeaponSelectionUI;

    public GameObject EndingUI;
    public GameObject PauseUI;
    public GameObject StoryImage;
    public GameObject SkipBtn;

    public GameObject BossRush;
    public CinemachineVirtualCamera CineCam = null;

    public bool isPause = false;
    [SerializeField]
    private string BtnSound;
    [SerializeField]
    private GameObject Bosszone;

    public GameManager gameManager;
    public Animator animator;
    private string animName = "CutScene";

    public GunController gunController;

    public PlayerController playerController;

    public GameObject TapTxt;



    void Start()
    {
        StartCoroutine(BtnAppear());
        StartCoroutine(storyImage());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            SoundManager.instance.PlaySE(BtnSound);
            gunController.noFire = true;
            playerController.axeSwingInProgress = true;
            inWeaponSelect();
        }
       
      

        if (Input.GetKeyDown(KeyCode.C))
        {
            SoundManager.instance.PlaySE(BtnSound);
            inEnding();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
           TapTxt.SetActive(false);
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            SoundManager.instance.PlaySE(BtnSound);
            TogglePause();
        }   
        if(Input.GetKeyDown(KeyCode.T))
        {
            //버튼 사운드 넣기
            Bosszone.SetActive(true);
        }
    }

    public void TogglePause()
    {
        // 일시정지 상태를 토글합니다.
        if (Time.timeScale == 0f)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    void PauseGame()
    {
        // 일시정지 UI를 활성화하고 게임 시간을 정지합니다.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameManager.PauseUI();
        isPause = true;
        PauseUI.SetActive(true);
        Time.timeScale = 0f;
    }

    void ResumeGame()
    {
        // 일시정지 UI를 비활성화하고 게임 시간을 재개합니다.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPause = false;
        PauseUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void inGame()
    {
        LoadingSceneManager.LoadScene("Stage1");
    }

    public void inGame2()
    {
        LoadingSceneManager.LoadScene("Stage2");
    }

    public void inGame3()
    {
        LoadingSceneManager.LoadScene("Stage3");
    }

    public void inBoss1()
    {
        LoadingSceneManager.LoadScene("BossRush1");
    }

    public void BossRushUI()
    {
        BossRush.SetActive(true);

    }




    public void inShop()
    {
        ShopUI.SetActive(true);
    }

    public void outShop()
    {
        ShopUI .SetActive(false);

    }

    public void inWeaponSelect()
    {
    
        SoundManager.instance.PlaySE(BtnSound);
        WeaponSelectionUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    public void outWeaponSelect()
    {
        WeaponSelectionUI .SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SelectionGun()
    {
        SoundManager.instance.PlaySE(BtnSound);
        weaponchanger.GunA();
        WeaponSelectionUI.SetActive(false);
    }
    public void SelectionGunB()
    {
        SoundManager.instance.PlaySE(BtnSound);
        weaponchanger.GunB();
        WeaponSelectionUI.SetActive(false);
    }

    public void SelectionAxe()
    {
        SoundManager.instance.PlaySE(BtnSound);
        weaponchanger.AxeA();
        WeaponSelectionUI .SetActive(false);
    }

     public void inEnding()
    {
        EndingUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameManager.LastUi();
    }

    public void outEnding()
    {
        EndingUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void HomeBtn()
    {
        SoundManager.instance.PlaySE(BtnSound);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainLobby");
    }

    public void BossHomeBtn()
    {
        SoundManager.instance.PlaySE(BtnSound);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainLobby");
    }

    public void RestartBtn()
    {
        SoundManager.instance.PlaySE(BtnSound);
        Time.timeScale = 1f;
        string curScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(curScene);
    }

    public void SkipButton()
    {
        SoundManager.instance.PlaySE(BtnSound);
        StoryImage.SetActive(false);
        StopCutScene(animator, animName, 100f);
    }

    IEnumerator BtnAppear()
    {
        yield return new WaitForSeconds(2f);
        if(SkipBtn != null)
        {
            SkipBtn.SetActive(true);
        }
    }

    IEnumerator storyImage()
    {
        yield return new WaitForSeconds(3f);
        if(StoryImage != null)
        {
            StoryImage.SetActive(true);
        }
    }

    void StopCutScene(Animator anim, string Name, float Speed)
    {
        int animHash = Animator.StringToHash(Name);

        //애니메이션 정보 가져오기
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        while(!stateInfo.IsName(Name))
        {
            anim.Play(animHash, -1, 0f);
            anim.Update(0f);
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        anim.speed = 100f;
    }
    
}
