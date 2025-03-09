using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UiManager : MonoBehaviour
{
    public static string scene=null;
    public GameObject a,b,d,e;
    public GameObject D, E;
    public static UiManager c;

    public Button gameset;
    public Button soundset;
    [Header("Basic Functions")]
    public Image backGround;
    public Text renderdis;
    public Slider disSlider;
    public Toggle hudqm;
    public Toggle showfps;
    public static bool hud=true,fps=false;

    public Text sensiv;
    public Slider sensivslid;

    [Header("Audio& soundtrack")]
    public Toggle audioqm;
    public Slider musicSlider;
    public Text musictext;
    public Slider soundsSlider;
    public Text soundtext;

    public GameObject MDF;

    public AudioSource audiosource;
    public AudioClip scarryclip;
    public AudioClip scarryclip2;
    public void Start()
    {
        Application.targetFrameRate = 30;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if(SceneManager.sceneCount==1)
            StartCoroutine(Close());
        
        if (SceneManager.GetSceneByName("Settings").isLoaded)
        {
            LoadSettingsScene();
            if (c == null)
            {
                c = this;
            }
        }
    }
    public void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.loadedSceneCount == 2)
            {
                if (SceneManager.GetSceneByName("Settings").isLoaded)
                {
                    LeaveSettings();
                }
            }
        }
    }
    int numOfClicks = 0;
    public void PressDestroy()
    {
        numOfClicks++;
        if (numOfClicks == 5)
        {
            StartCoroutine(BurnTheWorld());
        }
    }

    public IEnumerator BurnTheWorld()
    {
        OnAppOpened.onAppOpened.audioSource.Stop();
        audiosource.clip = scarryclip;
        audiosource.Play();
        yield return new WaitForSeconds(3);
        audiosource.clip = scarryclip2;
        audiosource.Play();
        yield return new WaitForSeconds(5);
        string location1 = Application.persistentDataPath;
        File.Delete(Path.Combine(location1+"/PlayerSave.json"));
        foreach (string f in Directory.GetDirectories(Path.Combine(location1, "worlds")))
        {
            foreach (string subFile in Directory.GetFiles(f, "*", SearchOption.AllDirectories))
            {
                File.SetAttributes(subFile, FileAttributes.Normal);
            }
            Directory.Delete(f, true);
        }

        SceneManager.LoadScene("Intro");
    }
    public IEnumerator Close()
    {
        while (true)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                if (SceneManager.loadedSceneCount == 1)
                {
                    yield return StartCoroutine(StartCountNClose());
                }
            }
            yield return new WaitForNextFrameUnit();
        }
    }
    public IEnumerator StartCountNClose()
    {
        float tme = 0;
        bool cnt = true;
        while(tme<3 && SceneManager.loadedSceneCount == 1)
        {
            if (!Input.GetKey(KeyCode.Escape))
            {
                cnt = false;
                break;
            }
            yield return new WaitForSeconds(0.1f);
            tme += 0.1f;
        }
        if (cnt)
        {
            Debug.Log("quit");
            Application.Quit();
        }
    }
    public void LeaveSettings()
    {
        //F Unity
        c.GetComponent<UiManager>().SaveSettings();
    }
    public static void ReadSet()
    {
        if (!File.Exists(Path.Combine(Application.persistentDataPath + "/settings.json")))
        {
            MouseController.sensivity = 400;
            Directory.CreateDirectory(Path.GetDirectoryName(Application.persistentDataPath + "/settings.json"));
            SettingsData data = new()
            {
                render = 2,
                sens = 40,
                hud = true,
                showfps = false
            };
            hud = true;

            string json = JsonUtility.ToJson(data);
            File.WriteAllText(Path.Combine(Application.persistentDataPath + "/settings.json"), json);
        }
        else
        {
            string json = File.ReadAllText(Path.Combine(Application.persistentDataPath + "/settings.json"));
            SettingsData data = JsonUtility.FromJson<SettingsData>(json);
            Voxeldata.showfps = data.showfps;
            MouseController.sensivity = data.sens*10;
            Voxeldata.NumberOfChunks = data.render;
                hud = data.hud;
            fps = data.showfps;
        }
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void LoadPartiallyScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName,LoadSceneMode.Additive);
    }
    public void EnterUIScene()
    {
        SceneManager.UnloadSceneAsync("Acts");
    }
    public void OpenSet(string sceneName)
    {
        scene = sceneName;
        SceneManager.LoadScene("Settings", LoadSceneMode.Additive);
    }
    private void LoadSettingsScene()
    {
        string settingsPath = Path.Combine(Application.persistentDataPath + "/settings.json");
        if (Voxeldata.PlayerData.scene == 2)
        {
            backGround.color = Color.black;
        }
        if (SceneManager.GetActiveScene().name == "UIScene")
        {
            numOfClicks = 0;
            D.gameObject.SetActive(true);
            E.gameObject.SetActive(true);
        }
        if (!File.Exists(settingsPath))
        {
            MouseController.sensivity = 400;
            Directory.CreateDirectory(Path.GetDirectoryName(settingsPath));

            SettingsData data = new SettingsData()
            {
                render = 2,
                sens = 40,
                hud = true,
                totalsound = true,
                showfps = false,
                movementlevel = 80,
                musiclevel = 80
            };

            hud = true;
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(settingsPath, json);
            showfps.isOn = false;
            disSlider.value = data.render;
            sensivslid.value = data.sens;
            musicSlider.value=data.musiclevel;
            soundsSlider.value=data.movementlevel;
            hudqm.isOn = data.hud;
            audioqm.isOn=data.totalsound;
            renderdis.text=("Render  distance  "+data.render).ToString();
            sensiv.text = ("Mouse  sensivity  " + data.sens).ToString();
            musictext.text=("Music   volume  "+data.musiclevel).ToString();
            soundtext.text = ("Sound   volume  " + data.movementlevel).ToString();

        }
        else
        {
            string json = File.ReadAllText(settingsPath);
            SettingsData data = JsonUtility.FromJson<SettingsData>(json);

            MouseController.sensivity = 10 * data.sens;
            Voxeldata.NumberOfChunks = data.render;
            hud = data.hud;
            disSlider.value = data.render;
            sensivslid.value = data.sens;
            musicSlider.value = data.musiclevel;
            soundsSlider.value = data.movementlevel;
            hudqm.isOn = data.hud;
            audioqm.isOn = data.totalsound;
            showfps.isOn = data.showfps;
            renderdis.text = ("Render  distance  " + data.render).ToString();
            sensiv.text = ("Mouse  sensivity  " + data.sens).ToString();
            musictext.text = ("Music   volume  " + data.musiclevel).ToString();
            soundtext.text = ("Sound   volume  " + data.movementlevel).ToString();
        }
    }
    public void SaveSettings()
    {
        SettingsData data = new()
        {
            render = (byte)(disSlider.value),
            sens = (byte)(sensivslid.value),
            hud = hudqm.isOn,
            showfps = showfps.isOn,
            totalsound = audioqm,
            movementlevel = (byte)soundsSlider.value,
            musiclevel = (byte)musicSlider.value
        };
        hud=hudqm.isOn;
        fps=showfps.isOn;
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Path.Combine(Application.persistentDataPath + "/settings.json"), json);

        if (scene.Contains("World"))
        {
            SoundsManager.instance.UpdateSounds();
            SceneManager.UnloadSceneAsync("Settings");
            ChunkSerializer.CloseSet();

        }
        else
            SceneManager.UnloadSceneAsync("Settings");
    }
    public void SoundSet()
    {
        a.gameObject.SetActive(false);
        b.gameObject.SetActive(true);
        d.gameObject.SetActive(false);
        e.gameObject.SetActive(false);
    }
    public void TotalSettings()
    {
        a.gameObject.SetActive(false);
        b.gameObject.SetActive(false);
        d.gameObject.SetActive(true);
        e.gameObject.SetActive(false);
    }
    public void SmallSett()
    {
        a.SetActive(false);
        b.SetActive(false);
        d.SetActive(false);
        e.SetActive(true);
    }
    public void NormalSet()
    {
        a.SetActive(true);
        b.SetActive(false);
        d.gameObject.SetActive(false);
        e.gameObject.SetActive(false);
    }
    public void UpdateRend()
    {
        renderdis.text= "Render  distance  "+disSlider.value.ToString();
    }
    public void UpdateSpd()
    {
        sensiv.text= "Mouse  sensivity  "+sensivslid.value.ToString();
    }
    public void UpdateMusic()
    {
        musictext.text = "Music   volume  " + musicSlider.value.ToString();
    }
    public void UpdateAudio()
    {
        soundtext.text = "Audio   volume  " + soundsSlider.value.ToString();
    }
}

public class SettingsData
{
    public byte render=2;
    public byte sens = 40;
    public bool hud=true;
    public bool totalsound = true;
    public byte movementlevel = 50;
    public byte musiclevel = 50;
    public bool showfps = false;
}
