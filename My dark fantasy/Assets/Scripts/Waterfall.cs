using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Waterfall : MonoBehaviour
{
    public static float speed = 1f;
    public Image img;
    public Text dialogueTextUI;
    public bool slow = true, activate = false,andr=true;
    public TextAsset dialogueFile;
    public string[] dialogueLines;
    public int currentLine = 0;
    public AudioClip clip1, clip2, clip3;
    private string s = "Once upon a time";
    public AudioSource source,background;
    Rigidbody2D rb;
    public static float sprintspeed = 1;
    private void Awake()
    {
#if UNITY_ANDROID
    andr=true;
        
#endif
    }
    void Start()
    {
        Screen.sleepTimeout=SleepTimeout.NeverSleep; 
        Application.targetFrameRate = 60;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        background.loop = true;
        background.clip = clip2;
        background.Play();
        source.clip = clip1;
        rb = GetComponent<Rigidbody2D>();
    }
    bool pt1 = false,txtpt2=false;
    void FixedUpdate()
    {
        Vector3 movement= Vector3.zero;
        if ((andr && AndrOnly()) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || activate)
        {
            movement += Vector3.right;
        }
        movement = movement.normalized * speed * Time.deltaTime;
        if(transform.position.x+movement.x>-100 && transform.position.x + movement.x < 100) 
        transform.position += movement;
        if (!pt1 && transform.position.x > -91)
        {
            pt1 = true;
            StartCoroutine(part1());
        }
        if(!txtpt2 && transform.position.x > -26)
        {
            txtpt2 = true;
            dialogueTextUI.rectTransform.localPosition = new Vector3(800, dialogueTextUI.rectTransform.localPosition.y, dialogueTextUI.rectTransform.localPosition.z);
        }
    }
    public void Read()
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
    public bool AndrOnly()
    {
        return Input.touches.Length > 0;
    }
    public IEnumerator DisplayNextLine()
    {
        if (currentLine < dialogueLines.Length)
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
                    yield return StartCoroutine(TypeLine(dialogueLines[currentLine].Trim(), 0.17f));
                else
                {
                    yield return StartCoroutine(TypeLine(dialogueLines[currentLine].Trim(), 0.15f));
                }
            }
        }
        else
        {
            dialogueTextUI.text = null;
            yield return new WaitForSeconds(10);
            Application.Quit();
        }
    }

    private IEnumerator TypeLine(string line, float spd)
    {
        dialogueTextUI.text = "";

        foreach (char c in line)
        {
            dialogueTextUI.text += c;
            yield return new WaitForSeconds(spd);
        }

        currentLine++;
        yield return new WaitForSeconds(1f);
        StartCoroutine(DisplayNextLine());
    }

    private IEnumerator part1()
    {
        yield return new WaitForSeconds(1);

        partt1();
        yield return new WaitForSeconds(186);
        source.clip = clip3;
        source.volume = 85;
        source.Play();
        yield return new WaitForSeconds(55);
        StartCoroutine(MakeLight(4));
    }
    public void partt1()
    {
        speed = 0.5f;
        source.Play();
        background.volume = 65;
        Read();
        activate = true;
    }
    public IEnumerator MakeLight(float time)
    {
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, Mathf.Clamp01(elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);
    }


}
