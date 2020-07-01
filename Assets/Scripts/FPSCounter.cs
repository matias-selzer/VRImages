﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public Text fpsLabel;
    public float timeInterval;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("ShowFPS", 0, timeInterval);
    }


    void ShowFPS()
    {
        int FPS = (int)(1f / Time.unscaledDeltaTime);
        fpsLabel.text = "FPS = " + FPS;
    }
}
