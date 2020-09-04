using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position
{
    private Vector3 position; //posición en el mundo
    private Vector3 discretePosition;
    private Vector3 nearestDiscretePosition;
    private Vector3Int currentIndex;
    private Vector3Int nearestIndex;

    private Vector3 mins;
    private float delta;

    public Position(Vector3 worldPosition,Vector3 mins, float delta)
    {
        this.mins = mins;
        this.delta = delta;
        position = worldPosition;
        currentIndex=PosToIndex(position);
        discretePosition = PosToDiscretePos(position);
        nearestDiscretePosition=PosToNearestDiscretePosition(position);
        nearestIndex = PosToIndex(discretePosition);
        //parche
        nearestIndex.y = 0;
    }

    public Vector3 GetPosition (){ return position; }
    public Vector3 GetDiscretePosition() { return discretePosition;  }
    public Vector3Int GetCurrentIndex() { return currentIndex; }
    public Vector3Int GetNearestIndex() { return nearestIndex; }
    public Vector3 GetNearestDiscretePosition() { return nearestDiscretePosition; }

    private Vector3Int PosToIndex(Vector3 pos)
    {
        Vector3Int newIndex = new Vector3Int();
        newIndex.x = Mathf.FloorToInt((pos.x - mins.x) / delta);
        newIndex.y = Mathf.FloorToInt((pos.y - mins.y) / delta);
        newIndex.z = Mathf.FloorToInt((pos.z - mins.z) / delta);
        return newIndex;
    }

    private Vector3 PosToDiscretePos(Vector3 position)
    {
        Vector3 discretePosition = new Vector3();
        discretePosition.x = DiscreteFloor(position.x);
        discretePosition.y = DiscreteFloor(position.y);
        discretePosition.z = DiscreteFloor(position.z);
        return discretePosition;
    }

    private Vector3 PosToNearestDiscretePosition(Vector3 position)
    {
        Vector3 nearestDiscretePosition = new Vector3();
        nearestDiscretePosition.x = DiscreteRound(position.x);
        nearestDiscretePosition.y = DiscreteRound(position.y);
        nearestDiscretePosition.z = DiscreteRound(position.z);
        return nearestDiscretePosition;
    }

    public float DiscreteFloor(float f)
    {
        return Mathf.Floor(f / delta) * delta;
    }

    private float DiscreteRound(float f)
    {
        return Mathf.Round(f / delta) * delta;
    }




    public void ToString()
    {
        Debug.Log("Current position: " + position);
        Debug.Log("Discrete position: " + discretePosition);
        Debug.Log("Current Index: " + currentIndex);
        Debug.Log("Nearest Index: " + nearestIndex);
        Debug.Log("********************************");

    }


    /*
    public Vector3 IndexToPos(int i, int j, int k)
    {
        float x = (i * sampleDelta) + minx;
        float y = (j * sampleDelta) + miny;
        float z = (k * sampleDelta) + minz;
        return new Vector3(x, y, z);
    }*/
    /*
    public Vector3 IndexToTruncatedPos(int i, int j, int k)
    {
        float x = Truncate((i * sampleDelta) + minx);
        float y = Truncate((j * sampleDelta) + miny);
        float z = Truncate((k * sampleDelta) + minz);
        return new Vector3(x, y, z);
    }*/





}
