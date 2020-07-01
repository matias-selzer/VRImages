using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class TexturesManager : MonoBehaviour
{
    public Transform cameraCenter,cameraL,cameraR;
    
    public float XMin, XMax, YMin, YMax, ZMin, ZMax;
    public float jumpDelta;
    public int toleranceDistanceCells;
    public int kernelRadioX, kernelRadioY, kernelRadioZ;

    private TextureMatrix textureMatrix;
    public TexturedSphere sphereL,sphereR;
    private ImageLoader imageLoader;


    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        imageLoader = GetComponent<ImageLoader>();
        textureMatrix = new TextureMatrix(XMin, XMax, YMin, YMax, ZMin, ZMax, toleranceDistanceCells, jumpDelta);

        UpdateActualPosition();
        UpdateTextureMatrix();
    }


    void Update()
    {
        if (IsNewPosition())
        {
            UpdateSpherePosition();

            UpdateActualPosition();

            UpdateShaderTextures();

            UpdateTextureMatrix();
        }
    }


    void UpdateSpherePosition()
    {
        sphereL.MoveSphere(cameraL.position);
        sphereR.MoveSphere(cameraR.position);
    }

    void UpdateActualPosition()
    {
        textureMatrix.UpdateActualPosition(cameraCenter.position);
    }

    bool IsNewPosition()
    {
        return textureMatrix.IsNewPosition(cameraCenter.position);
    }

    void UpdateShaderTextures()
    {
        Vector3Int indexPosL = textureMatrix.PosToIndex(cameraL.position);
        sphereL.UpdateShaderTexture("_Tex000", textureMatrix.Get(indexPosL.x,indexPosL.y,indexPosL.z));

        Vector3Int indexPosR = textureMatrix.PosToIndex(cameraR.position);
        sphereR.UpdateShaderTexture("_Tex000", textureMatrix.Get(indexPosR.x, indexPosR.y, indexPosR.z));
    }

    void UpdateTextureMatrix()
    {
        imageLoader.EmptyImageList();
        Vector3Int indexPos = textureMatrix.GetActualIndexPosition();

        //cuando agrego el eje y ya empieza a andar re mal en el teléfono

        for (int i = indexPos.x - kernelRadioX; (i <= indexPos.x + kernelRadioX) && (i < textureMatrix.GetLengthX()); i++)
            //for (int j = indexPos.y - 1; (j <= indexPos.y + 1) && (j < textureMatrix.GetLengthY()); j++)
                for (int k = indexPos.z - kernelRadioZ; (k <= indexPos.z + kernelRadioZ) && (k < textureMatrix.GetLengthZ()); k++)
                    if ((i >= 0) && (indexPos.y >= 0) && (k >= 0))
                    {
                    if (!textureMatrix.HasTextureLoaded(i, indexPos.y, k))
                            imageLoader.LoadImage(i, indexPos.y, k, textureMatrix);
                    }
        

        textureMatrix.CleanMatrix();

    }




    








}