using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FightSistem : MonoBehaviour
{
    //I can't spell system ;(
    //Is is sistem or system ?
    public GameObject img;
    public Image dialogue;
    public Image bullet;
    public Image character;

    public AudioClip swordsound;
    public AudioSource AudioSource;

    public Text text;
    public GameObject heart;
    public GameObject arena,truearena;
    public GameObject projectiles;
    public Image sword;
    public GameObject fightobj;

    public byte life = 20;
    public byte enemylife = 255;
    public byte fightnr = 0;

    public Coroutine attack;
    public Coroutine inputt;

    public bool isTyping=false;
    public Text dialogueTextUI;
    public TextAsset dialogueFile;
    public string[] dialogueLines;

    public string s = "Wizzard1";
    void Start()
    {
        Application.targetFrameRate = 60;
        dialogueFile = Resources.Load<TextAsset>($"Dialogues/{s}");
        dialogueLines = dialogueFile.text.Split('\n');
        Debug.Log(dialogueLines.Length);
        StartCoroutine(DisplayNextLine());
    }
    public int currentLine = 0;
    public IEnumerator DisplayNextLine()
    {
            dialogue.gameObject.SetActive(true);
        if (currentLine<dialogueLines.Length)
        {
            if (dialogueLines[currentLine][0] != '{')
                StartCoroutine(TypeLine(dialogueLines[currentLine].Trim(), 0.1f));
            else
            {
                if (dialogueLines[currentLine][1] == 'P')
                {
                    Attack();
                }
                else if (dialogueLines[currentLine][1] == 'A')
                {
                    StartCoroutine(Attacked());
                }
            }
        }
        yield return null;
    }
    bool fighting = false;
    public IEnumerator Inputt()
    {
        while (fighting)
        {
            if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
            {
                int dammage = (int)sword.rectTransform.anchoredPosition.x;
                dammage = 1000-Mathf.Abs(dammage);
                if (dammage < 320)
                    dammage = 0;
                else
                    dammage = dammage / 50 + 10;
                Debug.Log(dammage);
                StopCoroutine(attack);
                fighting = false;
                StartCoroutine(Calculate());

            }
            yield return new WaitForSeconds(0.01f);
        }
    }
    public void Attack()
    {
        dialogue.gameObject.SetActive(false);
        bullet.gameObject.SetActive(true);
        sword.rectTransform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        sword.rectTransform.anchoredPosition = new Vector3(-1000, -289, 0);
        sword.gameObject.SetActive(true);
        attack=StartCoroutine(StartAttacking());
        fighting = true;
        inputt=StartCoroutine(Inputt());
    }
    public IEnumerator StartAttacking()
    {
        float duration = 2f; 
        float elapsed = 0f;  
        Vector3 startPosition = sword.rectTransform.anchoredPosition; 
        Vector3 endPosition = new Vector3(1000, -289,0);

        while (elapsed < duration)
        {
            sword.rectTransform.anchoredPosition = Vector3.Lerp(startPosition, endPosition, elapsed / duration);
            elapsed += Time.deltaTime; 
            yield return null; 
        }
        fighting= false;
        StartCoroutine(Calculate());
    }
    public IEnumerator Calculate()
    {
        StopCoroutine(inputt);
        yield return new WaitForSeconds(0.3f);
        AudioSource.clip=swordsound;
        AudioSource.Play();
        yield return new WaitForSeconds(0.5f);

        sword.gameObject.SetActive(false);

        yield return new WaitForSeconds(2.2f);

        bullet.gameObject.SetActive(false);
        currentLine++;
        StartCoroutine(DisplayNextLine());

    }
    private IEnumerator TypeLine(string line, float spd)
    {
        isTyping = true;
        dialogueTextUI.text = "";

        foreach (char c in line)
        {
            dialogueTextUI.text += c;
            yield return new WaitForSeconds(spd); 
        }

        isTyping = false;
        currentLine++;
        yield return new WaitForSeconds(1);
        StartCoroutine(DisplayNextLine());
    }
    public IEnumerator Attacked()
    {
        dialogue.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);
        arena.gameObject.SetActive(true);
        GameObject a=Instantiate(heart,truearena.transform);
        currentLine++;
        yield return StartCoroutine(AssignProjectiles());
        dialogue.gameObject.SetActive(false);
        arena.gameObject.SetActive(false);
        Destroy(a);
        StartCoroutine(DisplayNextLine());
    }
    public IEnumerator AssignProjectiles()
    {
        switch(fightnr)
        {
            case 0:
                {
                    yield return StartCoroutine(SpawnBones());
                    break;
                }
            case 1:
                {
                    yield return StartCoroutine(Attack2());
                    break;
                }
            default:
                yield return StartCoroutine(Attack1());
                break;
        }
        fightnr++;
        yield return new WaitForSeconds(0.3f);
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("obstacle")) 
            Destroy(g);

    }
    public IEnumerator Attack1()
    {
        ProjectilesManager.speed = 210;
        for (int i = 1; i < 10; i++)
        {
            for (int k = 1; k < 3; k++)
            {
                for (int j = 1; j < 3; j++)
                {
                    GameObject newProjectile = Instantiate(projectiles, truearena.transform);
                    ProjectilesManager boneScript = newProjectile.GetComponent<ProjectilesManager>();
                    if (j == 1)
                    {
                        newProjectile.GetComponent<Image>().rectTransform.anchoredPosition = new Vector3(-210, Random.Range(-5, 5) * 30);
                        boneScript.direction = Vector2.right;
                    }
                    else
                    {
                        newProjectile.GetComponent<Image>().rectTransform.anchoredPosition = new Vector3(210, Random.Range(-5, 5) * 30);
                        boneScript.direction = Vector2.left;
                    }

                    yield return new WaitForSeconds(1.2f);
                }
            }
        }
    }
    public IEnumerator SpawnBones()
    {
        ProjectilesManager.speed = 100;
        for (int i = 0; i < 10; i++)
        {
            int w = (i - 5) * 40;
            for (int j = 0; j <=1; j++)
            {
                if (j == 1) 
                    w = -w;
                GameObject newProjectile = Instantiate(projectiles, truearena.transform);
                newProjectile.GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(w, 200);
                ProjectilesManager boneScript = newProjectile.GetComponent<ProjectilesManager>();

                boneScript.direction = Vector2.down;
            }
            yield return new WaitForSeconds(1.2f);
        }
        yield return new WaitForSeconds(2);
        for (int i = 0; i < 20; i++)
        {
            int w = (i - 10) * 40;
            for (int j = 0; j <= 3; j++)
            {
                w+=(i<2)? 150: -150;
                GameObject newProjectile = Instantiate(projectiles, truearena.transform);
                newProjectile.GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(w, 200);
                ProjectilesManager boneScript = newProjectile.GetComponent<ProjectilesManager>();

                boneScript.direction = Vector2.down;
            }
            yield return new WaitForSeconds(1.2f);
        }
        yield return new WaitForSeconds(6);
    }
    public IEnumerator Attack2()
    {
        ProjectilesManager.speed = 150;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j <= 1; j++)
            {
                GameObject newProjectile = Instantiate(projectiles, truearena.transform);
                newProjectile.GetComponent<Image>().rectTransform.anchoredPosition = new Vector2((i*j-5)*40, -200);
                ProjectilesManager boneScript = newProjectile.GetComponent<ProjectilesManager>();
                ProjectilesManager.speed = 250;
                boneScript.direction = Vector2.up;
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
