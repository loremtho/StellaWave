using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
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


    //상태 변수
    private bool isWalk = false;
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = true;
    public bool aim = true;

    

    private bool isInGunMode = false;

    private bool isInAxeMode = false;

    private bool isDefaultmode = true;

    public bool playergun = false;  //총을 들었을떄 모드

    public bool playeraxe = false; //도끼 모드
    private bool axeSwingInProgress = false; //도끼가 나가는 중인지 판단

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

    public int score = 0;
    public int hitscore = 0;
    public int killcount = 0;

    float timeBetweenClicks = 1f; // 연속 공격 시간
    float lastClickTime = 0f;

    private float earlyspeed;

    public int needskillpoint;

    public GameObject swordskilleffect;

    [SerializeField] private Slider SkillSlider;


    

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
        TryCrrouch();
        Move();
        if(SkillSlider != null)
        {
            SkillUpdate();

        }
       
        if(!Inventory.inventoryActivated)
        {
            CameraRotation();
            CharacterRotation();
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
            if(!axeSwingInProgress && axeobject.activeSelf && isGround)
            {
                
                if(Input.GetButton("Fire1"))
                {  
                 
                    myRigid.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | (myRigid.constraints & RigidbodyConstraints.FreezeRotation);
                    Playeranim.SetTrigger("Axemode_Swing");
                    SetDefaultMode(true);
                    axeSwingInProgress = true;
                   
                    StartCoroutine(DisableAxeSwing());
                    
                }

                if(hitscore == 100)
                {
                    if(Input.GetKey(KeyCode.Q))
                    {
                        myRigid.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | (myRigid.constraints & RigidbodyConstraints.FreezeRotation);
                        //이펙트 + 애니메이션 적용  
                        Playeranim.SetTrigger("sword_skills");
                        swordskilleffect.SetActive(true);
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
        yield return new WaitForSeconds(1.8f); // 대기
        myRigid.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ);

        yield return new WaitForSeconds(0.5f); // 대기
        axeSwingInProgress = false; // 후에 false로 설정
        
    }

    private IEnumerator DisableAxeSkillSwing()
    {
        yield return new WaitForSeconds(3f); // 대기
        myRigid.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ);

        yield return new WaitForSeconds(0.5f); // 대기
        swordskilleffect.SetActive(false);
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

            Playeranim.SetBool("Gunmode", true);
            Playeranim.SetBool("Idle", false);
        }
        else
        {
            Playeranim.SetBool("Gunmode", false);
             
        }
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

    private void TryRun() //달리기 시도
    {
        if(Input.GetKey(KeyCode.LeftShift) && theStatusController.GetCurrentSP() > 0)
        {
            Running();
            
            if(playergunActive)
            {
                gunObject.SetActive(false);
            }
            /*else if(playergunActive)
            {
                gunObject[1].SetActive(false);
            }*/
            else if(playeraxeActice)
            {
                axeobject.SetActive(false);
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
            /*else if(playergunActive)
            {
                SetGunMode(true);
                gunObject[1].SetActive(true);
               
            }*/
            else if(playeraxeActice)
            {
                SetAxeMode(true);
                axeobject.SetActive(true);
               
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

/*
    private void Move() //플레이어 이동
    {
        if(playergun)
        {
            Playeranim.SetBool("Gunmode_walk" , true);
        }
        else
        {
            Playeranim.SetBool("Walk" , true);
        }
      
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right* _moveDirX; 
        Vector3 _moveVertical = transform.forward* _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized *applySpeed; 

        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    } 
     private void MoveCheck()
    {
        if (!isRun && !isCrouch && isGround)
        {
            if (Vector3.Distance(lastPos, transform.position) >= 0.01f)
            {
                isWalk = true;
                if(isGround)
                {
                   
                }
            
            }
            else
            {
                isWalk = false;
           
                 
            }

            if(!isWalk && isGround)
            {
                if(playergun)
                 {
                    Playeranim.SetBool("Gunmode_walk" , false);
                }
                else
                {
                    Playeranim.SetBool("Walk" , false);
                }
             }
             

            theCrosshair.WalkingAnimation(isWalk);
            lastPos = transform.position;
        }
       
    }
*/

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

    private void CameraRotation() //카메라 시점 설정
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;

        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);

        gunPivot.rotation = Quaternion.Euler(currentCameraRotationX, transform.eulerAngles.y, 0f);
        axePivot.rotation = Quaternion.Euler(currentCameraRotationX, transform.eulerAngles.y, 0f);
        //gunPivot.position = rightHandMount.position;
    }

    public void damageprocess()
    {
        Playeranim.SetTrigger("Hit");
    }


    
}
