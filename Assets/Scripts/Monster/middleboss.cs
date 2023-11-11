using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class middleboss : Enemy
{

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();
        Invoke("ChaseStart", 1);
    }
    

    // Update is called once per frame
    void Update()
    {
       // nav.SetDestination(target.position);
       
        if(isDead)
        {
            StopAllCoroutines();
            return;
        }

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
              StartCoroutine(Taunt());

             break;
        }

    }


     IEnumerator Taunt() //패턴 추가 시 늘리기
    {
    

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




}
