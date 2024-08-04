using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEditor;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEditor.UIElements;


public class Crafting : MonoBehaviour
{
    public Toolbar tool;
    public craftinginv craftinginv;
    public WorldManager world;
    public GameObject Prefa;
    public Recipes[] recipes=new Recipes[10];
    public byte[] ind=new byte[10];
    public int[] sz=new int[10];
    public byte options = 0;
    public byte itemcreated=0;
    public byte itemcraft = 0;
    public byte sizeofitem=0;
    public void OpenCraft(byte id)
    {
        switch (id)
        {
            case 0:
                Inventory();
                break;
            case 1:
                CraftingTable();
                break;
            case 2:
                Furnace();
            break;
            case 3:
                BlastFurnace();
                break;

        }
    }
    public void Inventory()
    {
        int p = 0;
        options = 0;
        //p=marimea itemelor posibile -1
        while (p<2)
        {
            int r = 0,k=1000000;
            sz = new int[10];
            foreach (itemsneeded d in recipes[p].Recipe)
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (tool.item[i,j]==d.id)
                        {
                            sz[r] += tool.itemsize[i, j];
                        }
                    }
                }
                r++;
            }
            r = 0;
            foreach (itemsneeded d in recipes[p].Recipe)
            {
                if (sz[r] >= d.size)
                {
                    if (k > sz[r]/d.size)
                    k = (sz[r] / d.size)*recipes[p].size;
                }
                else
                {
                    k = 0;
                    break;
                }
            }
            if (k>0)
            {

                ind[options] = recipes[p].id;
                sz[options] =k;
                GameObject c = Instantiate(Prefa,transform);
                c.GetComponentInChildren<Image>().sprite = world.blockTypes[recipes[p].id].icon;
                if(k<255)
                c.GetComponentInChildren<Text>().text = k.ToString();
                else
                {
                    c.GetComponentInChildren<Text>().text=("255+").ToString();
                }
                options++;

            }
            p++;
        }
    }
    public void CraftingTable()
    {

    }
    public void Furnace()
    {

    }
    public void BlastFurnace()
    {

    }
    public void CraftScreen(byte id)
    {
        byte p = 0;
        int k = 20000;
        foreach (itemsneeded d in recipes[id].Recipe)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (tool.item[i, j] == d.id)
                    {
                        sz[p] += tool.itemsize[i, j];
                    }
                }
            }
            p++;
        }
        p = 0;
        foreach (itemsneeded d in recipes[id].Recipe)
        {
            if (sz[p] >= d.size)
            {
                if (k > sz[p] / d.size)
                    k = (sz[p] / d.size) * recipes[p].size;
            }
            else
            {
                k = 0;
                break;
            }
        }
        GameObject[] g = GameObject.FindGameObjectsWithTag("fin");
        foreach(GameObject i in g)
         Destroy(i);
        craftinginv.Screening(id);
    }
    public void Craft()
    {
        byte id = itemcreated;
        if (sizeofitem<255 &&  id != 255 && sz[id]>0)
        {
            byte[] szz = new byte[9];
            byte r = 0;
            foreach (itemsneeded d in recipes[id].Recipe)
            {
                szz[r] = d.size;
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (tool.item[i, j] == d.id && tool.itemsize[i, j] >= d.size)
                        {
                            szz[r] -= d.size;
                            tool.UpdateItem((byte)(i * 9 + j), d.size);
                        }
                        else if (tool.item[i, j] == d.id && tool.itemsize[i, j] < d.size)
                        {
                            szz[r] -= tool.itemsize[i, j];
                            tool.UpdateItem((byte)(i * 9 + j), tool.itemsize[i, j]);
                        }
                        if (szz[r] == 0)
                        {
                            i = 4;
                            j = 9;
                        }
                    }
                }
                r++;
            }
            itemcreated = id;
            sz[id]-=recipes[id].size;
            sizeofitem += recipes[id].size;
            tool.invimg[36].num.text = sizeofitem.ToString();
            GameObject[] a = GameObject.FindGameObjectsWithTag("crf");
            a[2*id].GetComponentInChildren<Text>().text = sz[id].ToString();
            if (sz[id] < 255)
            {
                a[2 * id].GetComponentInChildren<Text>().text = sz[id].ToString();
            }
            else
            {
                a[id * 2].GetComponentInChildren<Text>().text = ("255+").ToString();
            }
        }
    }
}

[System.Serializable]
public class Recipes
{
    public string Name;
    public byte id;
    public byte type;
    public byte size;
    public itemsneeded[] Recipe;

}
[System.Serializable]
public class itemsneeded
{
    public byte id;
    public byte size;
}