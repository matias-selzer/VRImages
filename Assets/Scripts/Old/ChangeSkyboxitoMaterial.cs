using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSkyboxitoMaterial : MonoBehaviour
{
    public GameObject camera;
    public Texture[] images;
    private Material myMaterial;
    public Material materialA,materialB;

    // Start is called before the first frame update
    void Start()
    {
        myMaterial = camera.GetComponent<Skybox>().material;
        //camera.GetComponent<Skybox>().material.mainTexture = imagencita;
    }

    // Update is called once per frame
    void Update()
    {
        int index = ((int)(camera.transform.position.z * 2.0f));
        Debug.Log(index);
        materialA.mainTexture = myMaterial.mainTexture;
        if(index>0 && index < images.Length)
        {
            myMaterial.mainTexture = images[index];
            //materialB.mainTexture = images[index];
            //float lerp = (camera.transform.position.z * 2.0f)-index;
            //myMaterial.Lerp(materialA, materialB,lerp);
        }
       
    }
}
