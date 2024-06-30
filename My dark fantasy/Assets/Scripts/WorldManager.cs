using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public PerlinGenerator PerlinGen;
    public CameraController CImp;
    public Material material;
    public BlockProprieties[] blockTypes;
    List<ChunkCoord> activechunks =new List<ChunkCoord>();

    Chunk[,] chunks=new Chunk[Voxeldata.SizeOfWorld, Voxeldata.SizeOfWorld];
    private void Start()
    {
        GenerateWorld();
    }

    private void Update()
    {
        CheckViewDistance();
    }
    private bool IsChunkInWorld(ChunkCoord coord)
    {
        if(GameObject.Find("Chunk "+coord.x.ToString()+" "+coord.y.ToString())!=null)
        return true;
        return false;
    }

    private void CreateNewChunk(int x,int y)
    {
        Chunk chunk= new Chunk(new ChunkCoord(x, y), this);
        activechunks.Add(new ChunkCoord(x, y));
    }

    public void GenerateWorld()
    {
        for(int i = -Voxeldata.NumberOfChunks; i < Voxeldata.NumberOfChunks; i++)
        {
            for(int j = -Voxeldata.NumberOfChunks; j < Voxeldata.NumberOfChunks; j++) {
                CreateNewChunk(i, j);
            }
        }

    }
    public int lastX=0,lastY=0;
    public void CheckViewDistance()
    {
        ChunkCoord plpos = CImp.GetPosition();
        plpos = new ChunkCoord(plpos.x/16, plpos.y/16);
        List<ChunkCoord> notactivechunks = new (activechunks);

        for(int x = plpos.x-Voxeldata.NumberOfChunks; x<=plpos.x+Voxeldata.NumberOfChunks; x++)
        {
            for(int z=plpos.y-Voxeldata.NumberOfChunks; z<=plpos.y+Voxeldata.NumberOfChunks; z++)
            {
                if(!IsChunkInWorld(new ChunkCoord(x, z)))
                {
                    CreateNewChunk(x,z);
                }
                for(int i=0; i<notactivechunks.Count; i++)
                {
                    if (notactivechunks[i].x==x && notactivechunks[i].y==z)
                    {
                        notactivechunks.RemoveAt(i);
                    }

                }
            }
        }

        for(int i = 0; i < notactivechunks.Count; i++)
        {
            Destroy(GameObject.Find("Chunk " + notactivechunks[i].x.ToString() + " " + notactivechunks[i].y.ToString()));
            activechunks.Remove(notactivechunks[i]);
        }
    }
}
[System.Serializable]
public class BlockProprieties
{
    public string Name;
    public byte place;

    [Header("Textures")]
    public int backfacetexture;
    public int frontfacetexture;
    public int topfacetexture;
    public int bottomfacetexture;
    public int leftfacetexture;
    public int rightfacetexture;

    public int GetTextureID(byte index)
    {
        switch (index)
        {
            case 0:
            return frontfacetexture;
            case 1:
            return backfacetexture;
            case 2:
            return rightfacetexture;
            case 3:
            return leftfacetexture;
            case 4:
            return topfacetexture;
            case 5:
            return bottomfacetexture;
            default:
            return -1;
        }
    }
}

public class BidimensionalArray
{
    private readonly Dictionary<(int, int), bool> data = new Dictionary<(int, int), bool>();

    public bool this[int x, int y]
    {
        get
        {
            return data.TryGetValue((x, y), out bool value) && value;

        }
        set
        {
            data[(x, y)] = true;
        }
    }
}