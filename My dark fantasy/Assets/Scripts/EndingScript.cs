using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingScript : MonoBehaviour
{
    int currentLine = 0;
    public Animator credits,armageddon;
    public GameObject credit, arm;
    public TextAsset dialogueFile;
    public string[] dialogueLines;
    public Text dialogueTextUI;
    public AudioClip[] clips = new AudioClip[5];
    public AudioSource AudioSource;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
        /*
         if(Voxeldata.PlayerData.pacifist)
         {
             Pacifist();
         }
         else if (Voxeldata.PlayerData.genocide)
         {
             StartCoroutine(Genocide());
         }
         else
         {
             FinalCredits();
         }
        */
        StartCoroutine(Genocide());
    }

    public void Pacifist()
    {
        //read beautiful txt file 
        Read("Once upon a time");
        AudioSource.clip = clips[1];
        AudioSource.Play();
    }
    public IEnumerator Genocide()
    {
        arm.gameObject.SetActive(true);
        AudioSource.clip = clips[3];
        AudioSource.Play();
        armageddon.SetTrigger("playing");
        yield return new WaitForSeconds(5);
        arm.gameObject.SetActive(false);
        Read("Your worst nightmare");

        yield return new WaitForSeconds(13);
        AudioSource.clip = clips[4];
        AudioSource.Play();
        yield return new WaitForSeconds(10);
        AudioSource.clip = clips[2];
        AudioSource.Play();
        //read a serious and unsettling postapocaliptic message 
        //which ends by ending your life and getting judged for your actions
        //and ends by shutting down the game by breaking itself
        yield return null;
    }
    public void FinalCredits()
    {
        AudioSource.clip = clips[0];
        AudioSource.Play();
        credits.SetTrigger("playing");
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
            }

            while (dialogueLines[currentLine][0] == '[')
            {
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
                yield return null;
            }
            else if (dialogueLines[currentLine][0] == '$')
            {
                //StartCoroutine(BeforeQuit());
                yield return null;
            }

            else if (currentLine < dialogueLines.Length)
            {
                yield return StartCoroutine(TypeLine(dialogueLines[currentLine].Trim(), 0.15f));
                
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
        yield return new WaitForSeconds(1.4f);
        StartCoroutine(DisplayNextLine());

    }
    public IEnumerator Waiting()
    {
        yield return new WaitForSeconds(300);
    }
    
    public void EndingSounds()
    {

    }
}
