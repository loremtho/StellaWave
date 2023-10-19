using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class GunController : MonoBehaviour
{
    //활성화여부
    public static bool isActivate= false;

    //현재 장착된 총
    [SerializeField]
    private Gun currentGun;

    private Enemy enemy; //몬스터 관리

    private float currentFireRate;

    //상태변수
    private bool isReload = false;
    [HideInInspector]
    public bool isFIneSightMode = false;

    //본래 포지션 값
    [SerializeField]
    private Vector3 originPos;
    private AudioSource audioSource;

    //레이저 충돌 정보 받아옴
    private RaycastHit hitlnfo;

    [SerializeField]
    private Camera theCam;
    private Crosshair theCrosshair;

    [SerializeField]
    private GameObject hit_effect;
    [SerializeField]
    private TrailRenderer BulletTrail;
    [SerializeField]
    private Transform BulletPos;

    public Rigidbody Bullet;
    public float BulletSpeed;

    public GameObject gunObject; //총 활성화 확인

    // Update is called once per frame
    private void Start()
    {
        //originPos = Vector3.zero;
        audioSource= GetComponent<AudioSource>();
        theCrosshair = FindObjectOfType<Crosshair>();
    }

    public void GunMounting()
    {
        isActivate= true;
    }


    void Update()
    {

        if (isActivate == true)
        {
            
            GunFireRateCalc();
            TryFire();
            TryReload();
            //TryFineSight();
        }
       
    }



    //연사속도 재계산
    private void GunFireRateCalc()
    {
        if (currentFireRate > 0)
            currentFireRate -= Time.deltaTime; 
    }

    private void TryFire()
    {
        if(Input.GetButton("Fire1") && currentFireRate <= 0 && !isReload)
        {
            
            if(gunObject.activeSelf)  //등록한 총만 발사 이후 수정필요 한개만 임시
            {
               Fire();
            }
          
        }
    }

    //발사전 계산
    private void Fire()
    {
        if(!isReload)
        {
            if (currentGun.currentBulletCount > 0)
                Shoot();
            else
            {
                //CancelFinSight();
                StartCoroutine(ReloadCoroutine());
            }
        }
    }

    //발사후 계산
    private void Shoot()
    {
        theCrosshair.FireAnimation();
        //currentGun.currentBulletCount--; //장전 필요하면 추가 ㄱㄱ 일단 무한 탄창
        currentFireRate = currentGun.fireRate; //연사속도 재계산
        PlaySE(currentGun.fire_Sound);
        currentGun.muzzleFlash.Play();
    

        Hit(); //다르게 할거면 오브젝트 폴링 쓰기
        //총기 반동 코루틴
        StopAllCoroutines();
        StartCoroutine(RetroActionCoroutine());

    }

    private void Calloffhit()
    {
        theCrosshair.offHit();
    }



    //맞는 판정
    private void Hit()
    {
    // 레이캐스트 방향 설정 임시 고정
        Vector3 raycastDirection = theCam.transform.forward/* +
            new Vector3(Random.Range(-theCrosshair.GetAccuracy() - currentGun.accuracy, theCrosshair.GetAccuracy() + currentGun.accuracy),
                       Random.Range(-theCrosshair.GetAccuracy() - currentGun.accuracy, theCrosshair.GetAccuracy() + currentGun.accuracy),
                        0)*/;
                        

        
        if (Physics.Raycast(theCam.transform.position, raycastDirection, out hitlnfo/*, currentGun.Range*/))
        {
            Debug.DrawLine(BulletPos.position, hitlnfo.point, Color.red);
   
            Enemy enemy = hitlnfo.collider.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(currentGun.damage);
            }
            var clone = Instantiate(hit_effect, hitlnfo.point, Quaternion.LookRotation(hitlnfo.normal));
            Destroy(clone, 0.7f);

            // 방향을 수정하지 않고 그대로 사용
            FireBullet(raycastDirection);
        }
    }

    public void hitreaction()
    {
        theCrosshair.onHit();
        Invoke("Calloffhit", 0.1f);
    }

    private void FireBullet(Vector3 shootDirection)   //투사체 새로 한것
    {
        Vector3 bulletSpawnPosition = BulletPos.position;
        Rigidbody rd = Instantiate(Bullet, bulletSpawnPosition, transform.rotation);
        rd.velocity = shootDirection.normalized * BulletSpeed;
        Destroy(rd.gameObject, 1.0f);
    }

    private void TryReload()
    {
        if(Input.GetKeyDown(KeyCode.R) && !isReload && currentGun.currentBulletCount < currentGun.reloadBulletCount)
        {
            //CancelFinSight();
            StartCoroutine(ReloadCoroutine());
        }
    }

    public void CanvelReload()
    {
        if(isReload)
        {
            StopAllCoroutines();
            isReload= false;
        }
    }
    //장전 코루틴
    IEnumerator ReloadCoroutine()
    {
        if(currentGun.currentBulletCount >= 0 )
        {
            isReload = true;
            //currentGun.anim.SetTrigger("Reload");

            currentGun.currentBulletCount += currentGun.currentBulletCount;
            currentGun.currentBulletCount = 0;

            yield return new WaitForSeconds(currentGun.reloadTime);

            if(currentGun.carryBulletCount >= currentGun.reloadBulletCount)
            {
                currentGun.currentBulletCount = currentGun.reloadBulletCount;
                currentGun.carryBulletCount -= currentGun.reloadBulletCount;
            }
            else
            {
                currentGun.currentBulletCount = currentGun.carryBulletCount;
                currentGun.carryBulletCount = 0;
            }

            isReload = false;
        }
        else
        {
            Debug.Log("소유한 총알이 없습니다.");
        }
    }

    //정조준
    /*private void TryFineSight()
    {
        if(Input.GetButtonDown("Fire2") && !isReload)
        {
            FineSight();
        }
    }

    public void CancelFinSight()
    {
        if (isFIneSightMode)
            FineSight();
            
    }*/

    //정조준 로직
    /*private void FineSight()
    {
        isFIneSightMode = !isFIneSightMode;
        currentGun.anim.SetBool("FineSightMode", isFIneSightMode);
        theCrosshair.FineSightAnimation(isFIneSightMode);
        
        if(isFIneSightMode )
        {
            StopAllCoroutines();
            StartCoroutine(FineSughtActivateCoroutine());

        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(FineSughtDeactivateCoroutine());
        }

    }*/

    //정조준 활성화
    /*IEnumerator FineSughtActivateCoroutine()
    {
        while(currentGun.transform.localPosition != currentGun.fineSightOriginPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.2f);
            yield return null;
        }
    }

    //정조준 비활성화
    IEnumerator FineSughtDeactivateCoroutine()
    {
        while (currentGun.transform.localPosition != originPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.2f);
            yield return null;
        }
    }*/

    //반동
    IEnumerator RetroActionCoroutine()
    {
        Vector3 recoilBack = new Vector3(currentGun.retroActionForce, originPos.y, originPos.z);
        Vector3 retroActionRecoilBack = new Vector3(currentGun.retroActionFineSightForce, currentGun.fineSightOriginPos.y, currentGun.fineSightOriginPos.z);
        
        if(!isFIneSightMode)
        {
            currentGun.transform.localPosition = originPos;

            //반동시작
            while(currentGun.transform.localPosition.x <= currentGun.retroActionForce -0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }

            //원위치

            while(currentGun.transform.localPosition != originPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.1f);
                yield return null;
            }


        }
        else
        {
            currentGun.transform.localPosition = currentGun.fineSightOriginPos;
            //반동시작
            while (currentGun.transform.localPosition.x <= currentGun.retroActionFineSightForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, retroActionRecoilBack, 0.4f);
                yield return null;
            }

            //원위치

            while (currentGun.transform.localPosition != currentGun.fineSightOriginPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.1f);
                yield return null;
            }
        }

    }

    //사운드
    private void PlaySE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }

    public Gun GetGun()
    {
        return currentGun;
    }

    public bool GetFineSightMode()
    {
        return isFIneSightMode;
    }

    public void GunChange(Gun _gun)
    {
        if(WeaponManager.currentWeapon!= null)
        {
            WeaponManager.currentWeapon.gameObject.SetActive(false);
        }
        currentGun = _gun;
        WeaponManager.currentWeapon = currentGun.GetComponent<Transform>();
        //WeaponManager.currentWeaponAnim = currentGun.anim;

        currentGun.transform.localPosition = Vector3.zero;
        currentGun.gameObject.SetActive(true);
        isActivate= true;
    }
}
