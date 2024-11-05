using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.IO;
using System;
public class Toolbar : MonoBehaviour
{
    
    public WorldManager World;
    public NewControls inputActions;
    public ControllerImput control;
    public Crafting craft;
    public RectTransform highlight;
    public AllSlots[] itemSlots = new AllSlots[9];
    public ItemSlot[] invimg = new ItemSlot[38];
    public PlayerData[] invData=new PlayerData[36];
    public Image crosshair;
    public Image inventory;
    public Image UIEscape;
    public Image select;
    public Button playSong;
    public Button openInv;
    public Button closeInv;

    public byte[,] item = new byte[4, 9];
    public byte[,] itemsize = new byte[4, 9];
    public byte selectedsloth;
    //slothIndex este intre 0->9 iar selectedsloth e pentru cand deschizi inv
    public static byte slothIndex = 0;
    float time = 0f;
    public bool openedInv = false;
    public static bool escape = false;

    public PointerEventData faranume;
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;
    void Start()
    {
        inputActions = new NewControls();
        inputActions.Android.Enable();
        for(int j=0; j<4; j++) {
            for (int i = 0; i < 9; i++)
            {
                item[j, i] = 0;
                itemsize[j, i] = 0;
            }
        }
        ReadForInventory();
        for (int i=0; i<9; i++)
        {
            if (item[0,i] >0)
            {
                itemSlots[i].image.sprite = World.blockTypes[item[0, i]].itemSprite;
                if (World.blockTypes[item[0, i]].Items.isblock)
                {
                    itemSlots[i].num.text = itemsize[0, i].ToString();
                    
                }
                else
                {
                    itemSlots[i].num.text = null;
                }
            }
        }
    }
    void Update()
    {
        if (!openedInv)
        {
            float Scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Scroll != 0)
            {
                ControllerImput.a = 2300;
                if (Scroll > 0)
                {
                    if (slothIndex == 0)
                        slothIndex = 8;
                    else
                    slothIndex--;
                }
                else
                    slothIndex++;
                if (slothIndex == 9)
                    slothIndex = 0;
                highlight.position = itemSlots[slothIndex].image.transform.position;
            }

        }
        else 
        {
            if(Input.GetMouseButtonDown(0))
            SearchForImput();
        }
        if (!escape)
        {
            if (inputActions.Android.OpenInv.triggered)
            {
                if (openedInv)
                {
                    CloseInventory();
                }
                else
                {
                    OpenInventory(0);

                }
            }
        }
            if (Input.GetKey(KeyCode.Escape) && time <= 0)
            {
                time = 0.3f;
                if (!escape)
                    Escape();
                else
                    Again();
            }
            if (time > 0)
                time -= Time.deltaTime;
        
    }
    public void SearchForImput()
    {
        faranume = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new();
        raycaster.Raycast(faranume,results);
        foreach (var slot in results)
        {
            if (slot.gameObject.CompareTag("inv"))
            {
                Image img = slot.gameObject.GetComponent<Image>();
                byte f = selectedsloth;
                for (byte i = 0; i <= 37; i++)
                {
                    if (invimg[i].image == img)
                    {
                        selectedsloth = i;
                        break;
                    }
                }
                
                if (selectedsloth<36 && f!=36)
                {
                    if (f != 50 && f!=37)
                    {
                        if (selectedsloth == f)
                        {
                            selectedsloth = 50;
                            select.gameObject.SetActive(false);
                        }
                        else if (item[selectedsloth / 9, selectedsloth % 9] == 0)
                        {
                            item[selectedsloth / 9, selectedsloth % 9] = item[f / 9, f % 9];
                            itemsize[selectedsloth / 9, selectedsloth % 9] = itemsize[f / 9, f % 9];
                            item[f / 9, f % 9] = 0;
                            itemsize[f / 9, f % 9] = 0;

                            select.gameObject.SetActive(false);
                            invimg[selectedsloth].image.sprite = invimg[f].image.sprite;
                            invimg[f].image.sprite = World.blockTypes[0].itemSprite;
                            invimg[selectedsloth].num.text = invimg[f].num.text;
                            invimg[f].num.text = null;
                            selectedsloth = 50;
                        }
                        else if (item[selectedsloth / 9, selectedsloth % 9] == item[f / 9, f % 9])
                        {
                            if (itemsize[selectedsloth / 9, selectedsloth % 9] + itemsize[f / 9, f % 9] <= 96)
                            {
                                invimg[f].image.sprite = World.blockTypes[0].itemSprite;
                                itemsize[selectedsloth / 9, selectedsloth % 9] += itemsize[f / 9, f % 9];
                                itemsize[f / 9, f % 9] = 0;
                                item[f / 9, f % 9] = 0;
                                invimg[selectedsloth].num.text = itemsize[selectedsloth / 9, selectedsloth % 9].ToString();
                                invimg[f].num.text = null;
                            }
                            else
                            {
                                int a = (itemsize[selectedsloth / 9, selectedsloth % 9] + itemsize[f / 9, f % 9]) - 96;
                                itemsize[selectedsloth / 9, selectedsloth % 9] = 96;
                                itemsize[f / 9, f % 9] = (byte)a;
                                invimg[selectedsloth].num.text = itemsize[selectedsloth / 9, selectedsloth % 9].ToString();
                                invimg[f].num.text = itemsize[f / 9, f % 9].ToString();
                            }
                        }
                        else if (item[selectedsloth / 9, selectedsloth % 9] != item[f / 9, f % 9])
                        {
                            byte a = itemsize[f / 9, f % 9], b = item[f / 9, f % 9];
                            itemsize[f / 9, f % 9] = itemsize[selectedsloth / 9, selectedsloth % 9];
                            itemsize[selectedsloth / 9, selectedsloth % 9] = a;
                            item[f / 9, f % 9] = item[selectedsloth / 9, selectedsloth % 9];
                            item[selectedsloth / 9, selectedsloth % 9] = b;
                            invimg[selectedsloth].num.text = itemsize[selectedsloth / 9, selectedsloth % 9].ToString();
                            invimg[f].num.text = itemsize[f / 9, f % 9].ToString();
                            invimg[selectedsloth].image.sprite = World.blockTypes[item[selectedsloth / 9, selectedsloth % 9]].itemSprite;
                            invimg[f].image.sprite = World.blockTypes[item[f / 9, f % 9]].itemSprite;
                        }
                        select.gameObject.SetActive(false);
                        selectedsloth = 50;
                    }
                    else if (f == 37 && selectedsloth<36)
                    {
                        select.transform.position = img.transform.position;
                        select.gameObject.SetActive(true);
                    }
                    else
                    {
                        if (itemsize[selectedsloth / 9, selectedsloth % 9] != 0)
                        {
                            select.transform.position = img.transform.position;
                            select.gameObject.SetActive(true);
                        }
                    }
            }
                else if (selectedsloth == 36)
                {
                    if (craft.sizeofitem > 0)
                    {
                        select.transform.position = img.transform.position;
                        select.gameObject.SetActive(true);
                    }
                    else
                        selectedsloth = 50;
                }
                else if(selectedsloth==37)
                {
                    craft.Craft();
                    select.gameObject.SetActive(false);
                }
                else if (f == 36 && selectedsloth<36)
                {
                    if(selectedsloth != 50 && item[selectedsloth / 9, selectedsloth % 9] == 0)
                    {
                        if (craft.sizeofitem > 96) {
                            item[selectedsloth / 9, selectedsloth % 9] = craft.itemcraft;
                            craft.sizeofitem -= 96;
                            itemsize[selectedsloth / 9, selectedsloth % 9] = 96;
                            invimg[selectedsloth].image.sprite = invimg[36].image.sprite;
                            invimg[36].num.text = craft.sizeofitem.ToString();
                            invimg[selectedsloth].num.text = 96.ToString();
                        }
                        else
                        {
                            item[selectedsloth / 9, selectedsloth % 9] = craft.itemcraft;
                            itemsize[selectedsloth / 9, selectedsloth % 9] = craft.sizeofitem;
                            invimg[selectedsloth].image.sprite = invimg[36].image.sprite;
                            invimg[36].num.text = null;
                            invimg[selectedsloth].num.text = craft.sizeofitem.ToString();
                            craft.itemcreated = 0;
                            craft.sizeofitem = 0;
                        }

                       
                        select.gameObject.SetActive(false);
                        selectedsloth = 50;
                    }
                    else if(item[selectedsloth/9, selectedsloth%9] == craft.itemcraft)
                    {
                        byte a= (byte)(itemsize[selectedsloth / 9, selectedsloth % 9]+craft.sizeofitem);
                        if (a <= 96)
                        {
                            itemsize[selectedsloth / 9, selectedsloth % 9] = a;
                            invimg[36].num.text = null;
                            invimg[selectedsloth].num.text=a.ToString();
                            craft.sizeofitem=0;
                        }
                        else
                        {
                            itemsize[selectedsloth / 9, selectedsloth % 9] = 96;
                            craft.sizeofitem = (byte)(a - 96);
                            invimg[selectedsloth].num.text = 96.ToString();
                            invimg[36].num.text = craft.sizeofitem.ToString();
                        }

                        select.gameObject.SetActive(false);
                        selectedsloth = 50;
                    }
                    else
                    {
                        selectedsloth = 50;
                        select.gameObject.SetActive(false);
                    }
                }

                
                break;
            }
            else if (slot.gameObject.CompareTag("crf"))
            {
                Image img = slot.gameObject.GetComponent<Image>();
                for (byte i=0; i<craft.options; i++)
                {
                    if (World.blockTypes[craft.recipes[craft.ind[i]].id].itemSprite.name == img.sprite.name)
                    {
                        if (i != lastitem)
                        {
                            if (craft.sizeofitem == 0)
                            {
                                GameObject[] gO = GameObject.FindGameObjectsWithTag("fin");
                                foreach (GameObject go2 in gO)
                                {
                                    Destroy(go2);
                                }
                                lastitem = i;
                                i=craft.ind[i];
                                craft.itemcreated = i; craft.sizeofitem = 0;
                                craft.itemcraft = craft.recipes[i].id;
                                select.transform.position = img.transform.position;
                                select.gameObject.SetActive(true);
                                TryCrafting(i);
                                break;
                            }
                        }
                        else
                        {
                            
                            select.gameObject.SetActive(false);
                            lastitem = 255;
                            craft.itemcreated = 255;
                            if (craft.sizeofitem == 0)
                            {
                                invimg[36].image.sprite = World.blockTypes[0].itemSprite;
                                if(selectedsloth==36)
                                    selectedsloth = 50;
                            }
                        }
                    }
                }
            }
        }
    }
    public byte lastitem=255;
    public void Changesloth(byte a)
    {
        slothIndex = a;
        highlight.position = itemSlots[slothIndex].image.transform.position;
    }
    public void TryCrafting(byte id)
    {
        craft.CraftScreen(id);
    }
    public void OpenInventoryAnd()
    {
        OpenInventory(0);
        closeInv.gameObject.SetActive(true);
    }
    public void CloseInvAnd()
    {
        closeInv.gameObject.SetActive(false);
        CloseInventory();
    }
    public void ReadForInventory()
    {
        while (ChunkSerializer.savePath.Length == 0)
            ; 
        string filePath = Path.Combine(ChunkSerializer.savePath, "playerData.json");
        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath);
            PlayerDataOpener playerData = JsonUtility.FromJson<PlayerDataOpener>(jsonString);
            slothIndex =playerData.slothid;
            foreach (ItemSave data in playerData.inventory)
            {
                item[data.sloth/9, data.sloth%9]=data.itemID;
                itemsize[data.sloth/9,data.sloth%9]=data.quantity;
            }
        }
        else
        {
            PlayerDataOpener playerData = new ()
            {
                playerName = "Steve",
                health = 20,
                slothid = 0,
                experienceLevel = 0,
                inventory = new ItemSave[3]
            {
                new(){ itemName = "Wood Sword", itemID = 19, quantity = 1,sloth=0 ,
                   properties = new ItemProperties { damage = 5, speed =0.7f }},
                new() { itemName = "Wood Pickaxe", itemID = 13, quantity = 1,sloth=1 ,
                   properties = new ItemProperties { special=0, speed =0.7f } },
                new() { itemName = "Wood Axe", itemID = 20, quantity = 1,sloth=2 ,
                   properties = new ItemProperties { special=0,speed =0.7f } }
            }
            };
            string jsonString = JsonUtility.ToJson(playerData, true);
            File.WriteAllText(filePath, jsonString);
            foreach(ItemSave data in playerData.inventory)
            {
                item[data.sloth/9, data.sloth%9]=data.itemID;
                itemsize[data.sloth/9,data.sloth%9]=data.quantity;
            }
        }
        highlight.position = itemSlots[slothIndex].image.transform.position;

    }
    public void SaveInventory()
    {
        PlayerDataOpener playerData = new ()
        {
            playerName = "Steve",
            health = 20,
            slothid =slothIndex,
            experienceLevel = 0,
            inventory = new ItemSave[36]
        };
        byte r = 0;
        for(int i=0; i<4; i++)
        {
            for(int j=0; j<9; j++)
            {
                if (item[i, j] > 0)
                {
                    playerData.inventory[r] = new ItemSave { itemID = item[i, j], quantity = itemsize[i, j], sloth=(byte)(i*9+j) };
                    r++;
                }
            }
        }
        if (r < playerData.inventory.Length)
        {
            System.Array.Resize(ref playerData.inventory, r);
        }
        string jsonString = JsonUtility.ToJson(playerData, true);
        string filePath = Path.Combine(ChunkSerializer.savePath, "playerData.json");
        File.WriteAllText(filePath, jsonString);
    }
    public void Escape()
    {
        Application.targetFrameRate = 10;
        openedInv = true;
        escape = true;
        UIEscape.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void Again()
    {
        Application.targetFrameRate = 60;
        
        control.Posi=Vector3.zero;
        UIEscape.gameObject.SetActive(false);
        openedInv = false;
        escape = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        CloseInventory();
    }
    public void OpenInventory(byte tpe)
    {
        openedInv = true;
        inventory.gameObject.SetActive(true);
        crosshair.gameObject.SetActive(false);
        invimg[37].image.gameObject.SetActive(true);
        invimg[36].image.gameObject.SetActive(true);
        invimg[36].image.sprite = World.blockTypes[0].itemSprite;
        selectedsloth = 50;
        
        for(int i=0; i<4; i++)
        {
            for(int j=0; j<9; j++)
            {
                if (itemsize[i,j] != 0)
                {
                    invimg[(i * 9) + j].image.gameObject.SetActive(true);
                    invimg[i*9+j].image.sprite = World.blockTypes[item[i,j]].itemSprite;
                    if (World.blockTypes[item[i, j]].Items.isblock)
                    {
                        invimg[i * 9 + j].num.gameObject.SetActive(true);
                        invimg[i * 9 + j].num.text = itemsize[i, j].ToString();
                    }
                }
                else if (itemsize[i,j] == 0 || item[i,j]==0)
                {
                    invimg[i * 9 + j].image.gameObject.SetActive(true);
                    invimg[i * 9 + j].image.sprite = World.blockTypes[0].itemSprite;
                }
            }
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //Thread t=new Thread(() => craft.Inventory(tpe));
        //t.Start();
        craft.Inventory(tpe);
    }
    public void PickUp(byte id, byte size,Item obj)
    {
        for(int i = 0; i<4; i++)
        {
            for (int j = 0; j<9; j++)
            {
                if (item[i, j]==id)
                {
                    if (itemsize[i,j]+size<=96)
                    {
                        itemsize[i, j]+=size;
                        size = 0;
                        if (i < 1)
                        {
                            itemSlots[j].num.text = itemsize[i,j].ToString();
                        }
                        j = 9;
                        i = 4;
                    }
                    else if(itemsize[i, j] < 96)
                    {
                        size -= (byte)(96 - itemsize[i, j]);
                        itemsize[i, j] = 96;
                        if (i < 1)
                        {
                            itemSlots[j].num.text = itemsize[i, j].ToString();
                        }
                    }
                }
            }
        }
        if (size > 0)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (itemsize[i, j]==0)
                    {
                        item[i, j] = id;
                        itemsize[i, j] += size;
                        size = 0;
                        if (i < 1)
                        {
                            itemSlots[j].image.sprite = World.blockTypes[id].itemSprite;
                            itemSlots[j].num.text = itemsize[i,j].ToString();
                        }
                        i = 4;
                        j = 9;
                    }
                }
            }
        }

        if (size == 0)
        {
            Destroy(obj.obj);
        }
        else
        {
            obj.size = size;
        }
    }
    public void CloseInventory()
    {
        GameObject[] go = GameObject.FindGameObjectsWithTag("crf");
        foreach (GameObject go2 in go)
        {
            Destroy(go2);
        }
        GameObject[] gO = GameObject.FindGameObjectsWithTag("fin");
        foreach (GameObject go2 in gO)
        {
            Destroy(go2);
        }
        if (craft.sizeofitem > 0)
        {
            for(int i = 0; i<4; i++)
            {
                for(int j = 0; j<9; j++)
                {
                    if (item[i, j] == 0)
                    {
                        if(craft.sizeofitem > 96) {
                            item[i,j] = craft.itemcraft;
                            itemsize[i,j]=96;
                            craft.sizeofitem -= 96;
                        }
                        else
                        {
                            item[i, j] = craft.itemcraft;
                            itemsize[i, j] =craft.sizeofitem;
                            craft.sizeofitem=0;
                            invimg[36].num.text = null;
                            break;
                        }
                    }
                    else if (item[i, j] == craft.itemcraft)
                    {
                        int a=craft.sizeofitem+itemsize[i,j];
                        if (a > 96)
                        {
                            itemsize[i, j] = 96;
                            craft.sizeofitem=(byte)(a-96);
                        }
                        else
                        {
                            itemsize[i,j] =(byte)a;
                            craft.sizeofitem = 0;
                            invimg[36].num.text = null;
                            break;
                        }
                    }
                    if(craft.sizeofitem== 0)
                    {
                        invimg[36].num.text = null;
                        break;
                    }
                }
            }
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inventory.gameObject.SetActive(false);
        crosshair.gameObject.SetActive(true);
        openedInv = false;
        select.gameObject.SetActive(false);
        for (int i = 0; i < 9; i++)
        {
            if (item[0, i] > 0)
            {
                itemSlots[i].image.sprite = World.blockTypes[item[0, i]].itemSprite;
                if (World.blockTypes[item[0, i]].Items.isblock)
                    itemSlots[i].num.text = itemsize[0, i].ToString();
                else
                    itemSlots[i].num.text = null;
            }
            else 
            {
                itemSlots[i].image.sprite = World.blockTypes[0].itemSprite;
                itemSlots[i].num.text = null;
            }
        }
        lastitem = 255;
        invimg[36].image.gameObject.SetActive(false);

    }
    public void CloseScene()
    {
        Debug.Log(WorldManager.chunkstosave.Count);
        Application.runInBackground = true;
        SaveInventory();
        ChunkSerializer.savePlayerData(control.PlayerPos(), control.PlayerRot());
        for (int i = 0; i<WorldManager.chunkstosave.Count; i++)
        {
            ChunkSerializer.loadedChunks[(WorldManager.chunkstosave[i].x, WorldManager.chunkstosave[i].y)] = WorldManager.chunks[WorldManager.chunkstosave[i].x+100, WorldManager.chunkstosave[i].y+100].Voxels;
            ChunkSerializer.SaveChunk(WorldManager.chunkstosave[i].x, WorldManager.chunkstosave[i].y);
        }
        World.ClearData();
        ChunkSerializer.loadedChunks.Clear();
        Application.runInBackground = false;
        System.GC.Collect();
        SceneManager.LoadScene(1);
        escape = false;
        openedInv = false;
    }
    public void UpdateInventory()
    {
        ;
    }
    public void EscapeNSetUp()
    {
        //sunt obosit si plictisit ;<
        Application.runInBackground = true;
        SaveInventory();
        ChunkSerializer.savePlayerData(control.PlayerPos(), control.PlayerRot());
        for (int i = 0; i < WorldManager.chunkstosave.Count; i++)
        {
            ChunkSerializer.loadedChunks[(WorldManager.chunkstosave[i].x, WorldManager.chunkstosave[i].y)] = WorldManager.chunks[WorldManager.chunkstosave[i].x + 100, WorldManager.chunkstosave[i].y + 100].Voxels;
            ChunkSerializer.SaveChunk(WorldManager.chunkstosave[i].x, WorldManager.chunkstosave[i].y);
        }
        ChunkSerializer.loadedChunks.Clear();
        World.ClearData();
        Application.runInBackground = false;
        UiManager e = new();
        e.OpenSet("World");
        //escape = false;
        //openedInv = false;
    }
    public void UpdateAnItem(byte id)
    {
        itemsize[0, id]= (byte)(itemsize[0, id]-1);
        byte num= itemsize[0, id];
        if (num == 0)
        {
            itemSlots[id].image.sprite = World.blockTypes[0].itemSprite;
            itemSlots[id].num.text=null;
            invimg[id].image.sprite= World.blockTypes[0].itemSprite;
            invimg[id].num.text = null;
            item[0, id] = 0;
            itemsize[0, id] =0;
        }
        else
        {
            itemSlots[id].num.text=num.ToString();
        }
    }
    public void UpdateItem(byte id,byte less)
    {
        itemsize[id/9,id%9] = (byte)(itemsize[id / 9, id % 9] - less);
        byte num = itemsize[id / 9, id % 9];
        if (num == 0)
        {
            invimg[id].image.sprite = World.blockTypes[0].itemSprite;
            invimg[id].num.text = null;
            item[id / 9, id % 9] = 0;
            itemsize[id / 9, id % 9] = 0;
        }
        else
        {
            invimg[id].num.text = num.ToString();
        }
    }
    public void SearchInInventory(byte id)
    {
        for(byte i = 0; i<9; i++)
        {
            if (item[0, i] == id)
            {
                slothIndex = i;
                highlight.position = itemSlots[slothIndex].image.transform.position;
                return;
            }
        }
        for(int i=1; i<4; i++)
        {
            for(int j=0; j<9; j++)
            {
                if (item[i, j] == id)
                {
                    if (item[0, slothIndex] == 0)
                    {
                        itemsize[0, slothIndex] = itemsize[i, j];
                        itemSlots[slothIndex].image.sprite = World.blockTypes[id].itemSprite;
                        itemSlots[slothIndex].num.text = itemsize[i,j].ToString();

                        itemsize[i, j] = 0;
                        item[0, slothIndex] = id;
                        item[i, j] = 0;
                        return;
                    }
                    else
                    {
                        for(byte k=0; k<9; k++)
                        {
                            if (item[0, k] == 0)
                            {
                                slothIndex = k;
                                highlight.position = itemSlots[slothIndex].image.transform.position;
                                itemsize[0, slothIndex] = itemsize[i, j];
                                itemSlots[slothIndex].image.sprite = World.blockTypes[id].itemSprite;
                                itemSlots[slothIndex].num.text = itemsize[i, j].ToString();
                                itemsize[i, j] = 0;
                                item[0, slothIndex] = id;
                                item[i, j] = 0;
                                return;
                            }
                        }
                    }
                    itemSlots[slothIndex].image.sprite = World.blockTypes[id].itemSprite;
                    
                    byte a = itemsize[0,slothIndex], b= item[0, slothIndex];
                    itemSlots[slothIndex].num.text = itemsize[i, j].ToString();
                    itemsize[0, slothIndex] = itemsize[i, j];
                    itemsize[i, j] = a;
                    item[0, slothIndex] = id;
                    item[i, j] = b;
                    return;
                }
            }
        }

    }
}
    [System.Serializable]
    public class ItemSlot
    {
    public byte id;
    public Image image;
    public Text num;
}
    [System.Serializable]
    public class AllSlots
    {
    public byte id;
    public Image image;
    public Text num;
    }
    [Serializable]
    public class Enchantments
    {
        public int sharpness;
        public int unbreaking;
    }
    [Serializable]
    public class ItemProperties
    {
        public byte special;
        public byte damage;
        public float speed;
    }
    [Serializable]
    public class ItemSave
    {
        public string itemName;
        public byte itemID;
        public byte quantity;
        public byte sloth;
        public ItemProperties properties;
        public Enchantments enchantments; // This can be null for items without enchantments
    }
    [Serializable]
    public class PlayerDataOpener
    {
        public string playerName;
        public int health;
        public int experienceLevel;
        public ItemSave[] inventory;
        public byte slothid;
    }

