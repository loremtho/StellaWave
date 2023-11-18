using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZone : MonoBehaviour
{
    [SerializeField]public GameManager manager;
    public StageCount stageCount;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            manager.StageStart();
            stageCount.AnimStart();
        }
        
    }

}
