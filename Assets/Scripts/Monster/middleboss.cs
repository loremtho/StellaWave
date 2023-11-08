using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class middleboss : Enemy
{

  
    Vector3 lookvec;
    Vector3 tauntVec;
    public bool isLook;

    // Start is called before the first frame update
    void Awake()
    {

        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        
        Invoke("ChaseStart", 2);
        
        //nav.isStopped = true;
        //StartCoroutine(Think());
        
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
              StartCoroutine(Taunt());

             break;
        }

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




}
