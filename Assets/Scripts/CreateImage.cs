using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CreateImage : MonoBehaviour
{
    public GameObject camera;
    public string url= "D:\\xampp2\\htdocs\\img\\pictures\\";
    public float jumpDelta;
    public float xMin, xMax, yMin, yMax, zMin, zMax;
    public int imageSize;
    private float x, y, z;

    private void Start()
    {
        x = xMin;
        y = yMin;
        //z = zMin-1;
        z = zMin - jumpDelta;

    }

    private string FloatToString(float f)
    {
        return (Mathf.Floor(f / jumpDelta) * jumpDelta)+"";
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //CreateSomeImages();
            SaveImage();
        }
    }


    public void SaveImage()
    {
        //z++;
        z += jumpDelta;

        if (z >= zMax)
        {
            z = zMin;
            //y++;
            y += jumpDelta;
        }

        if (y >= yMax)
        {
            y = yMin;
            //x++;
            x += jumpDelta;
        }

        if (x < xMax)
        {
            Invoke("SaveImage", 0.1f); //funciono con 0.005
            string route = url;
            string name = route+FloatToString(x)+" "+FloatToString(y)+" "+FloatToString(z)+".jpg";
            camera.transform.position = new Vector3(x, y, z);
            //Debug.Log("Comienza imagen");
            File.WriteAllBytes(name, I360Render.Capture(imageSize, true, camera.GetComponent<Camera>(), true));
            //Debug.Log("Termina imagen: " + name);
        }
        else
        {
            Debug.Log("termine");
        }

    }




}
