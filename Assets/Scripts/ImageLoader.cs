using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ImageLoader : MonoBehaviour
{
    public string url = "http://localhost:1234/img/pictures3/";
    private List<Vector3Int> toLoadImages;
    private TextureMatrix textureMatrix;

    public float loadImageIntervalTime;

    void Awake()
    {
        toLoadImages = new List<Vector3Int>();
        //InvokeRepeating("ExecuteLoadImage", 0, loadImageIntervalTime);
    }

    public void LoadImage(int i, int j, int k, TextureMatrix textureMatrix)
    {
        if (this.textureMatrix == null)
        {
            this.textureMatrix = textureMatrix;
        }
        //toLoadImages.Add(new Vector3Int(i, j, k));
        StartCoroutine(LoadLocalTextureWebRequestInMatrix(i, j, k, textureMatrix));
    
    }

    void ExecuteLoadImage()
    {
        if (toLoadImages.Count > 0)
        {
            Vector3Int imageToLoad = toLoadImages[0];
            toLoadImages.RemoveAt(0);
            StartCoroutine(LoadLocalTextureWebRequestInMatrix(imageToLoad.x, imageToLoad.y, imageToLoad.z, textureMatrix));
        }
    }

    IEnumerator LoadLocalTextureWebRequestInMatrix(int i, int j, int k, TextureMatrix textureMatrix)
    {
        Vector3 truncatedPos = textureMatrix.IndexToTruncatedPos(i, j, k);

        string imageName = truncatedPos.x + "%20" + truncatedPos.y + "%20" + truncatedPos.z;
        imageName = imageName.Replace('.', ',');
        imageName=imageName+ ".jpg";
        string newUrl = url + imageName;

        //versión web
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("http://192.168.0.5:1234/img/pictures2/" + imageName, false);

        //UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + newUrl, false);
        yield return www.SendWebRequest();

        while (!www.isDone)
        {
            yield return www;
        }

        if (www.isNetworkError)
        {
            Debug.Log("Error:" + www.error);
            Debug.Log(newUrl + " ERROR");
        }
        else
        {
            try
            {
                Texture2D myTexture = DownloadHandlerTexture.GetContent(www);
                textureMatrix.Set(i, j, k, myTexture);
                www.Dispose();
            }
            catch (InvalidOperationException e)
            {
                Debug.Log("image not found - " + newUrl + " ERROR");
            }
        }
    }
}
