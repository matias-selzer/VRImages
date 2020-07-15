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
    private RoomsInformationManager rooms;

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        imageLoader = GetComponent<ImageLoader>();
        rooms = GetComponent<RoomsInformationManager>();
        textureMatrix = new TextureMatrix(XMin, XMax, YMin, YMax, ZMin, ZMax, toleranceDistanceCells, jumpDelta);

        UpdateActualPosition();
        UpdateTextureMatrix();

        //InvokeRepeating("UpdateShaderValues", 0, 0.01f);

    
    }


    void Update()
    {
        //UpdateShaderValues();


        if (IsNewPosition())
        {
            UpdateRoomInformation();

            UpdateSpherePosition();

            UpdateActualPosition();

            UpdateShaderTextures();

            //puse esto
            UpdateShaderValues();

            UpdateTextureMatrix();
        }
        
    }

    void UpdateRoomInformation()
    {
        rooms.UpdateCurrentRoom(cameraCenter.position);
    }


    void UpdateSpherePosition()
    {
        sphereL.MoveSphere(cameraL.position);
        sphereR.MoveSphere(cameraR.position);
        //sphereL.MoveSphere(rooms.GetCenter());
        sphereL.transform.localScale = new Vector3(1, 1, -1) * rooms.GetRadius()*10;
        sphereR.transform.localScale = new Vector3(1, 1, -1) * rooms.GetRadius() * 10;
        //sphereL.MoveSphere(cameraCenter.position);

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
        sphereL.UpdateShaderTexture("_Tex", textureMatrix.Get(indexPosL.x,indexPosL.y,indexPosL.z));

        Vector3Int indexPosR = textureMatrix.PosToIndex(cameraR.position);
        sphereR.UpdateShaderTexture("_Tex", textureMatrix.Get(indexPosR.x, indexPosR.y, indexPosR.z));

        Debug.Log("updating texture");
    }

    public void UpdateShaderValues()
    {
        Vector3 roomCenter = rooms.GetCenter();
        float roomRadius = rooms.GetRadius();

        //Debug.Log("Center: " + roomCenter);
        //Debug.Log("Radius: " + roomRadius);



        Vector3 truncatedL = new Vector3(textureMatrix.Truncate(cameraL.position.x), textureMatrix.Truncate(cameraL.position.y), textureMatrix.Truncate(cameraL.position.z));
        Vector3 truncatedR = new Vector3(textureMatrix.Truncate(cameraR.position.x), textureMatrix.Truncate(cameraR.position.y), textureMatrix.Truncate(cameraR.position.z));

        //Debug.Log("enviando x:"+cameraL.position.x+" - "+truncatedL.x);
        //Debug.Log("z:" + cameraL.position.z + " - " + truncatedL.z);

        sphereL.UpdateShaderVariable("_radio", roomRadius);
        sphereL.UpdateShaderVariable("_centro", roomCenter);
        sphereL.UpdateShaderVariable("_pos", cameraL.position);
        sphereL.UpdateShaderVariable("_posImg", truncatedL);

        sphereR.UpdateShaderVariable("_radio", roomRadius);
        sphereR.UpdateShaderVariable("_centro", roomCenter);
        sphereR.UpdateShaderVariable("_pos", cameraR.position);
        sphereR.UpdateShaderVariable("_posImg", truncatedR);
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