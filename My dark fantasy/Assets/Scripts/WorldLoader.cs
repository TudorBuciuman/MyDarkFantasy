using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class ChunkSerializer
{
    /*
     * Chunkurile sunt in total de la -2^28 la 2^28 in cele 4 directii
     * valoarea maxima pe care o poate atinge este 268.435.456 
     * alegerea a fost pentru eficienta
     * 
     * 
     */
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

