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
        myMaterial.SetTexture(shaderTextureName, newTexture);
        //myMaterial.mainTexture = newTexture;
        
    }

    public void MoveSphere(Vector3 newPos)
    {
        transform.position = newPos;
    }


    public void UpdateShaderVariable(string variableName,float f)
    {
        //f=Mathf.Round(f * 100f) / 100f;
        myMaterial.SetFloat(variableName,f);
    }

    public void UpdateShaderVariable(string variableName,Vector3 v)
    {
        myMaterial.SetVector(variableName, new Vector4(v.x, v.y, v.z, 1));
    }

}
