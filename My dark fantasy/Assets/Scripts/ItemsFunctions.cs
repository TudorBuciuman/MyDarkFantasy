using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

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
    public static void CutDownTree(Vector3 pos, byte type)
    {
        int x = Mathf.RoundToInt(pos.x), z = Mathf.RoundToInt(pos.z), y = Mathf.RoundToInt(pos.y);
        byte size = 1;

        for (int a = -2; a <= 2; a++)
        {
            for (int b = -2; b <= 2; b++)
            {
                for (int i = y + 1; i < y+9 && i<150; i++)
                {
                    if (wman.Block(x + a, i, z+b) == 7)
                    {
                        WorldManager.SetTo(x + a, i, z + b, 0);
                        size++;
                    }
                }
            }
        }
        item.SetItem(wman.Block(x, y, z), size, new Vector3(pos.x, Mathf.RoundToInt(pos.y) - 0.3f, pos.z));
        wman.ModifyMesh(x, y, z, new Chunk.VoxelStruct(0, 0));

        img.StartCoroutine(RemoveLeaves(new Vector3(x, y, z)));
    }

    public static IEnumerator RemoveLeaves(Vector3 pos)
    {
        int x = Mathf.RoundToInt(pos.x), z = Mathf.RoundToInt(pos.z), y = Mathf.RoundToInt(pos.y);

        List<Vector3> leavesToRemove = new List<Vector3>();

        for (int dx = -2; dx <= 2; dx++)
        {
            for (int dz = -2; dz <= 2; dz++)
            {
                for (int dy = 0; dy <= 8; dy++) 
                {
                    if (wman.Block(x + dx, y + dy, z + dz) == 11) 
                    {
                        leavesToRemove.Add(new Vector3(x + dx, y + dy, z + dz));
                    }
                }
            }
        }

        foreach (Vector3 leafPos in leavesToRemove)
        {
            WorldManager.SetTo(Mathf.RoundToInt(leafPos.x), Mathf.RoundToInt(leafPos.y), Mathf.RoundToInt(leafPos.z), 0);
            wman.ModifyMesh(Mathf.RoundToInt(leafPos.x), Mathf.RoundToInt(leafPos.y), Mathf.RoundToInt(leafPos.z), new Chunk.VoxelStruct(0, 0));
            yield return new WaitForSeconds(0.1f); 
        }
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

    public static void RealKnife()
    {
        Voxeldata.PlayerData.sleep = 1;
        if (Voxeldata.PlayerData.deaths != 0 && Voxeldata.PlayerData.sleep != 0 && Voxeldata.PlayerData.scene == 0)
        {
<<<<<<< Updated upstream
            //activate protocol
            SoundsManager.instance.mouseSource.Stop();
        }
        HealthSistem.istance.UpdateHealth(-1);
        SoundsManager.instance.Placement(11);
        if (!SoundsManager.instance.songs.isPlaying)
        {
            SoundsManager.instance.PlaySong(7);
        }
        else if (SoundsManager.instance.songs.clip.name != "song7")
        {
            SoundsManager.instance.PlaySong(7);
=======
            if (HealthSistem.health <= 1)
            {
                //activate protocol
                HealthSistem.istance.Protocol();
                SoundsManager.instance.mouseSource.Stop();
                ControllerImput.Instance.toolbar.UpdateAnItem(Toolbar.slothIndex);
                return;
            }
            else
                HealthSistem.istance.UpdateHealth(-1);
            SoundsManager.instance.Placement(11);
            if (!SoundsManager.instance.songs.isPlaying)
            {
                SoundsManager.instance.ForceSong(7);
            }

            if (SoundsManager.lastSong != "song7")
            {
                SoundsManager.instance.ForceSong(7);
            }
>>>>>>> Stashed changes
        }
    }
}

