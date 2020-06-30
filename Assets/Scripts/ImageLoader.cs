using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ImageLoader : MonoBehaviour
{
    public string localURL = "D:/xampp2/htdocs/img/pictures2/";
    public string webURL = "http://192.168.0.5:1234/img/pictures5/";
    public bool useWebURL;
    private List<Vector3Int> toLoadImages;
    private TextureMatrix textureMatrix;

    public int maxCoroutinesNumber;
    private int concurrentCoroutinesNumber = 0;

    void Awake()
    {
        toLoadImages = new List<Vector3Int>();
    }

    private void Update()
    {
        if (concurrentCoroutinesNumber < maxCoroutinesNumber)
        {
            ExecuteLoadImage();
        }
    }

    public void EmptyImageList()
    {
        toLoadImages.Clear();
    }

    public void LoadImage(int i, int j, int k, TextureMatrix textureMatrix)
    {
        if (this.textureMatrix == null)
        {
            this.textureMatrix = textureMatrix;
        }
        toLoadImages.Add(new Vector3Int(i, j, k));
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
        concurrentCoroutinesNumber++;

        Vector3 truncatedPos = textureMatrix.IndexToTruncatedPos(i, j, k);

        string imageName = truncatedPos.x + "%20" + truncatedPos.y + "%20" + truncatedPos.z;
        imageName = imageName.Replace('.', ',');
        imageName=imageName+ ".jpg";

        string urlToUse = "";

        if (useWebURL)
        {
            urlToUse = webURL + imageName;
        }
        else
        {
            urlToUse = "file://" + localURL + imageName;
        }

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(webURL + imageName, false);

        yield return www.SendWebRequest();

        while (!www.isDone)
        {
            Debug.Log("is not done yet");
            yield return www;
        }

        if (www.isNetworkError)
        {
            Debug.Log("Error:" + www.error);
            Debug.Log(urlToUse + " ERROR");
        }
        else
        {
            try
            {
                concurrentCoroutinesNumber--;
                Texture2D myTexture = DownloadHandlerTexture.GetContent(www);
                textureMatrix.Set(i, j, k, myTexture);
                //www.Dispose();
                
            }
            catch (InvalidOperationException e)
            {
                Debug.Log("image not found - " + urlToUse + " ERROR");
            }

        }
    }
}
