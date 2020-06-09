using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadImageURL : MonoBehaviour
{
    public string url = "http://localhost:1234/img/pictures3/";
    private string x, y, z;
    public float jumpDelta;
    public Transform target;


    private string Truncate(float f)
    {
        return (Mathf.Floor(f / jumpDelta) * jumpDelta) + "";
    }

    IEnumerator LoadTexture()
    {
        Texture2D tex;
        tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        string imageName = x + "%20" + y + "%20" + z + ".jpg";
        string newUrl = url + imageName;
        //Debug.Log(newUrl);
        using (WWW www = new WWW(newUrl))
        {
            yield return www;
            www.LoadImageIntoTexture(tex);
            //www.Dispose();
            //DestroyImmediate(GetComponent<Renderer>().material.mainTexture);
            GetComponent<Renderer>().material.mainTexture = tex;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdatePositions();
    }

    void UpdatePositions()
    {
        x = Truncate(-transform.position.x);
        y = Truncate(transform.position.y);
        z = Truncate(transform.position.z);
    }

    bool IsNewPosition()
    {
        return Truncate(transform.position.x) != x || Truncate(transform.position.y) != y || Truncate(transform.position.z) != z;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position;
        //Debug.Log(transform.name+" - "+target.position);
        //Debug.Log(x + " - " + y + " " + z);
        if (IsNewPosition())
        {
            UpdatePositions();
            StartCoroutine(LoadTexture());
        }
    }
}
