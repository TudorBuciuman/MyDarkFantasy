using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Reflection;
using System.Threading;
using UnityEngine;
public class Chunk
{
    public byte[,,] Voxels = new byte[16, 96, 16];
    private ChunkCoord Coord;
    public BiomeAttributes[,] biome;
    public byte[,] biom;
    public Lode lode;
    public bool mademesh = false;
    public bool ready = false;
    public WorldManager world;
    public GameObject chunkObject;
    public GameObject obj;
    public System.Random random;
    public Mesh mesh;
    public bool start=false;
    public int seed = 3345;
    Vector2 Offset;
    Vector3 offset;
    byte[,] height = new byte[16, 16];
    public bool strmade=false;
    List<Vector3> vertices = new List<Vector3>(20000);
    List<int> triangles = new List<int>(30000);
    List<Vector2> uv = new List<Vector2>(20000);
    Dictionary<Vector3, int> vertexDict = new Dictionary<Vector3, int>();


    public Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();
    public Chunk(ChunkCoord coord, WorldManager wmanager)
    {
        Coord = coord;
        Coord.x -= 100;
        Coord.y -= 100;
        random = new System.Random(seed);
        Offset = new Vector2(random.Next(-100000, 100000), random.Next(-100000, 100000));
        offset = new Vector3(random.Next(-100000, 100000), random.Next(-100000, 100000), random.Next(-100000, 100000));
        world = wmanager;
        biome = new BiomeAttributes[16, 16];
        biom = new byte[16, 16];
        chunkObject = new GameObject("Chunk " + (Coord.x).ToString() + " " + (Coord.y).ToString(), typeof(MeshFilter), typeof(MeshRenderer));
        chunkObject.transform.position = new Vector3(Coord.x * 16, 0f, Coord.y * 16);
        Material material = wmanager.material;
        chunkObject.GetComponent<MeshRenderer>().material = material;
        
    }
    public void MakeTerrain()
    {
        float[,] heightm = generateHeightMap();
        for (int x = 0; x < 16; x++)
        {
            for (int z = 0; z < 16; z++)
            {

                int y = (byte)heightm[x, z];
                height[x, z] = (byte)heightm[x, z];
                SetBlock(x, 0, z, 5);
                for (int i = 1; i < y - 3; i++)
                    SetBlock(x, i, z, 4);
                for (int i = (y - 3); i < y; i++)
                    SetBlock(x, i, z, biome[x, z].middleblock);

                SetBlock(x, y, z, biome[x, z].topblock);

            }
        }
    }
    public float[,] generateHeightMap()
    {
        float[,] heightMap = new float[16, 16];
        float[,] valleyMap = new float[16, 16];
        float[,] humidityMap = new float[16, 16];


        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                float heightValue = PerlinNoise(i, j, 2, 0.5f, 2.0f);
                float valley = PerlinNoise((Coord.x + 100) * 16 + 1000+i, (Coord.y + 100) *16 + 1000+j, 10, 1f, 2.0f)-0.1f;

                heightMap[i, j] = heightValue*5;
                valleyMap[i, j] = valley*10;
                float strong=0, medheight = 0;
                int index=0,l=0;
                for (byte p = 0; p < world.Biome.Length; p++)
                {
                    
                    float weight = Noise.Get2DNoise(i + (Coord.x + 100)*16, j + (Coord.y + 100) * 16, new Vector2(world.Biome[p].offset, Offset.y), world.Biome[p].scale);
                    if (weight > strong)
                    {
                        strong = weight;
                        index = p;
                    }
                    if (weight > 0)
                    {
                        medheight += weight*world.Biome[p].multiplier;
                        l++;
                    }
                }

                biome[i, j] = world.Biome[index];
                medheight = (medheight / l)*2;
                heightMap[i, j] += valley*20;
                heightMap[i,j]*=(medheight);
                heightMap[i, j] -= 130;
                if (heightMap[i, j] < 0)
                    heightMap[i, j] += 20;
                if (heightMap[i, j] > 90)
                    heightMap[i, j] = 80;
            }
        }


        return heightMap;
    }

    public int[] permutation;
    public int[] p;
    public void PerlinNoise3D()
    {
        permutation = new int[256];
        p = new int[512];
        for (int i = 0; i < 256; i++)
            permutation[i] = i;

        for (int i = 0; i < 256; i++)
        {
            int j = random.Next(256);
            int temp = permutation[i];
            permutation[i] = permutation[j];
            permutation[j] = temp;
        }

        for (int i = 0; i < 512; i++)
            p[i] = permutation[i % 256];
    }
    public float noise(float x, float y, float z)
    {
        int X = (int)Math.Floor(x) & 255;
        int Y = (int)Math.Floor(y) & 255;
        int Z = (int)Math.Floor(z) & 255;

        x -= (float)Math.Floor(x);
        y -= (float)Math.Floor(y);
        z -= (float)Math.Floor(z);

        float u = Fade(x);
        float v = Fade(y);
        float w = Fade(z);
        int A = p[X] + Y;
        int AA = p[A] + Z;
        int AB = p[A + 1] + Z;
        int B = p[X + 1] + Y;
        int BA = p[B] + Z;
        int BB = p[B + 1] + Z;

        return Lerp(w, Lerp(v, Lerp(u, Grad(p[AA], x, y, z), Grad(p[BA], x - 1, y, z)),
                               Lerp(u, Grad(p[AB], x, y - 1, z), Grad(p[BB], x - 1, y - 1, z))),
                       Lerp(v, Lerp(u, Grad(p[AA + 1], x, y, z - 1), Grad(p[BA + 1], x - 1, y, z - 1)),
                               Lerp(u, Grad(p[AB + 1], x, y - 1, z - 1), Grad(p[BB + 1], x - 1, y - 1, z - 1))));
    }

    private float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private float Lerp(float t, float a, float b)
    {
        return a + t * (b - a);
    }

    private float Grad(int hash, float x, float y, float z)
    {
        int h = hash & 15;
        float u = h < 8 ? x : y;
        float v = h < 4 ? y : (h == 12 || h == 14 ? x : z);
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }
    public void SetBlock(int x, int y, int z, byte id)
    {
        Voxels[x, y, z] = id;
    }
    public void Make3d()
    {
        strmade = true;
        PerlinNoise3D();
        
        for (int x = 0; x < 16; x++)
        {
            for (int z = 0; z < 16; z++)
            {
                for (int y = 2; y < height[x, z] - 5; y++)
                {
                    int e = x + (Coord.x + 100) * 16;
                    int r = z + (Coord.y + 100) * 16;
                    float n = noise(e * 0.1f, y * 0.1f, r * 0.1f);
                    if (n > 0.3 && n < 0.6f)
                    {
                        Voxels[x, y, z] = 0;
                    }

                    else if (n > 0 && n < 0.2)
                    {
                        foreach (Lode lode in biome[x, z].lodes)
                        {
                            if (lode.minheight <= y && lode.maxheight >= y)
                            {
                                if (Noise.GetPerlin3D(new Vector3(e, y, r), offset.x, offset.y, offset.z, lode.scale, lode.threshold) > lode.threshold)
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
        
        for (int x = 0; x < 16; x++)
        {
            for (int z = 0; z < 16; z++)
            {
                int e = x + Coord.x * 16;
                int r = z + Coord.y * 16;
                if ((biom[x, z] == 1) || (height[x, z] > 30 && Noise.GetThe2DPerlin(new Vector2(x, z), Offset, biome[x, z].treesize, biome[x, z].treethreshold)))
                {
                    Structures.MakeStructures(biom[x, z], new Vector3(e, height[x, z] + 1, r), Offset);
                }
            }
        }

    }
    public void FinishMesh()
    {
        
        mademesh = true;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        mesh.RecalculateNormals();
        chunkObject.GetComponent<MeshFilter>().mesh = mesh;
        ready = true;

    }
    
    public void CreateMesh()
    {
            mesh=new Mesh();
            triangles.Clear();
            vertices.Clear(); uv.Clear();
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 96; y++)
                {
                    for (int z = 0; z < 16; z++)
                    {

                        byte voxel = Voxels[x, y, z];

                        if (voxel == 0)
                        {
                            continue;
                        }
                        if (IsVoxelAir(x, y, z + 1)) AddFace(vertices, 0, triangles, uv, vertexDict, voxel, new Vector3(x, y, z), Vector3.forward);
                        if (IsVoxelAir(x, y, z - 1)) AddFace(vertices, 1, triangles, uv, vertexDict, voxel, new Vector3(x, y, z), Vector3.back);
                        if (IsVoxelAir(x + 1, y, z)) AddFace(vertices, 2, triangles, uv, vertexDict, voxel, new Vector3(x, y, z), Vector3.right);
                        if (IsVoxelAir(x - 1, y, z)) AddFace(vertices, 3, triangles, uv, vertexDict, voxel, new Vector3(x, y, z), Vector3.left);
                        if (IsVoxelAir(x, y + 1, z)) AddFace(vertices, 4, triangles, uv, vertexDict, voxel, new Vector3(x, y, z), Vector3.up);
                        if (IsVoxelAir(x, y - 1, z)) AddFace(vertices, 5, triangles, uv, vertexDict, voxel, new Vector3(x, y, z), Vector3.down);

                    }
                }
            }
        world.unmeshedchk.Enqueue(Coord);

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
        if (y < 0 ||y >=96 )
            return true;
        if((x>=0 && x<16) && z==16)
        {
            return WorldManager.chunks[Coord.x+100,Coord.y+101].Voxels[15,y,15]==0;
        }
        if(x==16 && z < 0)
        {
            return WorldManager.chunks[Coord.x+101, Coord.y +99].Voxels[0, y, 15] == 0;
        }
        if (x<0 && z < 0)
        {
            return WorldManager.chunks[Coord.x + 99, Coord.y +99].Voxels[15, y, 15] == 0;
        }
        if ((x >= 0 && x<16) && z < 0)
        {
            return WorldManager.chunks[Coord.x +100, Coord.y+99].Voxels[x, y, 15] == 0;
        }
        if (x < 0 && (z >= 0 && z<16))
        {
            return WorldManager.chunks[Coord.x +99, Coord.y+100].Voxels[15, y, z] == 0;
        }
        if (x < 0 && z==16)
        {
            return WorldManager.chunks[Coord.x + 99, Coord.y+101].Voxels[15, y, 0] == 0;
        }
        if(x==16 && z>=0 && z<16)
            return WorldManager.chunks[Coord.x + 101, Coord.y + 100].Voxels[0, y, z] == 0;
        if (x==16 && z==16)
            return WorldManager.chunks[Coord.x + 101, Coord.y + 101].Voxels[0, y, 0] == 0;
        return (Voxels[x, y, z]==0);
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
    public float PerlinNoise(float x, float y, int octaves, float persistence, float lacunarity)
    {
        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0;  // Used for normalizing result to 0.0 - 1.0

        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * frequency, y * frequency) * amplitude;

            maxValue += amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return total / maxValue;
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
public static class Noise
{
    public static float Get2DNoise(float x, float z, Vector2 offset, float scale)
    {
        return Mathf.PerlinNoise((x+0.1f)/(16*scale+0),(z+0.1f)/(16*scale+0));    
    }
    public static float GetPerlin3D(Vector3 position, float X, float Y, float Z, float scale, float treshold)
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
        return result;
    }
    public static bool GetThe2DPerlin(Vector2 position, Vector2 offset, float scale, float threshold)
    {
        position.x += (offset.x + scale + 0.1f);
        position.y += (offset.y + scale + 0.1f);

        return Mathf.PerlinNoise(position.x / 16 * scale, position.y / 16 * scale) > threshold;

    }
}