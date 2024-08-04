using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
public class ChunkSerializer
{
    public static string savePath;

    public static string[] loadpath;

    public void Sync(string sv, string[] lp)
    {
        savePath = sv;
        loadpath = lp;
    }
    public static void SaveWorld()
    {
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        //Debug.Log("Saving " + world.worldName);

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(savePath + "world.world", FileMode.Create);

      //  formatter.Serialize(stream, world);
       // stream.Close();

      //  Thread thread = new Thread(() => SaveChunks(world));
       // thread.Start();

    }
    public static void SerializeChunk(string filePath, BidimensionalArray chunkData)
    {
        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            // Example: Serialize chunk data
            for (int x = -15; x <= 0; x++)
            {
                for (int z = -15; z <= 0; z++)
                {
                    bool value = chunkData[x, z];
                    writer.Write(value);
                }
            }
        }
    }
}
public class ChunkDeserializer
{

    public static BidimensionalArray DeserializeChunk(string filePath)
    {
        BidimensionalArray chunkData = new BidimensionalArray();

        using (FileStream stream = new FileStream(filePath, FileMode.Open))
        using (BinaryReader reader = new BinaryReader(stream))
        {
            for (int x = -15; x <= 0; x++)
            {
                for (int z = -15; z <= 0; z++)
                {
                    bool value = reader.ReadBoolean();
                    chunkData[x, z] = value;
                }
            }
        }

        return chunkData;
    }
    
}

