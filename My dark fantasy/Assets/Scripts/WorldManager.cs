using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;


public class WorldManager : MonoBehaviour
{
    public CameraController CImp;
    public BiomeAttributes[] Biome;
    public Toolbar Toolbar;
    public Chunk chunk;
    public Material material;
    public BlockProprieties[] blockTypes;
    List<ChunkCoord> activechunks = new List<ChunkCoord>();
    public byte selectedSlot = 0;
    public Queue<ChunkCoord> unmeshedchk = new Queue<ChunkCoord>();
    public Queue<ChunkCoord> nextChunk = new Queue<ChunkCoord>();
    public static Chunk[,] chunks = new Chunk[Voxeldata.SizeOfWorld, Voxeldata.SizeOfWorld];
    private void Start()
    {
        GenerateWorld();        
    }

    private void Update()
    {
        if(!Toolbar.openedInv)
        CheckViewDistance();
    }
    private bool IsChunkInWorld(ChunkCoord coord)
    {
        if (chunks[coord.x + 100, coord.y + 100] != null)
        {
            
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
        Thread thread = new Thread(chunks[x,y].MakeTerrain);
        thread.Start();
        activechunks.Add(new ChunkCoord(x, y));
    }
    public static void SetTo(int x,int y, int z,byte id)
    {
        if (x > 0 && z > 0)
            chunks[(x / 16 + 100), (z / 16 + 100)].Voxels[x % 16, y, z % 16] = id;
        else if (x > 0 && z < 0)
        {
            if (z % 16 == 0)
                chunks[(x / 16 + 100), (z / 16 + 100)].Voxels[x % 16, y, 16 - (-z % 16)] = id;
            else
                chunks[(x / 16 + 100), (z / 16 + 99)].Voxels[x % 16, y , 16 - (-z % 16)] = id;
        }
        else if (x < 0 && z < 0)
        {
            if (z % 16 == 0 && x % 16 == 0)
                chunks[(x / 16 + 100), (z / 16 + 100)].Voxels[16 - (-x % 16), y , 16 - (-z % 16)] = id;
            else if (z % 16 == 0)
                chunks[(x / 16 + 99), (z / 16 + 100)].Voxels[16 - (-x % 16), y , 16 - (-z % 16)] = id;
            else if (x % 16 == 0)
                chunks[(x / 16 + 100), (z / 16 + 99)].Voxels[16 - (-x % 16), y , 16 - (-z    % 16)] = id;
            else
                chunks[(x / 16 + 99), (z / 16 + 99)].Voxels[16 - (-x % 16), y , 16 - (-z % 16)] = id;
        }
        else if (x < 0 && z > 0)
        {
            if (x % 16 == 0)
                chunks[(x / 16 + 100), (z / 16 + 100)].Voxels[16 - (-x % 16), y, z % 16] = id;
            else
                chunks[(x / 16 + 99), (z / 16 + 100)].Voxels[16 - (-x % 16), y, z % 16] = id;
        }
    }
    private void NextChunk(int x, int y)
    {
        nextChunk.Enqueue(new ChunkCoord(x,y));
    }
    public void ModifyMesh(int x, int y, int z,byte id)
    {
        if (x < 0 && z > -1)
        {
            if (x % 16 == 0)
            {
                chunks[x / 16 + 100, z / 16 + 100].Voxels[0, y, z % 16] = id;
            }
            else
            chunks[x / 16 + 99, z / 16 + 100].Voxels[(16-(-x % 16)), y, z % 16] = id;
        }
        else if (x >= 0 && z >= 0)
            chunks[x / 16 + 100, z / 16 + 100].Voxels[x % 16, y, z % 16] = id;
        else if (x > -1 && z < 0)
        {
            if (z % 16 == 0)
            {
                chunks[x / 16 +100, z / 16 + 100].Voxels[x%16, y, 0] = id;
            }
            else
            chunks[x / 16 + 100, z / 16 + 99].Voxels[x % 16, y, (16 - (-z % 16))] = id;
        }
        else
        {
            if(x % 16 == 0 && z % 16 == 0)
            {
                chunks[x / 16 + 100, z / 16 + 100].Voxels[0, y, 0] = id;
            } 
            if (z % 16 == 0)
            {
                chunks[x / 16 + 99, z / 16 + 100].Voxels[(16 - (-x % 16)), y, 0] = id;
            }
            else if (x % 16 == 0)
            {
                chunks[x / 16 + 100, z / 16 + 99].Voxels[0, y, (16 - (-z % 16))] = id;
            }
            else
            chunks[x / 16 + 99, z / 16 + 99].Voxels[(16 - (-x % 16)), y, (16 - (-z % 16))] = id;
        }
        if (x<0 && x%16!=0)
           x -= 16;
        if(z<0 && z%16!=0) 
           z -= 16;
        chunks[x/16 + 100, z / 16 + 100].CreateMesh();
        if(x%16==0 && z%16==0)
            chunks[x / 16 + 99, z / 16 + 99].CreateMesh();
        else if (x % 16 == 0)
            chunks[x / 16 + 99, z / 16+100 ].CreateMesh();
        else if (z % 16 == 0)
            chunks[x / 16 + 100, z / 16 + 99].CreateMesh();
        else if (x%16==15 && z%16==15)
            chunks[x / 16 + 101, z / 16 + 101].CreateMesh();
        else if (x % 16 == 15)
            chunks[x / 16 + 101, z / 16 + 100].CreateMesh();
        else if (z % 16 == 15)
            chunks[x / 16 + 100, z / 16 + 101].CreateMesh();
    }
    public void GenerateWorld()
    {
        for (int i = -Voxeldata.NumberOfChunks-2; i <= Voxeldata.NumberOfChunks+2; i++)
        {
            for (int j = -Voxeldata.NumberOfChunks-2; j <= Voxeldata.NumberOfChunks+2; j++)
            {
                CreateNewChunk(i, j);
            }
        }
        
        for (int i = -Voxeldata.NumberOfChunks-1; i <= Voxeldata.NumberOfChunks+1; i++)
        {
            for (int j = -Voxeldata.NumberOfChunks-1; j <= Voxeldata.NumberOfChunks+1; j++)
            {
                chunks[i + 100, j + 100].Make3d();
            }
        }
        for (int i = -Voxeldata.NumberOfChunks; i <= Voxeldata.NumberOfChunks; i++)
        {
            for (int j = -Voxeldata.NumberOfChunks; j <= Voxeldata.NumberOfChunks; j++)
            {
                chunks[i + 100, j + 100].CreateMesh();
            }
        }
        int u = unmeshedchk.Count;
        for (int i=0; i < u; i++)
        {
            chunks[unmeshedchk.Peek().x+100, unmeshedchk.Peek().y+100].FinishMesh();
            unmeshedchk.Dequeue();
        }
    
    }
    public int lastX = 0, lastY = 0;
    public bool IsBlock(float x, float y, float z)
    {
        int a = Mathf.RoundToInt(x), b = Mathf.RoundToInt(y), c = Mathf.RoundToInt(z);
        int g=a, h=c;
        if (a < 0)
            g = -a;
        if (c < 0)
            h = -c;
        if (a < 0 && c >= 0)
        {
            if (a % 16 == 0)
            {
            return chunks[a / 16 + 100, c / 16 + 100].Voxels[0, b, c % 16]!=0;
            }
            return chunks[(a / 16 + 99), (c / 16 + 100)].Voxels[16-(g% 16), b, h % 16] != 0;
        }
        else if (a >= 0 && c >= 0)
        {
            return chunks[(a / 16 + 100), (c / 16 + 100)].Voxels[g % 16, b, h % 16] != 0;
        }
        else if (a >= 0 && c < 0)
        {
            if (c % 16 == 0)
            {
                return chunks[a / 16 + 100, c / 16 + 100].Voxels[a%16, b,0] != 0;
            }
            return chunks[(a / 16 + 100), (c / 16 + 99)].Voxels[a % 16, b, (16 - (-c % 16))] != 0;
        }
        else if (a < 0 && c < 0)
        {
            if (a % 16 == 0 && c%16==0)
            {
                return chunks[a / 16 + 100, c / 16 + 100].Voxels[0, b, 0] != 0;
            }
            if (a % 16 == 0)
            {
                return chunks[a / 16 + 100, c / 16 + 99].Voxels[0, b, (16 - (-c % 16))] != 0;
            }
            if (c % 16 == 0)
            {
                return chunks[a / 16 + 99, c / 16 + 100].Voxels[(16 - (-a % 16)), b, 0] != 0;
            }
            return chunks[(a / 16 +99), (c / 16 + 99)].Voxels[(16 - (-a % 16)), b, (16 - (-c % 16))] != 0;
        }
        else
        {
            return false;
        }
    }
    public void CheckViewDistance()
    {
        ChunkCoord plpos = CImp.GetPosition();
        plpos = new ChunkCoord(plpos.x / 16, plpos.y / 16);
        List<ChunkCoord> notactivechunks = new(activechunks);
        
        for (int x = plpos.x - Voxeldata.NumberOfChunks-2; x <= plpos.x + Voxeldata.NumberOfChunks+2; x++)
        {
            for (int z = plpos.y - Voxeldata.NumberOfChunks-2; z <= plpos.y + Voxeldata.NumberOfChunks+2; z++)
            {
                if (!IsChunkInWorld(new ChunkCoord(x, z)))
                {
                    CreateNewChunk(x, z);
                }
            }
        }
        for (int x = plpos.x - Voxeldata.NumberOfChunks; x <= plpos.x + Voxeldata.NumberOfChunks; x++)
        {
            for (int z = plpos.y - Voxeldata.NumberOfChunks; z <= plpos.y + Voxeldata.NumberOfChunks; z++)
            {
                if (!chunks[x + 100, z + 100].mademesh && !chunks[x + 100, z + 100].start)
                {
                    NextChunk(x + 100, z + 100);
                    chunks[x + 100, z + 100].start = true;
                }
                else if (!chunks[x + 100, z + 100].chunkObject.activeSelf)
                {
                    chunks[x + 100, z + 100].chunkObject.gameObject.SetActive(true);
                    activechunks.Add(new ChunkCoord(x + 100, z + 100));
                }
                for (int i = 0; i < notactivechunks.Count; i++)
                {
                    if (notactivechunks[i].x == (x + 100) && notactivechunks[i].y == (z + 100))
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

        int r = nextChunk.Count;
        for (int i = 0; i<r && i < 4; i++)
        {
            if (chunks[nextChunk.Peek().x, nextChunk.Peek().y].strmade)
            {
                chunks[nextChunk.Peek().x, nextChunk.Peek().y].CreateMesh();
                nextChunk.Dequeue();
            }
            else
            {
                  Thread cm = new Thread(chunks[nextChunk.Peek().x, nextChunk.Peek().y].Make3d);
                  cm.Start();
                chunks[nextChunk.Peek().x, nextChunk.Peek().y].Make3d();
            }
        }
        int u = unmeshedchk.Count;
        for (int i = 0; i<u && i<1; i++)
        {
                chunks[(unmeshedchk.Peek().x + 100), (unmeshedchk.Peek().y + 100)].FinishMesh();
                unmeshedchk.Dequeue();
        }
    }
}
[System.Serializable]
public class BlockProprieties
{
    public string Name;
    public byte place;
    public byte utility;
    public float brktme;
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