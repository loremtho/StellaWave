using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{

    public GameObject ShopUI;

    private WeaponChanger weaponchanger; //무기 변경 스크립트

    public GameObject WeaponSelectionUI;

    public GameObject EndingUI;
    public GameObject PauseUI;
    public GameObject StoryImage;
    public GameObject SkipBtn;

    public bool isPause = false;
    [SerializeField]
    private string BtnSound;

    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BtnAppear());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            SoundManager.instance.PlaySE(BtnSound);
            inWeaponSelect();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            SoundManager.instance.PlaySE(BtnSound);
            inEnding();
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            SoundManager.instance.PlaySE(BtnSound);
            TogglePause();
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

    public void RestartBtn()
    {
        SoundManager.instance.PlaySE(BtnSound);
        Time.timeScale = 1f;
        string curScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(curScene);
        //SceneManager.LoadScene("Stage1");
    }

    public void SkipButton()
    {
        SoundManager.instance.PlaySE(BtnSound);
        StoryImage.SetActive(false);
    }

    IEnumerator BtnAppear()
    {
        yield return new WaitForSeconds(10f);
        if(SkipBtn != null)
        {
            SkipBtn.SetActive(true);
        }
    }
    
}
