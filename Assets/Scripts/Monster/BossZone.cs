using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossZone : MonoBehaviour
{
    [SerializeField]
    private string bgm2;

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player")
        {
            
            SceneManager.LoadScene("BossStage1");
        }
    }
}
