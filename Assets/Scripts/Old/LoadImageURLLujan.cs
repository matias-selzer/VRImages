using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class LoadImageURLLujan : MonoBehaviour
{
    public string url = "http://localhost:1234/img/pictures3/";
    private string x, y, z;
    public float jumpDelta;
    public Transform target;
    public Texture[,,] texture_matrix;

    private Material myMaterial;


    // Start is called before the first frame update
    void Start()
    {
        myMaterial = GetComponent<Renderer>().material;
        UpdatePositions();
        texture_matrix = new Texture[4,4,4];
        updateTextureMatrix();
    }



    // Update is called once per frame
    void Update()
    {
        //    transform.position = target.position;
        //Debug.Log(transform.name+" - "+target.position);
        //Debug.Log(x + " - " + y + " " + z);
        float newX;
        /*if (transform.position.x < 0)
        {
            newX = 1.0f - (-transform.position.x % jumpDelta) / jumpDelta;
        }
        else
        {
            newX = (transform.position.x % jumpDelta) / jumpDelta;
        }*/
        //Debug.Log("new x : " + newX);

        float papita = target.position.x / jumpDelta;
        newX=((float)(papita- Math.Floor(papita)));

        myMaterial.SetFloat("_x",newX);

        Vector3Int indexInMatrix = indexInMatrixCurrentPosition();
        
        //GetComponent<Renderer>().material.mainTexture = texture_matrix[indexInMatrix.x, indexInMatrix.y, indexInMatrix.z];
        
        if (IsNewPosition())
        {
            //StartCoroutine(LoadTexture());
            //StartCoroutine(LoadLocalTextureWebRequest());
            UpdatePositions();

            GetComponent<Renderer>().material.SetTexture("_Tex000", texture_matrix[indexInMatrix.x-1, indexInMatrix.y, indexInMatrix.z]);
            GetComponent<Renderer>().material.SetTexture("_Tex100", texture_matrix[indexInMatrix.x , indexInMatrix.y, indexInMatrix.z]);

            updateTextureMatrix();
            
            //string imageName = x + " " + y + " " + z + ".jpg";
            //string newUrl = url + imageName;
            //GetComponent<Renderer>().material.mainTexture = LoadImageURL.LoadLocalTexture(newUrl);
        }
    }


    Vector3Int indexInMatrixCurrentPosition() {
        Vector3Int output = new Vector3Int();
        string currentPosx = Truncate(target.position.x);
        string currentPosy = Truncate(target.position.y);
        string currentPosz = Truncate(target.position.z);

        int index_i = (int)((float.Parse(currentPosx) - float.Parse(x))/ jumpDelta) + 1;
        int index_j = (int)((float.Parse(currentPosy) - float.Parse(y)) / jumpDelta) + 1;
        int index_k = (int)((float.Parse(currentPosz) - float.Parse(z)) / jumpDelta) + 1;

        output.x = index_i;
        output.y = index_j;
        output.z = index_k;
        return output;

    }


    void UpdatePositions()
    {
        x = Truncate(target.position.x);
        y = Truncate(target.position.y);
        z = Truncate(target.position.z);
    }

    bool IsNewPosition()
    {
        return Truncate(target.position.x) != x || Truncate(target.position.y) != y || Truncate(target.position.z) != z;
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



    void updateTextureMatrix() {
   
        for (int i = 0; i < texture_matrix.GetLength(0); i++)
            for (int j = 0; j < texture_matrix.GetLength(1); j++)
                for (int k = 0; k < texture_matrix.GetLength(2); k++) 
                    StartCoroutine(LoadLocalTextureWebRequestInMatrix(i, j, k));

    }

    IEnumerator LoadLocalTextureWebRequestInMatrix(int i, int j, int k) {
       

        string x_aux = (float.Parse(x) + (i-1)*jumpDelta) + "";         
        string y_aux = (float.Parse(y) + (j-1)*jumpDelta) + "";
        string z_aux = (float.Parse(z) + (k-1)*jumpDelta) + "";

        y_aux = "1";
        z_aux = "0";

        string imageName = x_aux + "%20" + y_aux + "%20" + z_aux + ".jpg";
        string newUrl = url + imageName;

        UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + newUrl, false);
        yield return www.SendWebRequest();

        while (!www.isDone)
        {
            yield return www;
        }

        if (www.isNetworkError)
        { 
            Debug.Log("Error:" + www.error);
            Debug.Log(newUrl + " ERROR");
        }
        else
        {
         
            try
            {
                Texture myTexture = DownloadHandlerTexture.GetContent(www);
                texture_matrix[i,j,k] = myTexture;
                //Debug.Log(newUrl + " traje");
                //Debug.Log("posiciones en la matrix" + i + ", " + j + ", " + k);
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


    IEnumerator LoadLocalTextureWebRequest()
    {
        string imageName = x + "%20" + y + "%20" + z + ".jpg";
        string newUrl = url + imageName;
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + newUrl, false);
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
            }
            catch (InvalidOperationException e)
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