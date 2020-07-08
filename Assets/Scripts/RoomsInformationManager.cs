using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RoomsInformationManager : MonoBehaviour
{
    public string fileURL;
    private List<RoomInfo> rooms;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("LoadTextWebRequest");
    }


    void ParseRoomsInformation(string s)
    {
        s=s.Replace('.', ',');
        rooms = new List<RoomInfo>();
        string[] separator = new string[] { "$$$" };
        string[] eachRoomData = s.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        foreach(string roomData in eachRoomData)
        {
            float xmin = 0, xmax = 0, ymin = 0, ymax = 0, zmin = 0, zmax=0;
            string[] lines = roomData.Split('\n');
            foreach(string line in lines)
            {
                if (line.Split('=')[0].Equals("xmin"))
                {
                    xmin = float.Parse(line.Split('=')[1]);
                }else if (line.Split('=')[0].Equals("xmax"))
                {
                    xmax = float.Parse(line.Split('=')[1]);
                }
                else if (line.Split('=')[0].Equals("ymin"))
                {
                    ymin = float.Parse(line.Split('=')[1]);
                }
                else if (line.Split('=')[0].Equals("ymax"))
                {
                    ymax = float.Parse(line.Split('=')[1]);
                }
                else if (line.Split('=')[0].Equals("zmin"))
                {
                    zmin = float.Parse(line.Split('=')[1]);
                }
                else if (line.Split('=')[0].Equals("zmax"))
                {
                    zmax = float.Parse(line.Split('=')[1]);
                }
            }
            RoomInfo newRoom = new RoomInfo(xmin, xmax, ymin, ymax, zmin, zmax);
            rooms.Add(newRoom);
        }
        Debug.Log("Total Habitaciones: "+rooms.Count);
    }

    IEnumerator LoadTextWebRequest()
    {
        //UnityWebRequest www = UnityWebRequestTexture.GetTexture(webURL + imageName, false);
        UnityWebRequest www = UnityWebRequest.Get(fileURL);

        yield return www.SendWebRequest();

        while (!www.isDone)
        {
            Debug.Log("is not done yet");
            yield return www;
        }

        if (www.isNetworkError)
        {
            Debug.Log("Error:" + www.error);
        }
        else
        {
            try
            {
                //Debug.Log("Traje el texto:" + www.downloadHandler.text);
                ParseRoomsInformation(www.downloadHandler.text);
                //www.Dispose();

            }
            catch (InvalidOperationException e)
            {
                Debug.Log("file not found - " + fileURL + " ERROR");
            }

        }
    }
}
