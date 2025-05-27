using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDataData : MonoBehaviour
{
    public static string location = "PlayerSave.dat";
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
            try
            {
                byte[] compressedData = File.ReadAllBytes(settingsPath);

                using (MemoryStream memoryStream = new MemoryStream(compressedData))
                {
                    using (GZipStream decompressionStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                    {
                        using (MemoryStream resultStream = new MemoryStream())
                        {
                            decompressionStream.CopyTo(resultStream);
                            byte[] decompressedData = resultStream.ToArray();

                            string jsonData = System.Text.Encoding.UTF8.GetString(decompressedData);

                            DontForget playerData = JsonUtility.FromJson<DontForget>(jsonData);
                            Voxeldata.PlayerData = playerData;
                        }
                    }
                }
            }
            catch
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
                string filePath = Path.Combine(Application.persistentDataPath, location);

                try
                {
                    string jsonData = JsonUtility.ToJson(playerData, true);

                    byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

                    using FileStream fileStream = new(filePath, FileMode.Create);
                    using GZipStream compressionStream = new(fileStream, CompressionMode.Compress);
                    compressionStream.Write(dataBytes, 0, dataBytes.Length);
                }
                catch
                {
                    File.Delete(filePath);
                }
            }
            SawIntro = Voxeldata.PlayerData.sawIntro;
            if (SawIntro)
            {
                StartCoroutine(Play());
            }
            else
            {
                if(!Voxeldata.PlayerData.NeedsToSeeCredits)
                StartCoroutine(OtherScenes());
                else
                {
                    SceneManager.LoadScene("EndCredits");
                }
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
            SawIntro = Voxeldata.PlayerData.sawIntro;
            string filePath = Path.Combine(Application.persistentDataPath, location);

            try
            {
                string jsonData = JsonUtility.ToJson(playerData, true);

                byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

                using FileStream fileStream = new(filePath, FileMode.Create);
                using GZipStream compressionStream = new(fileStream, CompressionMode.Compress);
                compressionStream.Write(dataBytes, 0, dataBytes.Length);
            }
            catch
            {
                File.Delete(filePath);
            }
        }
        intro.Starting();
    }
    public static IEnumerator Play()
    {
        yield return new WaitForSeconds(6.3f);
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

        
        string filePath = Path.Combine(Application.persistentDataPath,location);
        try
        {
            string jsonData = JsonUtility.ToJson(playerData, true);

            byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

            using FileStream fileStream = new(filePath, FileMode.Create);
            using GZipStream compressionStream = new(fileStream, CompressionMode.Compress);
            compressionStream.Write(dataBytes, 0, dataBytes.Length);
        }
        catch
        {
            throw;
        }
        SceneManager.LoadScene("Fighting");
    }
    public static IEnumerator OtherScenes()
    {
        DontForget playerData=Voxeldata.PlayerData;
        if (playerData.scene < 5)
        {
            playerData.sawIntro = false;
            
            string filePath = Path.Combine(Application.persistentDataPath, location);
            try
            {
                string jsonData = JsonUtility.ToJson(playerData, true);

                byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

                using FileStream fileStream = new(filePath, FileMode.Create);
                using GZipStream compressionStream = new(fileStream, CompressionMode.Compress);
                compressionStream.Write(dataBytes, 0, dataBytes.Length);
            }
            catch
            {
                throw;
            }
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
        string filePath = Path.Combine(Application.persistentDataPath, location);
        try
        {
            string jsonData = JsonUtility.ToJson(Voxeldata.PlayerData, true);

            byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
            using FileStream fileStream = new(filePath, FileMode.Create);
            using GZipStream compressionStream = new(fileStream, CompressionMode.Compress);
            compressionStream.Write(dataBytes, 0, dataBytes.Length);
        }
        catch
        {
            throw;
        }
        SceneManager.LoadScene("Intro");
    }
    public static void SavePlayerFile()
    {
        string filePath = Path.Combine(Application.persistentDataPath, location);
        try
        {
            string jsonData = JsonUtility.ToJson(Voxeldata.PlayerData, true);

            byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

            using FileStream fileStream = new(filePath, FileMode.Create);
            using GZipStream compressionStream = new(fileStream, CompressionMode.Compress);
            compressionStream.Write(dataBytes, 0, dataBytes.Length);
        }
        catch
        {
            throw;
        }
    }
}
public class DontForget
{
    public byte scene;
    public int deaths;
    public bool sawIntro;
    public byte typeofrun;
    public bool SawEnding;
    public bool enteredWorld;
    public byte timesSlept;
    public byte currentSeparateScene;
    public bool genocide;
    public bool pacifist;
    public bool NeedsToSeeCredits;
    public byte sleep;
    public byte special;
}