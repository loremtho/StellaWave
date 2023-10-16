using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projcetile : MonoBehaviour
{
    [SerializeField] float speed;
    private Rigidbody bulletRig;
    void Awake()
    {
        bulletRig = GetComponent<Rigidbody>();
    }

    private void Start() {
        bulletRig.velocity = transform.forward * speed;
        StartCoroutine(NotCollide());
    }

    private void OnTriggerEnter(Collider other) 
    {
        Destroy(this.gameObject);
    }

    IEnumerator NotCollide()
    {
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }
}
