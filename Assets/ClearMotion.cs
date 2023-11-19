using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClearMotion : MonoBehaviour
{

    public GameManager gameManager;
    public ButtonController buttonController;
    public Animator animator;
    public GameObject ClearCanvas;

    private void Update() {
        if(Input.GetKeyDown(KeyCode.C))
        {   
            if(animator != null)
            {
                animator = ClearCanvas.GetComponent<Animator>();
                MotionStart();
            }
        }
    }

    private void MotionStart()
    {
        animator.SetTrigger("Clear");
        gameManager.LastUi();
    }

}
