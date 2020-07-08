using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    public Transform[] positions;
    private int index;
    public float moveTime,moveDistance, rotateSpeed;

    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        InvokeRepeating("Move", 0, moveTime);
    }

    private void Move()
    {
        Vector3 normalizedDirection = Vector3.Normalize(positions[index].position - transform.position);
        transform.position = transform.position + normalizedDirection * moveDistance;

        if (Vector3.Distance(transform.position, positions[index].position) < 0.1f)
        {
            index = (index + 1) % positions.Length;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = Vector3.MoveTowards(transform.position, positions[index].position, moveSpeed * Time.deltaTime);
        
        

        transform.rotation = Quaternion.RotateTowards(transform.rotation, positions[index].rotation, rotateSpeed * Time.deltaTime);
    }
}
