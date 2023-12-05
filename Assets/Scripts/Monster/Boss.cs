using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Redcode.Pools;
using Unity.Mathematics;
using UnityEngine.UI;

public class Boss : MonoBehaviour 
{
    public enum Type {A, B, C};
    public Type enemyType;
   
    public Transform target;

    public GameObject lasertw;

    public ParticleSystem muzzleFlashs;

    public BoxCollider meleeArea;

    public bool isAttack;

    public Animator anim;

    public bool isChase;

    private PlayerController player;

    public GameManager gameManager;

    public StatusController statusController;

   
    [SerializeField]
    private int Hp;

    private int currentHp;

    [SerializeField]
    
    private string BoomS;

    [SerializeField]
    
    private string BoomL;

    public GunController gunController;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public NavMeshAgent nav;

    public MeshRenderer[] meshs;

    public bool isDead = false;

    [Header("체력")]
    [SerializeField] Transform hpbar;
    public GameObject hpslider;

    [SerializeField] private Slider healthSlider;

    private bool hasBeenHit = false;

    public GameObject[] Dieeffect;

    public GameObject Lastdieeffect;

    public GameObject endingUi;

  


    void FreezeVelocity()
    {
        if(isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
 
    }


    void FixedUpdate()
    {
        FreezeVelocity();
        Targerting();

    }

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        currentHp = Hp;
    }



    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();
        meshs = GetComponentsInChildren<MeshRenderer>();
         
      
        if(enemyType != Type.C)
        Invoke("ChaseStart", 2);
  
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);


    }

    void Update()
    {
     
        if(nav.enabled && enemyType !=Type.C)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }

    }

    public void UpdateHealth()
    {
        float healthPercentage = (float)currentHp / (float)Hp; // 체력 비율 계산
        healthSlider.value = healthPercentage; // 슬라이더 업데이트
    }

    void Targerting()
    {
        /*
        float targetRadius = 1.5f;
        float targetRange = 0;

        switch (enemyType){
            case Type.A :
            targetRadius = 1.5f;
            targetRange = 3f;
            break;
            case Type.B :
            targetRadius = 1.5f;
            targetRange = 6f;
            break;
            case Type.C:
            targetRadius = 3f;
            targetRange = 20f;
            
            break;
        }

        RaycastHit[] rayHits =
        Physics.SphereCastAll(transform.position,
        targetRadius,
        transform.forward,
        targetRange,
        LayerMask.GetMask("Player"));
        

        if(rayHits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }
        */



    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);
        //SoundManager.instance.PlaySE(monsterAtt);

        switch(enemyType) {
            case Type.A:
            yield return new WaitForSeconds(0.2f);
            meleeArea.enabled = true;

            yield return new WaitForSeconds(0.5f);
            meleeArea.enabled = false;

            yield return new WaitForSeconds(0.5f);
            isChase = false;

            break;

            case Type.B:

            break;

            case Type.C:
            yield return new WaitForSeconds(0.2f);
            meleeArea.enabled = false;

            yield return new WaitForSeconds(0.5f);
            meleeArea.enabled = false;

            yield return new WaitForSeconds(0.5f);
            isChase = false;

            break;
        }
    

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isAttack) // 플레이어 태그와 공격 중인지 확인
        {
            statusController.DecreaseHP(100);
        }

        if(other.CompareTag("BaseCamp"))
        {
            TakeDamage(currentHp);
        }

        if(other.CompareTag("Skill") && !hasBeenHit)
        {
            hasBeenHit = true;
            TakeDamage(player.SwordslasheDamege);
            StartCoroutine(ResetHitStatusAfterDelay(0.5f));
        }
    }

    IEnumerator ResetHitStatusAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 피격 상태 초기화
        hasBeenHit = false;
    }


    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        isChase = false;
        Debug.Log(currentHp);
        UpdateHealth();

        /*foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.red;
        }

        if(currentHp >0)
        {
            foreach(MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.white;
            }

        }*/
        //player.AddHitScore(20);
        gunController.hitreaction();

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private IEnumerator ResumeChaseAfterDelay(float delay)
    {
        anim.SetTrigger("Hit");
        yield return new WaitForSeconds(delay);
        isChase = true;
    }





    private void Die()
    {
        isDead = true;
        isChase = false;
        isAttack = false;
        nav.enabled = false;

        StopAllCoroutines();

        
        boxCollider.enabled = false;
        if(lasertw != null)
        {
            lasertw.SetActive(false);
        }
        anim.SetTrigger("Die");
        //SoundManager.instance.PlaySE(monsterDie);
        StartCoroutine(Diecheck(10));
       
    }

     private IEnumerator Diecheck(int dietime)
    {
        for (int i = 0; i < Dieeffect.Length; i++)
        {
            Dieeffect[i].SetActive(true);
            yield return new WaitForSeconds(0.5f);
             SoundManager.instance.PlaySE(BoomS);
        }

         yield return new WaitForSeconds(1f);
         Lastdieeffect.SetActive(true);
         SoundManager.instance.PlaySE(BoomL);


        yield return new WaitForSeconds(dietime);
        Destroy(gameObject);
        endingUi.SetActive(true);
         



    }




 
}
