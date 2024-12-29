using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
//using Unity.Plastic.Newtonsoft.Json;

public class ItemsFunctions : MonoBehaviour
{
    public static WorldManager wman;
    public static itemsManager item;
    public static Image img;
    public static Image mapImg1,mapImg2;
    public static Texture2D mapTexture;
    public static void ItemsStart(WorldManager Wman,itemsManager Item,Image Img,Image mapImg,Image maptexture)
    { 
        wman=Wman;
        item=Item;
        img = Img;
        mapImg2=maptexture;
        mapImg1 = mapImg;

    }
    public static void CutDownTree(Vector3 pos,byte type)
    {
        int x = Mathf.RoundToInt(pos.x), z = Mathf.RoundToInt(pos.z), y = Mathf.RoundToInt(pos.y);
        byte size = 1;
        for(int i = y+1; i<150 && wman.Block(x,i,z)==type; i++)
        {
            WorldManager.SetTo(x,i,z,0);
            size++;
        }
        item.SetItem(wman.Block(x,y,z), size, new Vector3(pos.x, Mathf.RoundToInt(pos.y) - 0.3f, pos.z));
        wman.ModifyMesh(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z), new Chunk.VoxelStruct(0, 0));
    }
    public static void MakeMap()
    {
        mapTexture = new Texture2D(128,128);
        mapTexture.filterMode = FilterMode.Point;
        mapTexture.wrapMode = TextureWrapMode.Clamp;
        GenerateMap();
        Sprite mapSprite = Sprite.Create(mapTexture, new Rect(0, 0, mapTexture.width, mapTexture.height), new Vector2(0.5f, 0.5f));
        mapImg2.sprite = mapSprite;
        mapImg1.gameObject.SetActive(true);
        Toolbar.mapActive = true;

    }
    public static void GenerateMap()
    {
        for (int x = 0; x < 128; x++)
        {
            for (int y = 0; y < 128; y++)
            {
                Color pixelColor = (Random.value > 0.8f) ? Color.blue : Color.green;
                mapTexture.SetPixel(x, y, pixelColor);
            }
        }
        mapTexture.Apply(); 
    }

    //I dont even know how to make this ;(
    public static void OpenChest(string chestFileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, chestFileName);

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            ChestData chest = JsonUtility.FromJson<ChestData>(json);

            foreach (var item in chest.items)
            {
                Debug.Log($"Item: {item.id}, Count: {item.count}");
            }
        }
    }
}
[System.Serializable]
public class ChestData
{
    public string chest_id;
    public ChestItem[] items;
}

[System.Serializable]
public class ChestItem
{
    public string id;
    public int count;
}
