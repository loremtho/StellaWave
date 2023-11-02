using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Redcode.Pools;
using Unity.Mathematics;

public class Boss : MonoBehaviour 
{
    public enum Type {A, B, C};
    public Type enemyType;
   
    public Transform target;

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
    
    private string monsterAtt;

    [SerializeField]
    
    private string monsterDie;

    public GunController gunController;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public NavMeshAgent nav;

    public MeshRenderer[] meshs;

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
            statusController.DecreaseHP(20);
        }
    }


    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        isChase = false;
        Debug.Log(currentHp);

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
       
        //player.AddHitScore(20);
        gunController.hitreaction();
        if (currentHp < 0)
        {
            Die();
        }

        anim.SetTrigger("Hit");
        StartCoroutine(ResumeChaseAfterDelay(1f));
   
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
        boxCollider.enabled = false;
        anim.SetTrigger("Die");
        //SoundManager.instance.PlaySE(monsterDie);
        StartCoroutine(Diecheck(10));
       
    }

     private IEnumerator Diecheck(int dietime)
    {

        yield return new WaitForSeconds(dietime);
        Destroy(gameObject);
    }




 
}
