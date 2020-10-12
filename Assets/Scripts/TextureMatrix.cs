using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureMatrix 
{
    private Texture2D[,,] matrix;
    private List<Vector3Int> visitedPositions;
    private float sampleDelta;
    private float minx, miny, minz;
    private int toleranceDistance;
    private int timeToClean=0;

    public TextureMatrix(float minx,float maxx,float miny,float maxy,float minz,float maxz,int distance,float sampleDelta)
    {
        //cambié ConvertToIn32 por Math.ceiling porque antes daba 0 para valores < 0.5 (delta mayor a 0.2)
        int cantx = (int) Math.Ceiling((maxx - minx) / sampleDelta);
        int canty = (int)Math.Ceiling((maxy - miny) / sampleDelta);
        int cantz = (int)Math.Ceiling((maxz - minz) / sampleDelta);
        this.minx = minx;
        this.miny = miny;
        this.minz = minz;
        matrix = new Texture2D[cantx, canty, cantz];
        visitedPositions = new List<Vector3Int>();
        this.toleranceDistance = distance;
        this.sampleDelta = sampleDelta;
    }

    public bool HasTextureLoaded(int i, int j, int k)
    {
        return matrix[i, j, k] != null;
    }

    public Texture2D Get(int i, int j, int k)
    {
        //retorna textura negra cuando no existe
        try
        {
            return matrix[i, j, k];
        }
        catch (IndexOutOfRangeException e)
        {
            return Texture2D.blackTexture;
        }
    }

    public Texture2D Get(Vector3Int v)
    {
        return this.Get(v.x, v.y, v.z);
    }

    public void Set(int i,int j,int k, Texture2D newTexture)
    {
        matrix[i, j, k] = newTexture;
        visitedPositions.Add(new Vector3Int(i,j,k));
    }


    public void CleanMatrix(Vector3Int actualIndexPosition)
    {
        timeToClean=(timeToClean+1)%1;
        if (timeToClean == 0) {
            //esto es así porque voy borrando elementos de la lista mientras la recorro
            for (int i = visitedPositions.Count - 1; i >= 0; i--)
            {
                if (Vector3Int.Distance(visitedPositions[i], actualIndexPosition) > toleranceDistance)
                {
                    RemoveFromMatrix(visitedPositions[i]);
                    RemoveVisitedPosition(i);
                }
            }
            //si esto no va acá se rompe todo no sé por qué
            ReleaseData();
        }
    }

    public void ReleaseData()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    public bool HasActualTexture(Vector3Int indexPosition)
    {
        return Get(indexPosition.x, indexPosition.y, indexPosition.z) != null;
    }

    private void RemoveFromMatrix(Vector3Int pos)
    {
        matrix[pos.x, pos.y, pos.z] = null;
    }

    private void RemoveVisitedPosition(int index)
    {
        visitedPositions.RemoveAt(index);
    }

    public int GetLengthX()
    {
        return matrix.GetLength(0);
    }

    public int GetLengthY()
    {
        return matrix.GetLength(1);
    }

    public int GetLengthZ()
    {
        return matrix.GetLength(2);
    }
}
