using JetBrains.Annotations;
using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Threading;
using UnityEngine;

public class PerlinGenerator : MonoBehaviour
{
    public GameObject voxelchunk;
    public Material voxelMaterial;
    public WorldManager wManager;
    public Texture2D textureAtlas;
    public byte[,,] voxels = new byte[500, 150, 500];
    public Chunk blockGen;
    public BiomeAttributes biome;
    public Lode lode;
    public WorldManager worldManager;
    public int xpoz, zpoz;

    public float scale = 25.0f;
    public int octaves = 2;
    public float persistence = 0.2f;
    public float lacunarity = 2.4f;
    public float heightMultiplier = 33.0f;
    public int baseheight = 50;

    public Vector3 offset;
    public Vector2 Offset;
    public float xpos, zpos;
    public System.Random random;
    private int[] permutation;
    public int seed=434332;
    public void GenerateTerrain()
    {
        Debug.Log("incepe");
        random = new System.Random(seed);
        permutation = GeneratePermutation(seed);
        Offset = new Vector2(random.Next(-100000, 100000), random.Next(-100000, 100000));
        offset = new Vector3(random.Next(-100000, 100000), random.Next(-100000, 100000),random.Next(-100000,100000));
        float[,,] heightMap = GenerateHeightMap();
        float[,] hgtMap =generateHeightMap();
        
        int[,] p=new int[16,16];
        for (int z=0; z<16; z++)
        {
            for(int x=0; x<16; x++)
            {
                 int y= baseheight+Mathf.FloorToInt(hgtMap[x, z]*heightMultiplier);
                p[z,x] = y;
                 SetBlock(x, 0, z, 5);
                 for (int i = 1; i < y - 3; i++)
                 SetBlock(x, i, z,4);
                 for (int i = (y - 3); i < y; i++)
                 SetBlock(x, i, z, 1);
                 SetBlock(x, y, z, 2);
                 for (int i = y; i < 56; i++)
                 SetBlock(x, i, z, 6);
            }
        }
        for (int z = 0; z < 16; z++)
        {          
            for (int x = 0; x < 16; x++)
            {
                for (int y = 4; y < p[z,x]-6; y++)
                {
                    float n = heightMap[x, y, z];
                    if (n < 0.66f && n>0.63f )
                        SetBlock(x, y, z, 0);
                    else if (n > 0.59f)
                    {
                        foreach(Lode lode in biome.lodes)
                        {
                            if(lode.minheight<=y  && lode.maxheight >= y)
                            {
                                if(GetPerlin3D(new Vector3(x,y,z),offset.x,offset.y,offset.z,lode.scale,lode.threshold))
                                {
                                    SetBlock(x, y, z, lode.Idblock);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        for (int x = 0; x < 16; x++) {
            for (int z = 0; z < 16; z++)
            {

                if (p[z,x]>57 && Get2DPerlin(new Vector2(x+xpoz+500,z+zpoz+500),0, biome.treesize,biome.treethreshold))
                {
                    Structures.MakeTrees(new Vector3(x+ xpoz+500, p[z, x], z+ zpoz+500), 5,5);
                }
            }
        }
    }
    public bool Get2DPerlin(Vector2 position, float offset, float scale,float threshold)
    {
        position.x += (offset + seed + 0.1f);
        position.y += (offset + seed + 0.1f);

        return Mathf.PerlinNoise(position.x / 16 * scale, position.y / 16 * scale)> threshold;

    }
    float[,] generateHeightMap()
    {
        float[,] heightMap = new float[16, 16];
        int a = Mathf.FloorToInt(zpos), b = Mathf.FloorToInt(xpos);
        for (int y = a; y < a+16; y++)
        {
            for (int x = b; x < b+16; x++)
            {
                heightMap[x-b, y-a] = GeneratePerlinNoise(x, y);
            }
        }

        return heightMap;
    }
    float[,,] GenerateHeightMap() 
    {
        float[,,] heightMap = new float[16,100,16];
        int a= Mathf.FloorToInt(xpos),b= Mathf.FloorToInt(zpos);
        for (int z = a; z < a + 16; z++)
        {
            for (int y = 0; y < 70; y++)
            {
                for (int x = b; x < b + 16; x++)
                {
                    heightMap[z - a,y, x - b] = GeneratePerlinNoise(x, y,z);
                }
            }
        }
        return heightMap;
    }
    float GeneratePerlinNoise(int x, int y)
    {
        float amplitude = 1;
        float frequency = 1;
        float lacunarity = 0.4f;
        float noiseValue = 0;
        float maxValue = 0;  // Used for normalizing the result to [0, 1]
        float octaves = 4;
        for (int i = 0; i < octaves; i++)
        {
            float sampleX = (x + offset.x) / scale * frequency;
            float sampleY = (y + offset.y) / scale * frequency;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2;// - 1;
            noiseValue += perlinValue * amplitude;

            maxValue += amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return (noiseValue ) / octaves; // Normalized to [0, 1]
    }
    float GeneratePerlinNoise(int x, int y,int z)
    {
        float amplitude = 1;
        float frequency = 1;
        float noiseValue = 0;
        int octaves = 1;
        float maxValue = 1;

        for (int i = 0; i < octaves; i++)
        {
            float sampleX = (x + offset.x) / scale * frequency;
            float sampleY = (y + offset.y) / scale * frequency;
            float sampleZ = (z + offset.z) / scale * frequency;

            float perlinValue = Perlin3D(sampleX, sampleY, sampleZ);
            noiseValue += perlinValue * amplitude;

            maxValue += amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }
        return (noiseValue / maxValue + 1) / 2;
    }
    bool GetPerlin3D(Vector3 position, float X,float Y,float Z,float scale,float treshold)
    {
        float x = (position.x + X + 0.1f) * scale;
        float y = (position.y + Y + 0.1f) * scale;
        float z = (position.z + Z + 0.1f) * scale;

        float xy = Mathf.PerlinNoise(x, y);
        float xz = Mathf.PerlinNoise(x, z);
        float yz = Mathf.PerlinNoise(y, z);
        float yx = Mathf.PerlinNoise(y, x);
        float zx = Mathf.PerlinNoise(z, x);
        float zy = Mathf.PerlinNoise(z, y);

        float result = (xy + xz + yz + yx + zx + zy) / 6f;
        return result>treshold && result<treshold+scale;
    }
    float Perlin3D(float x, float y, float z)
    {
        int X = Mathf.FloorToInt(x) & 255;
        int Y = Mathf.FloorToInt(y) & 255;
        int Z = Mathf.FloorToInt(z) & 255;

        x -= Mathf.Floor(x);
        y -= Mathf.Floor(y);
        z -= Mathf.Floor(z);

        float u = Fade(x);
        float v = Fade(y);
        float w = Fade(z);

        int A = permutation[X] + Y;
        int AA = permutation[A] + Z;
        int AB = permutation[A + 1] + Z;
        int B = permutation[X + 1] + Y;
        int BA = permutation[B] + Z;
        int BB = permutation[B + 1] + Z;

        float lerp1 = Mathf.Lerp(Grad(permutation[AA], x, y, z), Grad(permutation[BA], x - 1, y, z), u);
        float lerp2 = Mathf.Lerp(Grad(permutation[AB], x, y - 1, z), Grad(permutation[BB], x - 1, y - 1, z), u);
        float lerp3 = Mathf.Lerp(lerp1, lerp2, v);

        lerp1 = Mathf.Lerp(Grad(permutation[AA + 1], x, y, z - 1), Grad(permutation[BA + 1], x - 1, y, z - 1), u);
        lerp2 = Mathf.Lerp(Grad(permutation[AB + 1], x, y - 1, z - 1), Grad(permutation[BB + 1], x - 1, y - 1, z - 1), u);
        float lerp4 = Mathf.Lerp(lerp1, lerp2, v);

        return (Mathf.Lerp(lerp3, lerp4, w) + 1) / 2; // Normalize to [0, 1]
    }
    float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    float Grad(int hash, float x, float y, float z)
    {
        int h = hash & 15;
        float u = h < 8 ? x : y;
        float v = h < 4 ? y : (h == 12 || h == 14 ? x : z);
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }

    int[] GeneratePermutation(int seed)
    {
        int[] p = new int[256];
        for (int i = 0; i < 256; i++)
        {
            p[i] = i;
        }

        System.Random rand = new System.Random(seed);
        for (int i = 255; i > 0; i--)
        {
            int swap = rand.Next(i + 1);
            int temp = p[i];
            p[i] = p[swap];
            p[swap] = temp;
        }

        int[] permutation = new int[512];
        for (int i = 0; i < 512; i++)
        {
            permutation[i] = p[i % 256];
        }

        return permutation;
    }
    public void SetBlock(int x, int y, int z, byte type)
    {
        Chunk.Voxels[x+xpoz+500, y, z+zpoz+500]= type;
    }
  
    public void ModifyVoxel(int x,int y,int z, byte id)
    {
        SetPos((x/16)*16,(z/16)*16);
        Chunk.Voxels[x+500, y, z+500] = id; 
        //Chunk.CreateMesh();
    }
    public void SetPos(int x, int z)
    {
        xpos = x;
        zpos = z;
        xpoz = x;
        zpoz = z;
    }
    public bool IsBlock(float x, float y, float z)
    {

        return Chunk.Voxels[Mathf.RoundToInt(x) + 500, Mathf.RoundToInt(y), Mathf.RoundToInt(z) + 500] != 0;
    }
}
public class Chunk
{
    public PerlinGenerator PerlinGen;
    private ChunkCoord Coord;
    public WorldManager world;
    public GameObject chunkObject;
    public GameObject obj;
    public static byte[,,] Voxels = new byte[1000, 150, 1000];

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uv = new List<Vector2>();
    Dictionary<Vector3, int> vertexDict = new Dictionary<Vector3, int>();

    public int worldSize = 4;
    public int chunkSize = 16;
    public int chunkheight = 80;
    public float voxelSize = 1f;

    public Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();
    public Chunk(ChunkCoord coord, WorldManager wmanager)
    {
        Coord = coord;
        Coord.x -=100;
        Coord.y -=100;
        GameObject perlinObject = new GameObject("PerlinGenerator");
        PerlinGen = perlinObject.AddComponent<PerlinGenerator>();
        PerlinGen.biome = wmanager.Biome;
        PerlinGen.SetPos((Coord.x) * 16, (Coord.y) * 16);
        world = wmanager;
        PerlinGen.voxelMaterial = wmanager.material;
        Thread prl = new Thread(PerlinGen.GenerateTerrain);
        prl.Start();
        chunkObject = new GameObject("Chunk " + (Coord.x).ToString() + " " + (Coord.y).ToString(), typeof(MeshFilter), typeof(MeshRenderer));
        Object.Destroy(perlinObject);
        Material material = PerlinGen.voxelMaterial;
        chunkObject.GetComponent<MeshRenderer>().material = material;
        chunkObject.transform.position = new Vector3(Coord.x * 16, 0f, Coord.y * 16);
    }
    public void Destro()
    {
        Object.Destroy(obj);
    }
    public void SetBlock(int x,int y,int z,byte id)
    {
        Voxels[x,y,z] = id;
    }
    
    public void CreateMesh()
    {
        Mesh mesh = new Mesh();
        chunkObject.GetComponent<MeshFilter>().mesh=null;
        triangles.Clear();
        vertices.Clear();uv.Clear();
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 100; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    
                    byte voxel = Voxels[x + 500 + Coord.x*16, y, z + 500 + Coord.y*16];

                    if (voxel == 0)
                        continue;
                    voxel--;
                    if (IsVoxelAir(x, y, z + 1)) AddFace(vertices, 0, triangles, uv, vertexDict, voxel, new Vector3(x, y, z), Vector3.forward);
                    if (IsVoxelAir(x, y, z - 1)) AddFace(vertices, 1, triangles, uv, vertexDict, voxel, new Vector3(x, y, z), Vector3.back);
                    if (IsVoxelAir(x + 1, y, z)) AddFace(vertices, 2, triangles, uv, vertexDict, voxel, new Vector3(x, y, z), Vector3.right);
                    if (IsVoxelAir(x - 1, y, z)) AddFace(vertices, 3, triangles, uv, vertexDict, voxel, new Vector3(x, y, z), Vector3.left);
                    if (IsVoxelAir(x, y + 1, z)) AddFace(vertices, 4, triangles, uv, vertexDict, voxel, new Vector3(x, y, z), Vector3.up);
                    if (IsVoxelAir(x, y - 1, z)) AddFace(vertices, 5, triangles, uv, vertexDict, voxel, new Vector3(x, y, z), Vector3.down);

                }
            }
        }



        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        mesh.RecalculateNormals();
        chunkObject.GetComponent<MeshFilter>().mesh = mesh;
        }
    void AddFace(List<Vector3> vertices, byte face, List<int> triangles, List<Vector2> uv, Dictionary<Vector3, int> vertexDict, byte voxel, Vector3 position, Vector3 direction)
    {
        Vector3 right;
        Vector3 up;

        if (direction == Vector3.up || direction == Vector3.down)
        {
            right = Vector3.right;
            up = (direction == Vector3.down) ? Vector3.forward : Vector3.back;
        }
        else
        {
            right = new Vector3(direction.z, 0, -direction.x);
            up = Vector3.up;
        }
        int vertexIndex = vertices.Count;

        vertices.Add(position + direction * 0.5f - right * 0.5f - up * 0.5f);
        vertices.Add(position + direction * 0.5f + right * 0.5f - up * 0.5f);
        vertices.Add(position + direction * 0.5f + right * 0.5f + up * 0.5f);
        vertices.Add(position + direction * 0.5f - right * 0.5f + up * 0.5f);

        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);

        float uvSize = 0.0625f;
        byte bid = (byte)world.blockTypes[voxel].GetTextureID(face);

        Vector2 uvBase = GetUVForVoxelType(bid);
        uv.Add(uvBase + new Vector2(0, 0) * uvSize);
        uv.Add(uvBase + new Vector2(1, 0) * uvSize);
        uv.Add(uvBase + new Vector2(1, 1) * uvSize);
        uv.Add(uvBase + new Vector2(0, 1) * uvSize);
    }
    bool IsVoxelAir(int x, int y, int z)
    {
        if (z < 0 || y < 0 || x < 0 || Voxels[x + 500 + Coord.x * 16, y, z + 500 + Coord.y * 16] == 0)
            return true;
        return Voxels[x + 500 + Coord.x*16, y, z + 500 + Coord.y*16] == 0;
    }
    Vector2 GetUVForVoxelType(byte id)
    {
        //nu modifica nimic, dar daca e ceva marimea atlasului e 16x16
        int atlasIndex;
        if (id % 16 != 0)
            atlasIndex = id % 16 + 16 * (15 - id / 16);
        else
            atlasIndex = 16 * (17 - id / 16);
        atlasIndex--;
        int x = atlasIndex % 16;
        int y = atlasIndex / 16;
        float uvSize = 0.0625f;
        return new Vector2((x * uvSize), (y * uvSize));
    }

}
public class ChunkCoord
{
    public int x;
    public int y;
    public ChunkCoord(int X, int Y)
    {
        x = X;
        y = Y;
    }
}