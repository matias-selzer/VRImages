using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixCreator : MonoBehaviour
{
    public GameObject cube,sphere;
    public float cantx,canty,cantz;
    public Texture[] textures;

    // Start is called before the first frame update
    void Start()
    {
        CreateMatrix();
        CreateSpheresMatrix();
    }

    void CreateSpheresMatrix()
    {
        for (int i = 0; i < cantx; i++)
        {
            for (int j = 0; j < canty; j++)
            {
                for (int k = 0; k < cantz; k++)
                {
                    Vector3 newPos = new Vector3(i, j, k);
                    GameObject newSphere = Instantiate(sphere, newPos, Quaternion.Euler(0,Random.Range(0,360),0));
                    //newSphere.transform.localScale *= 0.7f;
                    newSphere.GetComponent<Renderer>().material.mainTexture = textures[Random.Range(0, textures.Length)];
                }
            }
        }
    }

    void CreateMatrix()
    {
        for(int i = 0; i < cantx; i++)
        {
            for(int j = 0; j < canty; j++)
            {
                for(int k = 0; k < cantz; k++)
                {
                    Vector3 newPos = new Vector3(i, j, k);
                    GameObject newCube = Instantiate(cube, newPos, transform.rotation);
                }
            }
        }
    }
}
