using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruncateTests : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float x = -3.99999f;
        int x2=(int)(x);
        Debug.Log("casting: " + x2);

        int x3 = Convert.ToInt32(x);
        Debug.Log("convertTo32: " + x3);

        int x4 = Mathf.FloorToInt(x);
        Debug.Log("Floor: " + x4);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
