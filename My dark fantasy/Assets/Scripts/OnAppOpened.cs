using System.Collections;
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
    public static BlockProprieties[] blockTypes=new BlockProprieties[100];
    public static Recipes[] craftRec=new Recipes[100];
    public GameObject MDF,yeezy;
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
            ReadWhatNeedsTo();
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
            Debug.LogError("Failed to load JSON: " + request.error);
        }
    }
    public void ReadWhatNeedsTo()
    {

        TextAsset jsonFile = Resources.Load<TextAsset>("everyitem");
        if (jsonFile != null)
        {
            AllItems data = JsonUtility.FromJson<AllItems>(jsonFile.text);
            itemsnum = (byte)data.items.Length;
            Array.Resize(ref blockTypes, itemsnum);
            readytogo = true;
            byte w = 0;
            itemsAtlas = InitializeAtlas();
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
            Debug.LogError("JSON file not found!");
            Debug.LogError("You piece of shit");
            
        }
        TextAsset craftingFile = Resources.Load<TextAsset>("everyrecipe");
        if (jsonFile != null)
        {
            RecipesClass data = JsonUtility.FromJson<RecipesClass>(craftingFile.text);
            itemsnum = (byte)data.recipes.Length;
            Array.Resize(ref craftRec, itemsnum);
            readytogo = true;
            byte w = 0;
            itemsAtlas = InitializeAtlas();
            foreach (Recipes item in data.recipes)
            {
                craftRec[w] = new();
                craftRec[w] = item;
                w++;
            }
        }
        else
        {
            Debug.LogError("recipes file not found!");
            Debug.LogError("You piece of shit");

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
        if (Voxeldata.PlayerData.special == 12)
        {
            FunnyText.text = "Kill the king";
        }
        else if (Voxeldata.PlayerData.special == 13)
        {
            FunnyText.text = " X = 2300 \n Z = 5600";
        }
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
        RenderSettings.skybox.SetFloat("_Exposure", 0.66f);

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

