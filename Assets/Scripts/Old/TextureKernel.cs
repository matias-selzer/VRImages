using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureKernel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //LoadTextures();
    }


    private void LoadTextures()
    {

    }

    public Texture2D[] GetTextures(Vector3 cameraPos)
    {
        Texture2D[] textures = new Texture2D[8];
       // StartCoroutine(LoadLocalTextureWebRequest());

        return textures;
    }
}
