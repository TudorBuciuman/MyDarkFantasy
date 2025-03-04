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
    public AudioClip[] chorus = new AudioClip[3];
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
        if (Voxeldata.PlayerData.genocide)
        {
            switch (Voxeldata.PlayerData.scene)
            {
                case 0:
                    SceneLoc = "Fighting";
                    break;
                case 1:
                    SceneLoc = "Falling";
                    break;
                case 2:
                    SceneLoc = "Rising";
                    startingclip[2] = startingclip[1];
                    break;
                case 3:
                    SceneLoc = "Becoming";
                    slow = false;
                    startingclip[3] = clip[3];
                    break;
                case 4:
                    SceneLoc = "Judgement";
                    break;
            }
        }
        else
        {
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
                    slow = true;
                    StartCoroutine(Searching());
                    break;
                case 4:
                    SceneLoc = "Finding";
                    break;
            }
        }
        
        Read(SceneLoc);
    }
    public void Read(string s)
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        audioSource.loop = false;
        if (Voxeldata.PlayerData.scene != 2 && !Voxeldata.PlayerData.genocide)
        {
            audioSource.clip = startingclip[Voxeldata.PlayerData.scene];
            audioSource.Play();
        }
        else if(!Voxeldata.PlayerData.genocide)
        {
            StartCoroutine(LoadAndPlayMusic(7));
        }
        else if(Voxeldata.PlayerData.scene == 1 && Voxeldata.PlayerData.genocide)
        {
            StartCoroutine(LoadAndPlayMusic(7));
        }
        else if (Voxeldata.PlayerData.genocide)
        {
            audioSource.clip = startingclip[Voxeldata.PlayerData.scene];
            audioSource.Play();
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
    public IEnumerator Searching()
    {
        yield return new WaitForSeconds(105);
        audioSource.clip = clip[3];
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
            if(dialogueLines[currentLine][1] == '1')
            yield return StartCoroutine(YesOrYesButtons());
            currentLine++;
        }
        if (dialogueLines[currentLine][0] == '>')
        {
            float f = (float)(dialogueLines[currentLine][1]-'0')+(float)((dialogueLines[currentLine][2]-'0')/10.0f);
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
            yield return null;
        }
    }

    public IEnumerator BeforeQuit()
    {
        int c = Voxeldata.PlayerData.scene;
        if (Voxeldata.PlayerData.genocide)
        {
            if (c == 1)
                c = 2;
            else if (c == 2)
                c = 1;
        }
        switch (c) {
            case 0:
                {
                    audioSource.Stop();
                    dialogueTextUI.text = string.Empty;
                    yield return new WaitForSeconds(3.5f);
                    audioSource.clip = chorus[0];
                    audioSource.Play();
                    yield return new WaitForSeconds(27.5f);
                    audioSource.clip = clip[0];
                    audioSource.time = 20.2f;
                    audioSource.Play();
                    yield return new WaitForSeconds(70);
                    FightingImg.sprite = sprites[0];
                    FightingImg.gameObject.SetActive(true);
                    yield return new WaitForSeconds(30);
                    //StartCoroutine(ExtremeGlitchVisualizer.inst.upd());
                    PlayerDataData.SawIntro = true;
                    FightingImg.gameObject.SetActive(false);
                    yield return new WaitForSeconds(215);
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
            case 3:
                {
                    dialogueTextUI.text = string.Empty;
                    FightingImg.sprite = sprites[3];
                    FightingImg.gameObject.SetActive(true);
                    yield return new WaitForSeconds(10);
                    PlayerDataData.SavePlayer();
                    break;
                }
    }
    }

    public IEnumerator YesOrYesButtons()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        YesOrYes.SetActive(true);
        yield return new WaitForSeconds(3);
        YesOrYes.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

}
