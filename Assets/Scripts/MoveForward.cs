using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public float xJump, yJump, zJump, seconds;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Advance", 0,seconds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Advance()
    {
        transform.Translate(xJump, yJump, zJump);
    }
}
