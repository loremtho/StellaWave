using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using Cinemachine.Editor;
public class CamManager : MonoBehaviour
{
    public bool isAnimEnd = false;
    public CinemachineVirtualCamera Cam;
    private float OrgFOVCine;

    private bool isAim = false; //정조준 여부
    public Animator animator;
    public GameObject crosshair;
    public GameObject Startzone;
    public GameObject Bosszone;
    public GameObject storytxt;

    void Start()
    {
        animator = GetComponent<Animator>();

        if(animator != null && isAnimEnd)
        {
            animator.Play("CutScene");
        }

        OrgFOVCine = Cam.m_Lens.FieldOfView;
    }


    void Update()
    {
        OnAim();
    }

    private void OnAim()
    {
        if(Input.GetButtonDown("Fire2"))
        {
            isAim = !isAim;

            if(isAim)
            {
                Cam.m_Lens.FieldOfView = 30f;
            }
            else
            {
                Cam.m_Lens.FieldOfView = OrgFOVCine;
            }
        }
    }

    public void EndAnimationFuction()
    {
        isAnimEnd = true;
        crosshair.SetActive(true);
        Startzone.SetActive(true);
        Bosszone.SetActive(true);
        storytxt.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
