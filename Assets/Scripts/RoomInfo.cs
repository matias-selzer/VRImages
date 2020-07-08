using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo 
{
    private float xmin, xmax, ymin, ymax, zmin, zmax;
    private Vector3 center;
    private float radius;

    public RoomInfo(float xmin,float xmax,float ymin,float ymax,float zmin, float zmax)
    {
        this.xmin = xmin;
        this.xmax = xmax;
        this.ymin = ymin;
        this.ymax = ymax;
        this.zmin = zmin;
        this.zmax = zmax;

        center = new Vector3(xmax - xmin, ymax - ymin, zmax - zmin);

        radius = Vector3.Distance(center, new Vector3(xmax, ymax, zmax));
    }

    public bool IsInside(Vector3 pos)
    {
        return pos.x >= xmin && pos.x < xmax && pos.y >= ymin && pos.y < ymax && pos.z >= zmin && pos.z < zmax;
    }

    public float GetRadius()
    {
        return radius;
    }

    public Vector3 GetCenter()
    {
        return center;
    }

}
