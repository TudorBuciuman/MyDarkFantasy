using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;

public class OnAppOpened : MonoBehaviour
{
    public static bool readytogo=false;
    public TextAsset dialogueFile;
    public static OnAppOpened onAppOpened;
    public Text FunnyText;
    public Sprite moon;
    public GameObject screenn,but1,but2,but3;
    public static byte itemsnum;
    public static BlockProprieties[] blockTypes=new BlockProprieties[51];
    public GameObject MDF;

    public static bool pressed=false;
    
    public static Sprite[] itemsAtlas;
    void Awake()
    {
        onAppOpened = this;
        if (pressed)
        {
            MDF.SetActive(false);
        }
        dialogueFile = Resources.Load<TextAsset>($"UIMessage");
        DateTime currentTime = DateTime.Now;
        if (currentTime.Hour > 18 || currentTime.Hour<6)
        {
            screenn.GetComponent<RawImage>().texture=moon.texture;
            but1.GetComponent<Image>().color=Color.white;
            but2.GetComponent<Image>().color = Color.white;
            but3.GetComponent<Image>().color = Color.white;
        }
        
        SetFunnyText();
        if (!pressed)
        {
            StartCoroutine(Waiting());
        }
    }
    public void Start()
    {
        if (!readytogo)
        {
    #if UNITY_ANDROID
            StartCoroutine(AndroidRead());
    #else
            ReadWhatNeedsTo();
    #endif
        }
    }
    IEnumerator AndroidRead()
    {
        string settingsPath = Path.Combine(Application.streamingAssetsPath, "everyitem.json");

        UnityWebRequest request = UnityWebRequest.Get(settingsPath);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            AllItems data = JsonUtility.FromJson<AllItems>(json);
            itemsnum = (byte)data.items.Length;
            readytogo = true;
            byte w = 0;
            itemsAtlas = InitializeAtlas();
            foreach (var item in data.items)
            {
                blockTypes[w] = new();
                blockTypes[w].Items = item;
                if (itemsAtlas.Length > w)
                    blockTypes[w].itemSprite = itemsAtlas[w];
                w++;
            }
        }
        else
        {
            Debug.LogError("Failed to load JSON file: " + request.error);
        }
    }
    public void ReadWhatNeedsTo()
    {

        string settingsPath=Path.Combine(Application.streamingAssetsPath, "everyitem.json");
        string json = File.ReadAllText(settingsPath);
        AllItems data = JsonUtility.FromJson<AllItems>(json);
        itemsnum=(byte)data.items.Length;
        readytogo = true;
        byte w = 0;
        itemsAtlas = InitializeAtlas();
        foreach(var item in data.items)
        {
            blockTypes[w]=new();
            blockTypes[w].Items = item;
            if(itemsAtlas.Length>w)
            blockTypes[w].itemSprite = itemsAtlas[w];
            w++;
        }
    }
    private static Sprite[] InitializeAtlas()
    {
        return Resources.Load<ItemAtlas>("ItemAtlas").itemsAtlas;
    }
    public void SetFunnyText()
    {
        int line = UnityEngine.Random.Range(0, 28);
        string[] dialogueLines = dialogueFile.text.Split('\n');
        FunnyText.text = dialogueLines[line];

    }
    public IEnumerator Waiting()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        while (true)
        {
            //*android
            if (Input.touchCount > 0)
            {
                MDF.SetActive(false);
                pressed = true;
                break;
            }
            
            if (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return))
            {
                MDF.SetActive(false);
                pressed=true;
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

}

