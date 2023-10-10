using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed=1f;
    Rigidbody rb;
    float v;
    // Start is called before the first frame update
    void Start()
    {
        rb= GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        v = Input.GetAxis("Vertical");
        rb.velocity = Vector3.forward * speed*v;
    }

    public bool GetInput()
    { return !(v == 0); }
}
