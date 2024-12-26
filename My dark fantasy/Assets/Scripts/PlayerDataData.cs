using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDataData : MonoBehaviour
{
    public static string location = "PlayerSave.json";
    public static bool SawIntro=false;
    public void Start()
    {
        ReadFile();
    }

    public void ReadFile()
    {
        string settingsPath = Path.Combine(Application.persistentDataPath,location);
        if (File.Exists(settingsPath))
        {
            string json = File.ReadAllText(settingsPath);
            DontForget data = JsonUtility.FromJson<DontForget>(json);
            Voxeldata.PlayerData=data;
            SawIntro = data.sawIntro;
            if (SawIntro)
            {
                StartCoroutine(Play());
            }
        }
        else
        {
            DontForget playerData = new()
            {
                scene = 0,
                sawIntro = false,
                deaths = 0,
                typeofrun = 0,
            };
            Voxeldata.PlayerData = playerData;
            string jsonString = JsonUtility.ToJson(playerData, true);
            string filePath = Path.Combine(Application.persistentDataPath, location);
            File.WriteAllText(filePath, jsonString);
        }

    }
    public static IEnumerator Play()
    {
        yield return new WaitForSeconds(6);
        SceneManager.LoadScene("UIScene");
    }
    public static void Intro_Fighting()
    {
        DontForget playerData = new()
        {
           scene=0,
           sawIntro = false,
           typeofrun=0,
        };

        string jsonString = JsonUtility.ToJson(playerData, true);
        string filePath = Path.Combine(Application.persistentDataPath,location);
        File.WriteAllText(filePath, jsonString);
        SceneManager.LoadScene("Fighting");
    }
    public static void SavePlayer()
    {
        DontForget playerData = new()
        {
            scene = 0,
            sawIntro = true,
            deaths=0,
            typeofrun = 0,
        };

        string jsonString = JsonUtility.ToJson(playerData, true);
        string filePath = Path.Combine(Application.persistentDataPath, location);
        File.WriteAllText(filePath, jsonString);
        SceneManager.LoadScene("Intro");
    }
    public static void SavePlayerFile()
    {
        string jsonString = JsonUtility.ToJson(Voxeldata.PlayerData, true);
        string filePath = Path.Combine(Application.persistentDataPath, location);
        File.WriteAllText(filePath, jsonString);
    }
}
public class DontForget
{
    public byte scene;
    public int deaths;
    public bool sawIntro;
    public byte typeofrun;
    public byte[] action;
}