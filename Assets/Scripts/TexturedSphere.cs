using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexturedSphere : MonoBehaviour
{
    private Material myMaterial;
    public Vector3 position;
    public float minx, miny, minz, sampleDelta;

    // Start is called before the first frame update
    void Awake()
    {
        myMaterial = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3Int output = new Vector3Int();
            //output.x = Convert.ToInt32((position.x - minx) / sampleDelta);
            //output.y = Convert.ToInt32((position.y - miny) / sampleDelta);
            //output.z = Convert.ToInt32((position.z - minz) / sampleDelta);
            output.x = (int)((position.x - minx) / sampleDelta);
            output.y = (int)((position.y - miny) / sampleDelta);
            output.z = (int)((position.z - minz) / sampleDelta);
            Debug.Log(output);
        }
    }

    private float Truncate(float f)
    {
        return Mathf.Floor(f / sampleDelta) * sampleDelta;
    }

    public void UpdateShaderInterpolation(float newX,float newY,float newZ)
    {
        myMaterial.SetFloat("_x", newX);
        myMaterial.SetFloat("_y", newY);
        myMaterial.SetFloat("_z", newZ);
    }

    public void UpdateShaderTexture(string shaderTextureName, Texture2D newTexture)
    {
        myMaterial.SetTexture(shaderTextureName, newTexture);
    }
}
