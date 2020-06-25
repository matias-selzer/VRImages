using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class UpdateTexture : MonoBehaviour
{
    public string url = "http://localhost:1234/img/pictures3/";
    public float jumpDelta;
    public Transform target;

    private Texture2D[,,] texture_matrix;
    private float leftX, leftY,leftZ;

    private Material myMaterial;

   // private Texture2D[,,] pruebita;


    // Start is called before the first frame update
    void Start()
    {
        myMaterial = GetComponent<Renderer>().material;
        texture_matrix = new Texture2D[4,4,4];

        //pruebita = new Texture2D[100, 100, 100];

        UpdateLeftPositions();
        UpdateTextureMatrix();
    }


    void Update()
    {
        UpdateShaderInterpolation();

        if (IsNewPosition())
        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            if (MovedLeft())
            {
                UpdateShaderTextures(0,1,1);
            }else if (MovedRight())
            {
                UpdateShaderTextures(2, 1 ,1);
            }else if (MovedUp())
            {
                UpdateShaderTextures(1, 2, 1);
            }else if (MovedDown())
            {
                UpdateShaderTextures(1, 0, 1);
            }else if (MovedForward())
            {
                UpdateShaderTextures(1, 1, 2);
            }else if (MovedBackward())
            {
                UpdateShaderTextures(1, 1, 0);
            }
            UpdateLeftPositions();
            UpdateTextureMatrix();
        }
    }

    bool IsNewPosition()
    {
        return Truncate(target.position.x) != leftX || Truncate(target.position.y) != leftY || Truncate(target.position.z) != leftZ;
    }

    bool MovedLeft()
    {
        return Truncate(target.position.x)<leftX;
    }

    bool MovedRight()
    {
        return Truncate(target.position.x) > leftX;
    }

    bool MovedUp()
    {
        return Truncate(target.position.y) > leftY;
    }

    bool MovedDown()
    {
        return Truncate(target.position.y) < leftY;
    }

    bool MovedForward()
    {
        return Truncate(target.position.z) > leftZ;
    }

    bool MovedBackward()
    {
        return Truncate(target.position.z) < leftZ;
    }

    private float Truncate(float f)
    {
        return Mathf.Floor(f / jumpDelta) * jumpDelta;
    }

    void UpdateShaderTextures(int posX,int posY,int posZ)
    {
        GetComponent<Renderer>().material.SetTexture("_Tex000", texture_matrix[posX, posY, posZ]);
        GetComponent<Renderer>().material.SetTexture("_Tex001", texture_matrix[posX, posY, posZ+1]);
        GetComponent<Renderer>().material.SetTexture("_Tex010", texture_matrix[posX, posY + 1, posZ]);
        GetComponent<Renderer>().material.SetTexture("_Tex011", texture_matrix[posX, posY + 1, posZ + 1]);
        GetComponent<Renderer>().material.SetTexture("_Tex100", texture_matrix[posX + 1, posY, posZ]);
        GetComponent<Renderer>().material.SetTexture("_Tex101", texture_matrix[posX + 1, posY, posZ + 1]);
        GetComponent<Renderer>().material.SetTexture("_Tex110", texture_matrix[posX + 1, posY + 1, posZ]);
        GetComponent<Renderer>().material.SetTexture("_Tex111", texture_matrix[posX + 1, posY + 1, posZ + 1]);
    }

    void UpdateLeftPositions()
    {
        leftX = Truncate(target.position.x);
        leftY = Truncate(target.position.y);
        leftZ = Truncate(target.position.z);
    }

    void UpdateShaderInterpolation()
    {
        float newX = target.position.x / jumpDelta;
        newX = ((float)(newX - Math.Floor(newX)));
        myMaterial.SetFloat("_x", newX);

        float newY = target.position.y / jumpDelta;
        newY = ((float)(newY - Math.Floor(newY)));
        myMaterial.SetFloat("_y", newY);

        float newZ = target.position.z / jumpDelta;
        newZ = ((float)(newZ - Math.Floor(newZ)));
        myMaterial.SetFloat("_z", newZ);
    }


    void UpdateTextureMatrix()
    {
        for (int i = 0; i < texture_matrix.GetLength(0); i++)
            for (int j = 0; j < texture_matrix.GetLength(1); j++)
                for (int k = 0; k < texture_matrix.GetLength(2); k++)
                    StartCoroutine(LoadLocalTextureWebRequestInMatrix(i, j, k));
    }

    IEnumerator LoadLocalTextureWebRequestInMatrix(int i, int j, int k)
    {
        float newX = leftX + (i - 1) * jumpDelta;
        float newY = leftY + (j - 1) * jumpDelta;
        float newZ = leftZ + (k - 1) * jumpDelta;

        string imageName = newX + "%20" + newY + "%20" + newZ + ".jpg";
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
                Texture2D myTexture = DownloadHandlerTexture.GetContent(www);
                texture_matrix[i,j,k] = myTexture;
                
                //CleanCache();
                www.Dispose();
                
            }
            catch (InvalidOperationException e)
            {
                Debug.Log("image not found - " + newUrl + " ERROR");
            }
        }
    }








}