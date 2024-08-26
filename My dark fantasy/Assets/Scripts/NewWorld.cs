using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewWorld : MonoBehaviour
{
    public int random;
    public static byte nr = 0;
    public ChunkSerializer serializer;
    public InputField Wname;
    public Image img;
    public Text Text;
    public Button create;
    public Button later;
    public InputField seed;
    public GameObject world;
    public GameObject worldParent;
    public void Start()
    {
        Application.targetFrameRate = 20;
        if (worldParent != null)
        {
            string directoryPath = Path.Combine(Application.dataPath, "MyDarkFantasy/worlds");
            if (Directory.Exists(directoryPath))
            {
                string[] files = Directory.GetDirectories(directoryPath);
                foreach (string file in files)
                {
                    string[] smlfiles = Directory.GetFiles(file, "*.info");
                    string content = File.ReadAllText(smlfiles[0]);
                    string[] words = content.Split(new char[] { ' ', '\n', '\r', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
                    GameObject go = Instantiate(world, worldParent.transform);
                    go.GetComponentInChildren<Text>().text = words[1].ToString();
                    nr++;
                }
            }
        }
    }
    public void TryNewWorld()
    {
        Text.gameObject.SetActive(true);
        Wname.gameObject.SetActive(true);
        img.gameObject.SetActive(true);
        create.gameObject.SetActive(true);
        later.gameObject.SetActive(true);
        seed.gameObject.SetActive(true);
        random = Random.Range(0, 1000000000);
        seed.text = random.ToString();
    }
    public void Later()
    {
        Text.gameObject.SetActive(false);
        Wname.text = "MyWorld";
        Wname.gameObject.SetActive(false);
        img.gameObject.SetActive(false);
        create.gameObject.SetActive(false);
        later.gameObject.SetActive(false);
        seed.gameObject.SetActive(false);
    }
    public void Create()
    {
        random = Random.Range(0, 1000000000);
        if (Wname.Equals(""))
        {
            byte p = 1;
            Wname.text = ("MyWorld" +p).ToString();
            while (File.Exists(Path.Combine(Application.dataPath, "MyDarkFantasy/worlds"+Wname.text)))
            {
                p++;
                Wname.text = ("MyWorld" + p).ToString();
            }
            Wname.text = ("MyWorld"+p).ToString();
        }
        if(seed.Equals(""))
        seed.text=random.ToString();
        string savePath = Path.Combine(Application.dataPath, "MyDarkFantasy/worlds",Wname.text);
        if(!File.Exists(savePath))
        Directory.CreateDirectory(savePath);
        else
        {
            byte p = 1;
            savePath = (savePath+ p).ToString();
            while (File.Exists(Path.Combine(Application.dataPath, "MyDarkFantasy/worlds" ,Wname.text)))
            {
                p++;
                savePath = ("MyDarkFantasy/worlds/" + p).ToString();
            }
            savePath = (Path.Combine(Application.dataPath, "MyDarkFantasy/worlds/" + p)).ToString();
            Directory.CreateDirectory(savePath);
        }
        BinaryFormatter formatter = new BinaryFormatter();
        using (StreamWriter writer = new StreamWriter(savePath+"/world.info"))
        {
            writer.WriteLine("Name: " + Wname.text);
            writer.WriteLine("Seed: " + seed.text);
        }
        GameObject go = Instantiate(world, worldParent.transform);
        go.GetComponentInChildren<Text>().text = Wname.text.ToString();
        Later();
    }
    public void EnterWorld()
    {
        GameObject[] gm = GameObject.FindGameObjectsWithTag("open");
        for(int i = 0; i<nr; i++)
        {
            if (gm[i].gameObject == this.gameObject)
            {
                string[] files = Directory.GetDirectories(Path.Combine(Application.dataPath, "MyDarkFantasy/worlds"));
                string[] smlfiles = Directory.GetFiles(files[i], "*.info");
                string content = File.ReadAllText(smlfiles[0]);
                string[] words = content.Split(new char[] { ' ', '\n', '\r', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
                serializer = new();
                serializer.Sync(Path.Combine(Path.Combine(Application.dataPath, "MyDarkFantasy/worlds"),files[i]), int.Parse(words[3]));
                break;
            } 
        }
        SceneManager.LoadScene("World");
    }

    public void EditWorld()
    {
        Debug.Log("edit");
    }
}
