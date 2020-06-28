﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexturedSphere : MonoBehaviour
{
    private Material myMaterial;
    private Vector3 oldPos;

    // Start is called before the first frame update
    void Awake()
    {
        myMaterial = GetComponent<Renderer>().material;
        oldPos = transform.position;
    }


    public void UpdateShaderInterpolation(float newX,float newY,float newZ)
    {
        myMaterial.SetFloat("_x", newX);
        myMaterial.SetFloat("_y", newY);
        myMaterial.SetFloat("_z", newZ);
    }

    public void UpdateShaderTexture(string shaderTextureName, Texture2D newTexture)
    {
        //myMaterial.SetTexture(shaderTextureName, newTexture);
        myMaterial.mainTexture = newTexture;
        
    }

    public void MoveSphere(Vector3 newPos)
    {
        //esto para ver que onda
        //Vector3 newPos = transform.position;
        /*
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
            newPos.z = target.position.z;
        }
        else if (MovedBackward())
        {
            newPos.z = target.position.z - jumpDelta;
        }
        */
        transform.position = newPos;
    }
}