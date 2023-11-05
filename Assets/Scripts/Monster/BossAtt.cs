using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossAtt : Boss
{
    public GameObject missile;
    public GameObject StraightMissile;
    public Transform missilePortA;
    public Transform missilePortB;
    public GameObject childChildObject;
    private Animator childChildAnimator;

    public float throwForce = 10f; // 직선 미사일 속도

    public Transform StraighmissilePortA;
    public Transform StraighmissilePortB;

    public GameObject missileeffect;
    Vector3 lookvec;


    Vector3 tauntVec;
    public bool isLook;

    // Start is called before the first frame update
    void Awake()
    {
        childChildAnimator = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();
        meshs = GetComponentsInChildren<MeshRenderer>();

        nav.isStopped = true;
        StartCoroutine(Think());
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(isDead)
        {
            StopAllCoroutines();
            return;
        }

        if(isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input. GetAxisRaw("Vertical");
            lookvec = new Vector3(h, 0, v )* 5f;
            transform.LookAt(target.position + lookvec);
        }
        else
        nav.SetDestination(tauntVec);
    }

    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);

        int ranAction = Random.Range(0,7);
        switch(ranAction)
        {
           
              case 0:
              case 1:
              //유도 미사일
              StartCoroutine(MissileShot());

            break;
              case 2:
              case 3:
               //기본
                StartCoroutine(Taunt());
                

            break;
             case 4:
             case 5:
             //순간이동
               StartCoroutine(Dive());
               
               
            break;
             case 6:
             case 7:
              // 직선 미사일
             StartCoroutine(StraightMissileShot());
             

             break;
        }

    }

    IEnumerator MissileShot() //유도 미사일
    {
        missileeffect.SetActive(true);
        anim.SetTrigger("isAttack"); //임시 
        //childChildAnimator.SetTrigger("Open");
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        MonsterBullet bossMissileA = instantMissileA.GetComponent<MonsterBullet>();
        bossMissileA.target =target;
        MonsterBullet monsterBullet = instantMissileA.GetComponent<MonsterBullet>();
        monsterBullet.statusController = statusController;

         yield return new WaitForSeconds(0.3f);
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        MonsterBullet bossMissileB = instantMissileB.GetComponent<MonsterBullet>();
        bossMissileB.target =target;
        MonsterBullet monsterBullet2 = instantMissileB.GetComponent<MonsterBullet>();
        monsterBullet2.statusController = statusController;
        

        yield return new WaitForSeconds(2f);

        missileeffect.SetActive(false);

        StartCoroutine(Think());
    }

     IEnumerator Taunt() //패턴 추가 시 늘리기
    {
        tauntVec = target.position + lookvec;

        isLook = false;
        nav.isStopped = false;
        //boxCollider.enabled = false;
            
        //anim.SetTrigger("isShot");
        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
        //boxCollider.enabled = true;

        StartCoroutine(Think());
    }

      IEnumerator Dive() //패턴 추가 시 늘리기
    {
         Vector3 teleportPosition = target.position + Vector3.forward *10f; // 단위만큼 이동
        isLook = false;
        nav.isStopped = false;
        boxCollider.enabled = false;
        nav.Warp(teleportPosition);
         
        isLook = true; 

        MissileShot(); //근접 공격시 변경

        //anim.SetTrigger("isShot");
        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
        //boxCollider.enabled = true;

        StartCoroutine(Think());
    }


    IEnumerator StraightMissileShot() //직선 미사일
    {
        GameObject straighA = Instantiate(StraightMissile, StraighmissilePortA.position, StraighmissilePortA.rotation);
         // 플레이어 방향 벡터 계산
        Vector3 throwDirectionA = (target.position - StraightMissile.transform.position).normalized;

        // 플레이어 방향으로 힘을 적용.
        Rigidbody rbA = straighA.GetComponent<Rigidbody>();
        rbA.AddForce(throwDirectionA * throwForce, ForceMode.Impulse);

        GameObject straighB = Instantiate(StraightMissile, StraighmissilePortA.position, StraighmissilePortA.rotation);
         // 플레이어 방향 벡터 계산
        Vector3 throwDirectionB = (target.position - StraightMissile.transform.position).normalized;

        // 플레이어 방향으로 힘을 적용.
        Rigidbody rbB = straighB.GetComponent<Rigidbody>();
        rbB.AddForce(throwDirectionB * throwForce, ForceMode.Impulse);
    

        yield return new WaitForSeconds(2f);

        StartCoroutine(Think());
    }




}
