using UnityEngine;
using UnityEngine.UI;

public class Crafting : MonoBehaviour
{
    public Toolbar tool;
    public craftinginv craftinginv;
    public WorldManager world;
    public GameObject Prefa;
    public Recipes[] recipes=new Recipes[10];
    public byte[] ind=new byte[10];
    public int[] sz=new int[10];
    public int[] e=new int[10];
    public byte options = 0;
    public byte itemcreated=0;
    public byte itemcraft = 0;
    public byte itempos = 0;
    public byte sizeofitem=0;
    public void OpenCraft(byte id)
    {
        Inventory(id);

    }
    public void Inventory(byte tpe)
    {
        byte p = 0;
        options = 0;
        while (p < recipes.Length)
        {
            if (recipes[p].type < 1)
            {
                int r = 0, k = 1000000;
                sz = new int[10];
                foreach (itemsneeded d in recipes[p].Recipe)
                {

                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            if (tool.item[i, j] == d.id)
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
                        if (k > sz[r] / d.size)
                            k = (sz[r] / d.size) * recipes[p].size;
                    }
                    else
                    {
                        k = 0;
                        break;
                    }
                }
                if (k > 0)
                {

                    ind[options] = p;
                    e[p] = k;
                    GameObject c = Instantiate(Prefa, transform);
                    c.GetComponentInChildren<Image>().sprite = world.blockTypes[recipes[p].id].itemSprite;
                    if (k < 255)
                        c.GetComponentInChildren<Text>().text = k.ToString();
                    else
                    {
                        c.GetComponentInChildren<Text>().text = ("255+").ToString();
                    }
                    options++;

                }
                else
                {
                    Debug.Log(recipes[p].Name);
                }
                p++;
            }
            else
                break;
        }
        if (tpe > 0)
        {
            while (p < recipes.Length && recipes[p].type < tpe)
                p++;
            while (p < recipes.Length)
            {
                if (recipes[p].type ==tpe)
                {
                    int r = 0, k = 1000000;
                    sz = new int[10];
                    foreach (itemsneeded d in recipes[p].Recipe)
                    {

                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 9; j++)
                            {
                                if (tool.item[i, j] == d.id)
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
                            if (k > sz[r] / d.size)
                                k = (sz[r] / d.size) * recipes[p].size;
                        }
                        else
                        {
                            k = 0;
                            break;
                        }
                    }
                    if (k > 0)
                    {

                        ind[options] = p;
                        e[p] = k;
                        GameObject c = Instantiate(Prefa, transform);
                        c.GetComponentInChildren<Image>().sprite = world.blockTypes[recipes[p].id].itemSprite;
                        if (k < 255)
                            c.GetComponentInChildren<Text>().text = k.ToString();
                        else
                        {
                            c.GetComponentInChildren<Text>().text = ("255+").ToString();
                        }
                        options++;

                    }
                    p++;
                }
                else
                    break;
            }
        }
    }
    public void CraftScreen(byte id)
    {
        byte p = 0,q=0;
        int k = 20000;
        foreach(byte f in ind)
        {
            q++;
            if (f == id)
                break;
        }
        q--;
        itempos = q;
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
        byte w =itempos ;
        
        if (sizeofitem<255 &&  id != 255 && e[id]>0)
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
                        if (tool.item[i, j] == d.id && tool.itemsize[i, j] >= szz[r])
                        {
                            tool.UpdateItem((byte)(i * 9 + j), szz[r]);
                            szz[r] =0;                            
                        }
                        else if (tool.item[i, j] == d.id && tool.itemsize[i, j] < szz[r])
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
            e[id]-=recipes[id].size;
            sizeofitem += recipes[id].size;
            tool.invimg[36].num.text = sizeofitem.ToString();
            GameObject[] a = GameObject.FindGameObjectsWithTag("crf");
            a[w*2].GetComponentInChildren<Text>().text = e[id].ToString();
            if (e[w] < 255)
            {
                a[( w*2)].GetComponentInChildren<Text>().text = e[id].ToString();
            }
            else
            {
                a[(w * 2)].GetComponentInChildren<Text>().text = ("255+").ToString();
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