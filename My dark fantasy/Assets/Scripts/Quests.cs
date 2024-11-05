using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class Quests : MonoBehaviour
{
    public string FileLocation = Path.Combine(Application.dataPath + "Assets/Data/ImportantFiles/Quests.json");
    public string PlayerData = Path.Combine(Application.dataPath + "Assets/Data/ImportantFiles/QuestsCompleted.json");
    public void Start()
    {
        StartCoroutine(Read());
    }

    IEnumerator Read()
    {
        UnityWebRequest request = UnityWebRequest.Get(PlayerData);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
        }

    }
}

[System.Serializable]
public class Achievement
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsUnlocked { get; set; }

    public Achievement(int id, string name, string description)
    {
        ID = id;
        Name = name;
        Description = description;
        IsUnlocked = false;
    }
}
