using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class CastleStructure : MonoBehaviour
{
    public int chunkX, chunkY;
    public static bool madeCastle = false, started = false;
    public WorldManager wm;
    public byte size;
    public string location;
    int posX = -348, posY = 72, posZ = -232;
    int fposX = -297, fposY = 122, fposZ = -180;

    public Chunk.VoxelStruct[,,] blocks;

    //X = 2323, Z = 5677
    //chunkx=-355
    //chunkz=145
    public void Start()
    {

    }
    public void Initialize(int width, int height, int depth)
    {
        blocks = new Chunk.VoxelStruct[width, height, depth];
        size = (byte)Mathf.Max(width, height, depth);
    }
    public void CopyFromWorld(WorldManager world)
    {
        int minX = posX;
        int minY = posY;
        int minZ = posZ;

        int width = fposX - posX + 1;
        int height = fposY - posY + 1;
        int depth = fposZ - posZ + 1;

        Initialize(width, height, depth);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    Chunk.VoxelStruct voxel = new(world.Block(minX + x, minY + y, minZ + z), world.Block2(minX + x, minY + y, minZ + z));
                    SetBlock(x, y, z, voxel.Value1, voxel.Value2);
                }
            }
        }

        Debug.Log("Copied castle structure from world.");
    }
    public void SetBlock(int x, int y, int z, byte type, byte metadata)
    {
        if (IsInBounds(x, y, z))
        {
            blocks[x, y, z] = new Chunk.VoxelStruct(type, metadata);
        }
    }

    public Chunk.VoxelStruct GetBlock(int x, int y, int z)
    {
        if (IsInBounds(x, y, z))
        {
            return blocks[x, y, z];
        }
        return new Chunk.VoxelStruct();
    }

    private bool IsInBounds(int x, int y, int z)
    {
        return x >= 0 && x < blocks.GetLength(0) &&
               y >= 0 && y < blocks.GetLength(1) &&
               z >= 0 && z < blocks.GetLength(2);
    }

    public void SaveToFile(string filePath)
    {
        try
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                writer.Write(chunkX);
                writer.Write(chunkY);
                writer.Write(size);
                writer.Write(location);
                writer.Write(posX);
                writer.Write(posY);
                writer.Write(posZ);
                writer.Write(fposX);
                writer.Write(fposY);
                writer.Write(fposZ);

                for (int x = 0; x < blocks.GetLength(0); x++)
                {
                    for (int y = 0; y < blocks.GetLength(1); y++)
                    {
                        for (int z = 0; z < blocks.GetLength(2); z++)
                        {
                            var block = blocks[x, y, z];
                            writer.Write(block.Value1);
                        }
                    }
                }
                for (int x = 0; x < blocks.GetLength(0); x++)
                {
                    for (int y = 0; y < blocks.GetLength(1); y++)
                    {
                        for (int z = 0; z < blocks.GetLength(2); z++)
                        {
                            var block = blocks[x, y, z];
                            writer.Write(block.Value2);
                        }
                    }
                }
            }
            Debug.Log($"Structure saved successfully to {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving structure: {e.Message}");
        }
    }

    // Load the structure from a file, but I dont know which one
    public static CastleStructure LoadFromFile(string filePath)
    {
        try
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                GameObject structureObj = new GameObject("LoadedStructure");
                CastleStructure structure = structureObj.AddComponent<CastleStructure>();

                // Read metadata
                structure.chunkX = reader.ReadInt32();
                structure.chunkY = reader.ReadInt32();
                structure.size = reader.ReadByte();
                structure.location = reader.ReadString();
                structure.posX = reader.ReadInt32();
                structure.posY = reader.ReadInt32();
                structure.posZ = reader.ReadInt32();
                structure.fposX = reader.ReadInt32();
                structure.fposY = reader.ReadInt32();
                structure.fposZ = reader.ReadInt32();
                int width = structure.fposX - structure.posX + 1;
                int height = structure.fposY - structure.posY + 1;
                int depth = structure.fposZ - structure.posZ + 1;

                // Read voxel data
                structure.blocks = new Chunk.VoxelStruct[width, height, depth];
                for (int x = 0; x < structure.blocks.GetLength(0); x++)
                {
                    for (int y = 0; y < structure.blocks.GetLength(1); y++)
                    {
                        for (int z = 0; z < structure.blocks.GetLength(2); z++)
                        {
                            byte type = reader.ReadByte();
                            structure.blocks[x, y, z].Value1 = type;
                            structure.blocks[x, y, z].Value2 = 0;
                        }
                    }
                }
                /*
                for (int x = 0; x < structure.blocks.GetLength(0); x++)
                {
                    for (int y = 0; y < structure.blocks.GetLength(1); y++)
                    {
                        for (int z = 0; z < structure.blocks.GetLength(2); z++)
                        {
                            byte metadata = reader.ReadByte();
                            structure.blocks[x, y, z].Value2 = metadata;
                        }
                    }
                }
                */
                Debug.Log($"Structure loaded successfully from {filePath}");
                return structure;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading structure: {e.Message}");
            return null;
        }
    }

    public CastleStructure targetStructure;
    public string saveFileName = "castle.structure";
    private bool hasSaved = false;
    private bool made = false;

    void Updatee()
    {
        if (!hasSaved && Input.GetKeyDown(KeyCode.F5))
        {

            if (targetStructure == null)
            {
                Debug.LogError("No CastleStructure assigned to CastleSaver.");
                return;
            }

            string directory = Path.Combine(Application.persistentDataPath, "Structures");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            CopyFromWorld(wm);

            string fullPath = Path.Combine(directory, saveFileName);
            targetStructure.SaveToFile(fullPath);

            hasSaved = true;
            Debug.Log($"Castle saved to {fullPath}");
        }
        if (!made && Input.GetKeyDown(KeyCode.Minus))
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Structures/castle.structure");
            made = true;
            CastleStructure c = LoadFromFile(path);
            wm.PasteStructure(c, 0, 72, 50);
        }
    }
    public static void CreateCastle()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Structures/castle.structure");
        CastleStructure c = LoadFromFile(path);
        madeCastle = true;
        int height = WorldManager.GetChunk(144, 354).height[8, 0] + 3;
        ControllerImput.Instance.wmanager.PasteStructure(c, 2300, height, 5670);
    }
    public static IEnumerator WaitForCastleChunks()
    {
        while (WorldManager.castleChunksReady.Count < 25)
        {
            yield return null;

        }
        CreateCastle();
    }
    public static void StartCounting()
    {
        if (!started)
        {
            started = true;
            Toolbar.instance.StartCoroutine(WaitForCastleChunks());
        }
    }
}