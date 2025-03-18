using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LostInTheWorld : MonoBehaviour
{
    public Material Skybox,water;
    public Canvas canvas;
    public static LostInTheWorld instance;
    public static bool intro=true;
    void Awake()
    {
        intro=!Voxeldata.PlayerData.enteredWorld;
        if (intro)
        {
            instance = this;
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().enabled = false;
            canvas.enabled = false;
            RenderSettings.fog = false;
            RenderSettings.reflectionIntensity = 0.65f;
            RenderSettings.ambientLight = Color.black;
            RenderSettings.skybox = Skybox;
            BookManager.readingBook = true;
            water.SetColor("_AmbientLight", new Color(0f / 255f, 15f / 255f, 16f / 255f, 1f));
            SceneManager.LoadScene("OverlyDedicated", LoadSceneMode.Additive);
        }
        else
        water.SetColor("_AmbientLight", Color.white);

    }
    public static void Back()
    {
        SoundsManager.instance.StartCoroutine(SoundsManager.instance.PlaySongByName("A dark fantasy"));
        instance.StartCoroutine(instance.Waiting());
    }
    public IEnumerator Waiting()
    {
        yield return new WaitForSeconds(2);
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().enabled = true;

        yield return new WaitForSeconds(60);
        Voxeldata.PlayerData.enteredWorld = true;
        PlayerDataData.SavePlayerFile();
        yield return new WaitForSeconds(200);
        SceneManager.LoadScene("Intro");
    }
}
