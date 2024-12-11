using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Windows;
using Unity.VisualScripting;
using System.Linq;

public class NewWorld : MonoBehaviour
{
    public int random;
    public static byte nr = 0;
    public ChunkSerializer serializer;
    public string worldlocation;
    public InputField Wname;
    public Image img;
    public Text Text;
    public Button create;
    public Button later;
    public InputField seed;
    public GameObject world;
    public GameObject worldParent;

    public Image wset;
    public Button backup;
    public Button deleteqm;
    public GameObject deleteornot;
    public InputField changewname;
    public Button copywseed;
    public Button save;
    public void Start()
    {
        Application.targetFrameRate = 20;
        ReadFiles();
    }
    public void ReadFiles()
    {
        if (worldParent != null)
        {
            string directoryPath = Path.Combine(Application.persistentDataPath, "MyDarkFantasy/worlds");
            if (Directory.Exists(directoryPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                var directories = directoryInfo.GetDirectories()
                .AsParallel() // Parallel LINQ pentru multi-threading si performanta sporita
                .OrderByDescending(dir => dir.LastWriteTime)
                .ToArray();

                string[] files = Directory.GetDirectories(directoryPath);
                foreach (DirectoryInfo dir in directories)
                {

                    string[] smlfiles = Directory.GetFiles(dir.FullName, "*.info");

                    if (smlfiles.Length > 0)
                    {
                        string content = File.ReadAllText(smlfiles[0]);
                        string[] words = content.Split(new char[] { ' ', '\n', '\r', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);

                        GameObject go = Instantiate(world, worldParent.transform);

                        string a = null;
                        for (int j = 1; j < words.Length - 2; j++)
                        {
                            a += " " + words[j];
                        }
                        go.GetComponent<worldsmanager>().seed = int.Parse(words[words.Length - 1]);
                        go.GetComponent<worldsmanager>().Name = a;
                        go.GetComponent<worldsmanager>().location = dir.FullName;
                        go.GetComponentInChildren<Text>().text = a.ToString();
                    }
                    nr++;
                }
            }
            else
            {
                Directory.CreateDirectory(directoryPath);
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
        if (!Wname.text.Equals(""))
        {
            random = Random.Range(0, 1000000000);
            string a=Wname.text;
            if (Wname.text.Equals("MyWorld"))
            {
                byte p = 1;
                Wname.text = ("MyWorld" + p.ToString()).ToString();
                while (Directory.Exists(Path.Combine(Application.persistentDataPath, "MyDarkFantasy/worlds", Wname.text)))
                {
                    p++;
                    Wname.text = ("MyWorld" + p.ToString()).ToString();
                }
            }
            string h;
            if (seed.Equals(""))
                seed.text = random.ToString();
            string savePath = Path.Combine(Application.persistentDataPath, "MyDarkFantasy/worlds", Wname.text);
            if (!Directory.Exists(savePath))
            {
                h=Wname.text;
                Directory.CreateDirectory(savePath);
            }
            else
            {
                byte p = 1;
                savePath = Path.Combine(savePath, p.ToString()).ToString();
                while (File.Exists(savePath))
                {
                    p++;
                    savePath = Path.Combine(savePath, p.ToString()).ToString();
                }
                h=Wname.text+p;
                Directory.CreateDirectory(savePath);
            }
            BinaryFormatter formatter = new BinaryFormatter();
            using (StreamWriter writer = new StreamWriter(savePath + "/world.info"))
            {
                writer.WriteLine("Name: " + h);
                writer.WriteLine("Seed: " + seed.text);
            }
            serializer = new();
            serializer.Sync((savePath), int.Parse(seed.text));
            SceneManager.LoadScene("World");
        }
    }
    public void EnterWorld()
    {
        serializer = new();
        serializer.Sync(this.GetComponent<worldsmanager>().location, this.GetComponent<worldsmanager>().seed);
        SceneManager.LoadScene("World");
    }
    public void TryEditWorld() {
        GameObject a = GameObject.FindGameObjectWithTag("manager");
        a.GetComponent<NewWorld>().EditWorld(this.gameObject);    
    }
    public string worldLocation;
    public int SEED;
    public void EditWorld(GameObject c)
    {
        wset.gameObject.SetActive(true);
        backup.gameObject.SetActive(true);
        deleteqm.gameObject.SetActive(true);
        save.gameObject.SetActive(true);
        serializer = new();
        copywseed.gameObject.SetActive(true);
        changewname.gameObject.SetActive(true);
        copywseed.GetComponentInChildren<Text>().text = c.GetComponent<worldsmanager>().seed.ToString();
        string a = c.GetComponent<worldsmanager>().Name;
        worldLocation = c.GetComponent<worldsmanager>().location.ToString();
        SEED = c.GetComponent<worldsmanager>().seed;
        changewname.text = a.ToString();        
    }
    public void CloseEdit()
    {
        Debug.Log(changewname.text);
        BinaryFormatter formatter = new BinaryFormatter();
        using (StreamWriter writer = new StreamWriter(worldLocation + "/world.info"))
        {
            writer.WriteLine("Name: " + changewname.text);
            writer.WriteLine("Seed: " + SEED.ToString());
        }
        backup.gameObject.SetActive(false);
        deleteornot.gameObject.SetActive(false);
        deleteqm.gameObject.SetActive(false);
        save.gameObject.SetActive(false);
        changewname.gameObject.SetActive(false);
        copywseed.gameObject.SetActive(false);
        wset.gameObject.SetActive(false);
        foreach(GameObject a in GameObject.FindGameObjectsWithTag("open"))
            Destroy(a);
        ReadFiles();

    }
    public void TryDelete()
    {
        deleteornot.gameObject.SetActive(true);
    }
    public void canceldel()
    {
        deleteornot.gameObject.SetActive(false );
    }
    public void Deleteworld()
    {
        Directory.Delete(worldlocation, true);

        CloseEdit();
        SceneManager.LoadScene(1);
    }
    public void BackUpWorld()
    {
        if(!Directory.Exists(Path.Combine(Application.persistentDataPath, "MyDarkFantasy/backups"))){
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "MyDarkFantasy/backups"));
        }
        int y = 0;
        while (Directory.Exists(Path.Combine(Application.persistentDataPath, "MyDarkFantasy/backups/",y.ToString())))
        {
            y++;
        }
        Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "MyDarkFantasy/backups/", y.ToString()));

        //nu exista alt mod de a copia fisierele
        CopyDirectory(worldlocation, Path.Combine(Application.persistentDataPath, "MyDarkFantasy/backups/", y.ToString()));
    }
    static void CopyDirectory(string sourceDir, string destDir)
    {
        // Create the destination directory if it doesn't exist
        if (!Directory.Exists(destDir))
        {
            Directory.CreateDirectory(destDir);
        }

        // Copy all files from the source to the destination
        foreach (string file in Directory.GetFiles(sourceDir))
        {
            string fileName = Path.GetFileName(file);
            string destFile = Path.Combine(destDir, fileName);
            File.Copy(file, destFile, true);  // true overwrites existing files
        }

        // Recursively copy subdirectories
        foreach (string subDir in Directory.GetDirectories(sourceDir))
        {
            string dirName = Path.GetFileName(subDir);
            string destSubDir = Path.Combine(destDir, dirName);
            CopyDirectory(subDir, destSubDir);
        }
    }
    public void SaveSeed()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            CopyToClipboardAndroid(copywseed.GetComponentInChildren<Text>().text.ToString());
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            GUIUtility.systemCopyBuffer = copywseed.GetComponentInChildren<Text>().text.ToString(); // For Windows and Editor
        }
    }
    private void CopyToClipboardAndroid(string text)
    {
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject clipboard = activity.Call<AndroidJavaObject>("getSystemService", "clipboard");
            AndroidJavaObject clipData = new AndroidJavaClass("android.content.ClipData").CallStatic<AndroidJavaObject>("newPlainText", "Copied Text", text);
            clipboard.Call("setPrimaryClip", clipData);
            Debug.Log("Copied to Clipboard (Android): " + text);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to copy to clipboard on Android: " + e.Message);
        }
    }
}
