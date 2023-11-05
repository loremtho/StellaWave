using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightMissile : MonoBehaviour
{
    [SerializeField]
    private int Hp;

    private int currentHp;


    public MeshRenderer objectMeshRenderer; // 객체의 매쉬 렌더러
    public ParticleSystem existingEffect; // 현재 이펙트
    public GameObject newEffect; // 새로운 이펙트
    public StatusController statusController;
    
     private Transform target;

    public void Awake()
    {
        currentHp = Hp;
        target = GameObject.FindGameObjectWithTag("Player").transform;

        // 플레이어 스테이터스 컨트롤러 참조
        statusController = target.GetComponent<StatusController>();
    
    }


     public void TakeDamage(int damage) //미사일 자기 체력
    {
        currentHp -= damage;
        
        if(currentHp <= 0)
        {
            Rigidbody rb = GetComponent<Rigidbody>();

            rb.isKinematic = true;


            objectMeshRenderer.enabled = false;
            existingEffect.Stop();
            newEffect.gameObject.SetActive(true);
            newEffect.SetActive(true);
            StartCoroutine(DestroyMissileAfterDelay(2));

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
            statusController.DecreaseHP(200);
            TakeDamage(Hp);
        }
    }
}
