using UnityEngine;

public class ShaderControl : MonoBehaviour
{
    public float delay = 5f; // 비활성화 지연 시간(예: 5초)

    private void OnEnable()
    {
        // 활성화된 후 5초 뒤에 비활성화
        Invoke("DisableGameObject", delay);
    }

    void DisableGameObject()
    {
        // 오브젝트를 비활성화
        gameObject.SetActive(false);
    }
}