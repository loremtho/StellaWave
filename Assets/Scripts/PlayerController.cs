using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Transform gunPivot; //총 배치의 기준점
    public Transform axePivot; //도끼 배치의 기준점
   
    //public Transform rightHandMount;  //오른손 위치
    //스피드 조정
    [SerializeField]    
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    private float applySpeed;
    [SerializeField]
    private float crouchSpeed;
    [SerializeField] 
    private float swimSpeed;
    [SerializeField]
    private float swimFastSpeed;
    [SerializeField]
    private float upSwimSpeed;
    [SerializeField]
    private float jumpForce;

    [SerializeField]
    
    private string Skillsound;

    [SerializeField]
    private string Skillvoicesound;



    //상태 변수
    private bool isWalk = false;
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = true;
    public bool aim = true;
    public bool MouseRotation = false;

    

    private bool isInGunMode = false;

    private bool isInAxeMode = false;

    private bool isDefaultmode = true;

    public bool playergun = false;  //총을 들었을떄 모드

    public bool playeraxe = false; //도끼 모드
    public bool axeSwingInProgress = false; //도끼가 나가는 중인지 판단

    private bool axeSwingcombo = false;

    private bool playergunActive = false; //달리거나 총이 순간 필요없을때 사용

    private bool playeraxeActice = false;
   
    //움직임 체크 변수
    private Vector3 lastPos;
    
    public Animator Playeranim;

    public AudioClip footstepSound; // Inspector에서 할당할 플레이어 걸음 소리
    public AudioClip RunSound;

    private AudioSource audioSource;

    



    //앉았을때 얼마나 앉을지

    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyCrouchPosY;

    private CapsuleCollider capsuleCollider;

    //민감도
    [SerializeField]
    private float lookSensitivity;

    //카메라 안계
    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX = 0;

    //필요한 컴포넌트
    [SerializeField]
    private Camera theCamera;
    private Rigidbody myRigid;
    private GunController theGunController;
    private Crosshair theCrosshair;
    private StatusController theStatusController;

    private WeaponChanger weaponChanger;


    public GameObject gunObject; //총 활성화 확인

    public GameObject axeobject; //도끼 활성화 확인

    public GameObject swordobject; // 검 활성화 확인

    public int score = 0;
    public int hitscore = 0;
    public int killcount = 0;

    float timeBetweenClicks = 1f; // 연속 공격 시간
    float lastClickTime = 0f;

    private float earlyspeed;

    /// ////////////////////////근접
    public int needskillpoint;

    public GameObject swordskilleffect;

    public GameObject swordslasheffect;
    
    public GameObject swordeffect;

    public GameObject swordcomboeffect;

    public float slasheeffectSpeed;

    [SerializeField] private Slider SkillSlider;

    public Transform effectTransform; //슬레쉬 이펙트 초기 위치

    public int SwordslasheDamege;
    public int SwordDamege;


    /// ///////////////////////
   


    public CraftManual craftManual;
    public CamManager camManager;
    public ButtonController buttonController;
    private string CurrentSceneName;


    public void AddScore(int points) //플레이어 점수 추가
    {
        score += points;
    }

    public void AddHitScore(int points) //플레이어 점수 추가
    {
        if(hitscore != needskillpoint)
        {
            hitscore += points;
        }
    }
    
       public void AddKillcount(int points) //플레이어 점수 추가
    {
        killcount += points;
    }

    public void SkillUpdate()
    {
        float SkillPercentage = (float)hitscore/(float)needskillpoint ; // 비율 계산
        SkillSlider.value = SkillPercentage;
    }
    

    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid= GetComponent<Rigidbody>();
        theGunController = FindObjectOfType<GunController>();
        theCrosshair = FindObjectOfType<Crosshair>();
        theStatusController = FindObjectOfType<StatusController>();
        Playeranim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        //초기화
        applySpeed = walkSpeed;
        earlyspeed = applySpeed;
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;
        hitscore = 0;

        string CurrentSceneName = SceneManager.GetActiveScene().name; // 현재 씬 이름 받아오기
     
    }
    
    void FreezeRotation()
    {
        myRigid.angularVelocity = Vector3.zero;
    }


    void FixedUpdate()
    {
        FreezeRotation();

    }

    // Update is called once per frame
    void Update()
    {
        WaterCheck();
        IsGround();
        TryJump();
        TryRun();
        //TryCrrouch();
        Move();
        if(SkillSlider != null)
        {
            SkillUpdate();
        }
        if(!Inventory.inventoryActivated && camManager.isAnimEnd && !buttonController.isPause)
        {
            CameraRotation();
            CharacterRotation();
        }

        if(!camManager.isAnimEnd)
        {
            if(CurrentSceneName == "Stage1") //스테이지 1 일때만 이 위치에 고정
            {
                this.transform.position = new Vector3(-131.28f, 1.035501f, -31.306f);
            }
            else
            return;
        }
        //MoveCheck();

        if (gunObject.activeSelf)
        {
            // 오브젝트가 활성화되면 GunMode로 변경
            SetGunMode(true);
            
        }
        else if(axeobject.activeSelf)
        {
            SetAxeMode(true);
        }
        else
        {
            SetDefaultMode(true);
        }

         if (Input.GetKey(KeyCode.LeftShift)&& theStatusController.GetCurrentSP() > 0)
        {
            if (!audioSource.isPlaying)
             {
                audioSource.PlayOneShot(RunSound);
             }
        }
        else
        {
            // 키를 떼면 소리를 중지
            audioSource.Stop();
        }


    }


    private void SetAxeMode(bool enableAxeMode)
    {
        isInAxeMode = enableAxeMode;
       

        if (isInAxeMode)
        {
            playeraxe = true;
            playeraxeActice = true;
            playergun = false;
            playergunActive = false;
            Playeranim.SetBool("Gunmode", false);
            //도끼 idle 애니메이션 쓸거면 추가
            if(!axeSwingInProgress && axeobject.activeSelf && isGround &&!craftManual.Stopattak)
            {
                
                if(Input.GetButton("Fire1"))
                {  
                 
                    myRigid.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | (myRigid.constraints & RigidbodyConstraints.FreezeRotation);
                    Playeranim.SetTrigger("Axemode_Swing");
                    SetDefaultMode(true);
                    swordeffect.SetActive(true);
                    axeSwingInProgress = true;
                    axeSwingcombo = true;
                   
                    StartCoroutine(DisableAxeSwing());
                    
                }

                if(hitscore == needskillpoint)
                {
                    if(Input.GetKey(KeyCode.Q))
                    {
                        myRigid.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | (myRigid.constraints & RigidbodyConstraints.FreezeRotation);
                        //이펙트 + 애니메이션 적용  
                        Playeranim.SetTrigger("sword_skills");
                        swordskilleffect.SetActive(true);
                        SoundManager.instance.PlaySE(Skillsound);
                        SoundManager.instance.PlaySE(Skillvoicesound);
                        Vector3 playerCameraForward = effectTransform.forward;
                        swordslasheffect.transform.position = effectTransform.position;
                        swordslasheffect.transform.forward = playerCameraForward;
                        swordslasheffect.SetActive(true);
                        swordslasheffect.GetComponent<Rigidbody>().velocity = transform.forward * slasheeffectSpeed;

                        StartCoroutine(DisableAxeSkillSwing());
                        hitscore = 0;
                        axeSwingInProgress = true;

                    }
                }

            }
        }
        else
        {
            //도끼 idle 취소
            
        }

    }

    private IEnumerator DisableAxeSwing()
    {
        yield return new WaitForSeconds(0.2f);
        swordeffect.SetActive(false);

        yield return new WaitForSeconds(0.45f); // 대기

        if(Input.GetButton("Fire1") && axeSwingcombo)
        {  
            myRigid.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | (myRigid.constraints & RigidbodyConstraints.FreezeRotation);
            Playeranim.SetTrigger("Axemode_Swing2");
            SetDefaultMode(true);
            axeSwingInProgress = true;
            axeSwingcombo = false;

            yield return new WaitForSeconds(0.1f);
            swordcomboeffect.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            swordcomboeffect.SetActive(false);
        }

        if(axeSwingcombo)
        {
            yield return new WaitForSeconds(0.65f);
            myRigid.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ);
            axeSwingInProgress = false; // 후에 false로 설정
            yield break;
        }

        yield return new WaitForSeconds(1f);
        myRigid.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ);
        axeSwingInProgress = false; // 후에 false로 설정
        
    }


    private IEnumerator DisableAxeSkillSwing()
    {
        yield return new WaitForSeconds(2.6f); // 대기
        myRigid.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ);

        yield return new WaitForSeconds(0.1f); // 대기
        swordskilleffect.SetActive(false);
        swordslasheffect.SetActive(false);

        axeSwingInProgress = false; // 후에 false로 설정
        
    }



    private void SetGunMode(bool enableGunMode)  //총모드
    {
        isInGunMode = enableGunMode;


        if (isInGunMode)
        {
            playergun = true;
            playergunActive = true;
            playeraxe = false;
            playeraxeActice = false;

            if(!axeSwingInProgress && isGround)
            {
                if(hitscore == needskillpoint)
                {
                    if(Input.GetKey(KeyCode.Q))
                    {
                        gunObject.SetActive(false);
                        swordobject.SetActive(true);

                        myRigid.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | (myRigid.constraints & RigidbodyConstraints.FreezeRotation);
                          //이펙트 + 애니메이션 적용  
                        Playeranim.SetTrigger("sword_skills");
                        swordskilleffect.SetActive(true);
                        SoundManager.instance.PlaySE(Skillsound);
                        SoundManager.instance.PlaySE(Skillvoicesound);
                        Vector3 playerCameraForward = effectTransform.forward;
                        swordslasheffect.transform.position = effectTransform.position;
                        swordslasheffect.transform.forward = playerCameraForward;
                        swordslasheffect.SetActive(true);
                        swordslasheffect.GetComponent<Rigidbody>().velocity = transform.forward * slasheeffectSpeed;
                        Playeranim.SetBool("Idle", true);

                        StartCoroutine(DisableGunSkillSwing());
                        hitscore = 0;
                        axeSwingInProgress = true;

                    }
                }


            }
            Playeranim.SetBool("Gunmode", true);
            Playeranim.SetBool("Idle", false);



        }
        else
        {
            Playeranim.SetBool("Gunmode", false);
             
        }
    }

     private IEnumerator DisableGunSkillSwing() //검스킬 후처리
    {
        
        yield return new WaitForSeconds(2.4f); // 대기
        swordobject.SetActive(false);
        myRigid.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ);
        yield return new WaitForSeconds(0.5f); // 대기
       
        swordskilleffect.SetActive(false);
        swordslasheffect.SetActive(false);
        gunObject.SetActive(true);
        Playeranim.SetBool("Gunmode", true);
        axeSwingInProgress = false; // 후에 false로 설정
    }

    public void playerRigidOff()
    {
        myRigid.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY);
    }


    private void SetDefaultMode(bool enableDefaultMode) //기본 모드
    {
        isDefaultmode = enableDefaultMode;


        if (isDefaultmode)
        {
            Playeranim.SetBool("Idle", true);
         
        }

    }

    

    private void WaterCheck()
    {
        if(GameManager.isWater)
        {
            if(Input.GetKeyDown(KeyCode.LeftShift))
            {
                applySpeed = swimFastSpeed;
            }
            applySpeed = swimSpeed;
        }
    }
    
    private void TryCrrouch() //앉는 시도
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    private void Crouch()  //앉기 동작
    {
        isCrouch = !isCrouch;
        theCrosshair.CrouchingAnimation(isCrouch);
        if (isCrouch)
        {
            applySpeed = crouchSpeed;
            //applyCrouchPosY = crouchPosY;
        }
        else
        {
            applySpeed = walkSpeed;
            //applyCrouchPosY = originPosY;
        }

        StartCoroutine(CrouchCoroutine());
       
    }

    IEnumerator CrouchCoroutine()  //자연스럽게 앉기
    {
        float _posY = theCamera.transform.localPosition.y;
        int count = 0;


        while(_posY != applyCrouchPosY)
        {
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.3f);
            theCamera.transform.localPosition = new Vector3(0, _posY, 0);
            if (count > 15)
                break;
            yield return null;
        }
        theCamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0f);
     
    }

    public void TryRun() //달리기 시도
    {
        if(Input.GetKey(KeyCode.LeftShift) && theStatusController.GetCurrentSP() > 0)
        {
            Running();
            
            if(playergunActive)
            {
                gunObject.SetActive(false);
            }
            else if(playeraxeActice)
            {
                axeobject.SetActive(false);
                swordobject.SetActive(false);
            }
            

        }
        if(Input.GetKeyUp(KeyCode.LeftShift) || theStatusController.GetCurrentSP() <= 0)
        {
            RunningCancel();
            
            if(playergunActive)
            {
                SetGunMode(true);
                gunObject.SetActive(true);
               
            }
            else if(playeraxeActice)
            {
                SetAxeMode(true);
                axeobject.SetActive(true);
                swordobject.SetActive(true);
               
            }

        }
    }

    private void IsGround() //점프전 바닥 체크
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.001f);
        theCrosshair.JumpingAnimation(!isGround);

    }
    private void TryJump() //점프 시도
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGround && theStatusController.GetCurrentSP() > 0 && !GameManager.isWater)
            Jump();
        else if(Input.GetKey(KeyCode.Space) && GameManager.isWater )
            UpSwim();
    }

    private void UpSwim()
    {
        myRigid.velocity = transform.up * upSwimSpeed;
    }

    private void Jump() //점프
    {
        if (isCrouch)//않은 상태에서 점프로 해제
        {
            Crouch();
        } 

        Playeranim.SetTrigger("Jump");   
        theStatusController.DecreaseStamina(0);
        myRigid.velocity = transform.up * jumpForce;

    
    }



    

    private void Running()
    {
        if (isCrouch) //않은 상태에서 달리기로 해제
            Crouch();

        //theGunController.CancelFinSight();


        isRun = true;
        theCrosshair.RunningAnimation(isRun);
        if(isGround)
        {
            Playeranim.SetBool("Player_Run", true);
          
        }
        theStatusController.DecreaseStamina(0);
        applySpeed = runSpeed;
       
        }

    //달리기 취소
    private void RunningCancel()
    {
        isRun = false;
        theCrosshair.RunningAnimation(isRun);
        applySpeed = walkSpeed;
        if(isGround)
        {
            Playeranim.SetBool("Player_Run", false);
            
        }
     
    }
    private void Move() //신규 이동
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        // 이동 입력에 따라 애니메이션 설정
        if (playergun)
        {
            Playeranim.SetBool("Gunmode_walk", _moveDirX != 0 || _moveDirZ != 0);
            //SoundManager.instance.PlaySE(Run_sound);
        }
        else
        {
            Playeranim.SetBool("Walk", _moveDirX != 0 || _moveDirZ != 0);
            
        }

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                if (!audioSource.isPlaying)
                {
                    //audioSource.PlayOneShot(footstepSound);
                }
            }
            else
            {
                //audioSource.Stop();
            }
    }


    private void CharacterRotation() //좌우 캐릭터 회전
    {
        float _yRotation = Input.GetAxis("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));

    }

    public void CameraRotation() //카메라 시점 설정
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;

        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);

      
        gunPivot.rotation = Quaternion.Euler(currentCameraRotationX, transform.eulerAngles.y, 0f);
        axePivot.rotation = Quaternion.Euler(currentCameraRotationX, transform.eulerAngles.y, 0f);
        //gunPivot.position = rightHandMount.position;

        //Playeranim.SetFloat("CameraRotationX", currentCameraRotationX);
        
    }

    public void damageprocess()
    {
        Playeranim.SetTrigger("Hit");
        StartCoroutine(afterhit());
    }

    private IEnumerator afterhit()
    {
          if(playergunActive)
            {
                gunObject.SetActive(false);
            }
            else if(playeraxeActice)
            {
                axeobject.SetActive(false);
                swordobject.SetActive(false);
            }
        yield return new WaitForSeconds(1f);

           if(playergunActive)
            {
                SetGunMode(true);
                gunObject.SetActive(true);
               
            }
            else if(playeraxeActice)
            {
                SetAxeMode(true);
                axeobject.SetActive(true);
                swordobject.SetActive(true);
               
            }


    }

    public void PlayerDie()
    {
        StartCoroutine(Playerdie());
    }

    private IEnumerator Playerdie()
    {
        myRigid.constraints = RigidbodyConstraints.FreezePositionX |
                            RigidbodyConstraints.FreezePositionY |
                            RigidbodyConstraints.FreezePositionZ;
        capsuleCollider.enabled = false;
        Playeranim.SetTrigger("Die");
        yield return new WaitForSeconds(5f);
        Time.timeScale = 0f;

    }


    
}
