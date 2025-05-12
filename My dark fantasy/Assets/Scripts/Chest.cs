using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    public static Chest Instance;
    public static bool eKeyReleasedAfterChest = true;
    public GameObject chestManager;
    public Image chestIMG;
    public ItemSlot[] chestItem = new ItemSlot[27];
    public ItemSlot[] invItem = new ItemSlot[36];
    private Dictionary<Pos, ChestData> chestsData = new Dictionary<Pos, ChestData>();
    private string saveFilePath;
    public itemmm[,] chestitems = new itemmm[3, 9];
    public ChestData currentchest;
    public class itemmm
    {
        public byte id;
        public byte size;
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
            saveFilePath = Path.Combine(ChunkSerializer.savePath, "chests.dat");
            LoadAllChests();
    }
    public void OpenChest(int x, int y, int z)
    {
        eKeyReleasedAfterChest = false;
        Toolbar.instance.openedInv = true;
        BookManager.readingBook = true;
        chestIMG.gameObject.SetActive(true);
        ChestData data = LoadChest(new Pos(x, y, z));
        chestitems = new itemmm[3, 9];

        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 9; j++)
            {
                chestitems[i, j] = new()
                {
                    id = 0,
                    size = 0
                };
            }

        if (data != null)
        {
            currentchest = data;
            currentchest.position = new Pos(x, y, z);
            try
            {
                foreach (ItemSlott chdata in data.items)
                {
                    chestitems[chdata.slotIndex / 9, chdata.slotIndex % 9] = new itemmm
                    {
                        id = chdata.itemId,
                        size = chdata.quantity
                    };
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        else
        {
            currentchest = new()
            {
                position = new Pos(x, y, z),
                items = new ItemSlott[27]
            };
            chestitems = new itemmm[3, 9];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 9; j++)
                {
                    chestitems[i, j] = new()
                    {
                        id = 0,
                        size = 0
                    };
                }
        }

        ShowItems();

    }
    public void ShowItems()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Toolbar t = Toolbar.instance;
                if (t.itemsize[i, j] != 0)
                {
                    invItem[i * 9 + j].image.sprite = t.World.blockTypes[t.item[i, j]].itemSprite;
                    if (t.World.blockTypes[t.item[i, j]].Items.isblock)
                    {
                        invItem[i * 9 + j].num.text = t.itemsize[i, j].ToString();
                    }
                    else
                        invItem[i * 9 + j].num.text = null;
                }
                else if (t.itemsize[i, j] == 0 || t.item[i, j] == 0)
                {
                    invItem[i * 9 + j].image.sprite = t.World.blockTypes[0].itemSprite;
                    invItem[i * 9 + j].num.text = null;
                }
            }
        }
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Toolbar t = Toolbar.instance;

                if (chestitems[i,j].size!=0)
                {
                    chestItem[i * 9 + j].image.sprite = t.World.blockTypes[chestitems[i, j].id].itemSprite;
                    if (t.World.blockTypes[chestitems[i, j].id].Items.isblock)
                    {
                        chestItem[i * 9 + j].num.text = chestitems[i, j].size.ToString();
                    }
                    else
                        chestItem[i * 9 + j].num.text = null;
                }
                else if (chestitems[i, j].size == 0 || chestitems[i, j].id == 0)
                {
                    chestItem[i * 9 + j].image.sprite = t.World.blockTypes[0].itemSprite;
                    chestItem[i * 9 + j].num.text = null;
                }
            }
        }
        chestManager.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ChestInput.instance.Opening();
    }
    public IEnumerator WaitforChange()
    {
        while(Input.GetKey(KeyCode.E))
        {
            yield return null;
        }
        eKeyReleasedAfterChest = true;
        yield return null;
    }
    public void CallWaitForChange()
    {
        StartCoroutine(WaitforChange());
    }
    public void SaveChest(ChestData chestData)
    {
        chestData.items = new ItemSlott[27];
        for(byte i=0; i<27; i++)
        {
            chestData.items[i] = new ItemSlott();
            chestData.items[i].slotIndex = i;
            chestData.items[i].itemId = chestitems[i / 9, i % 9].id;
            chestData.items[i].quantity = chestitems[i / 9, i % 9].size;

        }
        if (chestsData.ContainsKey(chestData.position))
        {
            chestsData[chestData.position] = chestData;
        }
        else
        {
            chestsData.Add(chestData.position, chestData);
        }

        SaveAllChests();
    }
    public void CloseChest()
    {
        //SaveChest();
        //save all chests
        //save inv
        SaveChest(currentchest);
        chestIMG.gameObject.SetActive(false);
        chestManager.SetActive(false);
        BookManager.readingBook = false;
        Toolbar.instance.openedInv = false;
        Toolbar.instance.ReCalculate();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine(WaitforChange());
    }
    public ChestData LoadChest(Pos loc)
    {
        chestsData.TryGetValue(loc, out ChestData data);

        return data;
    }
    public void RemoveChest(Pos pos)
    {
        if (chestsData.ContainsKey(pos))
        {
            chestsData.Remove(pos);
            SaveAllChests();
        }
    }
    private void SaveAllChests()
    {
        //DONT TOUCH THIS
        //I WARN YOU
        try
        {
            List<ChestData> allChests = new List<ChestData>(chestsData.Values);

            // Serialize to JSON
            string jsonData = JsonUtility.ToJson(new ChestDataWrapper(allChests), true);

            // Convert to bytes
            byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

            // Compress and save
            using (FileStream fileStream = new FileStream(saveFilePath, FileMode.Create))
            {
                using (GZipStream compressionStream = new GZipStream(fileStream, CompressionMode.Compress))
                {
                    compressionStream.Write(dataBytes, 0, dataBytes.Length);
                }
            }


        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving chests: {e.Message}");
        }
    }
    private void LoadAllChests()
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                // Read compressed file
                byte[] compressedData = File.ReadAllBytes(saveFilePath);

                // Decompress
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

                            // Deserialize
                            ChestDataWrapper wrapper = JsonUtility.FromJson<ChestDataWrapper>(jsonData);

                            // Rebuild dictionary
                            chestsData.Clear();
                            foreach (var chest in wrapper.chests)
                            {
                                chestsData.Add(chest.position,chest);
                            }

                        }
                    }
                }

            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading chests: {e.Message}");
            }
        }
        else
        {
            Debug.Log("No chest save file found. Starting fresh.");
        }
    }

    [System.Serializable]
    public class ChestDataWrapper
    {
        public List<ChestData> chests;

        public ChestDataWrapper(List<ChestData> chests)
        {
            this.chests = chests;
        }
    }

}
[System.Serializable]
public class ChestData
{
    public Pos position = new();
    public ItemSlott[] items = new ItemSlott[27];

}

[System.Serializable]
public class ItemSlott
{
    public byte itemId;
    public byte quantity;
    public byte slotIndex;
}
[System.Serializable]
public class Pos
{
    public int x, y, z;
    public Pos() { }
    public Pos(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public override bool Equals(object obj)
    {
        if (obj is Pos other)
            return x == other.x && y == other.y && z == other.z;
        return false;
    }

    public override int GetHashCode()
    {
        unchecked 
        {
            int hash = 17;
            hash = hash * 31 + x;
            hash = hash * 31 + y;
            hash = hash * 31 + z;
            return hash;
        }
    }
}