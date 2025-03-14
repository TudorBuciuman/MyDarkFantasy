using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Structures 
{
    public static void MakeStructures(byte biome,Vector3 pos, Vector2 offset)
    {
        
        if (biome == 0)
            MakeCacti(pos, offset);
        else if (biome > 1)
        {
        
            MakeTrees(pos, offset);
        }
    }
    public static void MakeTrees(Vector3 pos, Vector2 offset)
    {
        int height = 5, x = (int)pos.x, y = (int)pos.y, z = (int)pos.z;
        
        if ((Noise.GetThe2DPerlin(new Vector2(pos.x, pos.z), new Vector2(offset.x, offset.y), 0.4f, 0.8f)))
            height += 2;
        for (int j =y+4; j<y+height; j++){
            for (int l = x - 2; l <= x + 2; l++) {
                for (int k = z - 2; k <= z + 2; k++) {
                    if(l>=0 &&  k>=0) 
                           WorldManager.GetChunk(l / 16, k / 16 ).Voxels[l % 16, j, k % 16].Value1 = 11;
                    else if(l>=0 && k < 0)
                    {
                        if(k%16==0)
                            WorldManager.GetChunk(l / 16, k / 16).Voxels[l % 16, j, 0].Value1 = 11;
                        else
                            WorldManager.GetChunk(l / 16, k / 16 - 1).Voxels[l % 16, j, 16 - (-k % 16)].Value1 = 11;
                    }
                    else if(l<0 && k < 0)
                    {
                        if (k % 16 == 0 && l%16==0)
                            WorldManager.GetChunk(l / 16, k / 16).Voxels[0, j, 0].Value1 = 11;
                        else if(k%16==0)
                            WorldManager.GetChunk(l / 16 - 1, k / 16).Voxels[16 - (-l % 16), j, 0].Value1 = 11;
                        else if(l%16==0)
                            WorldManager.GetChunk(l / 16, k / 16 - 1).Voxels[0, j, 16 - (-k % 16)].Value1 = 11;
                        else
                            WorldManager.GetChunk(l / 16 - 1, k / 16 - 1).Voxels[16 - (-l % 16), j, 16 - (-k % 16)].Value1 = 11;
                    }
                    else if (l < 0 && k >= 0)
                    {
                        if (l % 16 == 0)
                            WorldManager.GetChunk(l / 16, k / 16).Voxels[0, j, k % 16].Value1 = 11;
                        else
                            WorldManager.GetChunk(l / 16 - 1, k / 16).Voxels[16 - (-l % 16), j, k % 16].Value1 = 11;
                    }
                }
            }
        }

        for (int l = x - 2; l <= x + 2; l++)
        {
            for (int k = z - 2; k <= z + 2; k++)
            {
                WorldManager.SetTo(l, y + 3, k, 11);

            }
        }
        for (int l = x - 1; l <=x + 1; l++)
        {
            for (int k = z - 1; k <= z + 1; k++)
            {
                WorldManager.SetTo(l, y + height, k, 11);
            }
        }
        for (int i = y; i <= y + height; i++)
        {
            WorldManager.SetTo(x, i, z, 7);
        }
        WorldManager.SetTo(x -1, y+height+1, z, 11);
        WorldManager.SetTo(x, y + height + 1, z -1, 11);
        WorldManager.SetTo(x , y + height + 1, z, 11);
        WorldManager.SetTo(x , y + height + 1, z +1, 11);
        WorldManager.SetTo(x + 1, y + height + 1, z, 11);
        WorldManager.UpdateMesh(x - 2, z - 2);
        WorldManager.UpdateMesh(x - 2, z + 2);
        WorldManager.UpdateMesh(x + 2, z + 2);
        WorldManager.UpdateMesh(x + 2, z - 2);
    }
    //pluralul de la cactus
    public static void MakeCacti(Vector3 pos,Vector2 offset)
    {
        byte a = 1;
        int b=(int)pos.x;
        int c=(int)pos.z;
        if (b < 0 && b % 16 != 0)
        {
            b = 16 - (-c % 16);
            pos.x -=16;
        }
        if (c < 0 && c % 16 != 0)
        {
            c = 16 - (-c % 16);
            pos.z-=16;
        }
        if ((Noise.GetThe2DPerlin(new Vector2(pos.x, pos.z), new Vector2(offset.x, offset.y), 0.2f, 0.9f)))
        a = 2;
            
        for (int i = (int)pos.y; i <= (int)pos.y + a; i++)
        {
            WorldManager.GetChunk((int)pos.x / 16, (int)pos.z / 16).Voxels[b % 16, i, c % 16].Value1 = 12;
        }
        
    }
}
