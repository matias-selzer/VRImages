using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTouchMove : MonoBehaviour
{
    public float moveSpeed, rotateSpeed;
    public Transform target;
    public bool canMove = true;


    private void Start()
    {
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        float x;
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (Input.mousePosition.x < Screen.width / 4)
            {
                target.transform.Rotate(0, -1.0f * rotateSpeed * Time.deltaTime, 0);
            }
            else if (Input.mousePosition.x > Screen.width * 3 / 4)
            {
                target.transform.Rotate(0, 1.0f * rotateSpeed * Time.deltaTime, 0);
            }
            else
            {
                x = Mathf.Sin(Time.time * 10);
                x = x * 1;
                Vector3 rot = target.rotation.eulerAngles;
                rot.x = x;
                //target.rotation = Quaternion.Euler(rot);
                if (canMove)
                {
                    //Debug.Log("muevo la camara");
                    target.transform.position += target.transform.forward * moveSpeed * Time.deltaTime;
                }
                //.Log(canMove);
            }
        }
        else
        {
            x = Mathf.Sin(Time.time);
            Vector3 rot = target.rotation.eulerAngles;
            rot.x = x * 2;
            //target.rotation = Quaternion.Euler(rot);
        }
        
    }

    public void GoBack()
    {
        target.transform.position -= target.transform.forward*2 * moveSpeed * Time.deltaTime;
    }
}
