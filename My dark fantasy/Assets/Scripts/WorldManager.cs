using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using UnityEngine.SceneManagement;
using System.Linq;


public class WorldManager : MonoBehaviour
{
    public ControllerImput CImp;
    public Time time;
    public static bool ready = false;
    public static float currenttime;
    public BiomeAttributes[] Biome;
    [Range(0f, 0.85f)]
    public static float GlobalLight=0;
    public Toolbar Toolbar;
    public Chunk chunk;
    public Material material;
    public BlockProprieties[] blockTypes;
    readonly List<ChunkCoord> activechunks = new();
    public byte selectedSlot = 0;
    public Queue<ChunkCoord> unmeshedchk = new ();
    public static List<ChunkCoord> chunkstosave = new ();
    public Queue<ChunkCoord> nextChunk = new ();
    public static Chunk[,] chunks = new Chunk[Voxeldata.SizeOfWorld, Voxeldata.SizeOfWorld];

    private void FixedUpdate()
    {
        if (!Toolbar.openedInv)
        {
            CheckViewDistance();
            CalculateTime();
            Shader.SetGlobalFloat("globallight", GlobalLight);
        }
        }
    public void CalculateTime()
    {
        currenttime += Time.deltaTime;
        if (currenttime > 540 && currenttime<570)
        {
            GlobalLight += 0.0006f;
        }
        else if(currenttime >1140 && currenttime < 1170)
        {
            GlobalLight -= 0.0006f;
        }
        else if(currenttime >= 1200)
        {
            currenttime -= 1200;
        }

    }
    public void PreCalculateTime()
    {
        if (currenttime > 540 && currenttime < 570)
        {
            GlobalLight += ((currenttime-540)/Time.deltaTime)*0.0006f ;
        }
        else if(currenttime>570 && currenttime < 1140)
        {
            GlobalLight =0.85f;
        }
        else if (currenttime > 1140 && currenttime < 1170)
        {
            GlobalLight+= (30/Time.deltaTime)*0.0006f;
            GlobalLight -= ((currenttime - 1140) / Time.deltaTime) * 0.0006f;
        }
        Shader.SetGlobalFloat("globallight", GlobalLight);

    }
    public bool IsChunkInWorld(ChunkCoord coord)
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
    public void CreateNewChunk(int x, int y)
    {
        x += 100;
        y += 100;
        chunks[x, y] = new Chunk(new ChunkCoord(x, y), this);
        activechunks.Add(new ChunkCoord(x, y));
        if (!ChunkSerializer.IsReal(x-100, y-100))
        {
            Thread thread = new(chunks[x, y].MakeTerrain);
            thread.Start();
        }
        else
        {
            Thread thread = new(() => ChunkSerializer.LoadChunk(x - 100, y - 100));
            thread.Start();
            //ChunkSerializer.SaveChunk(x-100, y-100);
        }

    }
    public static void UpdateMesh(int x,int y)
    {
        chunks[x,y].CreateMesh();
    }
    public static void SetTo(int x,int y, int z,byte id)
    {
        if (x > 0 && z > 0)
            chunks[(x / 16 + 100), (z / 16 + 100)].Voxels[x % 16, y, z % 16] = id;
        else if (x > 0 && z < 0)
        {
            if (z % 16 == 0)
            {
                chunks[(x / 16 + 100), (z / 16 + 100)].Voxels[x % 16, y, 16 - (-z % 16)] = id;
            }
            else
            {
                chunks[(x / 16 + 100), (z / 16 + 99)].Voxels[x % 16, y, 16 - (-z % 16)] = id;
            }
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
    public void SaveChunk(int x, int y)
    {
        foreach(ChunkCoord a in chunkstosave)
        {
            if (a.x == x && a.y == y)
            {
                chunkstosave.Remove(a);
                break;
            }
              
        }
        ChunkSerializer.loadedChunks[(x, y)] = chunks[x + 100, y + 100].Voxels;
        activechunks.Remove(new ChunkCoord(x + 100, y + 100));
        ChunkSerializer.SaveChunk(x, y);
        chunkstosave.Remove(new ChunkCoord(x, y));
    }
    public void NextChunk(int x, int y)
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
        if (x % 16 == 0 && z % 16 == 0)
        {
            chunks[x / 16 + 100, z / 16 + 100].CreateMesh();
            chunks[x / 16 + 100, z / 16 + 99].CreateMesh();
            chunks[x / 16 + 99, z / 16 + 100].CreateMesh();
            chunks[x / 16 + 99, z / 16 + 99].CreateMesh();

        }
        else if (x % 16 == 0)
        {
            if (x >= 0)
                chunks[x / 16 + 99, z / 16 + 100].CreateMesh();
            else
                chunks[x / 16 + 100, z / 16 + 100].CreateMesh();
        }
        else if (z % 16 == 0)
        {
            if (z >= 0)
                chunks[x / 16 + 100, z / 16 + 99].CreateMesh();
            else
                chunks[x / 16 + 100, z / 16 + 100].CreateMesh();
        }
        else if (x % 16 == 15 && z % 16 == 15)
            chunks[x / 16 + 101, z / 16 + 101].CreateMesh();
        else if (x % 16 == 15)
            chunks[x / 16 + 101, z / 16 + 100].CreateMesh();
        else if (z % 16 == 15)
            chunks[x / 16 + 100, z / 16 + 101].CreateMesh();
    }
    public void GenerateWorld()
    {
        ChunkCoord plpos = CImp.GetPosition();
        plpos = new ChunkCoord(plpos.x / 16, plpos.y / 16);
        for (int i = plpos.x-(Voxeldata.NumberOfChunks+2); i <= plpos.x + (Voxeldata.NumberOfChunks+2); i++)
        {
            for (int j = plpos.y - (Voxeldata.NumberOfChunks+2); j <= plpos.y+ Voxeldata.NumberOfChunks+2; j++)
            {
                CreateNewChunk(i, j);
            }
        }
        
        for (int i = plpos.x - Voxeldata.NumberOfChunks; i <= plpos.x+Voxeldata.NumberOfChunks; i++)
        {
            for (int j = plpos.y - Voxeldata.NumberOfChunks; j <= plpos.y + Voxeldata.NumberOfChunks; j++)
            {
                chunks[i + 100, j + 100].Make3d();
            }
        }
        for (int i = plpos.x - Voxeldata.NumberOfChunks; i <= plpos.x+ Voxeldata.NumberOfChunks; i++)
        {
            for (int j = plpos.y - Voxeldata.NumberOfChunks; j <= plpos.y+ Voxeldata.NumberOfChunks; j++)
            {
                chunks[i + 100, j + 100].CreateMesh();
            }
        }
        ready = true;
    }
    public byte Block(float x, float y, float z)
    {
        int a = Mathf.RoundToInt(x), b = Mathf.RoundToInt(y), c = Mathf.RoundToInt(z);
        int g = a, h = c;
        if (a < 0)
            g = -a;
        if (c < 0)
            h = -c;
        if (a < 0 && c >= 0)
        {
            if (a % 16 == 0)
            {
                return chunks[a / 16 + 100, c / 16 + 100].Voxels[0, b, c % 16];
            }
            return chunks[(a / 16 + 99), (c / 16 + 100)].Voxels[16 - (g % 16), b, h % 16];
        }
        else if (a >= 0 && c >= 0)
        {
            return chunks[(a / 16 + 100), (c / 16 + 100)].Voxels[g % 16, b, h % 16];
        }
        else if (a >= 0 && c < 0)
        {
            if (c % 16 == 0)
            {
                return chunks[a / 16 + 100, c / 16 + 100].Voxels[a % 16, b, 0] ;
            }
            return chunks[(a / 16 + 100), (c / 16 + 99)].Voxels[a % 16, b, (16 - (-c % 16))];
        }
        else if (a < 0 && c < 0)
        {
            if (a % 16 == 0 && c % 16 == 0)
            {
                return chunks[a / 16 + 100, c / 16 + 100].Voxels[0, b, 0];
            }
            if (a % 16 == 0)
            {
                return chunks[a / 16 + 100, c / 16 + 99].Voxels[0, b, (16 - (-c % 16))];
            }
            if (c % 16 == 0)
            {
                return chunks[a / 16 + 99, c / 16 + 100].Voxels[(16 - (-a % 16)), b, 0] ;
            }
            return chunks[(a / 16 + 99), (c / 16 + 99)].Voxels[(16 - (-a % 16)), b, (16 - (-c % 16))];
        }
        else
        {
            return 0;
        }
    }
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
        
        for (int x = plpos.x - (Voxeldata.NumberOfChunks+2); x <= plpos.x + Voxeldata.NumberOfChunks+2; x++)
        {
            for (int z = plpos.y - (Voxeldata.NumberOfChunks+2); z <= plpos.y + Voxeldata.NumberOfChunks+2; z++)
            {
                if (!IsChunkInWorld(new ChunkCoord(x, z)))
                {
                    CreateNewChunk(x, z);
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
        for (int x = plpos.x - Voxeldata.NumberOfChunks; x <= plpos.x + Voxeldata.NumberOfChunks; x++)
        {
            for (int z = plpos.y - Voxeldata.NumberOfChunks; z <= plpos.y + Voxeldata.NumberOfChunks; z++)
            {
                if (!chunks[x + 100, z + 100].mademesh && !chunks[x + 100, z + 100].start)
                {
                    NextChunk(x + 100, z + 100);
                    chunks[x + 100, z + 100].start = true;
                }/*
                else if (!chunks[x + 100, z + 100])
                {
                    chunks[x + 100, z + 100].chunkObject.SetActive(true);
                    activechunks.Add(new ChunkCoord(x + 100, z + 100));
                }
                */
                
            }
        }
        
        for (int i = 0; i < notactivechunks.Count; i++)
        {
            //Debug.Log(notactivechunks[i].x + " " + notactivechunks[i].y + " " + chunks[notactivechunks[i].x, notactivechunks[i].y].mademesh);
            if (chunks[notactivechunks[i].x, notactivechunks[i].y].mademesh)
            {
                SaveChunk(notactivechunks[i].x - 100, notactivechunks[i].y - 100);
                if (chunkstosave.Contains(new ChunkCoord(notactivechunks[i].x - 100, notactivechunks[i].y - 100)))
                    chunkstosave.Remove(new ChunkCoord(notactivechunks[i].x - 100, notactivechunks[i].y - 100));
                
            }
            Destroy(chunks[notactivechunks[i].x, notactivechunks[i].y].chunkObject);
            chunks[notactivechunks[i].x, notactivechunks[i].y] = null;
            activechunks.Remove(notactivechunks[i]);
        }

        int r = nextChunk.Count;
        for (int i = 0; i<r && i < 1; i++)
        {
            if (chunks[nextChunk.Peek().x, nextChunk.Peek().y] != null)
            {
                if (chunks[nextChunk.Peek().x, nextChunk.Peek().y].strmade)
                {
                    int a = nextChunk.Peek().x, b = nextChunk.Peek().y;
                    nextChunk.Dequeue();
                    chunks[a,b].CreateMesh();
                }
                else
                {
                    Thread cm = new(chunks[nextChunk.Peek().x, nextChunk.Peek().y].Make3d);
                    cm.Start();
                    chunks[nextChunk.Peek().x, nextChunk.Peek().y].Make3d();
                }
            }
            else
            {
                nextChunk.Dequeue();
            }
        }
       
    }
    public void ClearData()
    {
        Array.Clear(chunks, 0, chunks.Length);
        chunkstosave.Clear();
        activechunks.Clear();
    }
}
[System.Serializable]
public class BlockProprieties
{
    public string Name;
    public byte place;
    public byte utility;
    public bool isblock;
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
        return index switch
        {
            0 => frontfacetexture,
            1 => backfacetexture,
            2 => rightfacetexture,
            3 => leftfacetexture,
            4 => topfacetexture,
            5 => bottomfacetexture,
            _ => -1,
        };
    }
}