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
    private Vector3Int actualIndexPosition;

    public TextureMatrix(float minx,float maxx,float miny,float maxy,float minz,float maxz,float distance,float sampleDelta)
    {
        int cantx = Convert.ToInt32((maxx - minx) / sampleDelta);
        int canty = Convert.ToInt32((maxy - miny) / sampleDelta);
        int cantz = Convert.ToInt32((maxz - minz) / sampleDelta);
        this.minx = minx;
        this.miny = miny;
        this.minz = minz;
        matrix = new Texture2D[cantx, canty, cantz];
        visitedPositions = new List<Vector3Int>();
        //this.toleranceDistance = distance;
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

    public void Set(int i,int j,int k, Texture2D newTexture)
    {
        matrix[i, j, k] = newTexture;
        //agregue esto para que la lista de visitados tenga cada posición con textura
        visitedPositions.Add(new Vector3Int(i,j,k));
       // Debug.Log("visitados: " + visitedPositions.Count);
    }


    public Vector3Int PosToIndex(Vector3 position)
    {
        Vector3Int output = new Vector3Int();
        //esto redondea al más cercano
        //output.x = Convert.ToInt32((position.x - minx) / sampleDelta);
        //output.y = Convert.ToInt32((position.y - miny) / sampleDelta);
        //output.z = Convert.ToInt32((position.z - minz) / sampleDelta);
        //así trunca para abajo
        output.x = (int)((position.x - minx) / sampleDelta);
        output.y = (int)((position.y - miny) / sampleDelta);
        output.z = (int)((position.z - minz) / sampleDelta);

        return output;
    }

    public Vector3 IndexToTruncatedPos(int i, int j, int k)
    {
        float x = Truncate((i * sampleDelta) + minx);
        float y = Truncate((j * sampleDelta) + miny);
        float z = Truncate((k * sampleDelta) + minz);
        //ver que onda con los decimales (pareciera que anda bien)
        return new Vector3(x, y, z);
    }

    private float Truncate(float f)
    {
        return Mathf.Floor(f / sampleDelta) * sampleDelta;
    }

    public bool IsNewPosition(Vector3 position)
    {
        Vector3Int newPos = PosToIndex(position);
        return !(newPos.x == actualIndexPosition.x && newPos.y == actualIndexPosition.y && newPos.z == actualIndexPosition.z);
    }

    public void UpdateActualPosition(Vector3 position)
    {
        Vector3Int newPosition = PosToIndex(position);
        actualIndexPosition = newPosition;
        Debug.Log(position+" - "+newPosition);
    }

    public Vector3Int GetActualIndexPosition()
    {
        return actualIndexPosition;
    }

    public void CleanMatrix()
    {
        Debug.Log("visitados antes de borrar: " + visitedPositions.Count);
        //esto es así porque voy borrando elementos de la lista mientras la recorro
        for (int i = visitedPositions.Count - 1; i >= 0; i--)
        {
            if (Vector3Int.Distance(visitedPositions[i], actualIndexPosition) > 3f)
            {
                //Debug.Log(actualIndexPosition+" : " +visitedPositions[i]);
                //sDebug.Log("actualPos: "+actualIndexPosition);
                RemoveFromMatrix(visitedPositions[i]);
                RemoveVisitedPosition(i);
            }
        }
        //si esto no va acá se rompe todo no sé por qué
        ReleaseData();

        Debug.Log("visitados despues de borrar: " + visitedPositions.Count);
    }

    public void ReleaseData()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    public bool HasActualTexture()
    {
        return Get(actualIndexPosition.x, actualIndexPosition.y, actualIndexPosition.z) != null;
    }


    private void RemoveFromMatrix(Vector3Int pos)
    {
        matrix[pos.x, pos.y, pos.z] = null;
    }

    private void RemoveVisitedPosition(int index)
    {
        visitedPositions.RemoveAt(index);
    }
}
