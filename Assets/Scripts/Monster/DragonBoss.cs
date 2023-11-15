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
              //패턴
             

             break;
        }

    }

 





}
