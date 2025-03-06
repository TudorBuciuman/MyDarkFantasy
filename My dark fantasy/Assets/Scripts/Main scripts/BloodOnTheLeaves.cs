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
    public Image FightingImg,twistedImg,background;
    public Sprite[] sprites = new Sprite[5];
    public AudioSource audioSource;
    public TextAsset dialogueFile;
    public Text dialogueTextUI;
    public string[] dialogueLines;
    public static string SceneLoc = "Blood on the leaves";
    public static byte SceneNum = 4;
    int currentLine = 0;
    float waittime = 1.4f;
    bool slow = false, isWhite = false;

    void Start()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        switch (SceneNum)
        {
            case 0:
                SceneLoc = "Blood on the leaves";
                audioSource.clip = clip[0];
                waittime = 1.3f;
                audioSource.Play();
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
                waittime = 1.2f;
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
                audioSource.clip = clip[7];
                audioSource.Play();
                break;
            case 7:
                SceneLoc = "Cold";
                slow = false;
                audioSource.clip = clip[3];
                audioSource.Play();
                break;
            case 8:
                SceneLoc = "Gone";
                slow = false;
                audioSource.clip = clip[4];
                audioSource.Play();
                break;
            case 9:
                SceneLoc = "Never see me again";
                slow = false;
                audioSource.loop = true;
                waittime = 0.5f;
                StartCoroutine(MakeImageAppear(10,twistedImg, new Color32(171, 171, 171, 255)));
                StartCoroutine(MakeImageAppear(10,background,new Color32(148,111,49,255)));
                audioSource.clip = clip[5];
                audioSource.Play();
                break;
        }
        Read(SceneLoc);
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
        {
            if (SceneNum==0)
            {
                audioSource.clip = clip[6];
                audioSource.Play();
            }
            yield return StartCoroutine(MakeLight(0.5f));

        }
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
            else if (dialogueLines[currentLine][0] == '$')
            {
                StartCoroutine(BeforeQuit());
                yield break;
            }

            else if (currentLine < dialogueLines.Length)
            {
                if (slow)
                    yield return StartCoroutine(TypeLine(dialogueLines[currentLine].Trim(), 0.15f));
                else
                {
                    yield return StartCoroutine(TypeLine(dialogueLines[currentLine].Trim(), 0.0782f));
                }
            }
        }
        else
        {
            PlayerDataData.SavePlayerFile();
            yield return new WaitForSeconds(5);
            SceneManager.LoadScene("Intro");
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
        yield return new WaitForSeconds(waittime);
        StartCoroutine(DisplayNextLine());

    }
    public IEnumerator MakeImageAppear(float time,Image Img,Color32 clr)
    {
        Img.gameObject.SetActive(true);
        Color startColor = Img.color;

        Color targetColor = clr;

        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            Img.color = Color.Lerp(startColor, targetColor, elapsedTime / time);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
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
        else if (SceneNum == 9)
        {
            StartCoroutine(NormalRoute());
        }
            yield return new WaitForSeconds(10);
        SceneManager.LoadScene("Intro");
    }
    public IEnumerator NormalRoute()
    {
        audioSource.clip = clip[6];
        audioSource.loop = false;
        audioSource.Play();
        twistedImg.gameObject.SetActive(false);
        background.color = Color.black;
        yield return StartCoroutine(MakeLight(1));
        yield return new WaitForSeconds(1.2f);
        yield return StartCoroutine(MakeDark(8));
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
