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
    public GameObject a;
    public GameObject b;

    public Button gameset;
    public Button soundset;
    [Header("Basic Functions")]
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

    public void Start()
    {
        Application.targetFrameRate = 30;
        if (SceneManager.GetActiveScene().name == "Settings")
            LoadSettingsScene();
        else if (SceneManager.loadedSceneCount == 2)
            LoadSettingsScene();
    }

    public static void ReadSet()
    {
        if (!File.Exists(Path.Combine(Application.persistentDataPath + "/Settings/settings.json")))
        {
            MouseController.sensivity = 400;
            Directory.CreateDirectory(Path.GetDirectoryName(Application.persistentDataPath + "/Settings/settings.json"));
            SettingsData data = new()
            {
                render = 2,
                sens = 40,
                hud = true,
                showfps = false
            };
            hud = true;

            string json = JsonUtility.ToJson(data);
            File.WriteAllText(Path.Combine(Application.persistentDataPath + "/Settings/settings.json"), json);
        }
        else
        {
            string json = File.ReadAllText(Path.Combine(Application.persistentDataPath + "/Settings/settings.json"));
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
    public void OpenSet(string sceneName)
    {
        scene = sceneName;
        SceneManager.LoadScene("Settings", LoadSceneMode.Additive);

    }
    private void LoadSettingsScene()
    {
        // Now the scene is loaded, proceed with your logic
        string settingsPath = Path.Combine(Application.persistentDataPath, "Settings/settings.json");
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
        File.WriteAllText(Path.Combine(Application.persistentDataPath + "/Settings/settings.json"), json);

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
    }
    public void NormalSet()
    {
        a.gameObject.SetActive(true);
        b.gameObject.SetActive(false);
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
    public void Quit()
    {
        Application.Quit();
    }
    public void OpenSettings()
    {
        ;
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
