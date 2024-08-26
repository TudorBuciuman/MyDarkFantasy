using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class UiManager : MonoBehaviour
{
    public static string scene=null;

    [Header("Basic Functions")]
    public Text renderdis;
    public Slider disSlider;
    public Toggle hudqm;
    public static bool hud=true;

    public Text sensiv;
    public Slider sensivslid;

    [Header("Audio& soundtrack")]
    public Text audio;
    public Slider audioSlider;

    public void Start()
    {
        Application.targetFrameRate = 30;

    }

    public void Awake()
    {
        if(SceneManager.GetActiveScene().name=="Settings")
        LoadSettingsScene();
    }
    public void ReadSet()
    {
        if (!File.Exists(Path.Combine(Application.dataPath + "/Settings/settings.json")))
        {
            MouseController.sensivity = 400;
            Directory.CreateDirectory(Path.GetDirectoryName(Application.dataPath + "/Settings/settings.json"));
            SettingsData data = new()
            {
                render = 2,
                sens = 40,
                hud = true
            };
            hud = true;
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(Path.Combine(Application.dataPath + "/Settings/settings.json"), json);
        }
        else
        {
            string json = File.ReadAllText(Path.Combine(Application.dataPath + "/Settings/settings.json"));
            SettingsData data = JsonUtility.FromJson<SettingsData>(json);

            MouseController.sensivity = 10 * data.sens;
            Voxeldata.NumberOfChunks = data.render;
                hud = data.hud;
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void OpenSet(string sceneName)
    {
        scene = sceneName;
        SceneManager.LoadScene("Settings");

    }
    private void LoadSettingsScene()
    {
        // Now the scene is loaded, proceed with your logic
        string settingsPath = Path.Combine(Application.dataPath, "Settings/settings.json");
        if (!File.Exists(settingsPath))
        {
            MouseController.sensivity = 400;
            Directory.CreateDirectory(Path.GetDirectoryName(settingsPath));

            SettingsData data = new SettingsData()
            {
                render = 2,
                sens = 40,
                hud = true
            };

            hud = true;
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(settingsPath, json);

            disSlider.value = data.render;
            sensivslid.value = data.sens;
            hudqm.isOn = data.hud;
            renderdis.text=("Render  distance  "+data.render).ToString();
            sensiv.text = ("Mouse  sensivity  " + data.sens).ToString();

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
            hudqm.isOn = data.hud;
            renderdis.text = ("Render  distance  " + data.render).ToString();
            sensiv.text = ("Mouse  sensivity  " + data.sens).ToString();
        }
    }
    public void SaveSettings()
    {
        SettingsData data = new()
        {
            render = (byte)(disSlider.value),
            sens = (byte)(sensivslid.value),
            hud = hudqm.isOn
        };
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Path.Combine(Application.dataPath + "/Settings/settings.json"), json);
        if (!scene.Equals("")    )
        {
            SceneManager.LoadScene(scene);
            if (scene.Contains("World"))
            {
                ChunkSerializer.CloseSet();
            }
        }
        else
            SceneManager.LoadScene(0);
    }
    public void Renderdis()
    {

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
}
