using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fighting_Intro : MonoBehaviour
{
    public Image lightingImg;
    public AudioClip[] clip = new AudioClip[6];
    public Image FightingImg;
    public Sprite[] sprites = new Sprite[5];
    public AudioSource audioSource;
    public TextAsset dialogueFile;
    public Text dialogueTextUI;
    public string[] dialogueLines;

    public static string SceneLoc = "Fighting";
    public int currentLine = 0;
    public bool isTyping = false,slow=false,isWhite=false;

    void Start()
    {
        Application.targetFrameRate = 60;
        Read(SceneLoc);
    }
    public void Read(string s)
    {
        dialogueFile = Resources.Load<TextAsset>($"Dialogues/{s}");
        dialogueLines = dialogueFile.text.Split('\n');
        for (int i = 0; i < dialogueLines.Length; i++)
        {
            dialogueLines[i] = dialogueLines[i].Replace("\\n", "\n");
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
    public IEnumerator DisplayNextLine()
    {
        while (string.IsNullOrWhiteSpace(dialogueLines[currentLine]))
        {
            currentLine++;
            slow = false;
        }
        while (dialogueLines[currentLine][0] == '[')
        {
            slow = true;
            currentLine++;
        }
        while (dialogueLines[currentLine][0] == '(')
        {
            currentLine++;
            //StartCoroutine(DisplayNextLine());
        }
        /*
        while (string.IsNullOrWhiteSpace(dialogueLines[currentLine]))
        {
            currentLine++;
            slow = false;
        }
        */
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
            if (slow)
                yield return StartCoroutine(TypeLine(dialogueLines[currentLine].Trim(), 0.05f));
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
        audioSource.Stop();
        dialogueTextUI.text = string.Empty;
        yield return new WaitForSeconds(3.5f);
        PlaySong(0);
        yield return new WaitForSeconds(90);
        FightingImg.sprite = sprites[0];
        FightingImg.gameObject.SetActive(true);
        StartCoroutine(GetInput());
        yield return new WaitForSeconds(30);
        PlayerDataData.SawIntro = true;
        FightingImg.gameObject.SetActive(false);
        yield return new WaitForSeconds(205);
        PlayerDataData.SavePlayer();
    }

}
