using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public Text fpsLabel;
    public float timeInterval;
    private string savedFPS = "";
    int FPS;
    float fps;

    private float deltaTime = 0;

    // Start is called before the first frame update
    void Start()
    {

        //Application.targetFrameRate = 60;
    }

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float msec = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;
        fpsLabel.text = "FPS = " + FPS+" - "+fps+" - "+msec;
    }

    public void Initialize()
    {
        InvokeRepeating("ShowFPS", 0, timeInterval);
    }

    void ShowFPS()
    {
        FPS = (int)(1f / Time.unscaledDeltaTime);
        
        savedFPS += fps + ";";
    }

    
    public void SendFPSData()
    {
        GetComponent<SendToGoogle>().Send(savedFPS);
    }
}
