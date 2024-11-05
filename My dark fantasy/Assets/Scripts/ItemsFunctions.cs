using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsFunctions : MonoBehaviour
{
    public static WorldManager wman;
    public static itemsManager item;

    public static void ItemsStart(WorldManager Wman,itemsManager Item)
    { 
        wman=Wman;
        item=Item;
    }
    public static void CutDownTree(Vector3 pos,byte type)
    {
        int x = Mathf.RoundToInt(pos.x), z = Mathf.RoundToInt(pos.z), y = Mathf.RoundToInt(pos.y);
        byte size = 1;
        for(int i = y+1; i<120 && wman.Block(x,i,z)==type; i++)
        {
            WorldManager.SetTo(x,i,z,0);
            size++;
        }
        item.SetItem(wman.Block(x,y,z), size, new Vector3(pos.x, Mathf.RoundToInt(pos.y) - 0.3f, pos.z));
        wman.ModifyMesh(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z), 0);
    }
}
