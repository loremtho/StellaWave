using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Battery : MonoBehaviour
{
    [SerializeField]
    private int hp; // 배터리 체력

    [SerializeField]
    private float destroyTime; // 파편 제거 시간

    [SerializeField]
    private CapsuleCollider col; //구체 콜라이더

    public float healingAmount = 20f;


//필요한 게임 오브젝트
    [SerializeField]
    private GameObject go_batt; //일반 배터리
    /*[SerializeField]
    private GameObject go_debris; // 깨진 바위*/
    [SerializeField]
    private GameObject go_effect_prefabs; //채굴 이펙트
    [SerializeField]
    private GameObject go_battery_item_prefab; //돌맹이 아이템

    [SerializeField] //배터리 아이템 등장 개수
    private int count;

    //필요한 사운드 이름
    [SerializeField]
    private string strike_Sound;
    [SerializeField]
    private string destroy_Sound;
    private GameManager gameManager;

    private Vector3 YOffSet = new Vector3(0f, 1f, 0f);

    private void Start() {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void Mining()
    {
        SoundManager.instance.PlaySE(strike_Sound);

        var clone = Instantiate(go_effect_prefabs, col.bounds.center, Quaternion.identity);
        Destroy(clone, destroyTime);

        hp--;

        if(hp <= 0)
        Destrcution();
    }

    private void Destrcution()
    {
        SoundManager.instance.PlaySE(destroy_Sound);

        col.enabled = false;

        for (int i = 0; i <= count; i++)
        {
            Instantiate(go_battery_item_prefab, go_batt.transform.position + YOffSet, Quaternion.identity);
        }
        Destroy(go_batt);
        this.gameObject.SetActive(false);

        //go_debris.SetActive(true);
        //Destroy(go_debris, destroyTime);
    }

}
