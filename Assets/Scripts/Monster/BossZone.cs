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
            SoundManager.instance.PlayBGM(bgm2); 
            SceneManager.LoadScene("BossStage1");
        }
    }
}
