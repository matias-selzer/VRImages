using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SendToGoogle : MonoBehaviour
{
    private string BASE_URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLScihJLXXm4tMqK-_c0rVIVqAc59nejM41RbdXtVKXQDHHhfMQ/formResponse";
    private static SendToGoogle instance;

    public static SendToGoogle GetInstance()
    {
        if (instance == null)
        {
            instance = GameObject.Find("WebCommunicationManager").GetComponent<SendToGoogle>();
            //instance = new WebCommunication();
        }
        return instance;
    }

   
    IEnumerator Post(string data)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.2020567888", data);
        byte[] rawData = form.data;
        WWW www = new WWW(BASE_URL, rawData);
        yield return www;
    }
    public void Send(string data)
    {
        StartCoroutine(Post(data));
        //Debug.Log("Sending data: " + data);
    }
}