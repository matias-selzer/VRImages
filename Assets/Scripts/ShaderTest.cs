using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderTest : MonoBehaviour
{
    private Material myMaterial;
    private Vector3 cameraPos;
    private bool forward = true;
    private float z = 0;
    public float timeStep;
    public Texture2D texturita;

    // Start is called before the first frame update
    void Start()
    {
        myMaterial = GetComponent<Renderer>().material;
        cameraPos = new Vector3(-5, 1.5f, -5);
        InvokeRepeating("UpdateValues", 0, timeStep);
    }


    void UpdateValues()
    {
        if (forward)
        {
            z += timeStep;
            if (z >= 1) forward = false;
        }
        else
        {
            z -= timeStep;
            if (z <= 0) forward = true;
        }
        cameraPos.z = -5 + z;

        float roomRadius = 5;
        Vector3 roomCenter = new Vector3(-6, 1.5f, -6);
        UpdateShaderVariable("_radio", roomRadius);
        UpdateShaderVariable("_centro", roomCenter);
        UpdateShaderVariable("_pos", cameraPos);
        Vector3 truncatedPos = new Vector3(-5, 1.5f, -5);
        UpdateShaderVariable("_posImg", truncatedPos);

        myMaterial.SetTexture("_Tex", texturita);
    }

    public void UpdateShaderVariable(string variableName, float f)
    {
        myMaterial.SetFloat(variableName, f);
    }

    public void UpdateShaderVariable(string variableName, Vector3 v)
    {
        myMaterial.SetVector(variableName, new Vector4(v.x, v.y, v.z, 1));
    }
}
