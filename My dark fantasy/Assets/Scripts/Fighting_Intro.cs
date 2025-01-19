using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Fighting_Intro : MonoBehaviour
{
    public Image lightingImg;
    public AudioClip[] clip = new AudioClip[6];
    public AudioClip[] startingclip=new AudioClip[5];
    public Image FightingImg;
    public Sprite[] sprites = new Sprite[5];
    public AudioSource audioSource;
    public TextAsset dialogueFile;
    public Text dialogueTextUI;
    public string[] dialogueLines;

    public static string SceneLoc = "Fighting";
    public int currentLine = 0;
    public bool isTyping = false,slow=false,isWhite=false;

    public GameObject YesOrYes;


    void Start()
    {
        Application.targetFrameRate = 60;

        switch (Voxeldata.PlayerData.scene)
        {
            case 0:
                SceneLoc = "Fighting";
                break;
            case 1:
                SceneLoc = "Rising";
                break;
            case 2:
                SceneLoc = "Falling";
                break;
            case 3:
                SceneLoc = "Searching";
                break;
            case 4:
                SceneLoc = "Finding";
                break;
        }
        Read(SceneLoc);
    }
    public void Read(string s)
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        audioSource.loop = false;

        if (Voxeldata.PlayerData.scene != 2)
        {
            audioSource.clip = startingclip[Voxeldata.PlayerData.scene];
            audioSource.Play();
        }
        else
        {
            StartCoroutine(LoadAndPlayMusic(7));
        }

        dialogueFile = Resources.Load<TextAsset>($"Dialogues/{s}");
        dialogueLines = dialogueFile.text.Split('\n');
        for (int i = 0; i < dialogueLines.Length; i++)
        {
            dialogueLines[i] = dialogueLines[i].Replace("\\n ", "\n");
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible=false;
        StartCoroutine(DisplayNextLine());
    }

    public IEnumerator Lighting()
    {
        if(!isWhite)
        yield return StartCoroutine(MakeLight(4.5f));
        else
        yield return StartCoroutine(MakeDark(4.5f));
        isWhite = !isWhite;
    }
    public void PlaySong(int n)
    {
        audioSource.clip = clip[n];
        audioSource.Play();
    }
    private IEnumerator LoadAndPlayMusic(byte id)
    {
        string musicFilePath;
        string path = Path.Combine(Application.streamingAssetsPath);
#if UNITY_STANDALONE_WIN
        //string[] musicFiles = Directory.GetFiles(path, "*.ogg");
        musicFilePath = Path.Combine(Application.streamingAssetsPath + $"/Songs/song{id}.ogg");
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

    public IEnumerator DisplayNextLine()
    {

        while (dialogueLines[currentLine][0] == '(')
        {
            currentLine++;
        }
        if (dialogueLines[currentLine][0] == '~')
        {
            currentLine++;
            slow = !slow;
        }
        if (dialogueLines[currentLine][0] == '{')
        {
            currentLine++;
            if(dialogueLines[currentLine][1] == '1')
            yield return StartCoroutine(YesOrYesButtons());
        }
        if (dialogueLines[currentLine][0] == '>')
        {
            float f = (float)(dialogueLines[currentLine][1]-'0')+(float)((dialogueLines[currentLine][2]-'0')/10.0f);
            yield return new WaitForSeconds(f); 
            currentLine++;
            StartCoroutine(DisplayNextLine());
            yield return null;
        }
        else if (dialogueLines[currentLine][0] == '@')
        {
            yield return StartCoroutine(Lighting());
            currentLine++;
            StartCoroutine(DisplayNextLine());
            yield return null;
        }
        else if (dialogueLines[currentLine][0] == '$')
        {
            StartCoroutine(BeforeQuit());
            yield return null;
        }
        else if (currentLine < dialogueLines.Length)
        {
            if (!slow)
                yield return StartCoroutine(TypeLine(dialogueLines[currentLine].Trim(), 0.07f));
            else
            {
                yield return StartCoroutine(TypeLine(dialogueLines[currentLine].Trim(), 0.1f));
            }
        }

    }
    private IEnumerator TypeLine(string line, float spd)
    {
        isTyping = true;
        dialogueTextUI.text = "";
        foreach (char c in line)
        {
            dialogueTextUI.text += c;            
            yield return new WaitForSeconds(spd); //typing speed, big=slow
        }
        currentLine++;
        yield return new WaitForSeconds(1.4f);
        StartCoroutine(DisplayNextLine());

    }
    public IEnumerator MakeLight(float time)
    {

        Color startColor = Color.black;

        Color targetColor = Color.white;

        float elapsedTime = 0f;

        while (elapsedTime < time/2){
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
    public IEnumerator GetInput()
    {
        while (true)
        {
            if(Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
            {
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public IEnumerator BeforeQuit()
    {
        switch (Voxeldata.PlayerData.scene) {
            case 0:
                {
                    audioSource.Stop();
                    dialogueTextUI.text = string.Empty;
                    yield return new WaitForSeconds(3.5f);
                    PlaySong(0);
                    yield return new WaitForSeconds(90);
                    FightingImg.sprite = sprites[0];
                    FightingImg.gameObject.SetActive(true);
                    yield return new WaitForSeconds(30);
                    PlayerDataData.SawIntro = true;
                    FightingImg.gameObject.SetActive(false);
                    yield return new WaitForSeconds(205);
                    PlayerDataData.SavePlayer();
                    break;
                }
            case 1:
                {
                    audioSource.Stop();
                    dialogueTextUI.text = string.Empty;
                    yield return new WaitForSeconds(3.5f);
                    PlaySong(1);
                    yield return new WaitForSeconds(71);
                    FightingImg.sprite = sprites[1];
                    FightingImg.gameObject.SetActive(true);
                    yield return new WaitForSeconds(30);
                    PlayerDataData.SawIntro = true;
                    FightingImg.gameObject.SetActive(false);
                    yield return new WaitForSeconds(35);
                    PlayerDataData.SavePlayer();
                    break;
                }
                case 2:
                {
                    audioSource.Stop();
                    dialogueTextUI.text = string.Empty;
                    StartCoroutine(LoadAndPlayMusic(6));
                    yield return new WaitForSeconds(75.5f);
                    FightingImg.sprite = sprites[2];
                    FightingImg.gameObject.SetActive(true);
                    yield return new WaitForSeconds(10);
                    FightingImg.gameObject.SetActive(false);
                    yield return new WaitForSeconds(39f);
                    PlayerDataData.SavePlayer();
                    break;
                }
    }
    }

    public IEnumerator YesOrYesButtons()
    {
        YesOrYes.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        YesOrYes.gameObject.SetActive(false);
    }

}
