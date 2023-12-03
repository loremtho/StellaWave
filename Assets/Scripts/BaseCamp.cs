using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class BaseCamp : MonoBehaviour
{
    public Slider BaseHPSlider;
    public TextMeshProUGUI BaseHpTxt;
    public float MaxHP;
    public float CurHP;

    [SerializeField]
    private float decreaseInterval = 1f;
    private float BaseTimer = 0;
    [SerializeField]
    private float DecreValue;

    private GameManager gameManager;
    public CinemachineVirtualCamera FailCam = null;

    public ParticleSystem Helingeffect;

    private void Start() 
    {
        FailCam.Priority = 9;
        CurHP = MaxHP;
        BaseHPSlider.value = (float) CurHP / (float) MaxHP;
        BaseHpTxt.text = CurHP.ToString();
        gameManager = FindObjectOfType<GameManager>(); 
    }

    private void Update() 
    {
        if(gameManager.isBattle == true)
        {
            BaseTimer += Time.deltaTime;

            if(BaseTimer >= decreaseInterval)
            {
                BaseDecrease(DecreValue);
                BaseTimer = 0f;
            }    
        }
        BaseCurHP();
    }

    IEnumerator Heling()
    {
        Helingeffect.Play();
        yield return new WaitForSeconds(2f);
        Helingeffect.Stop();
    }

    private void BaseCurHP()
    {
        BaseHPSlider.value = (float) CurHP / (float) MaxHP;
        BaseHpTxt.text = $"{CurHP} / {MaxHP}";
    }

    public void Heal(float healingAmount)
    {
        
        CurHP = Mathf.Min(CurHP, MaxHP); // 최대 체력을 넘지 않도록 제한
        if(CurHP + healingAmount < MaxHP)
        {
            CurHP += healingAmount;
           StartCoroutine(Heling());
        }
        else
        {
            CurHP = MaxHP;
            StartCoroutine(Heling());
        }

    }
    public void BaseDecrease(float amount)
    {
        if (CurHP > 0)
        {
            CurHP -= amount; //amount 만큼 베이스 체력을 감소시킴
            Debug.Log("베이스캠프 체력 감소됨");
        }
        else
        {
            FailCam.Priority = 15;
            Debug.Log("베이스캠프 체력이 0이 되었습니다");
        }
    }

    public void StartDecreaseBaseHP()
    {
        gameManager.isBattle = true;
    }
    public void EndDecreaseBaseHP()
    {
        gameManager.isBattle = false;
    }

    
    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Monster"))
        {
            BaseDamage(100f);
        }
    }
    

    public void BaseDamage(float Damage)
    {
        CurHP -= Damage;
        CurHP = Mathf.Clamp(CurHP, 0, MaxHP);

        if(CurHP <= 0)
        {
            this.gameObject.SetActive(false);
            FailCam.Priority = 15;
            Debug.Log("베이스캠프가 파괴되었습니다!!");
        }
        else
        {
            Debug.Log("베이스캠프 체력 : " + CurHP);
        }
    }

    
    
    
}
