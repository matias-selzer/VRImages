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

    private Position oldPosition;
    private Position currentPosition;

    public ScreenTouchMove cameraMover;

    public bool itsTime = false;

    void TimeToMove()
    {
        itsTime = true;
    }

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        imageLoader = GetComponent<ImageLoader>();
        rooms = GetComponent<RoomsInformationManager>();
        textureMatrix = new TextureMatrix(XMin, XMax, YMin, YMax, ZMin, ZMax, toleranceDistanceCells, jumpDelta);

        currentPosition = new Position(cameraCenter.position, new Vector3(XMin, YMin, ZMin), jumpDelta);
        oldPosition = new Position(cameraCenter.position, new Vector3(XMin, YMin, ZMin), jumpDelta);

        Invoke("TimeToMove", 5);
    }


    void Update()
    {
        //Debug.Log("hola: "+cameraCenter.position);
        currentPosition = new Position(cameraCenter.position, new Vector3(XMin, YMin, ZMin), jumpDelta);

        //currentPosition.ToString();

        UpdateShaderValues();



        if (itsTime)
        {
            bool hasImage= textureMatrix.HasTextureLoaded(currentPosition.GetCurrentIndex().x, currentPosition.GetCurrentIndex().y, currentPosition.GetCurrentIndex().z);
            cameraMover.canMove = hasImage;
            if (!hasImage) cameraMover.GoBack();
            //Debug.Log(currentPosition.GetCurrentIndex()+" - "+textureMatrix.HasTextureLoaded(currentPosition.GetCurrentIndex().x, currentPosition.GetCurrentIndex().y, currentPosition.GetCurrentIndex().z));
        }




        if (IsNewNearestImage())
        {
            //currentPosition.ToString();

            UpdateShaderTextures();

            UpdateRoomInformation();

            UpdateSpherePosition();

            UpdateTextureMatrix();
        }
       
    }

    bool IsNewNearestImage()
    {
        bool isNew = !currentPosition.GetNearestDiscretePosition().Equals(oldPosition.GetNearestDiscretePosition());
        if (isNew)
        {
            oldPosition = currentPosition;
        }
        return isNew;
    }

    void UpdateRoomInformation()
    {
        rooms.UpdateCurrentRoom(cameraCenter.position);
    }


    void UpdateSpherePosition()
    {
        sphereL.MoveSphere(cameraCenter.position);
        sphereL.transform.localScale = new Vector3(1, 1, -1) * rooms.GetRadius() * 1.2f;
    }


    void UpdateShaderTextures()
    {
        //Debug.Log(textureMatrix.Get(currentPosition.GetNearestIndex()) == null);
        sphereL.UpdateShaderTexture("_Tex", textureMatrix.Get(currentPosition.GetNearestIndex()));
    }

    public void UpdateShaderValues()
    {
        Vector3 roomCenter = rooms.GetCenter();
        float roomRadius = rooms.GetRadius();

        sphereL.UpdateShaderVariable("_radio", 1000);
        sphereL.UpdateShaderVariable("_centro", roomCenter);
        sphereL.UpdateShaderVariable("_pos", currentPosition.GetPosition());
        sphereL.UpdateShaderVariable("_posImg", currentPosition.GetNearestDiscretePosition());

    }

    void UpdateTextureMatrix()
    {
        //Debug.Log("nueva discrete position: "+ currentPosition.GetDiscretePosition());
        imageLoader.EmptyImageList();
        Vector3Int indexPos = currentPosition.GetCurrentIndex();

        imageLoader.UpdateCurrentDiscretePosition(currentPosition.GetDiscretePosition());

        //cuando agrego el eje y ya empieza a andar re mal en el teléfono
        for (int i = indexPos.x - kernelRadioX; (i <= indexPos.x + kernelRadioX) && (i < textureMatrix.GetLengthX()); i++)
        {
            //for (int j = indexPos.y - 1; (j <= indexPos.y + 1) && (j < textureMatrix.GetLengthY()); j++)
            for (int k = indexPos.z - kernelRadioZ; (k <= indexPos.z + kernelRadioZ) && (k < textureMatrix.GetLengthZ()); k++)
            {
                if ((i >= 0) && (indexPos.y >= 0) && (k >= 0))
                {
                    if (!textureMatrix.HasTextureLoaded(i, indexPos.y, k))
                    {
                        
                        imageLoader.LoadImage(i, indexPos.y, k, textureMatrix);
                    }
                }
            }
        }

        textureMatrix.CleanMatrix(currentPosition.GetCurrentIndex());
    }




    








}