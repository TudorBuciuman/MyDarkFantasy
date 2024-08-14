using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Threading;
using UnityEngine.EventSystems;
using UnityEditor.UIElements;
using System.Linq;
public class Toolbar : MonoBehaviour
{
    public WorldManager World;
    public Crafting craft;
    public RectTransform highlight;
    public AllSlots[] itemSlots=new AllSlots[9];
    public ItemSlot[] invimg = new ItemSlot[38];
    public Image crosshair;
    public Image inventory;
    public Image UIEscape;
    public Image select;
    
    public byte[,] item = new byte[4, 9];
    public byte[,] itemsize = new byte[4, 9];
    public byte selectedsloth;
    public byte slothIndex = 0;
    float time = 0f;
    public bool openedInv = false;
    public bool escape=false;

    public PointerEventData faranume;
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;

    private Material white;
    private Material gray;
    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j <9; j++)
            {
                item[i, j] = (byte)(j%13+1);
                itemsize[i,j] = (byte)(i*18+j+1);
            }
        }
        item[0, 0] = 13;
        for (int i=0; i<9; i++)
        {
            if (item[0,i] >0)
            {
                itemSlots[i].image.sprite = World.blockTypes[item[0, i]].icon;
                itemSlots[i].image.gameObject.SetActive(true);
                itemSlots[i].num.gameObject.SetActive(true);
                if (World.blockTypes[item[0, i]].utility == 0)
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
                World.selectedSlot = (byte)slothIndex;
            }
        }
        else 
        {
            if(Input.GetMouseButtonDown(0))
            SearchForImput();
        }
        if (!escape)
        {
            if (Input.GetKey(KeyCode.E) && time <= 0)
            {
                time = 0.4f;
                if (openedInv)
                {
                    CloseInventory();
                }
                else
                {
                    OpenInventory();

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
        faranume = new PointerEventData(eventSystem);
        faranume.position = Input.mousePosition ;
        List<RaycastResult> results = new List<RaycastResult>();
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
                    if (f != 50)
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
                            invimg[f].image.sprite = World.blockTypes[0].icon;
                            invimg[selectedsloth].num.text = invimg[f].num.text;
                            invimg[f].num.text = null;
                            selectedsloth = 50;
                        }
                        else if (item[selectedsloth / 9, selectedsloth % 9] == item[f / 9, f % 9])
                        {
                            if (itemsize[selectedsloth / 9, selectedsloth % 9] + itemsize[f / 9, f % 9] <= 96)
                            {
                                invimg[f].image.sprite = World.blockTypes[0].icon;
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
                            invimg[selectedsloth].image.sprite = World.blockTypes[item[selectedsloth / 9, selectedsloth % 9]].icon;
                            invimg[f].image.sprite = World.blockTypes[item[f / 9, f % 9]].icon;
                        }
                        select.gameObject.SetActive(false);
                        selectedsloth = 50;
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
                    if (World.blockTypes[craft.ind[i]].icon == img.sprite)
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
                                craft.itemcreated = i; craft.sizeofitem = 0;
                                craft.itemcraft = craft.ind[i];
                                select.transform.position = img.transform.position;
                                select.gameObject.SetActive(true);
                                TryCrafting(i);
                            }
                        }
                        else
                        {
                            
                            select.gameObject.SetActive(false);
                            lastitem = 255;
                            craft.itemcreated = 255;
                            if (craft.sizeofitem == 0)
                            {
                                invimg[36].image.sprite = World.blockTypes[0].icon;
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
    public void TryCrafting(byte id)
    {
        craft.CraftScreen(id);
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UIEscape.gameObject.SetActive(false);
        openedInv = false;
        escape = false;
        CloseInventory();
    }
    public void OpenInventory()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        invimg[36].image.sprite= World.blockTypes[0].icon;
        invimg[37].image.gameObject.SetActive(true);
        invimg[36].image.gameObject.SetActive(true);
        selectedsloth = 50;
        inventory.gameObject.SetActive(true);
        crosshair.gameObject.SetActive(false);
        openedInv = true;
        for(int i=0; i<4; i++)
        {
            for(int j=0; j<9; j++)
            {
                if (itemsize[i,j] != 0)
                {
                    invimg[i*9+j].image.sprite = World.blockTypes[item[i,j]].icon;
                    invimg[i*9+j].image.gameObject.SetActive(true);
                    if (World.blockTypes[item[i, j]].utility == 0)
                    {
                        invimg[i * 9 + j].num.text = itemsize[i, j].ToString();
                        invimg[i * 9 + j].num.gameObject.SetActive(true);
                    }
                }
                else if (itemsize[i,j] == 0 || item[i,j]==0)
                {
                    invimg[i * 9 + j].image.sprite = World.blockTypes[0].icon;
                    invimg[i * 9 + j].image.gameObject.SetActive(true);
                }
            }
        }
        Thread t=new Thread(craft.Inventory);
        craft.OpenCraft(0);
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
                itemSlots[i].image.sprite = World.blockTypes[item[0, i]].icon;
                if (World.blockTypes[item[0, i]].utility !=1)
                    itemSlots[i].num.text = itemsize[0, i].ToString();
                else
                    itemSlots[i].num.text = null;
            }
            else 
            {
                itemSlots[i].image.sprite = World.blockTypes[0].icon;
                itemSlots[i].num.text = null;
            }
        }
        lastitem = 255;
        invimg[36].image.gameObject.SetActive(false);

    }
    public void CloseScene()
    {
        Debug.Log(WorldManager.chunkstosave.Count);
        for (int i = 0; i<WorldManager.chunkstosave.Count; i++)
        {
            ChunkSerializer.SaveChunk(WorldManager.chunkstosave[i].x, WorldManager.chunkstosave[i].y);
        }
        ChunkSerializer.loadedChunks.Clear();
        SceneManager.LoadScene(1);
    }
    public void UpdateInventory()
    {
        ;
    }
    public void UpdateAnItem(byte id)
    {
        itemsize[0, id]= (byte)(itemsize[0, id]-1);
        byte num= itemsize[0, id];
        if (num == 0)
        {
            itemSlots[id].image.sprite = World.blockTypes[0].icon;
            itemSlots[id].num.text=null;
            invimg[id].image.sprite= World.blockTypes[0].icon;
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
            invimg[id].image.sprite = World.blockTypes[0].icon;
            invimg[id].num.text = null;
            item[id / 9, id % 9] = 0;
            itemsize[id / 9, id % 9] = 0;
        }
        else
        {
            invimg[id].num.text = num.ToString();
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

