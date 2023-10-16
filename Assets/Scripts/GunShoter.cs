using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class GunShoter : MonoBehaviour //언젠간 쓸수도? 총알 판정
{

    public Rigidbody Bullet;
    public float BulletSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void projectile()
    {
        Rigidbody rd = (Rigidbody)Instantiate(Bullet, transform.position, transform.rotation);
        
        rd.velocity = transform.TransformDirection(new Vector3(0, 0, BulletSpeed));
    }
}
