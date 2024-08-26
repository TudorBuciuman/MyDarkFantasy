using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System;
using System.IO.Compression;
using Unity.VisualScripting;
using NUnit.Framework;
public class ChunkSerializer
{
    public static string savePath;
    public static int seed;
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
            WorldManager.chunks[cx + 100, cz + 100].Voxels=loadedChunks[(cx, cz)];
        }
        else
        {
            // fiecare fisier contine 32x32 chunkuri
            int rx = cx / 32;
            if(cx<0 && cx % 32 != 0)
                rx--;
            int rz = cz / 32;
            if(cz<0 && cz % 32!=0)
                rz--;
            string fileName = savePath + $"/chunks/r.{rx}.{rz}.zlib";
            // Bazat pe pozitia chunkului pot citi din fisierul mare
            byte[,,] chunkData = ChunkReader(fileName, cx, cz);
            loadedChunks[(cx, cz)] = chunkData;

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
        int chunkIndex = regionChunkX + regionChunkZ * 32;

        // Move to the location of the chunk in the header (first 4 KiB)
        fs.Seek(chunkIndex * 4, SeekOrigin.Begin);
        int entry = reader.ReadInt32();

        // Extract the chunk offset and sector count
        int chunkOffset = (entry >> 8) & 0xFFFFFF; // Top 24 bits for offset (in 4 KiB sectors)
        int chunkSectorCount = entry & 0xFF; // Last 8 bits for sector count

        if (chunkOffset == 0 || chunkSectorCount == 0)
        {
            return null; // Chunk is not present in the region file
        }

        // Calculate the exact byte offset in the file
        long offsetPosition = (long)chunkOffset * 4096;

        if (offsetPosition >= fs.Length)
        {
            return null; // Offset is beyond the end of the file
        }

        // Move to the chunk's data position
        fs.Seek(offsetPosition, SeekOrigin.Begin);

        // Read the compressed chunk data
        byte[] compressedData = reader.ReadBytes(chunkSectorCount * 4096);

        // Decompress the data using Zlib (Deflate)
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
        // cx -= 100;
        // cz -= 100;
        int rx = cx / 32;
        int rz = cz / 32;
        if (cx < 0 && cx % 32 != 0)
            rx--;
        if (cz < 0 && cz % 32 != 0)
            rz--; string fileName =savePath+ $"/chunks/r.{rx}.{rz}.zlib";

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
        string fileName = Path.Combine(savePath, "chunks", $"r.{rx}.{rz}.zlib");
        if (!loadedChunks.TryGetValue((cx, cz), out byte[,,] chunkData))
        {
          //  Debug.Log("save uwu:)");
            // If the chunk is not loaded, initialize an empty chunk
            chunkData = new byte[16, 160, 16];
            loadedChunks[(cx, cz)] = chunkData;
        }

        // Serialize and save the chunk to the region file
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

        // Read the region header
        int headerSize = 8192; // 8 KiB for both chunk locations and timestamps
        byte[] header = new byte[headerSize];
        fs.Read(header, 0, headerSize);

        // Calculate chunk index within the region
        int regionChunkX = ((cx % 32) + 32) % 32;
        int regionChunkZ = ((cz % 32) + 32) % 32;
        int chunkIndex = regionChunkX + regionChunkZ * 32;

        // Calculate new chunk offset and sector count
        fs.Seek(0, SeekOrigin.End);
        int newChunkOffset = (int)(fs.Position / 4096);
        int chunkSectorCount = (compressedData.Length + 4095) / 4096;

        // Update chunk location in the header
        byte[] newChunkHeader = BitConverter.GetBytes((newChunkOffset << 8) | chunkSectorCount);
        Array.Copy(newChunkHeader, 0, header, chunkIndex * 4, 3);

        // Update the timestamp for the chunk
        int timestamp = (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        byte[] timestampBytes = BitConverter.GetBytes(timestamp);
        Array.Copy(timestampBytes, 0, header, 4096 + chunkIndex * 4, 4);

        // Write the updated header back to the file
        fs.Seek(0, SeekOrigin.Begin);
        fs.Write(header, 0, header.Length);

        // Write the compressed chunk data
        fs.Seek(newChunkOffset * 4096, SeekOrigin.Begin);
        writer.Write(compressedData);

        // Pad the last sector if necessary
        if (compressedData.Length % 4096 != 0)
        {
            writer.Write(new byte[4096 - (compressedData.Length % 4096)]);
        }
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
        }
        else
        {
            string json = File.ReadAllText(savePath + "/playerprefab.json");
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);

            pos= data.position;
            rot = data.rotation;
            MouseController.xrot = data.mx;
            MouseController.yrot = data.my;
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
        };
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath + "/playerprefab.json", json);

    }
    public static void SaveWorld()
    {
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);
        _ = new
        BinaryFormatter();
        _ = new FileStream(savePath + "world.world", FileMode.Create);

    }
    public static byte[,,] ChunkReader(string savePath, int cx, int cz)
    {
        // Calculate region coordinates
        int rx = cx / 32;
        int rz = cz / 32;
        if (cx < 0 && cx % 32 != 0)
            rx--;
        if( cz < 0 && cz % 32 != 0) 
            rz--;
        // Read the chunk data from the region file
        byte[] chunkData = FindChunkInRegion(savePath, cx, cz);

        if (chunkData == null)
        {
            // If no data is found, return a new empty chunk
            //Aici trebuie lucrat
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
}