using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryCleaner : MonoBehaviour
{
    public float timeInterval;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("CleanMemory", 10, timeInterval);
    }

    void CleanMemory()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}
