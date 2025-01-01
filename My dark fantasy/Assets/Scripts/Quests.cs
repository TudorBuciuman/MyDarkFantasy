using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Quests : MonoBehaviour
{
    public GameObject[] quest;
    public GameObject slider;
    public void Start()
    {
        int y = Voxeldata.PlayerData.scene;
        for (int i=0; i<=y; i++){
            quest[i].gameObject.SetActive(true);
        }
        Vector3 a = quest[y].transform.localPosition;
        a.y = 0;
        a.x = -a.x;
        a.z = 0;
        slider.transform.localPosition = a;
    }


}
