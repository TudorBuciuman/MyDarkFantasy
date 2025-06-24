using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BloodOnTheLeaves : MonoBehaviour
{
    public Image lightingImg;
    public AudioClip[] clip=new AudioClip[10];
    public Image FightingImg;
    public Sprite[] sprites = new Sprite[5];
    public AudioSource audioSource;
    public TextAsset dialogueFile;
    public Text dialogueTextUI;
    public string[] dialogueLines;
    public static string SceneLoc = "Blood on the leaves";
    public static byte SceneNum = 0;
    int currentLine = 0;
    bool slow = false, isWhite = false;
    private float slowspeed = 0.14f;
    private float fastspeed = 0.09f;
    private float waitTime = 1.4f;

    void Start()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        switch (SceneNum)
        {
            case 0:
                Voxeldata.PlayerData.special = 1;
                PlayerDataData.SavePlayerFile();
                slowspeed = 0.16f;
                slow = true;
                fastspeed = 0.09f;
                SceneLoc = "Blood on the leaves";
                StartCoroutine(Blood());
                break;
            case 1:
                SceneLoc = "Insomnia";
                break;
            case 2:
                SceneLoc = "To fight";
                break;
            case 3:
                SceneLoc = "Letting everything go";
                slow=false;
                break;
            case 4:
                SceneLoc = "Guilt trip";
                StartCoroutine(LoadAndPlayMusic(9));
                break;
            case 5:
                SceneLoc = "Fear";
                slow = true;
                audioSource.clip = clip[1];
                audioSource.Play();
                break;
            case 6:
                SceneLoc = "The winner takes it all";
                slow = false;
                audioSource.clip = clip[2];
                audioSource.Play();
                break;
            case 7:
                SceneLoc = "Cold";
                slow = false;
                audioSource.clip = clip[4];
                audioSource.Play();
                break;
        }
        Read(SceneLoc);
    }

    public IEnumerator TheDreamer()
    {
        audioSource.clip = clip[6];
        audioSource.Play();
        yield return new WaitForSeconds(170);
        audioSource.clip = clip[7];
        audioSource.Play();
    }
    public IEnumerator Blood()
    {
        audioSource.clip = clip[0];
        audioSource.loop = false;
        yield return new WaitForSeconds(4);
        audioSource.Play();
        yield return null;
    }
    public IEnumerator MortalMan()
    {
        audioSource.clip = clip[8];
        audioSource.Play();
        waitTime = 0.4f;
        yield return new WaitForSeconds(30);
        audioSource.clip = clip[9];
        audioSource.loop = true;
        audioSource.Play();

    }
    public void Read(string s)
    {
        dialogueFile = Resources.Load<TextAsset>($"Dialogues/{s}");
        dialogueLines = dialogueFile.text.Split('\n');
        for (int i = 0; i < dialogueLines.Length; i++)
        {
            dialogueLines[i] = dialogueLines[i].Replace("\\n ", "\n");
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine(DisplayNextLine());

    }

    public IEnumerator Lighting()
    {
        if (!isWhite)
            yield return StartCoroutine(MakeLight(0.5f));
        else
            yield return StartCoroutine(MakeDark(0.5f));
        isWhite = !isWhite;
    }
    public IEnumerator DisplayNextLine()
    {
        if (currentLine < dialogueLines.Length-1)
        {
            if (dialogueLines[currentLine][0] == '%')
            {
                currentLine++;
                slow = !slow;
            }

            while (dialogueLines[currentLine][0] == '[')
            {
                slow = true;
                currentLine++;
            }
            while (dialogueLines[currentLine][0] == '(')
            {
                currentLine++;
            }
            if (dialogueLines[currentLine][0] == '>')
            {
                float f = (float)(dialogueLines[currentLine][1] - '0') + (float)((dialogueLines[currentLine][2] - '0') / 10.0f);
                yield return new WaitForSeconds(f);
                currentLine++;
                StartCoroutine(DisplayNextLine());
                yield break;
            }
            else if (dialogueLines[currentLine][0] == '@')
            {
                yield return StartCoroutine(Lighting());
                currentLine++;
                StartCoroutine(DisplayNextLine());
                yield break;
            }
            else if (dialogueLines[currentLine][0] == '{')
            {
                if (dialogueLines[currentLine][1] == 's')
                {
                    audioSource.clip = clip[5];
                    audioSource.Play();
                }
                currentLine++;
                StartCoroutine(DisplayNextLine());
                yield break;
            }
            else if (dialogueLines[currentLine][0] == '$')
            {
                StartCoroutine(BeforeQuit());
                yield break;
            }

            else if (currentLine < dialogueLines.Length)
            {
                if (slow)
                    yield return StartCoroutine(TypeLine(dialogueLines[currentLine].Trim(), slowspeed));
                else
                {
                    yield return StartCoroutine(TypeLine(dialogueLines[currentLine].Trim(), fastspeed));
                }
            }
        }
        else
        {
            PlayerDataData.SavePlayerFile();
            yield return new WaitForSeconds(5);
            SceneManager.LoadScene("Intro");
            if (SceneNum == 0)
            {
                Voxeldata.PlayerData.special = 0;
                PlayerDataData.SavePlayerFile();
                yield return new WaitForSeconds(5);
                SceneManager.LoadScene("Intro");
            }
            else if (SceneNum == 2)
            {
                Voxeldata.PlayerData.special = 11;
                PlayerDataData.SavePlayerFile();
                yield return new WaitForSeconds(5);
                SceneManager.LoadScene("Fight");
            }
            else
            {
                yield return new WaitForSeconds(5);
                Voxeldata.PlayerData.special = 0;
                PlayerDataData.SavePlayerFile();
                SceneManager.LoadScene("Intro");
            }

        }
    }
    private IEnumerator TypeLine(string line, float spd)
    {
        dialogueTextUI.text = "";
        foreach (char c in line)
        {
            dialogueTextUI.text += c;
            yield return new WaitForSeconds(spd); //typing speed, big=slow
        }
        currentLine++;
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(DisplayNextLine());

    }
    public IEnumerator MakeLight(float time)
    {
        Color startColor = Color.black;

        Color targetColor = Color.white;

        float elapsedTime = 0f;

        while (elapsedTime < time / 2)
        {
            lightingImg.color = Color.Lerp(startColor, targetColor, elapsedTime / time);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //lightingImg.color = targetColor;
    }
    public IEnumerator MakeDark(float time)
    {

        Color startColor = lightingImg.color;

        Color targetColor = Color.black;

        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            lightingImg.color = Color.Lerp(startColor, targetColor, elapsedTime / time);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        lightingImg.color = targetColor;
    }
    public IEnumerator BeforeQuit()
    {
        if (SceneNum == 5)
        {
            audioSource.clip = clip[2];
            audioSource.Play();
        }
        if (SceneNum == 0)
        {
            Voxeldata.PlayerData.special = 0;
            PlayerDataData.SavePlayerFile();
        }
        yield return new WaitForSeconds(10);

        SceneManager.LoadScene("Intro");
    }
    public IEnumerator GetInput()
    {
        while (true)
        {
            if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
            {
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    private IEnumerator LoadAndPlayMusic(byte id)
    {
        string musicFilePath;
        string path = Path.Combine(Application.streamingAssetsPath);
#if UNITY_STANDALONE_WIN
        //string[] musicFiles = Directory.GetFiles(path, "*.ogg");
        musicFilePath = Path.Combine(Application.streamingAssetsPath+ $"/Songs/song{id}.ogg");
#elif UNITY_ANDROID
        musicFilePath = Application.streamingAssetsPath + $"/Songs/song{id}.ogg";
        if (!musicFilePath.StartsWith("jar:file://"))
        {
            musicFilePath = "jar:file://" + musicFilePath;
        }
#endif

        using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(musicFilePath, AudioType.OGGVORBIS);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error loading audio file: " + musicFilePath);
        }
        else
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }

    }

}
