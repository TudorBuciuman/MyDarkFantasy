using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class craftinginv : MonoBehaviour
{
    public GameObject Prefa;
    public Toolbar toolbar;
    public WorldManager world;
    public Crafting crf;
    public void Screening(byte id)
    {
        foreach (itemsneeded d in crf.recipes[id].Recipe)
        {
            GameObject c = Instantiate(Prefa, transform);
            c.GetComponentInChildren<Image>().sprite = world.blockTypes[d.id].itemSprite;
            c.GetComponentInChildren<Text>().text = (d.size).ToString();
        }
        toolbar.invimg[36].image.sprite = world.blockTypes[crf.recipes[id].id].itemSprite;
        toolbar.invimg[36].image.gameObject.SetActive(true);
        toolbar.invimg[36].num.gameObject.SetActive(true);
    }
}
