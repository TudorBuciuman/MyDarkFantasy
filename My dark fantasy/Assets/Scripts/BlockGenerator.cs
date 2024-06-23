using static Voxeldata;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
    public int width = 100;
    public int height = 100;
    public int depth = 100;
    public Material voxelMaterial; // Add this line

    byte[,,] voxels=new byte[1000,1000,1000];

    void Start()
    {

        Application.targetFrameRate = 50;
        GenerateTerrain();
        AddInitialBlocks();
        CreateMesh();
    }

    void GenerateTerrain()
    {
        voxels = new byte[width, 100, depth];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y <1; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    voxels[x, y, z] = (byte)0;
                }
            }
        }
    }

    void AddInitialBlocks()
    {
        // Add specific blocks before the game starts
        for (int i = 0; i <= 20; i++)
        {
            for (int j = 0; j <= 20; j++)
                SetBlock(i, 1, j, 1);
        }
        SetBlock(3, 1, 3, 1); // Grass block at (3, 1, 3)
        SetBlock(3, 2, 3, 2); // Stone block at (3, 2, 3)
        SetBlock(4, 1, 3, 1); // Grass block at (4, 1, 3)
        SetBlock(4, 2, 3, 2); // Stone block at (4, 2, 3)
        SetBlock(5, 1, 3, 1); // Grass block at (5, 1, 3)
    }

    void SetBlock(int x, int y, int z, byte type)
    {
        if (x >= 0 && x < width && y >= 0 && y < height && z >= 0 && z < depth)
        {
            voxels[x, y, z] = type;
        }
    }

    void CreateMesh()
    {
        Mesh mesh=new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    byte voxel = voxels[x, y, z];
                    if (voxel == 0) continue;
                    
                    // Add faces for each direction
                    if (IsVoxelAir(x, y, z + 1)) AddFace(vertices, triangles, uv, voxel, new Vector3(x, y, z), Vector3.forward);
                    if (IsVoxelAir(x, y, z - 1)) AddFace(vertices, triangles, uv, voxel, new Vector3(x, y, z), Vector3.back);
                    if (IsVoxelAir(x + 1, y, z)) AddFace(vertices, triangles, uv, voxel, new Vector3(x, y, z), Vector3.right);
                    if (IsVoxelAir(x - 1, y, z)) AddFace(vertices, triangles, uv, voxel, new Vector3(x, y, z), Vector3.left);
                    if (IsVoxelAir(x, y + 1, z)) AddFace(vertices, triangles, uv, voxel, new Vector3(x, y, z), Vector3.up);
                    if (IsVoxelAir(x, y - 1, z)) AddFace(vertices, triangles, uv, voxel, new Vector3(x, y, z), Vector3.down);
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        mesh.RecalculateNormals();

        GameObject voxelObject = new GameObject("Voxel Terrain", typeof(MeshFilter), typeof(MeshRenderer));
     //   voxelObject.GetComponent<MeshFilter>().mesh = mesh;
     //   voxelObject.GetComponent<MeshRenderer>().material = voxelMaterial; // Assign the material here
        GameObject terrain = new GameObject("Terrain");
        terrain.transform.parent = this.transform;
        MeshFilter meshFilter = terrain.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = terrain.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = terrain.AddComponent<MeshCollider>();

        meshFilter.mesh = mesh;
        meshRenderer.material = voxelMaterial; 
        meshCollider.sharedMesh = mesh;
    }

    bool IsVoxelAir(int x, int y, int z)
    {
        if (x>=width || y>=height || z>=depth ||z < 0 || y < 0 || x < 0 || voxels[x, y, z]==0)
            return true;
        return voxels[x, y, z] == 0;
    }

    void AddFace(List<Vector3> vertices, List<int> triangles, List<Vector2> uv, byte voxel, Vector3 position, Vector3 direction)
    {
        int vertexIndex = vertices.Count;

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

        Vector2 uvBase = GetUVForVoxelType(voxel);
        uv.Add(uvBase + new Vector2(0, 0));
        uv.Add(uvBase + new Vector2(1, 0));
        uv.Add(uvBase + new Vector2(1, 1));
        uv.Add(uvBase + new Vector2(0, 1));
    }

    Vector2 GetUVForVoxelType(byte voxelType)
    {
        // Assuming you have a texture atlas where each block type occupies a specific tile
        // Adjust the values to match your texture atlas
        float tileSize = 0.25f; // Example: 4x4 texture atlas
        switch (voxelType)
        {
            case 1: return new Vector2(0, 0); // Grass
            case 2: return new Vector2(tileSize, 0); // Stone
            default: return new Vector2(0, 0); // Default to grass
        }
    }

    public void PlaceBlock(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);
        int z = Mathf.RoundToInt(position.z);

        if (x < 0 || x >= width || y < 0 || y >= height || z < 0 || z >= depth) return;

        voxels[x, y, z] = 1; // Set the voxel type to grass (or any other type you want)
        CreateMesh(); // Recreate the mesh to reflect changes
    }

    public void RemoveBlock(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);
        int z = Mathf.RoundToInt(position.z);

        if (x < 0 || x >= width || y < 0 || y >= height || z < 0 || z >= depth) return;

        voxels[x, y, z] = 0; // Set the voxel type to air
        CreateMesh(); // Recreate the mesh to reflect changes
    }
}
