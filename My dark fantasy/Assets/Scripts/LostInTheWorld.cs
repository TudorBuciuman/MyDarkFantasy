using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LostInTheWorld : MonoBehaviour
{
    public Material water,m0,m1,m2,m3,m4;
    public WorldManager wm;
    public Text loveTxt;
    public static LostInTheWorld instance;
    public static bool intro=true;
    void Awake()
    {
        intro=!Voxeldata.PlayerData.enteredWorld;
        water.SetColor("_AmbientLight", Color.white);
        instance = this;
        if (intro)
        {
            wm.material = m0;
            m1 = m2 = m3 = m4 = null;
            StartCoroutine(PlayFallenDown());
        }
        else
        {
            switch (Voxeldata.PlayerData.scene)
            {
                case 0:
                    wm.material = m0;
                    m1 = m2 = m3 = m4 = null;
                    break;
                case 1:
                    wm.material = m1;
                    m0 = m2 = m3 = m4 = null;
                    break;
                case 2:
                    wm.material = m2;
                    m0 = m1 = m3 = m4 = null;
                    break;
                case 3:
                    wm.material = m3;
                    m0 = m1 = m2 = m4 = null;
                    break;
                case 4:
                    wm.material = m4;
                    m0 = m1 = m2 = m3 = null;
                    break;
            }
        }
        if (Voxeldata.PlayerData.love == 0)
        {
            Voxeldata.PlayerData.love = 1;
            PlayerDataData.SavePlayerFile();
        }
        loveTxt.text = Voxeldata.PlayerData.love.ToString();

    }
    public IEnumerator PlayFallenDown()
    {
        yield return new WaitForSeconds(2);
        SoundsManager.instance.StartCoroutine(SoundsManager.instance.PlaySongByName("Fallen-Down"));
        instance.StartCoroutine(instance.Waiting());
    }
    public IEnumerator Waiting()
    {
        yield return new WaitForSeconds(15);
        Voxeldata.PlayerData.enteredWorld = true;
        PlayerDataData.SavePlayerFile();
    }
}
