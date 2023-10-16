using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCam : MonoBehaviour
{
    [SerializeField] private GameObject target;

    [SerializeField] private float offsetX = 0.0f;
    [SerializeField] private float offsetY = 10.0f;
    [SerializeField] private float offsetZ = -10.0f;
    [SerializeField] private float CamSpeed = 10.0f;
    Vector3 TargetPos;

    void FixedUpdate() 
    {
        //타겟의 x,y,z 좌표에 카메라 좌표를 더하여 카메라 위치를 결정
        TargetPos = new Vector3(
            target.transform.position.x + offsetX,
            target.transform.position.y + offsetY,
            target.transform.position.z + offsetZ
        );

        //카메라 움직임을 부드럽게 하는 함수
        transform.position = Vector3.Lerp(transform.position, TargetPos, CamSpeed * Time.deltaTime);
    }
}
