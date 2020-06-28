using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class UpdateTextureBorra : MonoBehaviour
{
    public float jumpDelta;
    public Transform target;
    
    public float XMin, YMin, ZMin;
    public float XMax, YMax, ZMax;
    public float tolerance;

    private TextureMatrix textureMatrix;
    private TexturedSphere sphere;
    private ImageLoader imageLoader;

    // Start is called before the first frame update
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        sphere = GetComponent<TexturedSphere>();
        imageLoader = GetComponent<ImageLoader>();
        textureMatrix = new TextureMatrix(XMin, XMax, YMin, YMax, ZMin, ZMax, tolerance, jumpDelta);

        UpdateActualPosition();

        //UpdateShaderTextures();

        UpdateTextureMatrix();
    }


    void Update()
    {
        //UpdateShaderInterpolation();



        if (IsNewPosition())
        {

           // transform.position = target.position;
            sphere.MoveSphere(target.position);

            UpdateActualPosition();

            Debug.Log("tengo tex actual? " + textureMatrix.HasActualTexture());

            UpdateShaderTextures();

            UpdateTextureMatrix();

        }
    }




    void UpdateActualPosition()
    {
        textureMatrix.UpdateActualPosition(target.position);
    }

    bool IsNewPosition()
    {
        return textureMatrix.IsNewPosition(target.position);
    }

    void UpdateShaderTextures()
    {
        Vector3Int indexPos = textureMatrix.PosToIndex(target.position);
        sphere.UpdateShaderTexture("_Tex000", textureMatrix.Get(indexPos.x,indexPos.y,indexPos.z));
        /*
        sphere.UpdateShaderTexture("_Tex001", textureMatrix.Get(indexPos.x, indexPos.y, indexPos.z+1));
        sphere.UpdateShaderTexture("_Tex010", textureMatrix.Get(indexPos.x, indexPos.y+1, indexPos.z));
        sphere.UpdateShaderTexture("_Tex011", textureMatrix.Get(indexPos.x, indexPos.y+1, indexPos.z+1));
        sphere.UpdateShaderTexture("_Tex100", textureMatrix.Get(indexPos.x+1, indexPos.y, indexPos.z));
        sphere.UpdateShaderTexture("_Tex101", textureMatrix.Get(indexPos.x+1, indexPos.y, indexPos.z+1));
        sphere.UpdateShaderTexture("_Tex110", textureMatrix.Get(indexPos.x+1, indexPos.y+1, indexPos.z));
        sphere.UpdateShaderTexture("_Tex111", textureMatrix.Get(indexPos.x+1, indexPos.y+1, indexPos.z+1));
        */
    }

    /*
    void UpdateTextureMatrix()
    {
        Vector3Int indexPos = textureMatrix.PosToIndex(target.position);
        for (int i = indexPos.x-2; (i<= indexPos.x + 2)&&(i < textureMatrix.GetLengthX()); i++)
            for (int j = indexPos.y - 2; (j <= indexPos.y + 2) && (j < textureMatrix.GetLengthY()); j++)
                for (int k = indexPos.z - 2; (k <= indexPos.z + 2) && (k < textureMatrix.GetLengthZ()); k++)
                    if ((i >= 0) && (j >= 0) && (k >= 0)) {
                        if (!textureMatrix.HasTextureLoaded(i,j,k))
                            imageLoader.LoadImage(i, j, k,textureMatrix);
                    }
        textureMatrix.CleanMatrix();
                    
    }
    */

    void UpdateTextureMatrix()
    {
        imageLoader.EmptyImageList();
        Vector3Int indexPos = textureMatrix.GetActualIndexPosition();

        //cuando agrego el eje y ya empieza a andar re mal en el teléfono

        for (int i = indexPos.x - 1; (i <= indexPos.x + 1) && (i < textureMatrix.GetLengthX()); i++)
            //for (int j = indexPos.y - 1; (j <= indexPos.y + 1) && (j < textureMatrix.GetLengthY()); j++)
                for (int k = indexPos.z - 2; (k <= indexPos.z + 2) && (k < textureMatrix.GetLengthZ()); k++)
                    if ((i >= 0) && (indexPos.y >= 0) && (k >= 0))
                    {
                        if (!textureMatrix.HasTextureLoaded(i, indexPos.y, k))
                            imageLoader.LoadImage(i, indexPos.y, k, textureMatrix);
                    }
                    
        /*
        int i = indexPos.x;
        int j = indexPos.y;
        int k = indexPos.z;

        CallLoadImage(i , j, k);
        CallLoadImage(i - 1, j, k);
        CallLoadImage(i + 1, j, k);
        CallLoadImage(i, j, k - 1);
        CallLoadImage(i, j, k + 1);
        //CallLoadImage(i, j - 1, k);
        //CallLoadImage(i, j + 1, k);

        //diagonales
        CallLoadImage(i - 1, j, k-1);
        CallLoadImage(i + 1, j, k+1);
        CallLoadImage(i - 1, j, k+1);
        CallLoadImage(i + 1, j, k-1);

        //doble
        CallLoadImage(i, j, k);
        CallLoadImage(i - 1, j, k);
        CallLoadImage(i + 1, j, k);
        CallLoadImage(i, j, k - 1);
        CallLoadImage(i, j, k + 1);
        */

        textureMatrix.CleanMatrix();

    }

    private void CallLoadImage(int i, int j, int k)
    {
        if (!textureMatrix.HasTextureLoaded(i, j, k) 
            && (i >= 0 && i < textureMatrix.GetLengthX())
            && (j >= 0 && j < textureMatrix.GetLengthY())
            && (k >= 0 && k < textureMatrix.GetLengthZ()))
        {
                imageLoader.LoadImage(i, j, k, textureMatrix);  
        }
    }

    void UpdateShad3erInterpolation()
    {
        float newX = target.position.x / jumpDelta;
        newX = ((float)(newX - Math.Floor(newX)));
        float newY = target.position.y / jumpDelta;
        newY = ((float)(newY - Math.Floor(newY)));
        float newZ = target.position.z / jumpDelta;
        newZ = ((float)(newZ - Math.Floor(newZ)));

        sphere.UpdateShaderInterpolation(newX, newY, newZ);
    }

    








}