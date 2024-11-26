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
    public Text FunnyText;
    public Sprite moon;
    public GameObject screenn,but1,but2,but3;
    public static byte itemsnum;
    public static BlockProprieties[] blockTypes=new BlockProprieties[51];
    
    public Sprite[] itemsAtlas;
    void Awake()
    {
        dialogueFile = Resources.Load<TextAsset>($"UIMessage");
        DateTime currentTime = DateTime.Now;
        if (currentTime.Hour > 18 || currentTime.Hour<6)
        {
            screenn.GetComponent<RawImage>().texture=moon.texture;
            but1.GetComponent<Image>().color=Color.white;
            but2.GetComponent<Image>().color = Color.white;
            but3.GetComponent<Image>().color = Color.white;
        }
        if (!readytogo)
        {
        #if UNITY_ANDROID
            StartCoroutine(AndroidRead());
        #else
            ReadWhatNeedsTo();
        #endif
        } 
        SetFunnyText();
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
            foreach (var item in data.items)
            {
                blockTypes[w] = new();
                blockTypes[w].Items = item;
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
        foreach(var item in data.items)
        {
            blockTypes[w]=new();
            blockTypes[w].Items = item;
            if(itemsAtlas.Length>w)
            blockTypes[w].itemSprite = itemsAtlas[w];
            w++;
        }
    }
    public void SetFunnyText()
    {
        int line = UnityEngine.Random.Range(0, 28);
        string[] dialogueLines = dialogueFile.text.Split('\n');
        FunnyText.text = dialogueLines[line];

    }

}
