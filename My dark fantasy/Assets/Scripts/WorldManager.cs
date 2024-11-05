using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.IO;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using JetBrains.Annotations;
using System.Collections;
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
    [SerializeField] 
    private Light DirectionalLight;

    public Toolbar Toolbar;
    public Chunk chunk;
    public Material material;
    public Material waterMat;
    public BlockProprieties[] blockTypes;
    readonly HashSet<ChunkCoord> activechunks = new();
    public Queue<ChunkCoord> unmeshedchk = new ();
    public static List<ChunkCoord> chunkstosave = new ();
    public static Queue<ChunkCoord> nextChunk = new ();
    public static Chunk[,] chunks = new Chunk[Voxeldata.SizeOfWorld, Voxeldata.SizeOfWorld];
    public void Awake()
    {
        if (!OnAppOpened.readytogo)
        {
            OnAppOpened appOpened = new();
            appOpened.ReadWhatNeedsTo();
        }
        blockTypes = new BlockProprieties[OnAppOpened.itemsnum];
        for(int i=0; i<OnAppOpened.itemsnum; i++)
        {
            blockTypes[i]=OnAppOpened.blockTypes[i];

        }
    }
    public void Start()
    {
        StartCoroutine(UpdateChunksCoroutine());
    }

    private void FixedUpdate()
    {

        if (!Toolbar.openedInv)
        {
            //CheckViewDistance();
           CalculateTime();
            //Shader.SetGlobalFloat("globallight", GlobalLight);
        }
    }
    
    private IEnumerator UpdateChunksCoroutine()
    {
        while (true)
        {
            if(!Toolbar.openedInv)
            CheckViewDistance(); 

            yield return new WaitForSeconds(0.2f); 
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
        DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((currenttime/1200 * 360f) - 90f, 170f, 0));
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
    public void BakeNewWorld()
    {
        for (int i = -7; i <= 7; i++)
        {
            for (int j = -7; j <= 7; j++)
            {
            chunks[i+100, j+100] = new Chunk(new ChunkCoord(i+100,j+100), this);
            activechunks.Add(new ChunkCoord(i+100, j+100));
            chunks[i+100, j+100].MakeTerrain();
        
            }
        }
        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                chunks[i + 100, j + 100].Make3d();
            }
        }
        
        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                chunks[i + 100, j + 100].CreateMesh();
            }
        }
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
                chunks[(x / 16 + 100), (z / 16 + 100)].Voxels[x % 16, y, 0] = id;
            }
            else
            {
                chunks[(x / 16 + 100), (z / 16 + 99)].Voxels[x % 16, y, 16 - (-z % 16)] = id;
            }
        }
        else if (x < 0 && z < 0)
        {
            if (z % 16 == 0 && x % 16 == 0)
                chunks[(x / 16 + 100), (z / 16 + 100)].Voxels[0, y , 0] = id;
            else if (z % 16 == 0)
                chunks[(x / 16 + 99), (z / 16 + 100)].Voxels[16 - (-x % 16), y , 0] = id;
            else if (x % 16 == 0)
                chunks[(x / 16 + 100), (z / 16 + 99)].Voxels[0, y , 16 - (-z    % 16)] = id;
            else
                chunks[(x / 16 + 99), (z / 16 + 99)].Voxels[16 - (-x % 16), y , 16 - (-z % 16)] = id;
        }
        else if (x < 0 && z > 0)
        {
            if (x % 16 == 0)
                chunks[(x / 16 + 100), (z / 16 + 100)].Voxels[0, y, z % 16] = id;
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
    public static void NextChunk(int x, int y)
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
        int adjustedX = x < 0 ? (x / 16) - 1 : x / 16;
        int adjustedZ = z < 0 ? (z / 16) - 1 : z / 16;

        int baseChunkX = adjustedX + 100;
        int baseChunkZ = adjustedZ + 100;

        chunks[baseChunkX, baseChunkZ].CreateMesh();

        int localX = (x % 16 + 16) % 16;
        int localZ = (z % 16 + 16) % 16;

        bool onXEdge = localX == 0;
        bool onZEdge = localZ == 0;
        bool onPositiveXEdge = localX == 15;
        bool onPositiveZEdge = localZ == 15;

        if (onXEdge && onZEdge)
        {
            chunks[baseChunkX, baseChunkZ - 1].CreateMesh();
            chunks[baseChunkX - 1, baseChunkZ].CreateMesh();
            chunks[baseChunkX - 1, baseChunkZ - 1].CreateMesh();
        }
        else if (onXEdge)
        {
            chunks[baseChunkX - 1, baseChunkZ].CreateMesh();
        }
        else if (onZEdge)
        {
            chunks[baseChunkX, baseChunkZ - 1].CreateMesh();
        }
        else if (onPositiveXEdge && onPositiveZEdge)
        {
            chunks[baseChunkX + 1, baseChunkZ + 1].CreateMesh();
        }
        else if (onPositiveXEdge)
        {
            chunks[baseChunkX + 1, baseChunkZ].CreateMesh();
        }
        else if (onPositiveZEdge)
        {
            chunks[baseChunkX, baseChunkZ + 1].CreateMesh();
        }


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
        ChunkCoord playerPos = CImp.GetPosition();
        playerPos = new ChunkCoord(playerPos.x / 16, playerPos.y / 16);

        //Separarea chunkurilor active de inactive
        UpdateActiveChunks(playerPos);

        //Aici incepe deja prelucrarea, creerea meshurilor
        ProcessQueuedChunks();
    }
    private void UpdateActiveChunks(ChunkCoord playerPos)
    {
        HashSet<ChunkCoord> chunksToDeactivate = new(activechunks);
        int X = playerPos.x, Z = playerPos.y;
        for (int x =X - (Voxeldata.NumberOfChunks + 4); x <= X + (Voxeldata.NumberOfChunks + 4); x++)
        {
            for (int z = Z - (Voxeldata.NumberOfChunks + 4); z <= Z + (Voxeldata.NumberOfChunks + 4); z++)
            {
                ChunkCoord coord = new (x, z);

                if (!IsChunkInWorld(coord))
                {
                    CreateNewChunk(x, z);
                }
                else
                {
                    Chunk chunk = chunks[x + 100, z + 100];
                    if (chunk != null)
                    {
                        if (!chunk.mademesh && !chunk.start && AreNeighborChunksReady(x, z))
                        {
                            NextChunk(x + 100, z + 100); 
                            nextChunk.Enqueue(new ChunkCoord(x + 100, z + 100)); 
                            chunk.start = true; 
                        }

                        if (!chunk.chunkObject.activeSelf)
                        {
                            chunk.chunkObject.SetActive(true);
                            activechunks.Add(new ChunkCoord(x + 100, z + 100));
                        }
                    }
                }
                chunksToDeactivate.Remove(new ChunkCoord(x + 100, z + 100));
            }
        }
        DeactivateChunks(chunksToDeactivate);
    }
    private void DeactivateChunks(HashSet<ChunkCoord> chunksToDeactivate)
    {
        foreach (var coord in chunksToDeactivate)
        {
            Chunk chunk = chunks[coord.x, coord.y];
            if (chunk != null && chunk.mademesh)
            {
                SaveChunk(coord.x - 100, coord.y - 100);
                chunk.chunkObject.SetActive(false);
                activechunks.Remove(coord);
            }
        }
    }
    private void ProcessQueuedChunks()
    {
        int maxChunksPerFrame = 1;
        for (int i = 0; i < nextChunk.Count && i < maxChunksPerFrame; i++)
        {
            ChunkCoord coord = nextChunk.Peek();
            Chunk chunk = chunks[coord.x, coord.y];
            if (chunk != null)
            {
                if (chunk.strmade)
                {
                    chunk.CreateMesh();
                    nextChunk.Dequeue();
                }
                else
                {
                    Thread thread = new(() => chunk.Make3d());
                    thread.Start();
                }
            }
            else
            {
                nextChunk.Dequeue();
            }
        }
    }

    bool AreNeighborChunksReady(int x, int z)
    {
        return (chunks[x + 100 - 1, z + 100] != null && 
                chunks[x + 100 + 1, z + 100] != null && 
                chunks[x + 100, z + 100 - 1] != null && 
                chunks[x + 100, z + 100 + 1] != null);  
    }
    public void ClearData()
    {
        Array.Clear(chunks, 0, chunks.Length);
        chunkstosave.Clear();
        activechunks.Clear();
    }
}
public class BlockProprieties
{
    public Items Items;
    public Sprite itemSprite;
}

[System.Serializable]
public class AllItems
{
    public Items[] items;
}
[System.Serializable]
public class Items
{
    public string Name;
    public byte place;
    public byte itemtexture;
    public bool isblock;
    public Blocks blocks;
    public Tool tool;
    public Normalitem item;
}

[System.Serializable]
public class Blocks
{
    public byte type;
    public byte backfacetexture;
    public byte frontfacetexture;
    public byte topfacetexture;
    public byte bottomfacetexture;
    public byte leftfacetexture;
    public byte rightfacetexture;
    public float breakTime;
    public byte durability;
    public byte special;

    public byte GetTextureID(byte face)
    {
        return face switch
        {
            0 => backfacetexture,
            1 => frontfacetexture,
            2 => rightfacetexture,
            3 => leftfacetexture,
            4 => topfacetexture,
            5 => bottomfacetexture,
            _ => 0,
        };
    }
}
[System.Serializable]
public class Tool
{
    public byte type;
    public byte id;
    public byte damage;
}
[System.Serializable]
public class Normalitem
{
    public byte coolfunction;
}