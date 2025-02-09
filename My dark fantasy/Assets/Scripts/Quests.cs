using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Quests : MonoBehaviour
{
    public GameObject[] quest;
    public ScrollRect scrollRect;
    public GameObject slider,slgen;
    public GameObject pacifist, gen;
    public void Start()
    {
        int y = Voxeldata.PlayerData.scene;
        if (Voxeldata.PlayerData.genocide)
        {

            gen.SetActive(true);
            scrollRect.content = gen.GetComponent<RectTransform>();
            pacifist.SetActive(false);
            for (int i = 5; i <= y+5; i++)
            {
                quest[i].SetActive(true);
            }
            Vector3 a = quest[y].transform.localPosition;
            a.y = 0;
            a.x = -a.x;
            a.z = 0;
            slgen.transform.localPosition = a;
        }
        else
        {
            for (int i = 0; i <= y; i++)
            {
                quest[i].SetActive(true);
            }
            Vector3 a = quest[y].transform.localPosition;
            a.y = 0;
            a.x = -a.x;
            a.z = 0;
            slider.transform.localPosition = a;
        }
    }
    public void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.UnloadSceneAsync("Acts");
        }
    }

}
