using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardMove : MonoBehaviour
{
    public float moveSpeed, rotateSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, -1.0f * rotateSpeed * Time.deltaTime,0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, 1.0f * rotateSpeed * Time.deltaTime, 0);
        }
    }
}
