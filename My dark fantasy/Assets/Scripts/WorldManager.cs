using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;


public class WorldManager : MonoBehaviour
{
    public PerlinGenerator PerlinGen;
    public CameraController CImp;
    public BiomeAttributes Biome;
    public Chunk chunk;
    public Material material;
    public BlockProprieties[] blockTypes;
    List<ChunkCoord> activechunks = new List<ChunkCoord>();
    public byte selectedSlot = 0;
    Queue<ChunkCoord> unmeshedchk = new Queue<ChunkCoord>();
    Chunk[,] chunks = new Chunk[Voxeldata.SizeOfWorld, Voxeldata.SizeOfWorld];
    private void Start()
    {
       // Chunk = new Chunk();
        GenerateWorld();
    }

    private void Update()
    {
        CheckViewDistance();
    }
    private bool IsChunkInWorld(ChunkCoord coord)
    {
        if (chunks[coord.x + 100, coord.y + 100] != null)
        {
            if(!chunks[coord.x + 100, coord.y+100].chunkObject.activeSelf)
            chunks[coord.x + 100, coord.y].chunkObject.gameObject.SetActive(true);
            return true;
        }
            
        return false;
    }
    public Chunk GetChunkFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int z = Mathf.FloorToInt(pos.z);
        return chunks[x, z];
    }
    private void CreateNewChunk(int x, int y)
    {
        x += 100;
        y += 100;

        chunks[x,y]=new Chunk(new ChunkCoord(x, y), this);
        chunks[x,y].CreateMesh();
        activechunks.Add(new ChunkCoord(x, y));
    }
    public void ModifyMesh(int x, int y, int z,byte id)
    {
        Chunk.Voxels[x+500,y,z+500] = id;
        if (x < 0)x -= 16;
        if(z<0) z -= 16;
        chunks[x/16 + 100, z / 16 + 100].CreateMesh();
    }
    public void GenerateWorld()
    {
        for (int i = -Voxeldata.NumberOfChunks; i < Voxeldata.NumberOfChunks; i++)
        {
            for (int j = -Voxeldata.NumberOfChunks; j < Voxeldata.NumberOfChunks; j++)
            {
                CreateNewChunk(i, j);
            }
        }
        for (int i = -Voxeldata.NumberOfChunks; i < Voxeldata.NumberOfChunks; i++)
        {
            for (int j = -Voxeldata.NumberOfChunks; j < Voxeldata.NumberOfChunks; j++)
            {
                chunks[i+100, j+100].CreateMesh();
            }
        }
    }
    public int lastX = 0, lastY = 0;
    public void CheckViewDistance()
    {
        ChunkCoord plpos = CImp.GetPosition();
        plpos = new ChunkCoord(plpos.x / 16, plpos.y / 16);
        List<ChunkCoord> notactivechunks = new(activechunks);

        for (int x = plpos.x - Voxeldata.NumberOfChunks; x <= plpos.x + Voxeldata.NumberOfChunks; x++)
        {
            for (int z = plpos.y - Voxeldata.NumberOfChunks; z <= plpos.y + Voxeldata.NumberOfChunks; z++)
            {
                if (!IsChunkInWorld(new ChunkCoord(x, z)))
                {
                    CreateNewChunk(x, z);
                    unmeshedchk.Enqueue(new ChunkCoord(x, z));
                }
                for (int i = 0; i < notactivechunks.Count; i++)
                {
                    if (notactivechunks[i].x == (x+100) && notactivechunks[i].y == (z+100))
                    {
                        notactivechunks.RemoveAt(i);
                    }

                }
            }
        }

        for (int i = 0; i < notactivechunks.Count; i++)
        {
            chunks[notactivechunks[i].x, notactivechunks[i].y].chunkObject.gameObject.SetActive(false);
            activechunks.Remove(notactivechunks[i]);
        }
        foreach (ChunkCoord coord in unmeshedchk)
        {
            chunks[coord.x + 100, coord.y + 100].CreateMesh();
        }
        unmeshedchk.Clear();
    }
}
[System.Serializable]
public class BlockProprieties
{
    public string Name;
    public byte place;
    public Sprite icon;
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