using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class LoadImage8 : MonoBehaviour
{
    public string url = "http://localhost:1234/img/pictures3/";
    private string x, y, z,x1,y1,z1;
    public float jumpDelta;
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        UpdatePositions();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = target.position;
        //float newX=(target.position.x - (Mathf.Floor(target.position.x / jumpDelta) * jumpDelta))/jumpDelta;
        float newX;
        if (transform.position.x < 0)
        {
            newX = 1.0f-(-transform.position.x % jumpDelta) / jumpDelta;
        }
        else
        {
            newX = (transform.position.x % jumpDelta)/jumpDelta;
        }
        Debug.Log("new x : " + newX);
        GetComponent<Renderer>().material.SetFloat("_x",1.0f-newX);

        if (IsNewPosition())
        {
            UpdatePositions();
            string newUrl000 = url + x + "%20" + y + "%20" + z + ".jpg";
            //string newUrl001 = url + x + "%20" + y + "%20" + z1 + ".jpg";
            //string newUrl010 = url + x + "%20" + y1 + "%20" + z + ".jpg";
            //string newUrl011 = url + x + "%20" + y1 + "%20" + z1 + ".jpg";
            string newUrl100 = url + x1 + "%20" + y + "%20" + z + ".jpg";
            //string newUrl101 = url + x1 + "%20" + y + "%20" + z1 + ".jpg";
            //string newUrl110 = url + x1 + "%20" + y1 + "%20" + z + ".jpg";
            //string newUrl111 = url + x1 + "%20" + y1 + "%20" + z1 + ".jpg";
            StartCoroutine(LoadLocalTextureWebRequest(newUrl000, "_Tex000"));
            //StartCoroutine(LoadLocalTextureWebRequest(newUrl001, "_Tex001"));
            //StartCoroutine(LoadLocalTextureWebRequest(newUrl010, "_Tex010"));
            //StartCoroutine(LoadLocalTextureWebRequest(newUrl011, "_Tex011"));
            StartCoroutine(LoadLocalTextureWebRequest(newUrl100, "_Tex100"));
            //StartCoroutine(LoadLocalTextureWebRequest(newUrl101, "_Tex101"));
            //StartCoroutine(LoadLocalTextureWebRequest(newUrl110, "_Tex110"));
            //StartCoroutine(LoadLocalTextureWebRequest(newUrl111, "_Tex111"));
        }
    }


    void UpdatePositions()
    {
        x = Truncate(transform.position.x);
        y = Truncate(transform.position.y);
        z = Truncate(transform.position.z);
        x1 = Truncate(transform.position.x+jumpDelta);
        y1 = Truncate(transform.position.y + jumpDelta);
        z1 = Truncate(transform.position.z + jumpDelta);
        
    }

    bool IsNewPosition()
    {
        return Truncate(transform.position.x) != x || Truncate(transform.position.y) != y || Truncate(transform.position.z) != z;
    }

    private string Truncate(float f)
    {
        return ((Mathf.Floor(f / jumpDelta) * jumpDelta) + "").Replace('.', ',');
    }


    IEnumerator LoadLocalTextureWebRequest(string completeURL,string texName)
    {

        //versión local
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + completeURL, false);

        yield return www.SendWebRequest();

        while (!www.isDone)
        {
            yield return www;
        }

        if (www.isNetworkError)
        {
            Debug.Log("Error:" + www.error);
        }
        else
        {
            try
            {
                Texture myTexture = DownloadHandlerTexture.GetContent(www);
                //GetComponent<Renderer>().material.mainTexture = myTexture;
                GetComponent<Renderer>().material.SetTexture(texName, myTexture);
                Resources.UnloadUnusedAssets();
                //CleanCache();
                www.Dispose();
                System.GC.Collect();
            }
            catch (InvalidOperationException e)
            {
                Debug.Log("image not found - " + e);
            }
        }
    }








}
