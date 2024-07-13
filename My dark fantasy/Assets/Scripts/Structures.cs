using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Structures 
{
    public static void MakeTrees(Vector3 pos, int minpos,int maxpos)
    {
        int height = 5,x=(int)pos.x,y=(int)pos.y,z=(int)pos.z;
        for(int i = y; i <y+height; i++)
        {
            Chunk.Voxels[x,i,z] = 7;
        }

        for(int j =y+3; j<y+height; j++){
            for (int l = x - 2; l <= x + 2; l++) {
                for (int k = z - 2; k <= z + 2; k++) {
                    Chunk.Voxels[l, j, k] = 11;
                }
            }
        }
        for (int l = x - 1; l <= x + 1; l++)
        {
            for (int k = z - 1; k <= z + 1; k++)
            {
                Chunk.Voxels[l , y+height, k ] = 11;
            }
        }
        Chunk.Voxels[x-1, y + height+1, z] = 11;
        Chunk.Voxels[x, y + height+1, z-1] = 11;
        Chunk.Voxels[x, y + height+1, z] = 11;
        Chunk.Voxels[x, y + height+1, z+1] = 11;
        Chunk.Voxels[x+1, y + height+1, z] = 11;
    }
}
