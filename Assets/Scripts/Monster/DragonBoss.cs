using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DragonBoss : Boss
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
            lookvec = new Vector3(h, 0, v );
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
              //패턴
              StartCoroutine(Dive());

             break;
        }

    }

    IEnumerator Dive() //플레이어 근처 순간이동
    {
         Vector3 teleportPosition = target.position + Vector3.forward *30f; // 단위만큼 이동
        isLook = false;
        nav.isStopped = false;
        //boxCollider.enabled = false;
        nav.Warp(teleportPosition);
         
        isLook = true; 

        //anim.SetTrigger("isShot");
        yield return new WaitForSeconds(1.5f);
       

        yield return new WaitForSeconds(0.5f);
   
        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
        //boxCollider.enabled = true;

        StartCoroutine(Think());
    }

 





}
