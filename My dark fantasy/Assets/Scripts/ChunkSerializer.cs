using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System;
using System.IO.Compression;
using Unity.VisualScripting;
public class ChunkSerializer
{
    public static string savePath;
    public static int seed=-1;
    public static bool pret = false;
    public static Vector3 pos; 
    public static Quaternion rot;
    public static float mx=0,my=0;
    public static string[] loadpath;
    public static Dictionary<(int, int), byte[,,]> loadedChunks = new();
    public static void LoadChunk(int cx, int cz)
    {

        if (loadedChunks.ContainsKey((cx, cz)))
        {
            WorldManager.chunks[cx + 100, cz + 100].Voxels = loadedChunks[(cx, cz)];
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
            byte[,,] chunkData = ChunkReader(fileName, cx, cz);
            lock (loadedChunks) { 
            loadedChunks[(cx, cz)] = chunkData;
            }
            WorldManager.chunks[cx+100,cz+100].Voxels=chunkData;
        }
    }
    public static byte[] FindChunkInRegion(string fileName, int cx, int cz)
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
        using (MemoryStream memoryStream = new MemoryStream(compressedData))
        using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress))
        using (MemoryStream resultStream = new MemoryStream())
        {
            deflateStream.CopyTo(resultStream);
            return resultStream.ToArray();
        }
    }
    public static bool IsReal(int cx, int cz)
    {
        if (loadedChunks.TryGetValue((cx, cz), out byte[,,] value) && value != null)
        {
            return true;
        }
        int rx = cx / 32;
        int rz = cz / 32;
        if (cx < 0 && cx % 32 != 0)
            rx--;
        if (cz < 0 && cz % 32 != 0)
            rz--; 
        string fileName =savePath+ $"/chunks/r.{rx}.{rz}.zlib";
        byte[] data =  FindChunkInRegion(fileName,cx,cz);
        if (data == null)
        {
            return false;
        }
        return true;
        
    }
    public static void SaveChunk(int cx, int cz)
    {
        // Calculate region coordinates (region files are 32x32 chunks)
        int rx = cx / 32;
        int rz = cz / 32;
        if (cx < 0 && cx % 32 != 0)
            rx--;
        if(cz < 0 && cz % 32 != 0)
            rz--;
        string fileName = savePath + $"/chunks/r.{rx}.{rz}.zlib";
        if (!loadedChunks.TryGetValue((cx, cz), out byte[,,] chunkData))
        {
            return;
        }
        else
        SerializeChunkToRegion(fileName, cx, cz, chunkData);
    }
    public static void SerializeChunkToRegion(string fileName, int cx, int cz, byte[,,] data)
    {
        // Ensure directory exists
        if (!File.Exists(fileName))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
        }
        byte[] chunkData = new byte[16 * 16 * 160];
        int index = 0;

        // Flatten the 3D chunk data array into a 1D byte array
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 160; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    chunkData[index++] = data[x, y, z];
                }
            }
        }

        // Compress the chunk data using Zlib (Deflate)
        byte[] compressedData;
        using (var memoryStream = new MemoryStream())
        {
            using (var deflateStream = new DeflateStream(memoryStream, System.IO.Compression.CompressionLevel.Optimal, true))
            {
                deflateStream.Write(chunkData, 0, chunkData.Length);
            }
            compressedData = memoryStream.ToArray();
        }

        // Open or create the region file
        using FileStream fs = new(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        BinaryWriter writer = new(fs);
        BinaryReader reader = new(fs);

        // Calculate chunk index within the region
        int regionChunkX = ((cx % 32) + 32) % 32;
        int regionChunkZ = ((cz % 32) + 32) % 32;
        int chunkIndex = regionChunkX + (regionChunkZ * 32);

        int headerSize = 4096; // 4 KiB header with 3 bytes for location and 1 byte for size
        byte[] header = new byte[headerSize];

        // Read the header at the beginning of the file
        fs.Seek(0, SeekOrigin.Begin);
        fs.Read(header, 0, headerSize);

        // Find the current offset for the new chunk
        fs.Seek(0, SeekOrigin.End);
        int newChunkOffset = (int)(fs.Length / 4096); // Sector-aligned offset
        if (fs.Length % 4096 != 0) newChunkOffset++;  // Round up if necessary

        // Calculate the number of sectors required for the chunk
        int chunkSectorCount = (compressedData.Length + 4095) / 4096; // Number of 4KiB sectors needed

        // Update chunk location in the header (3 bytes for offset, 1 byte for sector count)
        int headerValue = (newChunkOffset << 8) | chunkSectorCount;
        header[chunkIndex * 4] = (byte)(headerValue >> 16);  // Offset upper byte
        header[chunkIndex * 4 + 1] = (byte)(headerValue >> 8);  // Offset middle byte
        header[chunkIndex * 4 + 2] = (byte)(headerValue);  // Offset lower byte
        header[chunkIndex * 4 + 3] = (byte)chunkSectorCount;  // Sector count

        // Write the updated header back to the beginning of the file
        fs.Seek(0, SeekOrigin.Begin);
        fs.Write(header, 0, headerSize);

        // Seek to the new chunk's offset and write the compressed data
        fs.Seek(newChunkOffset * 4096, SeekOrigin.Begin);
        writer.Write(compressedData);
    }
    public static void CloseSet()
    {
        pret = false;
        string json = File.ReadAllText(savePath + "/playerprefab.json");
        PlayerData data = JsonUtility.FromJson<PlayerData>(json);
        pos = data.position;
        rot = data.rotation;
        MouseController.xrot = data.mx;
        MouseController.yrot = data.my;
        pret = true;
    }
    public void Sync(string sv,int Seed)
    {
        seed = Seed;
        savePath = sv;
        if (!File.Exists(savePath+"/playerprefab.json"))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(savePath + "/playerprefab.json"));
            pos =Vector3.zero;
            rot =Quaternion.identity;
            WorldManager.currenttime = 300;
        }
        else
        {
            string json = File.ReadAllText(savePath + "/playerprefab.json");
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);

            pos= data.position;
            rot = data.rotation;
            MouseController.xrot = data.mx;
            MouseController.yrot = data.my;
            WorldManager.currenttime=data.currentTime;
            WorldManager w = new();
            w.PreCalculateTime();
        }
        pret = true;
    }
    public static void savePlayerData(Vector3 Pos,Quaternion Rot)
    {
        if (!File.Exists(savePath + "/playerprefab.json"))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(savePath + "/playerprefab.json"));
        }
        PlayerData data = new PlayerData
        {
            position = Pos,
            rotation = Rot,
            mx = MouseController.xrot,
            my = MouseController.yrot,
            currentTime = WorldManager.currenttime
        };
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath + "/playerprefab.json", json);

    }
    public static byte[,,] ChunkReader(string savePath, int cx, int cz)
    {
        // Read the chunk data from the region file
        byte[] chunkData = FindChunkInRegion(savePath, cx, cz);

        if (chunkData == null)
        {
            return new byte[16, 160, 16];
        }

        byte[,,] voxelData = new byte[16, 160, 16];
        int index = 0;

        // Populate the voxelData array with the chunk data
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 160; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    voxelData[x, y, z] = chunkData[index++];
                }
            }
        }
        WorldManager.chunks[cx + 100, cz + 100].Voxels = voxelData;
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
}