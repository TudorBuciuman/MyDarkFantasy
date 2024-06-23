using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Voxeldata
{
    public static readonly Vector3[] voxelVerts = new Vector3[8]
    {
        new Vector3(0.0f, 0.0f,0.0f),
        new Vector3(0.0f, 1.0f,0.0f),
        new Vector3(0.0f, 0.0f,1.0f),
        new Vector3(0.0f, 1.0f,1.0f),
        new Vector3(0.0f, 0.0f,1.0f),
        new Vector3(0.0f, 1.0f,1.0f),
        new Vector3(0.0f, 0.0f,1.0f),
        new Vector3(0.0f, 1.0f,1.0f),
    };

    public static readonly int[,] VoxelTris = new int[1, 6]
    {
        {3,7,2,2,7,6 }
    };
    public class Voxel
    {
        public Vector3 position;
        public int type; // 0 = air, 1 = grass, 2 = stone, etc.

        public Voxel(Vector3 pos, int type)
        {
            this.position = pos;
            this.type = type;
        }
    }
}
