using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Looked : MonoBehaviour
{
    Vector3 lookvec;
    public Transform target;
      
    public bool isLook;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         if(isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input. GetAxisRaw("Vertical");
            lookvec = new Vector3(h, 0, v )* 5f;
            transform.LookAt(target.position);
        }
    }
}
