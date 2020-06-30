using System;
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

    public void UpdateShaderTexture(string shaderTextureName, Texture2D newTexture)
    {
        //myMaterial.SetTexture(shaderTextureName, newTexture);
        myMaterial.mainTexture = newTexture;
        
    }

    public void MoveSphere(Vector3 newPos)
    {
        transform.position = newPos;
    }

    //acá hay que hacer algo más genérico
    public void UpdateShaderInterpolation(float newX, float newY, float newZ)
    {
        myMaterial.SetFloat("_x", newX);
        myMaterial.SetFloat("_y", newY);
        myMaterial.SetFloat("_z", newZ);
    }

}
