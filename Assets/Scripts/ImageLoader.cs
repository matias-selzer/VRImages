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
    private Vector3 currentDiscretePosition;

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

    public void UpdateCurrentDiscretePosition(Vector3 currentDiscretePosition)
    {
        this.currentDiscretePosition = currentDiscretePosition;
    }

    public void EmptyImageList()
    {
        //Debug.Log("cantidad antes: " + toLoadImages.Count);
        toLoadImages.Clear();
        //Debug.Log("cantidad despues: " + toLoadImages.Count);
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
        int aux = toLoadImages.Count;
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

        //string imageName = truncatedPos.x + "%20" + truncatedPos.y + "%20" + truncatedPos.z;
        string imageName = currentDiscretePosition.x + "%20" + "1.6" + "%20" + currentDiscretePosition.z;
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

        //Debug.Log("URL: " + urlToUse);

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
                //Debug.Log("ya traje imagen: " + i + ", " + j + ", " + k);
                concurrentCoroutinesNumber--;
                Texture2D myTexture = DownloadHandlerTexture.GetContent(www);
                if (myTexture.width != 8)
                {
                    textureMatrix.Set(i, j, k, myTexture);
                }
                //www.Dispose();
                
            }
            catch (InvalidOperationException e)
            {
                Debug.Log("image not found - " + urlToUse + " ERROR");
            }

        }
    }
}
