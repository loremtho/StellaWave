using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Redcode.Pools;
using UnityEngine.VFX;

public class Enemy : MonoBehaviour 
{
    public enum Type {A, B, C, D};
    public Type enemyType;

    private Vector3 lookvec;
    private Vector3 tauntVec;
    public bool isLook;
   
    public Transform target;
   
    public Rigidbody rigid;
    public BoxCollider boxCollider;

    public MeshRenderer[] meshs;

    public ParticleSystem muzzleFlashs;
    public GameObject bloodHit;
    public GameObject bloodDie;

    public BoxCollider meleeArea;

    public bool isAttack;
    private bool isDie = false;

    public Animator anim;

    public bool isChase;

    private PlayerController player;

    public GameManager gameManager;

    public StatusController statusController;

    public NavMeshAgent nav;

   
    [SerializeField]
    private int Hp;

    private int currentHp;

    [SerializeField]
    
    private string monsterAtt;

    [SerializeField]
    
    private string monsterDie;
     [SerializeField]
    
    private string monsterBlood;

    public GunController gunController;
    public bool isDead = false;


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

        //if(enemyType != Type.D)
        Invoke("ChaseStart", 1);
       
        currentHp = Hp;

    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);

    }

    void Update()
    {
       
        nav.SetDestination(target.position);
        nav.isStopped = !isChase;
        /*
        if(isDie)
        {
            StartCoroutine(Diecheck(2));
        }
        */

         if(isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input. GetAxisRaw("Vertical");
            lookvec = new Vector3(h, 0, v )* 5f;
            transform.LookAt(target.position);
        }

    }

    void Targerting()
    {
       
            float targetRadius = 1.5f;
            float targetRange = 3f;

            switch (enemyType){
                case Type.A :
                    targetRadius = 1.5f;
                    targetRange = 3f;
                    break;
            case Type.B :
                    targetRadius = 1.5f;
                    targetRange = 3f;
                    break;
            case Type.D:
                    targetRadius = 1.5f;
                    targetRange = 3f;
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

        

    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        isLook = true;
        anim.SetBool("isAttack", true);
        SoundManager.instance.PlaySE(monsterAtt);

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
            yield return new WaitForSeconds(0.2f);
            meleeArea.enabled = true;

            yield return new WaitForSeconds(0.5f);
            meleeArea.enabled = false;

            yield return new WaitForSeconds(0.5f);
            isChase = false;

            break;

            case Type.C:
            break;
        }
    

        isChase = true;
        isAttack = false;
        isLook = false;
        anim.SetBool("isAttack", false);
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isAttack) // 플레이어 태그와 공격 중인지 확인
        {
            statusController.DecreaseHP(100);
        }
    }


    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        isChase = false;
        player.AddHitScore(20);
        gunController.hitreaction();
        bloodHit.SetActive(true);
        SoundManager.instance.PlaySE(monsterBlood);

         foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.red;
        }

        if(currentHp >0)
        {
            foreach(MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.white;
            }

        }

        // 체력이 0 이하로 떨어지면 몬스터 등록수를 -한후  파괴
        if (currentHp <= 0)
        {
            switch(enemyType){
                case Type.A:
                gameManager.enemyCntA--;
                break;
                case Type.B:
                gameManager.enemyCntB--;
                break;
                case Type.C:
                gameManager.enemyCntC--;
                break;
                case Type.D:
                gameManager.enemyCntD--;
                break;
            }
            Die();
        }

        anim.SetTrigger("Hit");
        StartCoroutine(ResumeChaseAfterDelay(1f));
   
    }

    private IEnumerator ResumeChaseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isChase = true;
        bloodHit.SetActive(false);
    }





    private void Die()
    {
        isDie = true;
        isChase = false;
        isAttack = false;
        nav.enabled = false;
        boxCollider.enabled = false;
        anim.SetTrigger("Die");
        SoundManager.instance.PlaySE(monsterDie);
        bloodDie.SetActive(true);
        player.AddScore(20);
        player.AddKillcount(1);
        StartCoroutine(Diecheck(2));
    }

     private IEnumerator Diecheck(int dietime)
    {
        yield return new WaitForSeconds(dietime);
        
        Destroy(gameObject);
    }

    




 
}
