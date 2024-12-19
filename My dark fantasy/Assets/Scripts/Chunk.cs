using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Chunk
{
    private readonly object listLock = new object();
    public byte[,,] Voxels = new byte[16, 160, 16];
    public ChunkCoord Coord;
    public BiomeAttributes[,] biome;
    public Material waterMat;
    public byte[,] biom=new byte[16,16];
    public Lode lode;
    public byte maxheight = 4;
    public bool mademesh = false;
    public bool strstart;
    public bool ready = false;
    public WorldManager world;
    public GameObject chunkObject;
    public GameObject child1,child2,child3,child4,child5=null,child6=null;
    public System.Random random;
    public Mesh mesh;
    public bool start=false;
    public int seed = 3345;
    public bool needsAnUpdate = false;
    Vector2 Offset;
    Vector3 offset;
    readonly byte[,] height = new byte[16, 16];
    public bool strmade=false;
    readonly List<Vector3> vertices = new(20000);
    readonly List<Vector3> watervertices = new(20000);
    readonly List<int> triangles = new(30000);
    readonly List<int> watertriangles = new(90000);
    readonly List<Vector2> uv = new(20000);
    readonly List<Vector2> uw = new(20000);
    readonly Dictionary<Vector3, int> vertexDict = new();

    //the most important function
    public float CalculateWomanSalary(float salary)
    {
        return salary * 0.8f;
    }

    public Chunk(ChunkCoord coord, WorldManager wmanager)
    {
        Coord = coord;
        chunkObject = new GameObject("Chunk " + (Coord.x).ToString() + " " + (Coord.y).ToString());
        chunkObject.transform.position = new Vector3(Coord.x * 16, 0f, Coord.y * 16);
        child1 = new GameObject("1", typeof(MeshFilter), typeof(MeshRenderer));
        child1.transform.position = new Vector3(Coord.x * 16, 0f, Coord.y * 16);
        child2 = new GameObject("2", typeof(MeshFilter), typeof(MeshRenderer));
        child2.transform.position = new Vector3(Coord.x * 16, 0f, Coord.y * 16);
        child3 = new GameObject("3", typeof(MeshFilter), typeof(MeshRenderer));
        child3.transform.position = new Vector3(Coord.x * 16, 0f, Coord.y * 16);
        child4 = new GameObject("4", typeof(MeshFilter), typeof(MeshRenderer));
        child4.transform.position = new Vector3(Coord.x * 16, 0f, Coord.y * 16);
        child1.transform.SetParent(chunkObject.transform);
        child2.transform.SetParent(chunkObject.transform);
        child3.transform.SetParent(chunkObject.transform);
        child4.transform.SetParent(chunkObject.transform);
        waterMat = wmanager.waterMat;
        Material material = wmanager.material;
        child1.GetComponent<MeshRenderer>().materials =new Material[] { material,waterMat};
        child2.GetComponent<MeshRenderer>().materials = new Material[] { material, waterMat };
        child3.GetComponent<MeshRenderer>().materials = new Material[] { material, waterMat};
        child4.GetComponent<MeshRenderer>().materials = new Material[] { material, waterMat };

        seed = ChunkSerializer.seed;
        
        random = new System.Random(seed);
        Offset = new Vector2(random.Next(-100000, 100000), random.Next(-100000, 100000));
        offset = new Vector3(random.Next(-100000, 100000), random.Next(-100000, 100000), random.Next(-100000, 100000));
        world = wmanager;
        biome = new BiomeAttributes[16, 16];
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
               
                for (int i = y + 1; i < 65; i++)
                    SetBlock(x, i, z, 21);
            }
        }
        lock(listLock){
            ChunkSerializer.loadedChunks.Add((Coord.x, Coord.y), Voxels);
        }
    }
    public float CombinedNoise(float x, float z, int octaves, float persistence, float lacunarity)
    {
        float total = 0;
        float frequency = 1f;
        float amplitude = 1;
        float maxValue = 0;  
        for (int i = 0; i < octaves; i++)
        {
            float noiseValue = Mathf.PerlinNoise(0.005f*(x + Offset.x) * frequency,0.005f*(z + Offset.y) * frequency);
            total += noiseValue * amplitude;

            maxValue += amplitude;

            amplitude *= persistence; 
            frequency *= lacunarity;
        }
        
        return total / maxValue; 
    }
    public float[,] generateHeightMap()
    {
        float[,] heightMap = new float[16, 16];
        for (int i = 0; i < 16; i++)
        {
            for(int j=0; j<16; j++) { 
            byte index = 0;
                float worldX = Coord.x * 16 + i;
                float worldZ = Coord.y * 16 + j;
                float erosion = CombinedNoise(worldX, worldZ,4,0.01f,2f)-0.5f;
                float valley=CombinedNoise(worldX,worldZ,4,0.1f,2.6f)-0.5f;
                float continentalness = CombinedNoise(worldX , worldZ , 4, 0.1f, 2f);
                float height=60;

                if (continentalness < 0.3f)
                    height += 20 * continentalness;
                else if (continentalness < 0.6)
                    height += 25 * continentalness;
                else if (continentalness < 0.62)
                    height += 27 * continentalness;
                else if (continentalness < 0.65)
                    height += 29 * continentalness;
                else if (continentalness < 0.68)
                    height += 32 * continentalness;
                else if (continentalness < 0.7)
                    height += 35 * continentalness;
                else if (continentalness < 0.72)
                    height += 37 * continentalness;
                else if (continentalness < 0.75)
                    height += 39 * continentalness;
                else if (continentalness < 0.78)
                    height += 41 * continentalness;
                else if (continentalness < 0.8)
                    height += 44 * continentalness;
                else if (continentalness < 0.83)
                    height += 46 * continentalness;
                else if (continentalness < 0.85)
                    height += 48 * continentalness;
                else if (continentalness < 0.87)
                    height += 50 * continentalness;
                else if (continentalness < 0.90)
                    height += 52 * continentalness;
                else if (continentalness < 0.93)
                    height += 55 * continentalness;
                else if (continentalness < 0.95)
                    height += 57 * continentalness;
                else if (continentalness < 0.98)
                    height += 60 * continentalness;
                else
                    height += 64 * continentalness;
                
               
                if (valley < (-0.4f))
                    height =60- valley * 45;
                else if (valley < -0.3)
                    height += valley * 30;
                else if (valley < -0.2)
                    height += valley * 20;
                else if (valley < -0.1f)
                    height += valley * 12;
                else if (valley < 0.1f)
                    height += 3 * valley;
                else if (valley < 0.2)
                    height += 14 * valley;
                else if (valley < 0.25)
                    height += 17 * valley;
                else if (valley < 0.3)
                    height += 20 * valley;
                else if (valley < 0.35)
                    height += 25 * valley;
                else if (valley < 0.4)
                    height += 30 * valley;
                else if (valley < 0.45)
                    height += 35 * valley;
                else if (valley < 0.5)
                    height += 40 * valley;
                /*
                if(erosion<-0.4)
                    height -=60*erosion;
                else if(erosion<-0.35)
                    height -=50*erosion;
                else if(erosion<-0.3)
                    height -=40*erosion;
                else if(erosion<-0.25)
                    height -=30*erosion;
                else if(erosion<-0.2)
                    height -=25*erosion;
                else if(erosion<-0.1)
                    height-=30*erosion;
                else if (erosion < 0.1)
                    height += 10 * erosion;
                else if (erosion < 0.3f)
                    height += 10 * erosion;
                else if (erosion < 0.5)
                    height += 5*erosion;
                else if (erosion < 0.6)
                    height += erosion*3;
                else if (erosion < 0.8)
                    height += erosion * 2;
                else
                    height += 1;
                */
                heightMap[i, j] =height;
                if (continentalness < 0.3 && valley < 0.3)
                    index = 1;
                else if (continentalness < 0.4 && erosion > 0.5)
                    index = 0;
                else if (continentalness < 0.5 && valley < 0.3)
                    index = 2;
                else if (continentalness < 0.5 && valley < 0.5)
                    index = 3;
                else if (continentalness < 0.6 && erosion > 0.5)
                    index = 4;
                else if(continentalness<0.6 && valley<0.3 && erosion>0.6)
                    index = 5;
                else if(continentalness<0.7 && erosion>0.7)
                    index= 6;
                else
                    index = 7;
                
                biome[i, j] = world.Biome[index];
                biom[i, j] = index;

                if (heightMap[i, j] <= 0)
                heightMap[i, j] += 20;
            if (heightMap[i, j] > 127)
            {
                if (heightMap[i, j] > 160)
                {
                    maxheight = 6;

                }

                else
                {
                    maxheight = 5;
                }
            }
        }
        }
        return heightMap;
    }
    private float BilinearInterpolate(float topLeft, float topRight, float bottomLeft, float bottomRight, float tx, float ty)
    {
        float top = Mathf.Lerp(topLeft, topRight, tx);
        float bottom = Mathf.Lerp(bottomLeft, bottomRight, tx);
        return Mathf.Lerp(top, bottom, ty);
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
        strstart=true;
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

                    else if (n > 0.03 && n < 0.27f)
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
            ChunkSerializer.loadedChunks[(Coord.x, Coord.y)] = Voxels;
        if(MathF.Abs(Coord.x)>1 || MathF.Abs(Coord.y)>1)
        for (int x = 0; x < 16; x++)
        {
            for (int z = 0; z < 16; z++)
            {
                int e = x + Coord.x * 16;
                int r = z + Coord.y * 16;
                if ((biom[x, z] == 1) || (height[x, z] > 30 && Noise.GetThe2DPerlin(new Vector2(e, r), Offset, biome[x, z].treesize, biome[x, z].treethreshold)))
                {
                    Structures.MakeStructures(biom[x, z], new Vector3(e, (height[x, z] + 1), r), Offset);
                }
            }
        }
        strmade = true;
    }
    public void CreateMesh()
    {
        mademesh = true;

        if (maxheight==6 && child6== null)
        {
            child6 = new GameObject("6", typeof(MeshFilter), typeof(MeshRenderer));
            child6.transform.SetParent(chunkObject.transform);
            child6.GetComponent<MeshRenderer>().materials = new Material[] { world.material, world.waterMat };
            child6.transform.position = new Vector3(Coord.x * 16, 0f, Coord.y * 16);
        }
        if (maxheight>=5 && child5== null)
        {
            child5 = new GameObject("5", typeof(MeshFilter), typeof(MeshRenderer));
            child5.transform.SetParent(chunkObject.transform);
            child5.GetComponent<MeshRenderer>().materials = new Material[] {world.material, world.waterMat };
            child5.transform.position = new Vector3(Coord.x * 16, 0f, Coord.y * 16);
        }
        for (int i = 0; i < maxheight; i++)
        {
            mesh = new Mesh();
            triangles.Clear();
            vertices.Clear(); uv.Clear();
            watertriangles.Clear();
            for (int x = 0; x < 16; x++)
            {
                for (int y = i*32; y <(i+1)*32; y++)
                {
                    for (int z = 0; z < 16; z++)
                    {

                        byte voxel = Voxels[x, y, z];

                        if (voxel == 0)
                        {
                            continue;
                        }
                        switch (world.blockTypes[voxel].Items.blocks.type)
                        {
                            case 0:
                                {
                                    if (IsVoxelAir(x, y, z + 1)) AddFace(vertices, 0, triangles, uv, vertexDict, voxel, new Vector3(x, y, z), Vector3.forward);
                                    if (IsVoxelAir(x, y, z - 1)) AddFace(vertices, 1, triangles, uv, vertexDict, voxel, new Vector3(x, y, z), Vector3.back);
                                    if (IsVoxelAir(x + 1, y, z)) AddFace(vertices, 2, triangles, uv, vertexDict, voxel, new Vector3(x, y, z), Vector3.right);
                                    if (IsVoxelAir(x - 1, y, z)) AddFace(vertices, 3, triangles, uv, vertexDict, voxel, new Vector3(x, y, z), Vector3.left);
                                    if (IsVoxelAir(x, y + 1, z)) AddFace(vertices, 4, triangles, uv, vertexDict, voxel, new Vector3(x, y, z), Vector3.up);
                                    if (IsVoxelAir(x, y - 1, z)) AddFace(vertices, 5, triangles, uv, vertexDict, voxel, new Vector3(x, y, z), Vector3.down);
                                    break;
                                }
                            case 2:
                                {
                                    MakeFlowers(vertices, triangles, uv, vertexDict, voxel, new Vector3(x, y, z));
                                    break;
                                }
                            case 5:
                                {
                                    PlaceWater(vertices, watertriangles, uv, vertexDict, voxel, new Vector3(x, y, z));
                                    break;
                                }

                        }
                    }
                }
            }
            mesh.subMeshCount = 2;
            mesh.vertices = vertices.ToArray();
            mesh.SetTriangles(triangles.ToArray(), 0); 
            mesh.SetTriangles(watertriangles.ToArray(), 1);
            mesh.uv=uv.ToArray();
            mesh.RecalculateNormals();

            if (i==0)
                child1.GetComponent<MeshFilter>().mesh = mesh;
            else if(i==1)
                child2.GetComponent<MeshFilter>().mesh = mesh;
            else if(i==2)
                child3.GetComponent<MeshFilter>().mesh = mesh;
            else if(i==3)
                child4.GetComponent<MeshFilter>().mesh = mesh;
            else if(i==4)
                child5.GetComponent<MeshFilter>().mesh = mesh;
            else if(i==5)
                child6.GetComponent<MeshFilter>().mesh = mesh;
        }
        if (!WorldManager.chunkstosave.Contains(Coord))
        {
            WorldManager.chunkstosave.Add(Coord);
        }
    }
    void PlaceWater(List<Vector3> vertices, List<int> triangles, List<Vector2> uv, Dictionary<Vector3, int> vertexDict, byte voxel, Vector3 pos)
    {
        if (IsntWater((int)pos.x, (int)pos.y, (int)pos.z + 1)) AddWaterFace(vertices, 0, triangles, uv, vertexDict, voxel, pos, Vector3.forward);
        if (IsntWater((int)pos.x, (int)pos.y, (int)pos.z - 1)) AddWaterFace(vertices, 1, triangles, uv, vertexDict, voxel, pos, Vector3.back);
        if (IsntWater((int)pos.x + 1, (int)pos.y, (int)pos.z)) AddWaterFace(vertices, 2, triangles, uv, vertexDict, voxel, pos, Vector3.right);
        if (IsntWater((int)pos.x - 1, (int)pos.y, (int)pos.z)) AddWaterFace(vertices, 3, triangles, uv, vertexDict, voxel, pos, Vector3.left);
        if (IsntWater((int)pos.x, (int)(pos.y) + 1, (int)pos.z)) AddWaterFace(vertices, 4, triangles, uv, vertexDict, voxel, pos, Vector3.up);
        if (IsntWater((int)pos.x, (int)pos.y - 1, (int)pos.z)) AddWaterFace(vertices, 5, triangles, uv, vertexDict, voxel, pos, Vector3.down);
    }
    void MakeFlowers(List<Vector3> vertices, List<int> triangles, List<Vector2> uv, Dictionary<Vector3, int> vertexDict, byte voxel, Vector3 position)
    {
        if (IsVoxelAir((int)position.x, (int)position.y - 1, (int)position.z))
        {
            SetBlock((int)position.x, (int)position.y, (int)position.z, 0);
        }
        else
        {
            Vector3 g = new ();
            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        {
                            g = Vector3.forward;
                            break;
                        }
                    case 1:
                        {
                            g = Vector3.back;
                            break;
                        }
                    case 2:
                        {
                            g = Vector3.right;
                            break;
                        }
                    case 3:
                        {
                            g = Vector3.left;
                            break;
                        }
                }

                Quaternion rotation = Quaternion.AngleAxis(45f, Vector3.up);
                Vector3 right = g;
                Vector3 up = Vector3.up;
                right = rotation * right;
                int vertexIndex = vertices.Count;

                vertices.Add(position - right * 0.5f - up * 0.5f);
                vertices.Add(position + right * 0.5f - up * 0.5f);
                vertices.Add(position + right * 0.5f + up * 0.5f);
                vertices.Add(position - right * 0.5f + up * 0.5f);

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 3);

                float uvSize = 0.0625f;
                byte bid = (byte)world.blockTypes[voxel].Items.blocks.frontfacetexture;
                Vector2 uvBase = GetUVForVoxelType(bid);
                uv.Add(uvBase + new Vector2(0, 0) * uvSize);
                uv.Add(uvBase + new Vector2(1, 0) * uvSize);
                uv.Add(uvBase + new Vector2(1, 1) * uvSize);
                uv.Add(uvBase + new Vector2(0, 1) * uvSize);
            }
        }
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
        byte bid = (byte)world.blockTypes[voxel].Items.blocks.GetTextureID(face);
        Vector2 uvBase = GetUVForVoxelType(bid);
        uv.Add(uvBase + new Vector2(0, 0) * uvSize);
        uv.Add(uvBase + new Vector2(1, 0) * uvSize);
        uv.Add(uvBase + new Vector2(1, 1) * uvSize);
        uv.Add(uvBase + new Vector2(0, 1) * uvSize);
    }
    void AddWaterFace(List<Vector3> vertices, byte face, List<int> triangles, List<Vector2> uv, Dictionary<Vector3, int> vertexDict, byte voxel, Vector3 position, Vector3 direction)
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

        watertriangles.Add(vertexIndex);
        watertriangles.Add(vertexIndex + 1);
        watertriangles.Add(vertexIndex + 2);
        watertriangles.Add(vertexIndex);
        watertriangles.Add(vertexIndex + 2);
        watertriangles.Add(vertexIndex + 3);
        uv.Add(new Vector2(0, 0));
        uv.Add(new Vector2(1, 0));
        uv.Add(new Vector2(1, 1));
        uv.Add(new Vector2(0, 1));

    }
    bool IsVoxelAir(int x, int y, int z)
    {
        if (y < 0 ||y >=159 )
            return false;
        //performanta
        if (x == 16 && z >= 0 && z < 16)
            return !world.blockTypes[WorldManager.GetChunk(Coord.x+1,Coord.y).Voxels[0, y, z]].Items.isblock;
        if (x == 16 && z == 16)
            return !world.blockTypes[WorldManager.GetChunk(Coord.x + 1, Coord.y+1).Voxels[0, y, 0]].Items.isblock;
        if ((x>=0 && x<16) && z==16)
        {
            return !world.blockTypes[WorldManager.GetChunk(Coord.x, Coord.y + 1).Voxels[x,y,0]].Items.isblock;
        }
        if(x==16 && z < 0)
        {
            return !world.blockTypes[WorldManager.GetChunk(Coord.x +1,Coord.y-1).Voxels[0, y, 15]].Items.isblock;
        }
        if (x<0 && z < 0)
        {
            return !world.blockTypes[WorldManager.GetChunk(Coord.x-1, Coord.y-1).Voxels[15, y, 15]].Items.isblock;
        }
        if ((x >= 0 && x<16) && z < 0)
        {
            return !world.blockTypes[WorldManager.GetChunk(Coord.x , Coord.y-1).Voxels[x, y, 15]].Items.isblock;
        }
        if (x < 0 && (z >= 0 && z<16))
        {
            return !world.blockTypes[WorldManager.GetChunk(Coord.x - 1, Coord.y).Voxels[15, y, z]].Items.isblock;
        }
        if (x < 0 && z==16)
        {
            return !world.blockTypes[WorldManager.GetChunk(Coord.x-1, Coord.y + 1).Voxels[15, y, 0]].Items.isblock;
        }
        
        return (!world.blockTypes[Voxels[x, y, z]].Items.isblock);
    }
    bool IsntWater(int x, int y, int z)
    {
        if (y < 0 || y >= 159)
            return false;
        //performanta
        if (x == 16 && z >= 0 && z < 16)
            return !(world.blockTypes[WorldManager.GetChunk(Coord.x + 1, Coord.y).Voxels[0, y, z]].Items.blocks.type==5);
        if (x == 16 && z == 16)
            return !(world.blockTypes[WorldManager.GetChunk(Coord.x + 1, Coord.y+1).Voxels[0, y, 0]].Items.blocks.type==5);
        if ((x >= 0 && x < 16) && z == 16)
        {
            return !(world.blockTypes[WorldManager.GetChunk(Coord.x , Coord.y+1).Voxels[x, y, 0]].Items.blocks.type==5);
        }
        if (x == 16 && z < 0)
        {
            return !(world.blockTypes[WorldManager.GetChunk(Coord.x + 1, Coord.y-1).Voxels[0, y, 15]].Items.blocks.type==5);
        }
        if (x < 0 && z < 0)
        {
            return !(world.blockTypes[WorldManager.GetChunk(Coord.x - 1, Coord.y-1).Voxels[15, y, 15]].Items.blocks.type==5);
        }
        if ((x >= 0 && x < 16) && z < 0)
        {

            return !(world.blockTypes[WorldManager.GetChunk(Coord.x , Coord.y-1).Voxels[x, y, 15]].Items.blocks.type==5);
        }
        if (x < 0 && (z >= 0 && z < 16))
        {
            return !(world.blockTypes[WorldManager.GetChunk(Coord.x - 1, Coord.y).Voxels[15, y, z]].Items.blocks.type==5);
        }
        if (x < 0 && z == 16)
        {
            return !(world.blockTypes[WorldManager.GetChunk(Coord.x - 1, Coord.y+1).Voxels[15, y, 0]].Items.blocks.type==5);
        }

        return !(world.blockTypes[Voxels[x, y, z]].Items.blocks.type==5);
    }
    Vector2 GetUVForVoxelType(byte id)
    {
        //nu modifica nimic, dar daca e ceva marimea atlasului e 16x16
        int atlasIndex;
        int x;
        if (id % 16 != 0)
        {
            atlasIndex = id % 16 + 16 * (15 - id / 16);
            atlasIndex--;
            x = atlasIndex % 16;
        }
        else
        {
            atlasIndex = 16 * (17 - id / 16);
            atlasIndex--;
            x = 15;
        }
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
    public class ChunkCoord{

    public int x;
    public int y;
    public ChunkCoord(int X, int Y)
    {
        x = X;
        y = Y;
    }
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        ChunkCoord other = (ChunkCoord)obj;

        return x==other.x && y==other.y;
    }

    public override int GetHashCode()
    {
        return x.GetHashCode() ^ y.GetHashCode();
    }
}
    public static class Noise{
    
    public static float Get2DNoise(float x, float z, Vector2 offset, float scale)
    {
        return Mathf.PerlinNoise((x + 0.2f+offset.x) / (16 * (scale)), (z +0.3f+offset.x) / ((16 * scale )-offset.x- offset.y));
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