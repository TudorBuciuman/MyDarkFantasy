using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MyDarkFantasy : MonoBehaviour
{
    public GameObject obj1, obj2, obj3, obj4, obj5;
    public bool obj4a, obj5a,crs;
    public GameObject heart, Fightscene,background;
    public GameObject aa;
    public GameObject[] d=new GameObject[8];
    public Text[] t=new Text[8];
    public Text hoal;
    public Image top;
    public AudioClip music,sound;
    public AudioSource source,soundsource;
    public string[] dialogueLines;
    public TextAsset dialogueFile;
    int currentLine = 0;
    float fastspeed = 0.08f;
    float slowspeed = 0.15f;
    float waitTime = 1.4f;
    bool slow = false;
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        StartCoroutine(HellOfALife());
        if (Voxeldata.PlayerData.special == 12)
        {
            ShowStory();
        }
    }

    void ShowStory()
    {
        Toolbar.CanEsc = false;
        SoundsManager.canChange = false;
        StartCoroutine(TheWholeStory());
    }
    public IEnumerator PlaySongByName(string name)
    {
        string musicFilePath;
#if UNITY_STANDALONE_WIN
        musicFilePath = Path.Combine(Application.streamingAssetsPath, $"Songs/{name}.ogg");
#elif UNITY_ANDROID
        musicFilePath = Application.streamingAssetsPath + $"/Songs/{name}.ogg";
        if (!musicFilePath.StartsWith("jar:file://"))
        {
            musicFilePath = "jar:file://" + musicFilePath;
        }
#endif
        using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(musicFilePath, AudioType.OGGVORBIS);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error loading audio file: " + www.error);
        }
        else
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            if (clip != null)
            {
                soundsource.clip = clip;
                soundsource.Play();
            }
        }

    }
    public IEnumerator TheWholeStory()
    {
        yield return new WaitForSeconds(2);
        SoundsManager.instance.MuteTheWholeGame();
        StartCoroutine(PlaySongByName("Runaway"));
        yield return new WaitForSeconds(21);
        yield return StartCoroutine(OpenScene());
        yield return Writer(1, story[0]);
        StartCoroutine(CloseDialogues(new List<int> { 1 }));
        yield return CloseScene();
        yield return new WaitForSeconds(4);

        yield return StartCoroutine(OpenScene());
        yield return Writer(0, story[1]);
        yield return Writer(2, story[2]);
        StartCoroutine(CloseDialogues(new List<int> { 0, 2 }));
        yield return CloseScene();
        yield return new WaitForSeconds(4);

        yield return StartCoroutine(OpenScene());
        yield return Writer(0, story[3]);
        yield return Writer(1, story[4]);
        yield return Writer(2, story[5]);
        StartCoroutine(CloseDialogues(new List<int> { 0, 1, 2 }));
        yield return CloseScene();
        yield return new WaitForSeconds(4);

        yield return StartCoroutine(OpenScene());
        yield return Writer(0, story[6]);
        yield return Writer(2, story[7]);
        StartCoroutine(CloseDialogues(new List<int> { 0, 2 }));
        yield return CloseScene();
        yield return new WaitForSeconds(4);

        yield return StartCoroutine(OpenScene());
        yield return Writer(0, story[8]);
        yield return Writer(2, story[9]);
        StartCoroutine(CloseDialogues(new List<int> { 0, 2 }));
        yield return CloseScene();
        yield return new WaitForSeconds(4);

        yield return StartCoroutine(OpenScene());
        yield return Writer(0, story[10]);
        yield return Writer(2, story[11]);
        yield return Writer(1, story[12]);
        StartCoroutine(CloseDialogues(new List<int> { 0, 2, 1 }));
        yield return CloseScene();
        yield return new WaitForSeconds(2);

        yield return StartCoroutine(OpenScene());
        yield return Writer(3, story[13]);
        yield return Writer(4, story[14]);
        StartCoroutine(CloseDialogues(new List<int> { 3, 4 }));
        yield return CloseScene();
        yield return new WaitForSeconds(3);

        yield return StartCoroutine(OpenScene());
        yield return Writer(0, story[15]);
        yield return Writer(2, story[16]);
        StartCoroutine(CloseDialogues(new List<int> { 0, 2 }));
        yield return CloseScene();
        yield return new WaitForSeconds(4);

        yield return StartCoroutine(OpenScene());
        yield return Writer(0, story[17]);
        yield return Writer(1, story[18]);
        yield return Writer(2, story[19]);
        StartCoroutine(CloseDialogues(new List<int> { 0, 1, 2 }));
        yield return CloseScene();
        yield return new WaitForSeconds(4);

        yield return StartCoroutine(OpenScene());
        yield return Writer(1, story[20]);
        StartCoroutine(CloseDialogues(new List<int> {1}));
        yield return CloseScene();
        yield return new WaitForSeconds(4);

        yield return StartCoroutine(OpenScene());
        yield return Writer(0, story[21]);
        yield return Writer(2, story[22]);
        StartCoroutine(CloseDialogues(new List<int> { 0, 2 }));
        yield return CloseScene();
        yield return new WaitForSeconds(5);

        yield return StartCoroutine(OpenScene());
        yield return Writer(0, story[23]);
        yield return Writer(2, story[24]);
        StartCoroutine(CloseDialogues(new List<int> { 0, 2 }));
        yield return CloseScene();
        yield return new WaitForSeconds(4);

        yield return StartCoroutine(OpenScene());
        yield return Writer(0, story[25]);
        yield return Writer(1, story[26]);
        yield return Writer(2, story[27]);
        StartCoroutine(CloseDialogues(new List<int> { 0, 1, 2 }));
        yield return CloseScene();
        yield return new WaitForSeconds(4);

        yield return StartCoroutine(OpenScene());
        yield return Writer(1, story[28]);
        StartCoroutine(CloseDialogues(new List<int> { 1 }));
        yield return CloseScene();
        yield return new WaitForSeconds(4);

        yield return StartCoroutine(OpenScene());
        yield return Writer(0, story[29]);
        yield return Writer(1, story[30]);
        yield return Writer(2, story[31]);
        StartCoroutine(CloseDialogues(new List<int> { 0, 1, 2 }));
        yield return CloseScene();
        yield return new WaitForSeconds(4);

        yield return StartCoroutine(OpenScene());
        yield return Writer(1, story[32]);
        StartCoroutine(CloseDialogues(new List<int> { 1 }));
        yield return CloseScene();
        yield return new WaitForSeconds(4);

        yield return StartCoroutine(OpenScene());
        StartCoroutine(Writer(0, story[33]));
        StartCoroutine(Writer(1, story[34]));
        yield return Writer(2, story[35]);
        StartCoroutine(CloseDialogues(new List<int> { 0, 1, 2 }));
        yield return CloseScene();
        yield return new WaitForSeconds(4);

        yield return StartCoroutine(OpenScene());
        yield return Writer(5, story[36]);
        yield return Writer(6, story[37]);
        yield return Writer(7, story[38]);
        StartCoroutine(CloseDialogues(new List<int> { 5,6,7 }));
        yield return CloseScene();
        yield return new WaitForSeconds(4);

        yield return StartCoroutine(OpenScene());
        yield return Writer(1, story[39]);
        yield return new WaitForSeconds(4);
        StartCoroutine(CloseDialogues(new List<int> { 1 }));
        yield return CloseScene();

        BookManager.readingBook = false;
        Toolbar.CanEsc = true;
        yield return new WaitForSeconds(4);
        StartCoroutine(HellOfALife());
        yield return null;

    }
    public IEnumerator OpenScene()
    {
        top.GetComponent<Image>().color = new Color(0,0,0,0);
        Toolbar.instance.openedInv = true;
        BookManager.readingBook = true;
        obj1.SetActive(false);
        obj2.SetActive(false);
        obj3.SetActive(false);
        obj4a = obj4.activeSelf;
        obj5a = obj5.activeSelf;
        crs = Cursor.visible;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        obj4.SetActive(false);
        obj5.SetActive(false);
        Fightscene.SetActive(true);
        background.SetActive(true);
        aa = Instantiate(heart,Fightscene.transform);
        aa.GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(0, -175);
        StartCoroutine(aa.GetComponent<secondheart>().Starting());
        source.clip = sound;
        source.Play();
        yield return new WaitForSeconds(0.3f);
    }
    public IEnumerator CloseScene()
    {
        float t = 0;
        while (t < 1.2)
        {

            top.GetComponent<Image>().color = new Color(0,0,0,Mathf.Lerp(0, 1, t));
            t += Time.deltaTime;
            yield return null;
        }
        Destroy(aa);
        yield return new WaitForSeconds(0.6f);
        obj1.SetActive(true);
        obj2.SetActive(true);
        obj3.SetActive(true);
        if (obj4a)
        {
            obj4.SetActive(true);
        }
        if (obj5a)
        {
            obj5.SetActive(true);
        }
        if (crs)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        background.SetActive(false);
        Fightscene.SetActive(false);
        Toolbar.instance.openedInv = false;
        BookManager.readingBook = false;
        yield return null;
    }
    public IEnumerator CloseDialogues(List<int> v)
    {
        yield return new WaitForSeconds(0.2f);
        foreach(int f in v)
        {
            d[f].SetActive(false);
            t[f].gameObject.SetActive(false);
            t[f].text = null;
        }
        yield return new WaitForSeconds(0.3f);
    }

    public IEnumerator Writer(byte n, string s)
    {
        GameObject obj = d[n];
        Text txt = t[n];
        yield return new WaitForSeconds(0.3f);
        txt.text = null;
        obj.SetActive(true);
        txt.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        foreach (char c in s)
        {
            txt.text += c;
            yield return new WaitForSeconds(0.09f);
        }
        yield return new WaitForSeconds(0.3f);
    }
    private string[] story = new string[] {
"A long time ago,",

"A king wore a \ncrown of gold, \nand had a sword\ntoo heavy to lift.",
"He ruled \nthrough strength, \nand through \nsilence.",

"He called it peace.",
"But peace built \non fear was always \na war in disguise.",
"And so,   \nwe feared\nthe worst.",

"Light bled into \nthe world,",
///
"But men and women\nran towards \nthe darkness.",

"Then...        \nOne day...           ",
"The king\ndeclared war.",

"He did not\nwage war against\nbeasts.",
"He did not\nfight to be\nremembered.", //2
"He turned\nagainst\nhis own people.",

"When humanity\nrefused to kneel,\nhe raised his\nsword,",
"not to defend,\nbut to destroy.",

"It was not a\nconquest.",
"It was a\ncorrection.",

"And so,\nthe whole kingdom      ",
"no.     ",
"...     " ,

"The whole world\nfought back.",



"The next day.   ",

"The next day.   ",

"...    ",

"Both the king\nand the world\nfell silent.",


"And so,\nthe kingdom fell.",
"The whole world\nhad been lost\nin just one night.", //3
"Once again,\neverything was\ntaken from us.",

"The king decided\nit was better\nto end our\nsuffering.",

"Every being\nmust die.             ",
"With each soul,\nHe is closer\nto bring hope.",
"To bring justice.",

"It's not long\nnow.",

"King YEEZUS\nwill let us\ngo.",
"King YEEZUS\nwill give us\nhope.",
"King YEEZUS\nwill save us\nall.",

"You should be\nsmiling, too.",
"Aren't \nyou\nexcited?",
"Aren't \nyou \nhappy?",

"You're going \nto be free."};
    public IEnumerator HellOfALife()
    {
        yield return new WaitForSeconds(3);
        StartCoroutine(PlaySongByName("song3"));
        LostInTheWorld.instance.HellOfALife();
        Voxeldata.PlayerData.special = 9;
        PlayerDataData.SavePlayerFile();
        slow = false;
        waitTime = 0.5f;
        fastspeed = 0.053f;
        Read("Hell of a life");
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
    public IEnumerator DisplayNextLine()
    {
        if (currentLine < dialogueLines.Length - 1)
        {
            if (dialogueLines[currentLine][0] == '%')
            {
                currentLine++;
                slow = !slow;
            }
            if (dialogueLines[currentLine][0] == '>')
            {
                float f = (float)(dialogueLines[currentLine][1] - '0') + (float)((dialogueLines[currentLine][2] - '0') / 10.0f);
                yield return new WaitForSeconds(f);
                currentLine++;
                StartCoroutine(DisplayNextLine());
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
            yield return StartCoroutine(Waiting());
            yield return new WaitForSeconds(10);
            BloodOnTheLeaves.SceneNum = 9;
            SceneManager.LoadScene("Blood");
        }
    }
    public IEnumerator TypeLine(string line,float spd)
    {
        hoal.text = null;
        foreach (char c in line)
        {
            hoal.text += c;
            yield return new WaitForSeconds(spd);
        }
        currentLine++;
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(DisplayNextLine());
    }
    public IEnumerator Waiting()
    {
        while (soundsource.isPlaying)
        {
            yield return null;
        }
        yield return null;
    }

}
