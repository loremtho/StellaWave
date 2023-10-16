using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{

    //알맞은 애니메이션 재생 IK사용해 캐릭터 양손이 총으로 오게 조정

    public Gun gun; //사용할총
    public Transform gunPivot; //총 배치의 기준점
    
    public Transform leftHandMount; //왼손 위치
    public Transform rightHandMount;  //오른손 위치

    private PlayerInput playerInput;
    Animator playerAnimator;

    public PlayerController playerController;

    

  
    public void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<Animator>();

    }

    void Update()
    {
        
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable() {
        gun.gameObject.SetActive(false);
    }

    private void OnAnimatorIK(int layerIndex) 
    {
        
        gunPivot.position = rightHandMount.position;
        //Vector3 offset = new Vector3(-0.1f, -0.13f, 0.3f); // 원하는 내리기 위치 (여기서는 -0.1f만큼 아래로 이동)
        //gunPivot.position += offset;

        //IK를 사용하여 왼속의 위치와 회전을 왼쪽 손잡이에 맞춤
        
        //playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand,1.0f);
        //playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand,1.0f);
        
        //playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand,leftHandMount.position);
        //playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand,leftHandMount.rotation);

        //오른쪽
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand,1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand,1.0f);


        if(playerController.playergun)
        {
            playerAnimator.SetIKPosition(AvatarIKGoal.RightHand,rightHandMount.position);
        }
       
       
     
        //playerAnimator.SetIKRotation(AvatarIKGoal.RightHand,rightHandMount.rotation);
        
         
        
        
    }
}
