using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using Unity.VisualScripting;

public class OnAppOpened : MonoBehaviour
{
    public static bool readytogo=false;
    public TextAsset dialogueFile;
    public static OnAppOpened onAppOpened;
    public Text FunnyText;
    public Sprite moon;
    public GameObject screenn,but1,but2,but3;
    public static byte itemsnum;
<<<<<<< Updated upstream
    public static BlockProprieties[] blockTypes=new BlockProprieties[62];
    public GameObject MDF;
=======
    public static BlockProprieties[] blockTypes=new BlockProprieties[66];
    public GameObject MDF,yeezy;
>>>>>>> Stashed changes
    public GameObject fallingGObj;
    public AudioClip[] clips = new AudioClip[5];
    public AudioSource audioSource;
    public GameObject falling;
    public GameObject flowers;
    public static bool pressed=false;
    public GameObject cameraObj;
    private float rotationTime = 230;
    public Material Skyboxmat;
    
    public static Sprite[] itemsAtlas;
    void Awake()
    {
        onAppOpened = this;
        if (pressed)
        {
            MDF.SetActive(false);
            yeezy.SetActive(false);
        }
        dialogueFile = Resources.Load<TextAsset>($"UIMessage");
        DateTime currentTime = DateTime.Now;

        if (Voxeldata.PlayerData.scene == 2 && !Voxeldata.PlayerData.genocide)
        {
            audioSource.clip = clips[2];
            audioSource.loop = false;
            falling.SetActive(true);
        }
        else if (Voxeldata.PlayerData.genocide && Voxeldata.PlayerData.scene == 1)
        {
            audioSource.clip = clips[2];
            audioSource.loop = false;
            falling.SetActive(true);
        }
        else
        {
            audioSource.loop = true;
            audioSource.clip = clips[Voxeldata.PlayerData.scene];
        }
        audioSource.Play();
        SetFunnyText();
        if (!pressed)
        {
            StartCoroutine(Waiting());
        }
        else
        {
            StartCoroutine(Spinning());
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
        if (Voxeldata.PlayerData.scene == 1)
        {
            MDF.SetActive(false);
            yeezy.SetActive(true);
        }
        while (true)
        {
            //*android
            if (Input.touchCount > 0)
            {
                MDF.SetActive(false);
                yeezy.SetActive(false);
                pressed = true;
                break;
            }
            
            if (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.Z))
            {
                MDF.SetActive(false);
                yeezy.SetActive(false);
                pressed=true;
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
        StartCoroutine(Spinning());
        yield return null;
    }
    public IEnumerator Spinning()
    {
        if (Voxeldata.PlayerData.genocide && Voxeldata.PlayerData.scene == 1)
            yield return null;
        if (!Voxeldata.PlayerData.genocide && Voxeldata.PlayerData.scene == 2)
            yield return null;
        if (Voxeldata.PlayerData.scene == 0)
        {
            yield return new WaitForSeconds(0.5f);
            while (true)
            {
                float anglePerSecond = 360f / rotationTime;
                cameraObj.transform.Rotate(0, anglePerSecond * Time.deltaTime, 0, Space.Self);
                yield return null;
            }
        }
        else if(Voxeldata.PlayerData.scene == 1)
        {
            RenderSettings.skybox.SetFloat("_Exposure", 0.13f);
            DynamicGI.UpdateEnvironment();
            yield return new WaitForSeconds(0.2f);
            rotationTime = 300;
            while (true)
            {
                float anglePerSecond = 360f / rotationTime;
                cameraObj.transform.Rotate(0, anglePerSecond * Time.deltaTime, 0, Space.Self);
                yield return null;
            }
        }
        else if (Voxeldata.PlayerData.scene == 3)
        {
            yield return new WaitForSeconds(0.2f);
            rotationTime = 200;
            while (true)
            {
                float anglePerSecond = 360f / rotationTime;
                cameraObj.transform.Rotate(0, anglePerSecond * Time.deltaTime, 0, Space.Self);
                yield return null;
            }
        }
        else if (Voxeldata.PlayerData.scene == 4)
        {
            yield return new WaitForSeconds(0.2f);
            rotationTime = 500;
            while (true)
            {
                float anglePerSecond = 360f / rotationTime;
                cameraObj.transform.Rotate(0, anglePerSecond * Time.deltaTime, 0, Space.Self);
                yield return null;
            }
        }


        yield return null;
    }
}

