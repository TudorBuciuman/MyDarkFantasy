using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using System;
public class ChunkSerializer
{
    public static string savePath;
    public static int seed = -1;
    public static bool pret = false;
    public static Vector3 pos;
    public static Quaternion rot;
    public static float mx = 0, my = 0;
    public static string[] loadpath;
    public static Dictionary<(int, int), Chunk.VoxelStruct[,,]> loadedChunks = new();

    public static void LoadChunk(int cx, int cz)
    {

        if (loadedChunks.ContainsKey((cx, cz)))
        {
            WorldManager.GetChunk(cx, cz).Voxels = loadedChunks[(cx, cz)];

        }
        else
        {
            // fiecare fisier contine 32x32 chunkuri
            int rx = cx / 32;
            if (cx < 0 && cx % 32 != 0)
                rx--;
            int rz = cz / 32;
            if (cz < 0 && cz % 32 != 0)
                rz--;
            string fileName = savePath + $"/chunks/r.{rx}.{rz}.zlib";
            // Bazat pe pozitia chunkului pot citi din fisierul mare
            Chunk.VoxelStruct[,,] chunkData = ChunkReader(fileName, cx, cz);
            lock (loadedChunks)
            {
                loadedChunks[(cx, cz)] = chunkData;
            }
            WorldManager.GetChunk(cx, cz).strmade = true;
            WorldManager.GetChunk(cx, cz).Voxels = chunkData;
        }
    }
    public static Chunk.VoxelStruct[,,] FindChunkInRegion(string fileName, int cx, int cz)
    {
        if (!File.Exists(fileName))
        {
            return null;
        }

        using FileStream fs = new(fileName, FileMode.Open, FileAccess.Read);
        BinaryReader reader = new(fs);

        // Calculate chunk coordinates within the region (32x32 grid)
        int regionChunkX = ((cx % 32) + 32) % 32;
        int regionChunkZ = ((cz % 32) + 32) % 32;
        int chunkIndex = regionChunkX + (regionChunkZ * 32);

        // Seek to the chunk entry in the header
        fs.Seek(chunkIndex * 4, SeekOrigin.Begin);
        byte[] entry = reader.ReadBytes(4);

        // Extract chunkOffset (3 bytes) and chunkSectorCount (1 byte)
        int chunkOffset = (entry[0] << 8) | (entry[1]);
        int chunkSectorCount = entry[3];

        // Check if the chunk is empty or uninitialized
        if (chunkOffset == 0 || chunkSectorCount == 0)
        {
            return null;
        }

        long offsetPosition = (long)chunkOffset * 4096;

        // Check if offset is valid
        if (offsetPosition >= fs.Length)
        {
            return null;
        }

        // Seek to the chunk data and read the compressed chunk
        fs.Seek(offsetPosition, SeekOrigin.Begin);
        byte[] compressedData = reader.ReadBytes(chunkSectorCount * 4096);

        // Decompress the chunk data using Zlib (Deflate)
        using (MemoryStream memoryStream = new(compressedData))
        using (DeflateStream deflateStream = new(memoryStream, CompressionMode.Decompress))
        using (MemoryStream resultStream = new())
        {
            deflateStream.CopyTo(resultStream);
            byte[] decompressedData = resultStream.ToArray();

            // Reconstruct the 3D array from decompressed data
            int structSize = 2; // Each VoxelStruct is 2 bytes (Value1 and Value2)
            int voxelCount = decompressedData.Length / structSize;
            if (voxelCount != 16 * 160 * 16)
            {
                throw new InvalidDataException("Decompressed data size does not match expected voxel count.");
                //Ceva nu a functionat corespunzator, si nu stiu de ce
            }

            Chunk.VoxelStruct[,,] voxels = new Chunk.VoxelStruct[16, 160, 16];
            int index = 0;

            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 160; y++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        voxels[x, y, z].Value1 = decompressedData[index++];
                    }
                }
            }
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 160; y++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        voxels[x, y, z].Value2 = decompressedData[index++];
                    }
                }
            }


            return voxels;
        }
    }
    public static bool IsReal(int cx, int cz)
    {
        if (loadedChunks.TryGetValue((cx, cz), out Chunk.VoxelStruct[,,] value) && value != null)
        {
            return true;
        }
        int rx = cx / 32;
        int rz = cz / 32;
        if (cx < 0 && cx % 32 != 0)
            rx--;
        if (cz < 0 && cz % 32 != 0)
            rz--;
        string fileName = savePath + $"/chunks/r.{rx}.{rz}.zlib";
        Chunk.VoxelStruct[,,] data = FindChunkInRegion(fileName, cx, cz);
        if (data == null)
        {
            return false;
        }
        return true;

    }
    public static void SaveChunk(int cx, int cz)
    {
        // Calculate region coordinates (region files are 32x32 chunks)
        //daca citesti asta, Da, Eu am sris tot ce poti citi in acest script,
        //de asta nu merge
        int rx = cx / 32;
        int rz = cz / 32;
        if (cx < 0 && cx % 32 != 0)
            rx--;
        if (cz < 0 && cz % 32 != 0)
            rz--;
        string fileName = savePath + $"/chunks/r.{rx}.{rz}.zlib";
        if (!loadedChunks.TryGetValue((cx, cz), out Chunk.VoxelStruct[,,] chunkData))
        {
            return;
        }
        else
            SerializeChunkToRegion(fileName, cx, cz, chunkData);
    }
    public static void SerializeChunkToRegion(string fileName, int cx, int cz, Chunk.VoxelStruct[,,] data)
    {
        // Ensure the directory and file exist
        if (!File.Exists(fileName))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileName) ?? string.Empty);
            byte[] heade = new byte[4096]; // 4 KiB header
            using FileStream fss = new(fileName, FileMode.OpenOrCreate, FileAccess.Write);
            fss.Write(heade, 0, 4096);
        }

        // 3D `VoxelStruct` devine un vector 1D de bytes
        byte[] chunkData = new byte[16 * 16 * 160 * 2]; // Fiecare voxel are 2 bytes (Value1 si Value2)
        int index = 0;

        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 160; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    chunkData[index++] = data[x, y, z].Value1; //primul byte
                }
            }
        }
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 160; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    chunkData[index++] = data[x, y, z].Value2; // al doilea byte
                }
            }
        }

        // Compress the chunk data using Zlib (Deflate)
        //idk, pe asta chiar l-am copiat
        byte[] compressedData;
        using (var memoryStream = new MemoryStream())
        {
            using (var deflateStream = new DeflateStream(memoryStream, System.IO.Compression.CompressionLevel.Optimal, true))
            {
                deflateStream.Write(chunkData, 0, chunkData.Length);
            }
            compressedData = memoryStream.ToArray();
        }
        //Restul functiei e overkill
        //Deci nu incerca sa-l intelegi

        // Open or create the region file

        using FileStream fs = new(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        BinaryWriter writer = new(fs);
        BinaryReader reader = new(fs);


        // Calculate chunk index within the region (32x32 grid)
        int regionChunkX = ((cx % 32) + 32) % 32;
        int regionChunkZ = ((cz % 32) + 32) % 32;
        int chunkIndex = regionChunkX + (regionChunkZ * 32);

        int headerSize = 4096; // 4 KiB header with 3 bytes for location and 1 byte for size
        byte[] header = new byte[headerSize];

        // Read the existing header
        fs.Seek(0, SeekOrigin.Begin);
        fs.Read(header, 0, headerSize);

        // Determine the new chunk offset
        int newChunkOffset;
        fs.Seek(chunkIndex * 4, SeekOrigin.Begin);
        byte[] entry = reader.ReadBytes(4);
        int chunkOffset = (entry[0] << 8) | (entry[1]);


        if (chunkOffset <= 0)
        {
            // Create a new chunk position
            fs.Seek(0, SeekOrigin.End);
            newChunkOffset = (int)(fs.Length / 4096);
            if (fs.Length % 4096 != 0)
                newChunkOffset++;
        }
        else
        {
            // Use the existing chunk position
            newChunkOffset = chunkOffset;
        }

        // Calculate the number of 4 KiB sectors needed
        int chunkSectorCount = (compressedData.Length + 4095) / 4096;

        // Update the header with the new chunk offset and size
        int headerValue = (newChunkOffset << 8) | chunkSectorCount;
        header[chunkIndex * 4] = (byte)(headerValue >> 16);  // High byte
        header[chunkIndex * 4 + 1] = (byte)(headerValue >> 8); // Mid byte
        header[chunkIndex * 4 + 2] = (byte)(headerValue);      // Low byte
        header[chunkIndex * 4 + 3] = (byte)chunkSectorCount;   // Sector count

        // Write the updated header back to the file
        fs.Seek(0, SeekOrigin.Begin);
        fs.Write(header, 0, headerSize);

        // Write the compressed data to the determined chunk offset
        fs.Seek(newChunkOffset * 4096, SeekOrigin.Begin);
        writer.Write(compressedData);

    }

    public static void CloseSet()
    {
        ControllerImput.Instance.ReRead();
    }
    public void Sync(string sv, int Seed)
    {
        seed = Seed;
        savePath = sv;
        string path = Path.Combine(savePath, "playerprefab.dat");
        if (!File.Exists(path))
        {
            pos = Vector3.zero;
            rot = Quaternion.identity;
            WorldManager.currenttime = 300;
            CastleStructure.madeCastle = false;
        }
        else
        {
            try
            {
                byte[] compressedData = File.ReadAllBytes(path);

                using (MemoryStream memoryStream = new MemoryStream(compressedData))
                {
                    using (GZipStream decompressionStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                    {
                        using (MemoryStream resultStream = new MemoryStream())
                        {
                            decompressionStream.CopyTo(resultStream);
                            byte[] decompressedData = resultStream.ToArray();

                            // Convert back to JSON
                            string jsonData = System.Text.Encoding.UTF8.GetString(decompressedData);

                            PlayerData data = JsonUtility.FromJson<PlayerData>(jsonData);
                            pos = data.position;
                            rot = data.rotation;
                            MouseController.xrot = data.mx;
                            MouseController.yrot = data.my;
                            WorldManager.currenttime = data.currentTime;
                            CastleStructure.madeCastle = data.madeCastle;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                File.Delete(path);
                pos = Vector3.zero;
                rot = Quaternion.identity;
                WorldManager.currenttime = 300;
                CastleStructure.madeCastle = false;
            }


        }
        pret = true;
    }
    public static void SavePlayerData(Vector3 Pos, Quaternion Rot)
    {
        if (!File.Exists(savePath + "/playerprefab.dat"))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(savePath + "/playerprefab.dat"));
        }

        try
        {
            PlayerData data = new()
            {
                position = Pos,
                rotation = Rot,
                mx = MouseController.xrot,
                my = MouseController.yrot,
                currentTime = WorldManager.currenttime,
                madeCastle = CastleStructure.madeCastle
            };
            string jsonData = JsonUtility.ToJson(data, true);

            byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

            string filePath = Path.Combine(savePath, "playerprefab.dat");
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                using (GZipStream compressionStream = new GZipStream(fileStream, CompressionMode.Compress))
                {
                    compressionStream.Write(dataBytes, 0, dataBytes.Length);
                }
            }

        }
        catch (Exception e)
        {
            Debug.LogException(e);
            throw (e);
        }

    }
    public static Chunk.VoxelStruct[,,] ChunkReader(string savePath, int cx, int cz)
    {
        Chunk.VoxelStruct[,,] chunkData = FindChunkInRegion(savePath, cx, cz);

        if (chunkData == null)
        {
            return new Chunk.VoxelStruct[16, 160, 16];
        }

        Chunk.VoxelStruct[,,] voxelData = new Chunk.VoxelStruct[16, 160, 16];

        // 1D -> 3D
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 160; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    voxelData[x, y, z] = chunkData[x, y, z];
                }
            }
        }
        return voxelData;
    }

}

[System.Serializable]
public class PlayerData
{
    public Vector3 position;
    public Quaternion rotation;
    public float mx, my;
    public float currentTime;
    public bool madeCastle;
}