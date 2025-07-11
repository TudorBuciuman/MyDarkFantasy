using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Collections;

public class WorldManager : MonoBehaviour
{
    public ControllerImput CImp;
    public static bool ready = false;
    public static float currenttime;
    public BiomeAttributes[] Biome;
    [SerializeField]
    private Light DirectionalLight;
    public Toolbar Toolbar;
    public Chunk chunk;
    public Material material;
    public Material transparent;
    public Material waterMat;
    public BlockProprieties[] blockTypes;
    public readonly HashSet<ChunkCoord> activechunks = new();
    public static Queue<ChunkCoord> unmeshedchk = new();
    public static List<ChunkCoord> chunkstosave = new();
    public static Queue<ChunkCoord> nextChunk = new();
    public static HashSet<Vector2Int> castleChunksReady = new();
    public static Dictionary<Vector2Int, Chunk> chunks = new();
    public void Awake()
    {
        chunks.Clear();
        if (!OnAppOpened.readytogo)
        {
            OnAppOpened appOpened = new();
            appOpened.ReadWhatNeedsTo();
        }
        blockTypes = new BlockProprieties[OnAppOpened.itemsnum];
        for (int i = 0; i < OnAppOpened.itemsnum; i++)
        {
            blockTypes[i] = OnAppOpened.blockTypes[i];

        }
    }

    private void FixedUpdate()
    {
        if (!Toolbar.escape)
        {
            CalculateTime();
        }
        if (!Toolbar.openedInv)
        {
            CheckViewDistance();
        }
    }

