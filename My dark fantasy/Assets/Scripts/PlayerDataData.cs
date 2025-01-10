using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDataData : MonoBehaviour
{
    public static string location = "PlayerSave.json";
    public static bool SawIntro=false;
    public Intro intro;
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
            else
            {
                StartCoroutine(OtherScenes());
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
                SawEnding = false,
            };
            Voxeldata.PlayerData = playerData;
            string jsonString = JsonUtility.ToJson(playerData, true);
            string filePath = Path.Combine(Application.persistentDataPath, location);
            File.WriteAllText(filePath, jsonString);
        }
        intro.Starting();
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
            scene = 0,
            sawIntro = false,
            deaths = 0,
            typeofrun = 0,
            SawEnding = false
        };

        string jsonString = JsonUtility.ToJson(playerData, true);
        string filePath = Path.Combine(Application.persistentDataPath,location);
        File.WriteAllText(filePath, jsonString);
        SceneManager.LoadScene("Fighting");
    }
    public static IEnumerator OtherScenes()
    {
        //only applies before beating the game(last scene)
        DontForget playerData=Voxeldata.PlayerData;
        if (playerData.scene < 5)
        {
            playerData.sawIntro = false;
            //playerData.scene++;
            string jsonString = JsonUtility.ToJson(playerData, true);
            string filePath = Path.Combine(Application.persistentDataPath, location);
            File.WriteAllText(filePath, jsonString);
            yield return new WaitForSeconds(6);
            SceneManager.LoadScene("Fighting");

        }
        //else
        //something

    }
    public static void SavePlayer()
    {
        Voxeldata.PlayerData.sawIntro = true;
        DontForget playerData = Voxeldata.PlayerData;

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
    public bool SawEnding;
    public byte timesSlept;
    public byte currentSeparateScene;
}