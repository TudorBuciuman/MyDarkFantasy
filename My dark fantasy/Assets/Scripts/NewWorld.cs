using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class NewWorld : MonoBehaviour
{
    public int random;
    public InputField name;
    public Image img;
    public Text Text;
    public Button create;
    public Button later;
    public InputField seed;
    public GameObject world;
    public GameObject worldParent;
    public void Start()
    {
        Debug.Log("start");
        string directoryPath = Path.Combine(Application.dataPath, "MyDarkFantasy/worlds");
        Debug.Log(directoryPath + " " + Directory.Exists(directoryPath));
        if (Directory.Exists(directoryPath))
        {
            string[] files = Directory.GetDirectories(directoryPath);
            Debug.Log(files.Length+" " + files[0]);
            foreach (string file in files)
            {
                string[] smlfiles = Directory.GetFiles(file, "*.info");
                string content = File.ReadAllText(smlfiles[0]);
                string[] words = content.Split(new char[] { ' ', '\n', '\r', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
                Debug.Log(words[1] + " " + words[3]);
                GameObject go = Instantiate(world,worldParent.transform);
                go.GetComponentInChildren<Text>().text =words[1].ToString();
            }
        }
    }
    public void TryNewWorld()
    {
        Text.gameObject.SetActive(true);
        name.gameObject.SetActive(true);
        img.gameObject.SetActive(true);
        create.gameObject.SetActive(true);
        later.gameObject.SetActive(true);
        seed.gameObject.SetActive(true);
        random = Random.RandomRange(0, 1000000000);
        seed.text = random.ToString();
        Debug.Log("gata");
    }
    public void Later()
    {
        Text.gameObject.SetActive(false);
        name.text = "MyWorld";
        name.gameObject.SetActive(false);
        img.gameObject.SetActive(false);
        create.gameObject.SetActive(false);
        later.gameObject.SetActive(false);
        seed.gameObject.SetActive(false);
    }
    public void Create()
    {
        random = Random.RandomRange(0, 1000000000);
        if (name.Equals(""))
        {
            byte p = 1;
            name.text = ("MyWorld" +p).ToString();
            while (File.Exists(Path.Combine(Application.dataPath, "MyDarkFantasy/worlds"+name.text)))
            {
                p++;
                name.text = ("MyWorld" + p).ToString();
            }
            name.text = ("MyWorld"+p).ToString();
        }
        if(seed.Equals(""))
        seed.text=random.ToString();
        string savePath = Path.Combine(Application.dataPath, "MyDarkFantasy/worlds",name.text);
        if(!File.Exists(savePath))
        Directory.CreateDirectory(savePath);
        else
        {
            byte p = 1;
            savePath = (savePath+ p).ToString();
            while (File.Exists(Path.Combine(Application.dataPath, "MyDarkFantasy/worlds" ,name.text)))
            {
                p++;
                savePath = ("MyDarkFantasy/worlds/" + p).ToString();
            }
            savePath = (Path.Combine(Application.dataPath, "MyDarkFantasy/worlds/" + p)).ToString();
            Directory.CreateDirectory(savePath);
        }
        BinaryFormatter formatter = new BinaryFormatter();


        // Write additional information to a text file
        using (StreamWriter writer = new StreamWriter(savePath+"/world.info"))
        {
            writer.WriteLine("Name: " + name.text);
            writer.WriteLine("Seed: " + seed.text);
            // Add more information as needed
        }
        GameObject go = Instantiate(world, worldParent.transform);
        go.GetComponentInChildren<Text>().text = name.text.ToString();
        Later();
    }
    public void EnterWorld()
    {

    }
}
