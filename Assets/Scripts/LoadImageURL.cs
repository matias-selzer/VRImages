using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class LoadImageURL : MonoBehaviour
{
    public string url = "http://localhost:1234/img/pictures3/";
    private string x, y, z;
    public float jumpDelta;
    public Transform target;

    private Texture2D t000,t001,t010,t011,t100,t101,t110,t111;


    // Start is called before the first frame update
    void Start()
    {
        UpdatePositions();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position;
        //Debug.Log(transform.name+" - "+target.position);
        //Debug.Log(x + " - " + y + " " + z);
        if (IsNewPosition())
        {
            UpdatePositions();
            //StartCoroutine(LoadTexture());
            StartCoroutine(LoadLocalTextureWebRequest());
            //string imageName = x + " " + y + " " + z + ".jpg";
            //string newUrl = url + imageName;
            //GetComponent<Renderer>().material.mainTexture = LoadImageURL.LoadLocalTexture(newUrl);
        }
    }


    void UpdatePositions()
    {
        x = Truncate(transform.position.x);
        y = Truncate(transform.position.y);
        z = Truncate(transform.position.z);
    }

    bool IsNewPosition()
    {
        return Truncate(transform.position.x) != x || Truncate(transform.position.y) != y || Truncate(transform.position.z) != z;
    }

    private string Truncate(float f)
    {
        return ((Mathf.Floor(f / jumpDelta) * jumpDelta) + "").Replace('.', ',');
    }




    //nuevo metodo para cargar imágenes de forma local
    public static Texture2D LoadLocalTexture(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2, TextureFormat.BGRA32, false);
            //tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            tex.LoadRawTextureData(fileData); //..this will auto-resize the texture dimensions.
        }
        else
        {
            Debug.Log("file not found at: " + filePath);
        }
        return tex;
    }



    IEnumerator LoadOnlineTexture()
    {
        Texture2D tex;
        tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        string imageName = x + "%20" + y + "%20" + z + ".jpg";
        string newUrl = url + imageName;
        //Debug.Log(newUrl);
        using (WWW www = new WWW(newUrl))
        {
            yield return www;
            www.LoadImageIntoTexture(tex);
            //www.Dispose();
            //DestroyImmediate(GetComponent<Renderer>().material.mainTexture);
            GetComponent<Renderer>().material.mainTexture = tex;
        }
    }


    IEnumerator LoadLocalTextureWebRequest()
    {
        string imageName = x + "%20" + y + "%20" + z + ".jpg";
        string newUrl = url + imageName;

        //versión local
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://"+newUrl,false);

        //versión web
        //UnityWebRequest www = UnityWebRequestTexture.GetTexture("http://192.168.0.5:1234/img/pictures3/" + imageName, false);

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
                GetComponent<Renderer>().material.mainTexture = myTexture;
                Resources.UnloadUnusedAssets();
                //CleanCache();
                www.Dispose();
                System.GC.Collect();
            }catch(InvalidOperationException e)
            {
                Debug.Log("image not found - " + e);
            }
        }
    }


    



    //esto es para obtenr los bytes de una textura
    void GetTextureBytesData()
    {
        var texture = new Texture2D(128, 128, TextureFormat.RGBA32, false);
        GetComponent<Renderer>().material.mainTexture = texture;

        // RGBA32 texture format data layout exactly matches Color32 struct
        var data = texture.GetRawTextureData<Color32>();

        // fill texture data with a simple pattern
        Color32 orange = new Color32(255, 165, 0, 255);
        Color32 teal = new Color32(0, 128, 128, 255);
        int index = 0;
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                data[index++] = ((x & y) == 0 ? orange : teal);
            }
        }
        // upload to the GPU
        texture.Apply();
    }




}
