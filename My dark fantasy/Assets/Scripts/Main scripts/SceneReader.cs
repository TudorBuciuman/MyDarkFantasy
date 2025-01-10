using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneReader : MonoBehaviour
{
    public Text dialogueTextUI; 
    public Image TextBox;
    public SoundsManager SoundsManager;
    public TextAsset dialogueFile; 
    public string[] dialogueLines;

    public static string SceneLoc="EndScene";
    public int currentLine = 0; 
    public bool isTyping = false;

    private Coroutine typingCoroutine;

    private void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name!="World")
        {
            Read(SceneLoc);   
        }
    }
    public void Read(string s)
    {
        dialogueFile = Resources.Load<TextAsset>($"Dialogues/{s}");
        dialogueLines = dialogueFile.text.Split('\n');
        DisplayNextLine();
        StartCoroutine(GetInput());
    }

    bool character = false;
    private void DisplayNextLine()
    {
        while (string.IsNullOrWhiteSpace(dialogueLines[currentLine]))
        {
            currentLine++;
            character = false;
        }
        while (dialogueLines[currentLine][0] == '[')
        {
            character = true; 
            currentLine++;
        }
        while (string.IsNullOrWhiteSpace(dialogueLines[currentLine]))
        {
            currentLine++;
            character = false;
        }
        if (dialogueLines[currentLine][0] == '{')
        {
            if (dialogueLines[currentLine][1] == 'C')
            {
                TextBox.gameObject.SetActive(false);
                currentLine++;
                DisplayNextLine();
            }
            else if (dialogueLines[currentLine][1] == 'W')
            {
                int y = 0,t=3;
                while (dialogueLines[currentLine][t]>='0' && dialogueLines[currentLine][t] <= '9')
                {
                    y = y * 10 + (byte)(dialogueLines[currentLine][t] - '0');
                    t++;
                }
                StartCoroutine(WaitingAndGoOn(y));
            }
            else if (dialogueLines[currentLine][1] == 'O')
            {
                TextBox.gameObject.SetActive(true);
                currentLine++;
                DisplayNextLine();
            }
            else if (dialogueLines[currentLine][1] == 'P')
            {
                byte y = (byte)((dialogueLines[currentLine][3]-'0')*10), t = (byte)(dialogueLines[currentLine][4]-'0');
                SoundsManager.PlaySceneSong((byte)(y+t));
                currentLine++;
                DisplayNextLine();
            }
            else if (dialogueLines[currentLine][1] == 'S')
            {
                this.gameObject.SetActive(false);
                Toolbar.escape = false;
                Toolbar.instance.openedInv = false;
                SceneManager.LoadScene("World");
            }
            
        }
        else if (currentLine < dialogueLines.Length)
        {
            if(character)
            typingCoroutine= StartCoroutine(TypeLine(dialogueLines[currentLine].Trim(),0.05f));
            else
            {
              typingCoroutine=  StartCoroutine(TypeLine(dialogueLines[currentLine].Trim(), 0.1f));
            }
            currentLine++;
        }
        else
        {
            dialogueTextUI.text = "";
            Debug.Log("Dialogue finished!");
            StopAllCoroutines();
        }
    }
    private IEnumerator GetInput()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (!isTyping)
                {
                    DisplayNextLine();
                }
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                StopCoroutine(typingCoroutine);
                dialogueTextUI.text = dialogueLines[currentLine - 1].Trim();
                isTyping = false;
                yield return new WaitForSeconds(0.3f);
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator WaitingAndGoOn(float n)
    {
        isTyping = true;
        yield return new WaitForSeconds(n);
        currentLine++;
        isTyping = false;
        DisplayNextLine();
    }
    private IEnumerator TypeLine(string line,float spd)
    {
        isTyping = true;
        dialogueTextUI.text = "";

        foreach (char c in line)
        {
            dialogueTextUI.text += c;
            yield return new WaitForSeconds(spd); // Adjust typing speed
        }

        isTyping = false;
    }
}
