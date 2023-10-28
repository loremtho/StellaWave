using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public float rotationSpeed = 30.0f; // 회전 속도 (도/초)

    void Update()
    {
        // X 축 주위로 회전
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}
