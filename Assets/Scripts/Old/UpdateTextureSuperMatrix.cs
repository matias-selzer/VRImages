using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class UpdateTextureSuperMatrix : MonoBehaviour
{
    public string url = "http://localhost:1234/img/pictures3/";
    public float jumpDelta;
    public Transform target;
    private Texture2D[,,] texture_matrix;
    private float leftX, leftY,leftZ;

    private Material myMaterial;
    public float XMin, YMin, ZMin;
    public float XMax, YMax, ZMax;

    // Start is called before the first frame update
    void Start()
    {
        myMaterial = GetComponent<Renderer>().material;
        int cantx = Convert.ToInt32((XMax - XMin) / jumpDelta);
        int canty = Convert.ToInt32((YMax - YMin) / jumpDelta);
        int cantz = Convert.ToInt32((ZMax - ZMin) / jumpDelta);
        Debug.Log("matriz dimensions: ["+cantx+", "+canty+", "+cantz+"]");
        texture_matrix = new Texture2D[cantx,canty,cantz];

        UpdateLeftPositions();
        UpdateTextureMatrix();
    }


    void InterpolateSphere()
    {
        //esto para ver que onda
        Vector3 newPos = transform.position;

        if (MovedLeft())
        {
            newPos.x = target.position.x - jumpDelta;
        }
        else if (MovedRight())
        {
            newPos.x = target.position.x;
        }
        else if (MovedUp())
        {
            newPos.y = target.position.y;
        }
        else if (MovedDown())
        {
            newPos.y = target.position.y - jumpDelta;
        }
        else if (MovedForward())
        {
            newPos.z = target.position.z ;
        }
        else if (MovedBackward())
        {
            newPos.z = target.position.z - jumpDelta;
        }
        
        transform.position = newPos;
    }


    void Update()
    {
        //UpdateShaderInterpolation();

        if (IsNewPosition())
        {
            InterpolateSphere();

            Vector3Int indexLeftPos = indexInMatrix(leftX, leftY, leftZ);

            Resources.UnloadUnusedAssets();
            System.GC.Collect();

            if (MovedLeft())
            {  
                UpdateShaderTextures(indexLeftPos.x -1 ,indexLeftPos.y,indexLeftPos.z);
            }else if (MovedRight())
            {
                UpdateShaderTextures(indexLeftPos.x + 1, indexLeftPos.y, indexLeftPos.z);
            }else if (MovedUp())
            {
                UpdateShaderTextures(indexLeftPos.x, indexLeftPos.y + 1, indexLeftPos.z);
            }else if (MovedDown())
            {
                UpdateShaderTextures(indexLeftPos.x, indexLeftPos.y - 1, indexLeftPos.z);
            }else if (MovedForward())
            {
                UpdateShaderTextures(indexLeftPos.x, indexLeftPos.y, indexLeftPos.z + 1);
            }else if (MovedBackward())
            {
                UpdateShaderTextures(indexLeftPos.x, indexLeftPos.y, indexLeftPos.z - 1);
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
      
        if (texture_matrix[posX, posY, posZ] == null) Debug.Log("ERROR TEXTURE NULL");        
            GetComponent<Renderer>().material.SetTexture("_Tex000", texture_matrix[posX, posY, posZ]);

        if (texture_matrix[posX, posY, posZ+1] == null) Debug.Log("ERROR TEXTURE NULL");       
            GetComponent<Renderer>().material.SetTexture("_Tex001", texture_matrix[posX, posY, posZ+1]);

        if (texture_matrix[posX, posY+1, posZ] == null) Debug.Log("ERROR TEXTURE NULL");      
            GetComponent<Renderer>().material.SetTexture("_Tex010", texture_matrix[posX, posY + 1, posZ]);

        if (texture_matrix[posX, posY+1, posZ+1] == null) Debug.Log("ERROR TEXTURE NULL");     
            GetComponent<Renderer>().material.SetTexture("_Tex011", texture_matrix[posX, posY + 1, posZ + 1]);

        if (texture_matrix[posX+1, posY, posZ] == null) Debug.Log("ERROR TEXTURE NULL");      
            GetComponent<Renderer>().material.SetTexture("_Tex100", texture_matrix[posX + 1, posY, posZ]);

        if (texture_matrix[posX+1, posY, posZ+1] == null) Debug.Log("ERROR TEXTURE NULL");     
            GetComponent<Renderer>().material.SetTexture("_Tex101", texture_matrix[posX + 1, posY, posZ + 1]);

        if (texture_matrix[posX+1, posY+1, posZ] == null) Debug.Log("ERROR TEXTURE NULL");
            GetComponent<Renderer>().material.SetTexture("_Tex110", texture_matrix[posX + 1, posY + 1, posZ]);

        if (texture_matrix[posX+1, posY+1, posZ+1] == null) Debug.Log("ERROR TEXTURE NULL");
            GetComponent<Renderer>().material.SetTexture("_Tex111", texture_matrix[posX + 1, posY + 1, posZ + 1]);
    }

    void UpdateLeftPositions()
    {
        leftX = Truncate(target.position.x);
        leftY = Truncate(target.position.y);
        leftZ = Truncate(target.position.z);
        //Debug.Log("updated position: " + leftX + ", " + leftY + ", " + leftZ);
    }

    void UpdateShaderInterpolation()
    {
        float newX = target.position.x / jumpDelta;
        newX = ((float)(newX - Math.Floor(newX)));
        //this
        newX *= 2.0f;
        if (newX >1.0f)
            newX = 1.0f;
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
        Vector3Int indexLeftPos = indexInMatrix(leftX, leftY, leftZ);
        for (int i = indexLeftPos.x-2; (i<= indexLeftPos.x + 2)&&(i < texture_matrix.GetLength(0)); i++)
            for (int j = indexLeftPos.y - 2; (j <= indexLeftPos.y + 2) && (j < texture_matrix.GetLength(1)); j++)
                for (int k = indexLeftPos.z - 2; (k <= indexLeftPos.z + 2) && (k < texture_matrix.GetLength(2)); k++)
                    if ((i>=0)&&(j>=0)&&(k>=0))
                        StartCoroutine(LoadLocalTextureWebRequestInMatrix(i, j, k));
    }


    Vector3Int indexInMatrix(float posx, float posy, float posz) {
        Vector3Int output= new Vector3Int();
        output.x = Convert.ToInt32((posx - XMin) / jumpDelta);
        output.y = Convert.ToInt32((posy - YMin) / jumpDelta);
        output.z = Convert.ToInt32((posz - ZMin) / jumpDelta);        
        return output;
    }

    float[] imagePosInMatrix(int i, int j, int k)
    {
        float[] output = new float[3];
        output[0] = Truncate((i * jumpDelta) + XMin);
        output[1] = Truncate((j * jumpDelta) + YMin);
        output[2] = Truncate((k * jumpDelta) + ZMin);

        Vector3Int reversa = indexInMatrix(output[0], output[1], output[2]);
        if ((i != reversa.x) || (j != reversa.y) || (k != reversa.z))
            Debug.Log("VERIFICATION ERROR" );
        
        return output;
    }

    IEnumerator LoadLocalTextureWebRequestInMatrix(int i, int j, int k)
    {   
        
        float[] posImage;

        if (texture_matrix[i, j, k] == null)
        {
            posImage = imagePosInMatrix(i, j, k);

            float newX = posImage[0];
            float newY = posImage[1];
            float newZ = posImage[2];

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
                    texture_matrix[i, j, k] = myTexture;
                    // Resources.UnloadUnusedAssets();
                    //CleanCache();
                    www.Dispose();
                   // System.GC.Collect();
                }
                catch (InvalidOperationException e)
                {
                    Debug.Log("image not found - " + newUrl + " ERROR");
                }
            }
        }
       // else Debug.Log("image already loaded");
    }








}