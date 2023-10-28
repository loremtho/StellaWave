using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStat : MonoBehaviour
{
    
    //체력
    [SerializeField]
    private int hp;
    private int currentHp;


    //방어력
    [SerializeField]
    private int dp;
    private int currentDp;

   

    private const int HP = 0, DP = 1;


    void Start()
    {
        currentHp = hp;
        currentDp = dp;
    }

    // Update is called once per frame
    void Update()
    {
     
        GaugeUpdate();
     
    }

    private void GaugeUpdate()
    {
        

    }

    public void DecreaseHP(int _count)
    {
        if(currentDp > 0)
        {
            DecreaseDP(_count);
            return;
        }

        currentHp-= _count;

        if(currentHp <= 0)
        {
            //게임 오버 시키자
        }
    }


    public void DecreaseDP(int _count)
    {
        currentDp -= _count;

        if (currentDp <= 0)
        {
            //방어력 0
        }
    }

   
    




}
