using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueDisplay : MonoBehaviour
{
    public Text dialogueTextUI; 
    public Image TextBox;
    public SoundsManager SoundsManager;
    public TextAsset dialogueFile; 
    private string[] dialogueLines; 
    private int currentLine = 0; 
    private bool isTyping = false;
    private void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
        dialogueFile = Resources.Load<TextAsset>($"Dialogues/EndScene");
    }

    private void Start()
    {
        dialogueLines = dialogueFile.text.Split('\n');
        DisplayNextLine(); 
    }

    private void Update()
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
            StopAllCoroutines();
            dialogueTextUI.text = dialogueLines[currentLine - 1].Trim();
            isTyping = false;
            StartCoroutine(Waiting(0.3f));
        }
    }

    private void DisplayNextLine()
    {
        while (dialogueLines[currentLine][0]=='[' || string.IsNullOrWhiteSpace(dialogueLines[currentLine]))
            currentLine++;
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
                StartCoroutine(Waiting(y));
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
            
        }
        else if (currentLine < dialogueLines.Length)
        {
            StartCoroutine(TypeLine(dialogueLines[currentLine].Trim(),0.1f));
            currentLine++;
        }
        else
        {
            dialogueTextUI.text = "";
            Debug.Log("Dialogue finished!");
        }
    }
    private IEnumerator Waiting(float n)
    {
        yield return new WaitForSeconds(n);
        currentLine++;
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
