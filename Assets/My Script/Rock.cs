using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    private Rigidbody rb;
    public float force;
    public GameObject target;
    public Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        FlyToTarget();
    }


    public void FlyToTarget()
    {
        if(target==null)
            target=FindObjectOfType<PlayerController>().gameObject;
        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