    private IEnumerator UpdateChunksCoroutine()
    {
        while (true)
        {
            if (!Toolbar.openedInv)
                CheckViewDistance();

            yield return new WaitForSeconds(0.1f);
        }
    }
    public void CalculateTime()
    {
        currenttime += Time.deltaTime;
        if (currenttime >= 1200)
        {
            currenttime -= 1200;
        }
        DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((currenttime / 1200 * 360f) - 90f, 170f, 0));
    }
    public static void AddChunk(int x, int z, Chunk chunk)
    {
        var position = new Vector2Int(x, z);
        chunks[position] = chunk;
    }
    public static Chunk GetChunk(int x, int z)
    {
        var position = new Vector2Int(x, z);
        if (chunks.TryGetValue(position, out Chunk chunk))
        {
            return chunk;
        }
        return null;
    }
    public bool IsChunkInWorld(ChunkCoord coord)
    {
        if (GetChunk(coord.x, coord.y) != null)
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
                AddChunk(i, j, new Chunk(new ChunkCoord(i, j), this));
                activechunks.Add(new ChunkCoord(i, j));
                GetChunk(i, j).MakeTerrain();
            }
        }
        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                GetChunk(i, j).Make3d();
            }
        }
        for (int i = -4; i <= 4; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                if (!(MathF.Abs(i) == 4 && MathF.Abs(j) == 2))
                {
                    int k;
                    for (k = 125; k >= 58; k--)
                    {
                        if (IsBlock(i, k, j))
                            break;
                    }
                    SetTo(i, k, j, 49);
                    if (UnityEngine.Random.Range(0, 2) == 1)
                        SetTo(i, k + 1, j, 17);
                }
            }
        }

        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                GetChunk(i, j).CreateMesh();
            }
        }
    }
    public Chunk GetChunkFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int z = Mathf.FloorToInt(pos.z);
        return GetChunk(x, z);
    }
    public void CreateNewChunk(int x, int y)
    {
        AddChunk(x, y, new Chunk(new ChunkCoord(x, y), this));
        activechunks.Add(new ChunkCoord(x, y));
        if (!ChunkSerializer.IsReal(x, y))
        {
            Thread thread = new(GetChunk(x, y).MakeTerrain);
            thread.Start();
        }
        else
        {
            Thread thread = new(() => ChunkSerializer.LoadChunk(x, y));
            thread.Start();
        }

    }
    public static void UpdateMesh(int x, int z)
    {
        int adjustedX = x < 0 ? (x / 16) - 1 : x / 16;
        int adjustedZ = z < 0 ? (z / 16) - 1 : z / 16;

        if (x < 0 && x % 16 == 0)
            adjustedX++;
        if (z < 0 && z % 16 == 0)
            adjustedZ++;
        int baseChunkX = adjustedX;
        int baseChunkZ = adjustedZ;
        if (GetChunk(baseChunkX, baseChunkZ).mademesh)
            NextChunk(baseChunkX, baseChunkZ);

        int localX = (x % 16 + 16) % 16;
        int localZ = (z % 16 + 16) % 16;

        bool onXEdge = localX == 0;
        bool onZEdge = localZ == 0;
        bool onPositiveXEdge = localX == 15;
        bool onPositiveZEdge = localZ == 15;

        if (onXEdge && onZEdge)
        {
            if (GetChunk(baseChunkX, baseChunkZ - 1).mademesh)
                NextChunk(baseChunkX, baseChunkZ - 1);
            if (GetChunk(baseChunkX - 1, baseChunkZ).mademesh)
                NextChunk(baseChunkX - 1, baseChunkZ);
            if (GetChunk(baseChunkX - 1, baseChunkZ - 1).mademesh)
                NextChunk(baseChunkX - 1, baseChunkZ - 1);
        }
        else if (onXEdge)
        {
            if (GetChunk(baseChunkX - 1, baseChunkZ).mademesh)
                NextChunk(baseChunkX - 1, baseChunkZ);
        }
        else if (onZEdge)
        {
            if (GetChunk(baseChunkX, baseChunkZ - 1).mademesh)
                NextChunk(baseChunkX, baseChunkZ - 1);
        }
        else if (onPositiveXEdge && onPositiveZEdge)
        {
            if (GetChunk(baseChunkX + 1, baseChunkZ + 1).mademesh)
                NextChunk(baseChunkX + 1, baseChunkZ + 1);
        }
        else if (onPositiveXEdge)
        {
            if (GetChunk(baseChunkX + 1, baseChunkZ).mademesh)
                NextChunk(baseChunkX + 1, baseChunkZ);
        }
        else if (onPositiveZEdge)
        {
            if (GetChunk(baseChunkX, baseChunkZ + 1).mademesh)
                NextChunk(baseChunkX, baseChunkZ + 1);
        }
    }
    public static void SetTo(int x, int y, int z, byte id)
    {

        if (x >= 0 && z >= 0)
            GetChunk((x / 16), (z / 16)).Voxels[x % 16, y, z % 16].Value1 = id;
        else if (x >= 0 && z < 0)
        {
            if (z % 16 == 0)
            {
                GetChunk((x / 16), (z / 16)).Voxels[x % 16, y, 0].Value1 = id;
            }
            else
            {
                GetChunk((x / 16), (z / 16 - 1)).Voxels[x % 16, y, 16 - (-z % 16)].Value1 = id;
            }
        }
        else if (x < 0 && z < 0)
        {
            if (z % 16 == 0 && x % 16 == 0)
                GetChunk((x / 16), (z / 16)).Voxels[0, y, 0].Value1 = id;
            else if (z % 16 == 0)
                GetChunk((x / 16 - 1), (z / 16)).Voxels[16 - (-x % 16), y, 0].Value1 = id;
            else if (x % 16 == 0)
                GetChunk((x / 16), (z / 16 - 1)).Voxels[0, y, 16 - (-z % 16)].Value1 = id;
            else
                GetChunk((x / 16 - 1), (z / 16 - 1)).Voxels[16 - (-x % 16), y, 16 - (-z % 16)].Value1 = id;
        }
        else if (x < 0 && z >= 0)
        {
            if (x % 16 == 0)
                GetChunk((x / 16), (z / 16)).Voxels[0, y, z % 16].Value1 = id;
            else
                GetChunk((x / 16 - 1), (z / 16)).Voxels[16 - (-x % 16), y, z % 16].Value1 = id;
        }
    }

    public void SaveChunk(int x, int y)
    {
        foreach (ChunkCoord a in chunkstosave)
        {
            if (a.x == x && a.y == y)
            {
                chunkstosave.Remove(a);
                break;
            }
        }
        ChunkSerializer.loadedChunks[(x, y)] = GetChunk(x, y).Voxels;
        activechunks.Remove(new ChunkCoord(x, y));
        ChunkSerializer.SaveChunk(x, y);
        chunkstosave.Remove(new ChunkCoord(x, y));
    }
    public static void NextChunk(int x, int y)
    {
        nextChunk.Enqueue(new ChunkCoord(x, y));
    }
    public void ModifyMesh(int x, int y, int z, Chunk.VoxelStruct id)
    {
        if (x < 0 && z > -1)
        {
            if (x % 16 == 0)
            {
                GetChunk(x / 16, z / 16).Voxels[0, y, z % 16] = id;
            }
            else
                GetChunk(x / 16 - 1, z / 16).Voxels[(16 - (-x % 16)), y, z % 16] = id;
        }
        else if (x >= 0 && z >= 0)
            GetChunk(x / 16, z / 16).Voxels[x % 16, y, z % 16] = id;
        else if (x > -1 && z < 0)
        {
            if (z % 16 == 0)
            {
                GetChunk(x / 16, z / 16).Voxels[x % 16, y, 0] = id;
            }
            else
                GetChunk(x / 16, z / 16 - 1).Voxels[x % 16, y, (16 - (-z % 16))] = id;
        }
        else
        {
            if (x % 16 == 0 && z % 16 == 0)
            {
                GetChunk(x / 16, z / 16).Voxels[0, y, 0] = id;
            }
            else if (z % 16 == 0)
            {
                GetChunk(x / 16 - 1, z / 16).Voxels[(16 - (-x % 16)), y, 0] = id;
            }
            else if (x % 16 == 0)
            {
                GetChunk(x / 16, z / 16 - 1).Voxels[0, y, (16 - (-z % 16))] = id;
            }
            else
                GetChunk(x / 16 - 1, z / 16 - 1).Voxels[(16 - (-x % 16)), y, (16 - (-z % 16))] = id;
        }
        int adjustedX = x < 0 ? (x / 16) - 1 : x / 16;
        int adjustedZ = z < 0 ? (z / 16) - 1 : z / 16;

        if (x < 0 && x % 16 == 0)
            adjustedX++;
        if (z < 0 && z % 16 == 0)
            adjustedZ++;
        int baseChunkX = adjustedX;
        int baseChunkZ = adjustedZ;
        GetChunk(baseChunkX, baseChunkZ).CreateMesh();

        int localX = (x % 16 + 16) % 16;
        int localZ = (z % 16 + 16) % 16;

        bool onXEdge = localX == 0;
        bool onZEdge = localZ == 0;
        bool onPositiveXEdge = localX == 15;
        bool onPositiveZEdge = localZ == 15;

        if (onXEdge && onZEdge)
        {
            GetChunk(baseChunkX, baseChunkZ - 1).CreateMesh();
            GetChunk(baseChunkX - 1, baseChunkZ).CreateMesh();
            GetChunk(baseChunkX - 1, baseChunkZ - 1).CreateMesh();
        }
        else if (onXEdge)
        {
            GetChunk(baseChunkX - 1, baseChunkZ).CreateMesh();
        }
        else if (onZEdge)
        {
            GetChunk(baseChunkX, baseChunkZ - 1).CreateMesh();
        }
        else if (onPositiveXEdge && onPositiveZEdge)
        {
            GetChunk(baseChunkX + 1, baseChunkZ + 1).CreateMesh();
        }
        else if (onPositiveXEdge)
        {
            GetChunk(baseChunkX + 1, baseChunkZ).CreateMesh();
        }
        else if (onPositiveZEdge)
        {
            GetChunk(baseChunkX, baseChunkZ + 1).CreateMesh();
        }


    }
    public void GenerateWorld()
    {
        CheckViewDistance();
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
                return GetChunk(a / 16, c / 16).Voxels[0, b, c % 16].Value1;
            }
            return GetChunk(a / 16 - 1, c / 16).Voxels[16 - (g % 16), b, h % 16].Value1;
        }
        else if (a >= 0 && c >= 0)
        {
            return GetChunk(a / 16, c / 16).Voxels[g % 16, b, h % 16].Value1;
        }
        else if (a >= 0 && c < 0)
        {
            if (c % 16 == 0)
            {
                return GetChunk(a / 16, c / 16).Voxels[a % 16, b, 0].Value1;
            }
            return GetChunk(a / 16, c / 16 - 1).Voxels[a % 16, b, (16 - (-c % 16))].Value1;
        }
        else if (a < 0 && c < 0)
        {
            if (a % 16 == 0 && c % 16 == 0)
            {
                return GetChunk(a / 16, c / 16).Voxels[0, b, 0].Value1;
            }
            if (a % 16 == 0)
            {
                return GetChunk(a / 16, c / 16 - 1).Voxels[0, b, (16 - (-c % 16))].Value1;
            }
            if (c % 16 == 0)
            {
                return GetChunk(a / 16 - 1, c / 16).Voxels[(16 - (-a % 16)), b, 0].Value1;
            }
            return GetChunk(a / 16 - 1, c / 16 - 1).Voxels[(16 - (-a % 16)), b, (16 - (-c % 16))].Value1;
        }
        else
        {
            return 0;
        }
    }
    public byte Block2(float x, float y, float z)
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
                return GetChunk(a / 16, c / 16).Voxels[0, b, c % 16].Value2;
            }
            return GetChunk(a / 16 - 1, c / 16).Voxels[16 - (g % 16), b, h % 16].Value2;
        }
        else if (a >= 0 && c >= 0)
        {
            return GetChunk(a / 16, c / 16).Voxels[g % 16, b, h % 16].Value2;
        }
        else if (a >= 0 && c < 0)
        {
            if (c % 16 == 0)
            {
                return GetChunk(a / 16, c / 16).Voxels[a % 16, b, 0].Value2;
            }
            return GetChunk(a / 16, c / 16 - 1).Voxels[a % 16, b, (16 - (-c % 16))].Value2;
        }
        else if (a < 0 && c < 0)
        {
            if (a % 16 == 0 && c % 16 == 0)
            {
                return GetChunk(a / 16, c / 16).Voxels[0, b, 0].Value2;
            }
            if (a % 16 == 0)
            {
                return GetChunk(a / 16, c / 16 - 1).Voxels[0, b, (16 - (-c % 16))].Value2;
            }
            if (c % 16 == 0)
            {
                return GetChunk(a / 16 - 1, c / 16).Voxels[(16 - (-a % 16)), b, 0].Value2;
            }
            return GetChunk(a / 16 - 1, c / 16 - 1).Voxels[(16 - (-a % 16)), b, (16 - (-c % 16))].Value2;
        }
        else
        {
            return 0;
        }
    }
    public bool IsBlock(float x, float y, float z)
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
                return GetChunk(a / 16, c / 16).Voxels[0, b, c % 16].Value1 != 0;
            }
            return GetChunk(a / 16 - 1, c / 16).Voxels[16 - (g % 16), b, h % 16].Value1 != 0;
        }
        else if (a >= 0 && c >= 0)
        {
            return GetChunk(a / 16, c / 16).Voxels[g % 16, b, h % 16].Value1 != 0;
        }
        else if (a >= 0 && c < 0)
        {
            if (c % 16 == 0)
            {
                return GetChunk(a / 16, c / 16).Voxels[a % 16, b, 0].Value1 != 0;
            }
            return GetChunk(a / 16, c / 16 - 1).Voxels[a % 16, b, (16 - (-c % 16))].Value1 != 0;
        }
        else if (a < 0 && c < 0)
        {
            if (a % 16 == 0 && c % 16 == 0)
            {
                return GetChunk(a / 16, c / 16).Voxels[0, b, 0].Value1 != 0;
            }
            if (a % 16 == 0)
            {
                return GetChunk(a / 16, c / 16 - 1).Voxels[0, b, (16 - (-c % 16))].Value1 != 0;
            }
            if (c % 16 == 0)
            {
                return GetChunk(a / 16 - 1, c / 16).Voxels[(16 - (-a % 16)), b, 0].Value1 != 0;
            }
            return GetChunk(a / 16 - 1, c / 16 - 1).Voxels[(16 - (-a % 16)), b, (16 - (-c % 16))].Value1 != 0;
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
        for (int x = X - (Voxeldata.NumberOfChunks + 3); x <= X + (Voxeldata.NumberOfChunks + 3); x++)
        {
            for (int z = Z - (Voxeldata.NumberOfChunks + 3); z <= Z + (Voxeldata.NumberOfChunks + 3); z++)
            {
                ChunkCoord coord = new(x, z);

                if (!IsChunkInWorld(coord))
                {
                    CreateNewChunk(x, z);
                }
                else
                {
                    Chunk chunk = GetChunk(x, z);
                    if (chunk.chunkObject != null)
                    {
                        if (!chunk.mademesh && !chunk.start && AreNeighborChunksReady(x, z))
                        {
                            NextChunk(x, z);
                            chunk.start = true;
                        }
                        if (!chunk.chunkObject.activeSelf)
                        {
                            chunk.chunkObject.SetActive(true);
                            activechunks.Add(new ChunkCoord(x, z));
                        }
                    }
                }
                chunksToDeactivate.Remove(new ChunkCoord(x, z));
            }
        }
        DeactivateChunks(chunksToDeactivate);

    }
    private void DeactivateChunks(HashSet<ChunkCoord> chunksToDeactivate)
    {
        foreach (var coord in chunksToDeactivate)
        {
            Chunk chunk = GetChunk(coord.x, coord.y);
            if (chunk != null && chunk.mademesh)
            {
                SaveChunk(coord.x, coord.y);
                chunk.chunkObject.SetActive(false);
                activechunks.Remove(coord);
                break;
            }
        }
    }
    private void ProcessQueuedChunks()
    {
        int maxChunksPerFrame = 2;
        for (int i = 0; i < nextChunk.Count && i < maxChunksPerFrame; i++)
        {
            ChunkCoord coord = nextChunk.Peek();
            if (coord == null)
            {
                nextChunk.Dequeue();
                continue;
            }
            Chunk chunk = GetChunk(coord.x, coord.y);
            if (chunk != null)
            {
                if (chunk.strmade)
                {
                    chunk.CreateMesh();
                    nextChunk.Dequeue();
                }
                else if (!chunk.strstart)
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
        return (GetChunk(x - 1, z) != null &&
                GetChunk(x - 1, z - 1) != null &&
                GetChunk(x + 1, z - 1) != null &&
                GetChunk(x + 1, z + 1) != null &&
                GetChunk(x - 1, z + 1) != null &&
                GetChunk(x + 1, z) != null &&
                GetChunk(x, z - 1) != null &&
                GetChunk(x, z + 1) != null);
    }
    public void ClearData()
    {
        chunks.Clear();
        ChunkSerializer.loadedChunks.Clear();
        castleChunksReady.Clear();
        chunkstosave.Clear();
        activechunks.Clear();
        unmeshedchk.Clear();
        nextChunk.Clear();
    }

    public void PasteStructure(CastleStructure structure, int startX, int startY, int startZ)
    {
        int width = structure.blocks.GetLength(0);
        int height = structure.blocks.GetLength(1);
        int depth = structure.blocks.GetLength(2);
        HashSet<Vector2Int> updatedChunks = new HashSet<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    var voxel = structure.GetBlock(x, y, z);

                    int worldX = startX + x;
                    int worldY = startY + y;
                    int worldZ = startZ + z;
                    if (voxel.Value1 != 0 || structure.GetBlock(x, 0, z).Value1 != 0)
                    {
                        if (worldY < 160)
                        {
                            SetBlock(worldX, worldY, worldZ, voxel);
                            SetTo(worldX, worldY, worldZ, voxel.Value1);
                        }
                        int chunkX = Mathf.FloorToInt(worldX / 16f);
                        int chunkZ = Mathf.FloorToInt(worldZ / 16f);
                        updatedChunks.Add(new Vector2Int(chunkX, chunkZ));
                    }
                }
            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                if (Block(x + startX, startY, z + startZ) != 0)
                {
                    for (int y = 0; y < startY; y++)
                    {
                        if (Block(x + startX, startY - y - 1, z + startZ) == 0 || !blockTypes[Block(x + startX, startY - y - 1, z + startZ)].Items.isblock)
                        {
                            SetBlock(x + startX, startY - y - 1, z + startZ, structure.GetBlock(x, 0, z));
                            SetTo(x + startX, startY - y - 1, z + startZ, structure.GetBlock(x, 0, z).Value1);
                        }
                        else
                            break;
                    }
                }
            }
        }
        foreach (var chunkCoord in updatedChunks)
        {
            int chunkWorldX = chunkCoord.x * 16;
            int chunkWorldZ = chunkCoord.y * 16;

            ModifyMesh(chunkWorldX, 140, chunkWorldZ, new Chunk.VoxelStruct(0, 0));
        }
        ChestData chest1 = new();
        ChestData chest2 = new();
        chest1.position = new Pos(startX + 21, startY + 1, startZ + 24);
        chest2.position = new Pos(startX + 29, startY + 1, startZ + 24);

        chest1.items = new ItemSlott[27];

        chest1.items[0] = new ItemSlott { itemId = 50, quantity = 1, slotIndex = 0 };
        chest1.items[1] = new ItemSlott { itemId = 63, quantity = 1, slotIndex = 1 };

        chest2.items = new ItemSlott[27];

        chest2.items[0] = new ItemSlott { itemId = 55, quantity = 1, slotIndex = 0 };

        Chest.Instance.AddFullChest(chest1);
        Chest.Instance.AddFullChest(chest2);
        StartCoroutine(SoundsManager.instance.FoundCastle());
    }
    public void SetBlock(int x, int y, int z, Chunk.VoxelStruct id)
    {
        if (x >= 0 && z >= 0)
            GetChunk((x / 16), (z / 16)).Voxels[x % 16, y, z % 16].Value2 = id.Value2;
        else if (x >= 0 && z < 0)
        {
            if (z % 16 == 0)
            {
                GetChunk((x / 16), (z / 16)).Voxels[x % 16, y, 0].Value2 = id.Value2;
            }
            else
            {
                GetChunk((x / 16), (z / 16 - 1)).Voxels[x % 16, y, 16 - (-z % 16)].Value2 = id.Value2;
            }
        }
        else if (x < 0 && z < 0)
        {
            if (z % 16 == 0 && x % 16 == 0)
                GetChunk((x / 16), (z / 16)).Voxels[0, y, 0].Value2 = id.Value2;
            else if (z % 16 == 0)
                GetChunk((x / 16 - 1), (z / 16)).Voxels[16 - (-x % 16), y, 0].Value2 = id.Value2;
            else if (x % 16 == 0)
                GetChunk((x / 16), (z / 16 - 1)).Voxels[0, y, 16 - (-z % 16)].Value2 = id.Value2;
            else
                GetChunk((x / 16 - 1), (z / 16 - 1)).Voxels[16 - (-x % 16), y, 16 - (-z % 16)].Value2 = id.Value2;
        }
        else if (x < 0 && z >= 0)
        {
            if (x % 16 == 0)
                GetChunk((x / 16), (z / 16)).Voxels[0, y, z % 16].Value2 = id.Value2;
            else
                GetChunk((x / 16 - 1), (z / 16)).Voxels[16 - (-x % 16), y, z % 16].Value2 = id.Value2;
        }
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
    public byte positionType;

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
    public byte num;
    public float multiplier;
}
[System.Serializable]
public class Normalitem
{
    public byte coolfunction;
    public byte secondfunction;
}