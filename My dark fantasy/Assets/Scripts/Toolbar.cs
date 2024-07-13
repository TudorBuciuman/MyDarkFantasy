using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Toolbar : MonoBehaviour
{
    public WorldManager World;
    public RectTransform highlight;
    public ItemSlot[] itemSlots;
    public Image crosshair;
    public Image inventory;
    public byte[,] item = new byte[4, 9];
    public byte slothIndex = 0;
    float time = 0f;
    public bool openedInv = false;

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 9; j++)
                item[i, j] = 7;
        }
        for (int i=0; i<9; i++)
        {
            itemSlots[i].image.sprite = World.blockTypes[item[0,i]].icon;
            itemSlots[i].image.gameObject.SetActive(true);
        }
    }
    void Update()
    {
        float Scroll = Input.GetAxis("Mouse ScrollWheel");

        if( Scroll != 0 ) {
        if(Scroll>0)
                slothIndex--;
        else
                slothIndex++;
            if (slothIndex < 0)
                slothIndex = 8;
            if( slothIndex ==9 )
                slothIndex=0;
            highlight.position = itemSlots[slothIndex].image.transform.position;
            World.selectedSlot = (byte)slothIndex;
        }
        if (Input.GetKey(KeyCode.E) && time<=0)
        {
            time = 0.4f;
            if (openedInv)
            {
                CloseInventory();
            }
            else
                OpenInventory();
            
        }
        if(time>0)
            time -= Time.deltaTime;
    }
    public void OpenInventory()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        inventory.gameObject.SetActive(true);
        crosshair.gameObject.SetActive(false);
        openedInv = true;
    }
    public void CloseInventory()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inventory.gameObject.SetActive(false);
        crosshair.gameObject.SetActive(true);
        openedInv = false;
    }
}
    [System.Serializable]
    public class ItemSlot
{
    public byte id;
    public Image image;
}

