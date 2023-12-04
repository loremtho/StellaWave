using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamTxt : MonoBehaviour
{
    
    [Header("추적할 텍스트")]
    [SerializeField] Transform TxtUI;
    public GameObject Txt;
    [SerializeField] Camera cam;
     private PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        
        player = FindObjectOfType<PlayerController>();  
        cam = GameObject.Find("Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
         Quaternion q_txt = Quaternion.LookRotation(TxtUI.position - cam.transform.position);
        Vector3 txt_angle = Quaternion.RotateTowards(TxtUI.rotation, q_txt, 200).eulerAngles;
        TxtUI.rotation = Quaternion.Euler(0, txt_angle.y, 0);
        
    }
}
