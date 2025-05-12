using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChestInput : MonoBehaviour
{
    public Chest chest;
    public static ChestInput instance;
    public PointerEventData faranume;
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;
    public byte selectedsloth;
    public byte lastselectIorC = 100;
    public GameObject select;
    public float t = 0;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        chest = Chest.Instance;
        select.transform.position = new Vector2(-1000, -3000);
        lastselectIorC = 100;
        selectedsloth = 100;
        t = 0.3f;
    }

    void FixedUpdate()
    {
        if(Input.GetMouseButtonDown(0))
        Seek();
        if (Input.GetKey(KeyCode.E) && t<=0)
        {
            Chest.Instance.CloseChest();
        }
        t -= Time.deltaTime;
    }
    public void Opening()
    {
        select.transform.position = new Vector2(-1000, -3000);
        select.SetActive(false);
        lastselectIorC = 100;
        selectedsloth = 100;
        t = 0.3f;
    }
    public void Seek()
    {
        faranume = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new();
        raycaster.Raycast(faranume, results);
        foreach (var slot in results)
        {
            if (slot.gameObject.CompareTag("inv"))
            {
                Image img = slot.gameObject.GetComponent<Image>();
                byte f = selectedsloth;
                selectedsloth = 100;
                for (byte i = 0; i < 36; i++)
                {
                    if (chest.invItem[i].image == img)
                    {
                        selectedsloth = i;
                        break;
                    }
                }
                if (selectedsloth < 36 && f != 100)
                {
                    if (f != 100)
                    {
                        if (lastselectIorC == 1)
                        {
                            if (selectedsloth == f)
                            {
                                selectedsloth = 100;
                                lastselectIorC = 100;
                                select.SetActive(false);
                            }
                            else if (Toolbar.instance.item[selectedsloth / 9, selectedsloth % 9] == 0)
                            {
                                Toolbar.instance.item[selectedsloth / 9, selectedsloth % 9] = Toolbar.instance.item[f / 9, f % 9];
                                Toolbar.instance.itemsize[selectedsloth / 9, selectedsloth % 9] = Toolbar.instance.itemsize[f / 9, f % 9];
                                Toolbar.instance.item[f / 9, f % 9] = 0;
                                Toolbar.instance.itemsize[f / 9, f % 9] = 0;
                                select.SetActive(false);
                                chest.invItem[selectedsloth].image.sprite = chest.invItem[f].image.sprite;
                                chest.invItem[f].image.sprite = Toolbar.instance.World.blockTypes[0].itemSprite;
                                if (Toolbar.instance.World.blockTypes[Toolbar.instance.item[selectedsloth / 9, selectedsloth % 9]].Items.isblock)
                                    chest.invItem[selectedsloth].num.text = chest.invItem[f].num.text;
                                else
                                    chest.invItem[selectedsloth].num.text = null;
                                    chest.invItem[f].num.text = null;
                                selectedsloth = 100;
                            }
                            else if (Toolbar.instance.item[selectedsloth / 9, selectedsloth % 9] != Toolbar.instance.item[f / 9, f % 9])
                            {
                                (Toolbar.instance.item[selectedsloth / 9, selectedsloth % 9], Toolbar.instance.item[f / 9, f % 9]) = (Toolbar.instance.item[f / 9, f % 9], Toolbar.instance.item[selectedsloth / 9, selectedsloth % 9]);
                                (Toolbar.instance.itemsize[selectedsloth / 9, selectedsloth % 9], Toolbar.instance.itemsize[f / 9, f % 9]) = (Toolbar.instance.itemsize[f / 9, f % 9], Toolbar.instance.itemsize[selectedsloth / 9, selectedsloth % 9]);

                                (chest.invItem[selectedsloth].image.sprite, chest.invItem[f].image.sprite) = (chest.invItem[f].image.sprite, chest.invItem[selectedsloth].image.sprite);
                                (chest.invItem[selectedsloth].num.text, chest.invItem[f].num.text) = (chest.invItem[f].num.text, chest.invItem[selectedsloth].num.text);
                                selectedsloth = 100;
                                select.SetActive(false);

                            }
                            else if(Toolbar.instance.World.blockTypes[Toolbar.instance.item[selectedsloth / 9, selectedsloth % 9]].Items.isblock)
                            {
                                if (Toolbar.instance.item[selectedsloth / 9, selectedsloth % 9]+ Toolbar.instance.item[f / 9, f % 9]<=96)
                                {
                                    Toolbar.instance.itemsize[selectedsloth / 9, selectedsloth % 9] += Toolbar.instance.itemsize[f / 9, f % 9];
                                    Toolbar.instance.item[f / 9, f % 9] = 0;
                                    Toolbar.instance.itemsize[f / 9, f % 9] = 0;

                                    chest.invItem[f].image.sprite = Toolbar.instance.World.blockTypes[0].itemSprite;
                                    chest.invItem[selectedsloth].num.text = Toolbar.instance.itemsize[selectedsloth / 9, selectedsloth % 9].ToString();
                                    chest.invItem[f].num.text = null;
                                }
                                else
                                {
                                    Toolbar.instance.itemsize[selectedsloth / 9, selectedsloth % 9] =96;
                                    Toolbar.instance.itemsize[f / 9, f % 9] =(byte)(96- Toolbar.instance.itemsize[f / 9, f % 9]);

                                    chest.invItem[selectedsloth].num.text = Toolbar.instance.itemsize[selectedsloth / 9, selectedsloth % 9].ToString();
                                    chest.invItem[f].num.text = Toolbar.instance.itemsize[f / 9, f % 9].ToString();

                                }
                                select.SetActive(false);
                                selectedsloth = 100;
                            }
                            else
                            {
                                lastselectIorC = 100;
                                select.SetActive(false);
                            }
                        }
                        else if (lastselectIorC == 2)
                        {
                            if (Toolbar.instance.item[selectedsloth / 9, selectedsloth % 9] == 0)
                            {
                                Toolbar.instance.item[selectedsloth / 9, selectedsloth % 9] = chest.chestitems[f / 9, f % 9].id;
                                Toolbar.instance.itemsize[selectedsloth / 9, selectedsloth % 9] = chest.chestitems[f / 9, f % 9].size;
                                chest.chestitems[f / 9, f % 9].id = 0;
                                chest.chestitems[f / 9, f % 9].size = 0;

                                (chest.invItem[selectedsloth].image.sprite, chest.chestItem[f].image.sprite) = (chest.chestItem[f].image.sprite, chest.invItem[selectedsloth].image.sprite);
                                (chest.invItem[selectedsloth].num.text, chest.chestItem[f].num.text) = (chest.chestItem[f].num.text, chest.invItem[selectedsloth].num.text);

                                selectedsloth = 100;
                                select.SetActive(false);
                            }
                            else if (Toolbar.instance.item[selectedsloth / 9, selectedsloth % 9] != chest.chestitems[f / 9, f % 9].id)
                            {
                                (Toolbar.instance.item[selectedsloth / 9, selectedsloth % 9], chest.chestitems[f / 9, f % 9].id) =(chest.chestitems[f / 9, f % 9].id, Toolbar.instance.item[selectedsloth / 9, selectedsloth % 9]);
                                (Toolbar.instance.itemsize[selectedsloth / 9, selectedsloth % 9], chest.chestitems[f / 9, f % 9].size) =(chest.chestitems[f / 9, f % 9].size, Toolbar.instance.itemsize[selectedsloth / 9, selectedsloth % 9]);

                                (chest.chestItem[f].image.sprite, chest.invItem[selectedsloth].image.sprite) = (chest.invItem[selectedsloth].image.sprite, chest.chestItem[f].image.sprite);
                                (chest.chestItem[f].num.text, chest.invItem[selectedsloth].num.text) = (chest.invItem[selectedsloth].num.text, chest.chestItem[f].num.text);
                                selectedsloth = 100;
                                select.SetActive(false);

                            }
                            else if (Toolbar.instance.World.blockTypes[Toolbar.instance.item[selectedsloth / 9, selectedsloth % 9]].Items.isblock)
                            {
                                if (Toolbar.instance.item[selectedsloth / 9, selectedsloth % 9] + chest.chestitems[f/9,f%9].size <= 96)
                                {
                                    Toolbar.instance.itemsize[selectedsloth / 9, selectedsloth % 9] += chest.chestitems[f / 9, f % 9].size;
                                    chest.chestitems[f / 9, f % 9].id = 0;
                                    chest.chestitems[f / 9, f % 9].size = 0;

                                    chest.chestItem[f].image.sprite = Toolbar.instance.World.blockTypes[0].itemSprite;
                                    chest.invItem[selectedsloth].num.text = Toolbar.instance.itemsize[selectedsloth / 9, selectedsloth % 9].ToString();
                                    chest.chestItem[f].num.text = null;
                                }
                                else
                                {
                                    Toolbar.instance.itemsize[selectedsloth / 9, selectedsloth % 9] = 96;
                                    chest.chestitems[f / 9, f % 9].size = (byte)(96 - chest.chestitems[f / 9, f % 9].size);

                                    chest.invItem[selectedsloth].num.text = Toolbar.instance.itemsize[selectedsloth / 9, selectedsloth % 9].ToString();
                                    chest.invItem[f].num.text = chest.chestitems[f / 9, f % 9].size.ToString();

                                }
                                select.SetActive(false);
                                selectedsloth = 100;
                            }
                            else
                            {
                                lastselectIorC = 100;
                                select.SetActive(false);
                            }
                        }
                        else if(Toolbar.instance.item[selectedsloth / 9, selectedsloth % 9] != 0)
                        {
                            lastselectIorC = 1;
                            select.transform.position = img.transform.position;
                            select.SetActive(true);
                        }
                    }
                }
                else if (selectedsloth < 36)
                {
                    if(Toolbar.instance.item[selectedsloth / 9, selectedsloth % 9] != 0){
                        lastselectIorC = 1;
                        select.transform.position = img.transform.position;
                        select.SetActive(true);
                    }
                }
            }
            
            else if (slot.gameObject.CompareTag("crf"))
            {
                Image img = slot.gameObject.GetComponent<Image>();
                byte f = selectedsloth;
                selectedsloth = 100;
                for (byte i = 0; i < 27; i++)
                {
                    if (chest.chestItem[i].image == img)
                    {
                        selectedsloth = i;
                        break;
                    }
                }
                if (selectedsloth < 27 && f != 100)
                {
                    if (f != 100)
                    {
                        if (lastselectIorC == 2)
                        {
                            if (selectedsloth == f)
                            {
                                selectedsloth = 100;
                                lastselectIorC = 100;
                                select.SetActive(false);
                            }
                            else if (chest.chestitems[selectedsloth / 9, selectedsloth % 9].id == 0)
                            {
                                chest.chestitems[selectedsloth / 9, selectedsloth % 9].id = chest.chestitems[f / 9, f % 9].id;
                                chest.chestitems[f / 9, f % 9].id = 0;
                                chest.chestitems[selectedsloth / 9, selectedsloth % 9].size = chest.chestitems[f / 9, f % 9].size;
                                chest.chestitems[f / 9, f % 9].size = 0;

                                select.SetActive(false);
                                chest.chestItem[selectedsloth].image.sprite = chest.chestItem[f].image.sprite;
                                chest.chestItem[f].image.sprite = Toolbar.instance.World.blockTypes[0].itemSprite;
                                (chest.chestItem[selectedsloth].num.text,chest.chestItem[f].num.text)=(chest.chestItem[f].num.text, chest.chestItem[selectedsloth].num.text);
                                selectedsloth = 100;
                            }
                            else if (chest.chestitems[selectedsloth / 9, selectedsloth % 9].id != chest.chestitems[f / 9, f % 9].id)
                            {
                                (chest.chestitems[selectedsloth / 9, selectedsloth % 9].id, chest.chestitems[f / 9, f % 9].id) = (chest.chestitems[f / 9, f % 9].id, chest.chestitems[selectedsloth / 9, selectedsloth % 9].id);

                                (chest.chestitems[selectedsloth / 9, selectedsloth % 9].size, chest.chestitems[f / 9, f % 9].size) = (chest.chestitems[f / 9, f % 9].size, chest.chestitems[selectedsloth / 9, selectedsloth % 9].size);

                                (chest.chestItem[selectedsloth].image.sprite, chest.chestItem[f].image.sprite) = (chest.chestItem[f].image.sprite,chest.chestItem[selectedsloth].image.sprite);
                                (chest.chestItem[selectedsloth].num.text, chest.chestItem[f].num.text) = (chest.chestItem[f].num.text, chest.chestItem[selectedsloth].num.text);

                                selectedsloth = 100;
                                select.SetActive(false);
                            }
                            else if(Toolbar.instance.World.blockTypes[chest.chestitems[selectedsloth / 9, selectedsloth % 9].id].Items.isblock)
                            {
                                Debug.Log(2);

                                if (chest.chestitems[selectedsloth / 9, selectedsloth % 9].size+ chest.chestitems[f / 9, f % 9].size <= 96)
                                {
                                    chest.chestitems[selectedsloth / 9, selectedsloth % 9].size += chest.chestitems[f / 9, f % 9].size;
                                    chest.chestitems[f / 9, f % 9].size = 0;
                                    chest.chestitems[f / 9, f % 9].id = 0;
                                    chest.chestItem[f].image.sprite = Toolbar.instance.World.blockTypes[0].itemSprite;
                                    chest.chestItem[selectedsloth].num.text = chest.chestitems[selectedsloth / 9, selectedsloth % 9].size.ToString();
                                    chest.chestItem[f].num.text = null;
                                }
                                else
                                {
                                    chest.chestitems[selectedsloth / 9, selectedsloth % 9].size =96;
                                    chest.chestitems[f / 9, f % 9].size = (byte)(96- chest.chestitems[f / 9, f % 9].size);
                                    chest.chestItem[selectedsloth].num.text = chest.chestitems[selectedsloth / 9, selectedsloth % 9].size.ToString();
                                    chest.chestItem[f].num.text = chest.chestitems[f / 9, f % 9].size.ToString();
                                }
                                selectedsloth = 100;
                                select.SetActive(false);
                            }
                        }
                        else if (lastselectIorC == 1)
                        {
                            if (chest.chestitems[selectedsloth / 9, selectedsloth % 9].id == 0)
                            {
                                chest.chestitems[selectedsloth / 9, selectedsloth % 9].id = Toolbar.instance.item[f / 9, f % 9];
                                chest.chestitems[selectedsloth / 9, selectedsloth % 9].size = Toolbar.instance.itemsize[f / 9, f % 9];
                                Toolbar.instance.item[f / 9, f % 9] = 0;
                                Toolbar.instance.itemsize[f / 9, f % 9] = 0;
                                select.SetActive(false);
                                chest.chestItem[selectedsloth].image.sprite = chest.invItem[f].image.sprite;
                                chest.invItem[f].image.sprite = Toolbar.instance.World.blockTypes[0].itemSprite;
                                (chest.chestItem[selectedsloth].num.text, chest.invItem[f].num.text) = (chest.invItem[f].num.text, chest.chestItem[selectedsloth].num.text);
                                selectedsloth = 100;
                            }
                            else if (chest.chestitems[selectedsloth / 9, selectedsloth % 9].id != Toolbar.instance.item[f / 9, f % 9])
                            {
                                (chest.chestitems[selectedsloth / 9, selectedsloth % 9].id, Toolbar.instance.item[f / 9, f % 9]) = (Toolbar.instance.item[f / 9, f % 9], chest.chestitems[selectedsloth / 9, selectedsloth % 9].id);
                                (chest.chestitems[selectedsloth / 9, selectedsloth % 9].size, Toolbar.instance.itemsize[f / 9, f % 9]) = (Toolbar.instance.itemsize[f / 9, f % 9], chest.chestitems[selectedsloth / 9, selectedsloth % 9].size);

                                (chest.chestItem[selectedsloth].image.sprite, chest.invItem[f].image.sprite) = (chest.invItem[f].image.sprite, chest.chestItem[selectedsloth].image.sprite);
                                (chest.chestItem[selectedsloth].num.text, chest.invItem[f].num.text) = (chest.invItem[f].num.text, chest.chestItem[selectedsloth].num.text);
                                selectedsloth = 100;
                                select.SetActive(false);

                            }
                            else if (Toolbar.instance.World.blockTypes[chest.chestitems[selectedsloth / 9, selectedsloth % 9].id].Items.isblock)
                            {
                                if (chest.chestitems[selectedsloth / 9, selectedsloth % 9].size + Toolbar.instance.itemsize[f / 9, f % 9] <= 96)
                                {
                                    chest.chestitems[selectedsloth / 9, selectedsloth % 9].size += Toolbar.instance.itemsize[f / 9, f % 9];
                                    Toolbar.instance.itemsize[f / 9, f % 9] = 0;
                                    Toolbar.instance.item[f / 9, f % 9] = 0;
                                    chest.invItem[f].image.sprite = Toolbar.instance.World.blockTypes[0].itemSprite;
                                    chest.chestItem[selectedsloth].num.text = chest.chestitems[selectedsloth / 9, selectedsloth % 9].size.ToString();
                                    chest.invItem[f].num.text = null;
                                }
                                else
                                {
                                    chest.chestitems[selectedsloth / 9, selectedsloth % 9].size = 96;
                                    Toolbar.instance.itemsize[f / 9, f % 9] = (byte)(96 - Toolbar.instance.itemsize[f / 9, f % 9]);
                                    chest.chestItem[selectedsloth].num.text = chest.chestitems[selectedsloth / 9, selectedsloth % 9].size.ToString();
                                    chest.chestItem[f].num.text = Toolbar.instance.itemsize[f / 9, f % 9].ToString();
                                }
                                selectedsloth = 100;
                                select.SetActive(false);
                            }
                            else
                            {
                                lastselectIorC = 100;
                                select.SetActive(false);
                            }
                                
                        }
                        else if (chest.chestitems[selectedsloth / 9, selectedsloth % 9].id != 0)
                        {
                            lastselectIorC = 2;
                            select.transform.position = img.transform.position;
                            select.SetActive(true);
                        }
                    }

                }
                else if (selectedsloth < 27)
                {
                    if (chest.chestitems[selectedsloth / 9, selectedsloth % 9].id != 0)
                    {
                        lastselectIorC = 2;
                        select.transform.position = img.transform.position;
                        select.SetActive(true);
                    }
                }
            }
            
        }

    }
}
