using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageCount : MonoBehaviour
{

    public TextMeshProUGUI stageCount;
    public GameManager gameManager;
    public GameObject StageCountobject;

    public Animator animator;

    private void Start() {
    }

    void LateUpdate()
    {
        stageCount.text = "Wave  " + gameManager.stagecount + "  Start";
    }

    public void AnimStart()
    {
        StageCountobject.SetActive(true);
        animator.SetTrigger("Start");

        StartCoroutine(AnimEnd());
    }

    IEnumerator AnimEnd()
    {
        yield return new WaitForSeconds(6f);
        StageCountobject.SetActive(false);
    }
}
