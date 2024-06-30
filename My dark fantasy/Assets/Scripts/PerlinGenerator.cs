using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using TreeEditor;
using Unity.Android.Gradle;
using Unity.VisualScripting;
using UnityEngine;
using static Voxeldata;

public class PerlinGenerator : MonoBehaviour
{
    public static readonly int sizeAtlas = 5;
    public GameObject voxelchunk;
    public Material voxelMaterial;
    public WorldManager wManager;
    public Texture2D textureAtlas;
    public byte[,,] voxels = new byte[500, 150, 500];
    public Chunk blockGen;
    public WorldManager worldManager;
    public float scale = 25.0f;
    public int octaves = 2;
    public float persistence = 0.8f;
    public float lacunarity = 2.0f;
    public float heightMultiplier = 15.0f;
    public int baseheight = 30;
    public Vector2 offset;
    public float xpos, zpos;
    public System.Random random;
    public int seed=4343;
    public void GenerateTerrain()
    {
        random = new System.Random(seed);
        offset = new Vector2(random.Next(-100000, 100000), random.Next(-100000, 100000));
        float[,] heightMap = GenerateHeightMap();
        int xpoz = Mathf.FloorToInt(xpos),zpoz= Mathf.FloorToInt(zpos);
        for (int z = 0; z < 16; z++)
        {
            for (int x = 0; x < 16; x++)
            {
                int y = baseheight+Mathf.FloorToInt(heightMap[x, z] * heightMultiplier);
                //bedrock
                SetBlock(x, 0, z, 5);
                for(int i=1; i< y-3; i++)
                SetBlock(x, i, z,4);
                for (int i = (y-3); i < y; i++)
                SetBlock(x, i, z, 1);
                SetBlock(x, y, z, 3);

            }
        }
        CreateMesh();
    }

    float[,] GenerateHeightMap() 
    {
        float[,] heightMap = new float[16,16];
        int a= Mathf.FloorToInt(xpos),b= Mathf.FloorToInt(zpos);
        for (int y = a; y < a+16; y++)
        {
            for (int x = b; x < b+16; x++)
            {
                heightMap[y-a, x-b] = GeneratePerlinNoise(x, y);
            }
        }
        return heightMap;
    }

    float GeneratePerlinNoise(int x, int y)
    {
        float amplitude = 1;
        float frequency = 1;
        float noiseValue = 0;
        int octaves = 2; 
        
        for (int i = 0; i < octaves; i++)
        {
            float sampleX = (x + offset.x) / scale * frequency;
            float sampleY = (y + offset.y) / scale * frequency;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

            noiseValue += perlinValue * amplitude;
           
            amplitude *= persistence;
            frequency *= lacunarity;
        }
        return noiseValue / 2;
    }

    public void SetBlock(int x, int y, int z, byte type)
    {
            voxels[x, y, z] = type; 
    }

    public void CreateMesh()
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();
        Dictionary<Vector3, int> vertexDict = new Dictionary<Vector3, int>();
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 100; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    byte voxel = voxels[x, y, z];
                    
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

        GameObject voxelObject = new GameObject("Chunk "+(xpos/16).ToString()+" "+(zpos/16).ToString(), typeof(MeshFilter), typeof(MeshRenderer));
        voxelObject.GetComponent<MeshFilter>().mesh = mesh;
        Material material = voxelMaterial;
        voxelObject.GetComponent<MeshRenderer>().material = material;
      //  MeshCollider meshCollider = voxelObject.AddComponent<MeshCollider>();
      //  meshCollider.sharedMesh = mesh;
        voxelObject.transform.position =new Vector3(xpos, 0f, zpos);
        voxelchunk = voxelObject;
    }

    bool IsVoxelAir(int x, int y, int z)
    {
        if (z < 0 || y < 0 || x < 0 || voxels[x, y, z] == 0)
            return true;
        return voxels[x, y, z] == 0;
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

        float uvSize = 0.2f;
        byte bid = (byte)wManager.blockTypes[voxel].GetTextureID(face);

        Vector2 uvBase = GetUVForVoxelType(bid);
        uv.Add(uvBase + new Vector2(0, 0) * uvSize);
        uv.Add(uvBase + new Vector2(1, 0) * uvSize);
        uv.Add(uvBase + new Vector2(1, 1) * uvSize);
        uv.Add(uvBase + new Vector2(0, 1) * uvSize);
    }
    Vector2 GetUVForVoxelType(byte id)
    {
        int atlasIndex;
        if (id % 5 != 0)
            atlasIndex = id % 5 + 5 * (4 - id / 5);
        else
            atlasIndex = 5 * (6 - id / 5);
        atlasIndex--;

        int x = atlasIndex % sizeAtlas;
        int y = atlasIndex / sizeAtlas;
        float uvSize = 0.2f;
        return new Vector2((x * uvSize), (y * uvSize));
    }
   

    public void SetPos(int x, int z)
    {
        xpos = x;
        zpos = z;
    }
}
public class Chunk
{
    public PerlinGenerator PerlinGen;
    public ChunkCoord coord;
    public GameObject chunkObject;

    public int worldSize = 4;
    public int chunkSize = 16;
    public int chunkheight = 80;
    public float voxelSize = 1f;

    public Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();
    public Chunk(ChunkCoord coord, WorldManager wmanager)
    {
        GameObject perlinObject = new GameObject("PerlinGenerator");
       // PerlinGen = new PerlinGenerator();
       PerlinGen=perlinObject.AddComponent<PerlinGenerator>();
        PerlinGen.SetPos(coord.x * 16, coord.y * 16);
        PerlinGen.wManager = wmanager;
        PerlinGen.voxelMaterial=wmanager.material;
        PerlinGen.GenerateTerrain();
        chunkObject=PerlinGen.voxelchunk;
        Object.Destroy(perlinObject);
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