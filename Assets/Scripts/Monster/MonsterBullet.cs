using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBullet : MonoBehaviour
{
    public Transform target;
    NavMeshAgent nav;

     [SerializeField]
    private int Hp;

    private int currentHp;

    public MeshRenderer[] meshs;
    public MeshRenderer objectMeshRenderer; // 객체의 매쉬 렌더러
    public GameObject existingEffect; // 현재 이펙트
    public GameObject newEffect; // 새로운 이펙트

    private bool navgate = true;

    public StatusController statusController;


    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        currentHp = Hp;
          
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(navgate)
        {
            nav.SetDestination(target.position);

        }
    }

    public void TakeDamage(int damage) //미사일 자기 체력
    {
        currentHp -= damage;
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }

        if(currentHp >0)
        {
            foreach(MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.red;
            }

        }
        if(currentHp <= 0)
        {
            objectMeshRenderer.enabled = false;
            existingEffect.gameObject.SetActive(false);
            newEffect.gameObject.SetActive(true);
            navgate = false;
            nav.enabled = false;
            newEffect.SetActive(true);
            StartCoroutine(DestroyMissileAfterDelay(3));

        }


    }

    private IEnumerator DestroyMissileAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);   
        Destroy(gameObject);

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어 태그 확인
        {
            statusController.DecreaseHP(100);
            TakeDamage(Hp);
        }
    }
}
