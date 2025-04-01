using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FightSistem : MonoBehaviour
{
    //I can't spell system ;(
    //Is is sistem or system ?
    public static FightSistem instance;
    public GameObject img;
    public Image dialogue;
    public Image bullet;
    public Image character;

    public AudioClip swordsound,music,goodbye;
    public AudioClip switch_sound, damage, heal, starting;
    public AudioSource AudioSource,SoundsSource;

    public Text text;
    public GameObject heart;
    public GameObject arena,secondarena;
    public GameObject projectiles,projectile1;
    public Image sword;
    public GameObject fightobj;

    private static byte deaths = 0;
    public static float life = 20;
    private byte fightnr = 0;

    public Coroutine attack;
    public Coroutine inputt;

    private bool isTyping=false;
    public Text dialogueTextUI;
    public TextAsset dialogueFile;
    private string[] dialogueLines;

    public GameObject deathscene;
    public Text dsc1, ds2;

    private bool isWhite = false,slow=true;
    public Image lightingImg;
    public Coroutine textManager;
    public GameObject heartImgInv, Inventory;
    public Slider healthslider;
    public Text textBox,hlt;
    public GameObject selectheart;
    public Image[] options=new Image[4];
    public Sprite[] sprites=new Sprite[8];
    private readonly string s = "Yeezus";
    public struct FightAct
    {
        public byte attacks;
        public byte deaths;
        public byte acts;
        public byte spares;
        public bool gen, pac;
    };
    private static FightAct fightAct;
    void Start()
    {
        instance= this;
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
        Read();
    }
    private int currentLine = 0;
    public void Read()
    {
        if (deaths!=0)
        {
            currentLine = 12;
            AudioSource.clip = music;
            AudioSource.loop = true;
            AudioSource.Play();
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
        else
        {
            AudioSource.clip = goodbye;
            AudioSource.Play();
            StartCoroutine(Musicdellay());
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
    }
    public IEnumerator Musicdellay()
    {
        yield return new WaitForSeconds(24);
        AudioSource.Pause();
        AudioSource.clip = music;
        AudioSource.loop = true;
        yield return new WaitForSeconds(9f);
        AudioSource.Play();
    }
    public void PlayDamageSound()
    {
        SoundsSource.clip = damage;
        SoundsSource.Play();
    }
    public IEnumerator PlayDeathMusic()
    {
        deaths++;
        AudioSource.Stop();
        StopAllCoroutines();
        ds2.text = null;
        ds2.gameObject.SetActive(true);
        deathscene.SetActive(true);
        yield return new WaitForSeconds(3);
        StartCoroutine(LoadAndPlayMusic(8));
        yield return new WaitForSeconds(1);
        dsc1.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        string s = "Don't lose hope..";
        foreach (char c in s) {
            ds2.text +=c;
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(2f);
        ds2.text = null;
        yield return new WaitForSeconds(0.5f);
        s = " we count on you!";
        foreach (char c in s)
        {
            ds2.text += c;
            yield return new WaitForSeconds(0.1f);
        }
        while (true)
        {
            if(Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
            {
                break;
            }
            yield return null;
        }
        yield return new WaitForSeconds(1);
        ds2.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);
        dsc1.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);
        ds2.text = null;
        AudioSource.Stop();
        life = 20;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private IEnumerator LoadAndPlayMusic(byte id)
    {
        string musicFilePath;
        string path = Path.Combine(Application.streamingAssetsPath);
#if UNITY_STANDALONE_WIN
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
                AudioSource.clip = clip;
                AudioSource.Play();
            }
        }

    }
    public IEnumerator PacifistDeath()
    {
        //flying projectiles
        //it ends after dying
        //custom death animation
        //death anthem
        //it starts then rising
        yield return null;
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

            while (dialogueLines[currentLine][0] == '[')
            {
                slow = true;
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
                //StartCoroutine(BeforeQuit());
                yield break;
            }
            if (dialogueLines[currentLine][0] == '{')
            {
                if (dialogueLines[currentLine][1] == 'A')
                {
                    StartCoroutine(Attacked());
                    yield break;
                }
                else if (dialogueLines[currentLine][1] == 'I')
                {
                    StartCoroutine(OpenInventory());
                    yield break;
                }
            }
            else if (currentLine < dialogueLines.Length)
            {
                if (slow)
                    yield return StartCoroutine(TypeLine(dialogueLines[currentLine].Trim(), 0.12f));
                else
                {
                    yield return StartCoroutine(TypeLine(dialogueLines[currentLine].Trim(), 0.0782f));
                }
            }
        }
        else
        {
            //PlayerDataData.SavePlayerFile();
            dialogueTextUI.text = null;
            yield return new WaitForSeconds(5);
            SceneManager.LoadScene("Intro");
        }
    }

    bool fighting = false,fought=false,oninventory=false;
    byte index = 0;
    public IEnumerator Inputt()
    {
        while (fighting)
        {
            if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Z))
            {
                fought = true;
                int dammage = (int)sword.rectTransform.anchoredPosition.x;
                dammage = 1000-Mathf.Abs(dammage);
                if (dammage < 320)
                    dammage = 0;
                else
                    dammage = dammage / 50 + 10;
                if (dammage > 0)
                {
                    fightAct.attacks++;
                }
                StopCoroutine(attack);
                fighting = false;
                StartCoroutine(Calculate());

            }
            yield return new WaitForSeconds(0.01f);
        }
    }
    public IEnumerator Lighting()
    {
        if (!isWhite)
            yield return StartCoroutine(MakeLight(0.5f));
        else
            yield return StartCoroutine(MakeDark(0.5f));
        isWhite = !isWhite;
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
    public IEnumerator StartAttacking()
    {
        fought = false;
        float duration = 2f; 
        float elapsed = 0f;  
        Vector3 startPosition = sword.rectTransform.anchoredPosition; 
        Vector3 endPosition = new(1000, -289,0);

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
        if(inputt!=null)
        StopCoroutine(inputt);
        yield return new WaitForSeconds(0.3f);
        if (fought)
        {
            SoundsSource.clip = swordsound;
            SoundsSource.Play();
        }
        yield return new WaitForSeconds(0.5f);

        sword.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.2f);

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
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(DisplayNextLine());
    }
    public IEnumerator Attacked()
    {
        text.text = string.Empty;
        yield return new WaitForSeconds(1);
        GameObject a;
        if (fightnr != 2)
        {
            arena.SetActive(true);
            a = Instantiate(heart, arena.transform);
        }
        else
        {
            secondarena.SetActive(true);
            a = Instantiate(heart, secondarena.transform);
        }
            yield return StartCoroutine(AssignProjectiles());
        if (fightnr != 3)
            arena.SetActive(false);
        else
            secondarena.SetActive(false);
        Destroy(a);
        yield return new WaitForSeconds(1);
        currentLine++;
        StartCoroutine(DisplayNextLine());
    }
    public IEnumerator AssignProjectiles()
    {
        switch(fightnr)
        {
            case 0:
                {
                    yield return StartCoroutine(Atk4.ts.upd());
                    break;
                }
            case 10:
                {
                    yield return StartCoroutine(Atk1.ts.upd());
                    break;
                }
            case 1:
                {
                    yield return StartCoroutine(Atk2.ts.upd());
                    break;
                }
            case 2:
                {
                    yield return StartCoroutine(Atk3.ts.upd());
                    break;
                }
            case 3:
                {
                    yield return StartCoroutine(Atk2.ts.upd());
                    break;
                }
            case 4:
                {
                    yield return StartCoroutine(Atk1.ts.upd());
                    break;
                }
            case 5:
                {
                    yield return StartCoroutine(Attack2());
                    break;
                }
            case 6:
                {
                    yield return StartCoroutine(Atk2.ts.upd());
                    fightnr = 255;
                    break;
                }
            default:
                {
                    yield return StartCoroutine(Attack1());
                    break;
                }
        }
        fightnr++;
        yield return new WaitForSeconds(0.3f);
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("obstacle")) 
            Destroy(g);

    }
    public IEnumerator Attack1()
    {
        ProjectilesManager.speed = 2.5f;
        for (int i = 1; i < 3; i++)
        {
            for (int k = 1; k < 4; k++)
            {
                for (int j = 1; j < 4; j++)
                {
                    GameObject newProjectile = Instantiate(projectiles, arena.transform);
                    newProjectile.transform.localScale = new Vector3(2, 2, 1);
                    ProjectilesManager boneScript = newProjectile.GetComponent<ProjectilesManager>();
                    if (j%2 == 1)
                    {
                        newProjectile.transform.position = new Vector3(-2.1f, Random.Range(-5, 6) * 0.3f);
                        boneScript.direction = Vector2.right;
                    }
                    else
                    {
                        newProjectile.transform.position = new Vector3(2.1f, Random.Range(-5, 6) * 0.3f);
                        boneScript.direction = Vector2.left;
                    }

                    yield return new WaitForSeconds(0.4f);
                }
            }
        }
        GameObject newProjectil = Instantiate(projectile1, arena.transform);
        GameObject newProjectil1 = Instantiate(projectile1, arena.transform);
        GameObject newProjectil2 = Instantiate(projectile1, arena.transform);
        newProjectil.transform.localScale = new Vector3(3, 3, 1);
        newProjectil.transform.position = new Vector3(-1.78f, 2.2f, 0);
        newProjectil1.transform.localScale = new Vector3(3, 3, 1);
        newProjectil1.transform.position = new Vector3(0, 2.2f, 0);
        newProjectil2.transform.localScale = new Vector3(3, 3, 1);
        newProjectil2.transform.position = new Vector3(1.78f, 2.2f, 0);
        yield return new WaitForSeconds(2);
    }
    public IEnumerator AssignText()
    {
        switch(fightnr)
        {
            case 0:
                yield return textManager = StartCoroutine(Write("* It's already too late"));
                break;
            case 1:
                yield return textManager = StartCoroutine(Write("* A flicker of light fills \n the world"));
                break;
            case 2:
                yield return textManager = StartCoroutine(Write("* Your life starts to flicker \n before your eyes"));
                break;
            case 3:
                yield return textManager = StartCoroutine(Write("* Your best friend!"));
                break;
            case 4:
                yield return textManager = StartCoroutine(Write("* You can't end this"));
                break;
            case 5:
                yield return textManager = StartCoroutine(Write("* you suddenly feel like \n taking a nap"));
                break;
            case 6:
                yield return textManager = StartCoroutine(Write("* you might want to cry \n but you have no eyes.."));
                break;
            default:
                yield return textManager = StartCoroutine(Write("* the end has come..."));

                break;
        }
    }
    public IEnumerator SpawnBones()
    {
        ProjectilesManager.speed = 1.5f;
        for (int i = 0; i < 1; i++)
        {
            int w = (i - 5) * 40;
            for (int j = 0; j <=1; j++)
            {
                if (j == 1) 
                    w = -w;
                GameObject newProjectile = Instantiate(projectiles, arena.transform);
                newProjectile.transform.position = new Vector2(w*0.01f, 2);
                ProjectilesManager boneScript = newProjectile.GetComponent<ProjectilesManager>();

                boneScript.direction = Vector2.down;
            }
            yield return new WaitForSeconds(1.2f);
        }
        yield return new WaitForSeconds(2);
        for (int i = 0; i < 2; i++)
        {
            int w = (i - 10) * 40;
            for (int j = 0; j <= 3; j++)
            {
                w+=(i<2)? 150: -150;
                GameObject newProjectile = Instantiate(projectiles, arena.transform);
                newProjectile.transform.position = new Vector2(w*0.01f, 2);
                ProjectilesManager boneScript = newProjectile.GetComponent<ProjectilesManager>();

                boneScript.direction = Vector2.down;
            }
            yield return new WaitForSeconds(1.2f);
        }
        yield return new WaitForSeconds(6);
    }
    public IEnumerator Attack2()
    {
        ProjectilesManager.speed = 250;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j <= 1; j++)
            {
                GameObject newProjectile = Instantiate(projectiles, arena.transform);
                newProjectile.transform.position = new Vector2((i*j-5)*0.4f,-2);
                ProjectilesManager boneScript = newProjectile.GetComponent<ProjectilesManager>();
                ProjectilesManager.speed = 2.5f;
                boneScript.direction = Vector2.up;
            }
            yield return new WaitForSeconds(1f);
        }
    }
    public IEnumerator OpenInventory()
    {
        oninventory= true;
        textBox.text = null;
        yield return new WaitForSeconds(1);
        Inventory.SetActive(true);
        StartCoroutine(Sellection());
        yield return StartCoroutine(AssignText());
        UpdateIndex(0);
    }
    public void UpdateIndex(int n)
    {
        if (n > 0)
        {
            index++;
            index %= 4;
        }
        else if(n<0)
        {
            if (index == 0)
            {
                index = 3;
            }
            else
                index--;
        }
        heartImgInv.transform.SetParent(options[index].transform, false);
        for(int i = 0; i < 4; i++)
        {
            if (i != index)
            {
                options[i].sprite = sprites[i];
            }
        }
        options[index].sprite = sprites[index+4];
    }
    public IEnumerator Attack()
    {
        textBox.text = "  *YEEZUS";
        heartImgInv.SetActive(false);
        selectheart.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        while (true)
        {
            if ((Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.Return)))
            {
                SoundsSource.clip = switch_sound;
                SoundsSource.Play();
                heartImgInv.SetActive(true);
                selectheart.SetActive(false);
                break;
            }
            if (Input.GetKeyUp(KeyCode.X))
            {
                heartImgInv.SetActive(true);
                selectheart.SetActive(false);
                SoundsSource.clip = switch_sound;
                SoundsSource.Play();
                textBox.text = null;
                yield return StartCoroutine(AssignText());
                yield break;
            }
            yield return null;
        }
        Inventory.SetActive(false);
        text.text = string.Empty;
        bullet.gameObject.SetActive(true);
        sword.rectTransform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        sword.rectTransform.anchoredPosition = new Vector3(-1000, -289, 0);
        sword.gameObject.SetActive(true);
        attack = StartCoroutine(StartAttacking());
        fighting = true;
        yield return new WaitForSeconds(0.2f);
        inputt = StartCoroutine(Inputt());
        yield break;
    }
    string message;
    byte actnum=0;
    bool acted = false;
    static bool diedTxt=false;
    public IEnumerator Write(string s)
    {
        isTyping = true;
        message = s;
        textBox.text = null;
        foreach(char c in s)
        {
            textBox.text += c;
            yield return new WaitForSeconds(0.04f);
        }
        isTyping = false;
        yield return new WaitForSeconds(0.2f);
    }
    public IEnumerator Spare()
    {
        textBox.text = "  *Spare";
        heartImgInv.SetActive(false);
        selectheart.SetActive(true);
        while (true)
        {
            if ((Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.Return)))
            {
                SoundsSource.clip = switch_sound;
                SoundsSource.Play();
                heartImgInv.SetActive(true);
                selectheart.SetActive(false);
                break;
            }
            if (Input.GetKeyUp(KeyCode.X))
            {
                heartImgInv.SetActive(true);
                selectheart.SetActive(false);
                SoundsSource.clip = switch_sound;
                SoundsSource.Play();
                textBox.text = null;
                yield return StartCoroutine(AssignText());
                yield break;
            }
            yield return null;
        }
        fightAct.spares++;
        yield return StartCoroutine(Write("*But He didn't accept."));

    }
    public IEnumerator Item()
    {
        textBox.text = "  *Pie";
        heartImgInv.SetActive(false);
        selectheart.SetActive(true);
        while (true)
        {
            if ((Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.Return)))
            {
                heartImgInv.SetActive(true);
                selectheart.SetActive(false);
                break;
            }
            if (Input.GetKeyUp(KeyCode.X))
            {
                heartImgInv.SetActive(true);
                selectheart.SetActive(false);
                SoundsSource.clip = switch_sound;
                SoundsSource.Play();
                textBox.text = null;
                yield return StartCoroutine(AssignText());
                yield break;
            }
            yield return null;
        }
        SoundsSource.clip = heal;
        SoundsSource.Play();
        if (life < 10)
        {
            life += 10;
            healthslider.value = life;
            hlt.text = life.ToString();
            yield return StartCoroutine(Write("* You recovered 10hp"));
        }
        else
        {
            life = 20;
            healthslider.value = life;
            hlt.text = 20.ToString();
            yield return StartCoroutine(Write("* You're healed"));

        }
        yield return null;
    }
    public IEnumerator Act()
    {
        textBox.text = "  * YEEZUS";
        selectheart.SetActive(true);
        heartImgInv.SetActive(false);
        while (true)
        {
            if ((Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.Return)))
            {
                SoundsSource.clip = switch_sound;
                SoundsSource.Play();
                
                yield return null;
                break;
            }
            if (Input.GetKeyUp(KeyCode.X))
            {
                heartImgInv.SetActive(true);
                selectheart.SetActive(false);
                SoundsSource.clip = switch_sound;
                SoundsSource.Play();
                textBox.text = null;
                yield return StartCoroutine(AssignText());
                yield break;
            }
                yield return null;
        }
        yield return StartCoroutine(ActText());
        if (acted)
        {
            heartImgInv.SetActive(true);
            selectheart.SetActive(false);
            fightAct.acts++;
            if (deaths == 0 && !diedTxt)
            {
                diedTxt = true;
                fightAct.acts=0;
                yield return StartCoroutine(Write("* You called for help."));
                yield return new WaitForSeconds(1);
                yield return StartCoroutine(Write("* But nobody came.."));
                yield return new WaitForSeconds(1);
                yield return StartCoroutine(Write("* soo sad!"));
                yield return null;
            }
            else if (deaths == 1 && !diedTxt)
            {
                diedTxt = true;
                fightAct.acts = 0;
                yield return StartCoroutine(Write("* You told Him that He has \n killed you too many times"));
                yield return new WaitForSeconds(1);
                yield return StartCoroutine(Write("* He nods sadly."));
                yield return null;
            }
            else
            {
                switch (actnum)
                {
                    case 0:
                        yield return StartCoroutine(Write("* You tell YEEZUS that He will \nface the consequences for what \nHe did."));
                        yield return new WaitForSeconds(1);
                        yield return StartCoroutine(Write("* His breathing only gets \nfunny"));
                        break;
                    case 1:
                        yield return StartCoroutine(Write("* You bow to him."));
                        yield return new WaitForSeconds(1);
                        yield return StartCoroutine(Write("* This won't help.."));
                        break;
                    case 2:
                        yield return StartCoroutine(Write("* The Justice fills your soul."));
                        yield return new WaitForSeconds(0.5f);
                        yield return StartCoroutine(Write("* Justice will be served.."));
                        break;
                    case 3:
                        yield return StartCoroutine(Write("* Don't cry there is always \ntomorrow!"));
                        yield return new WaitForSeconds(1);
                        yield return StartCoroutine(Write("* But this time.. \nit's over.."));
                        break;
                    case 4:
                        yield return StartCoroutine(Write("* This is what you'll do \nif you lose."));
                        break;
                    case 5:
                        yield return StartCoroutine(Write("* He agrees to take your soul."));
                        yield return new WaitForSeconds(0.5f);
                        yield return StartCoroutine(Write("* While you get nothing."));
                        break;
                    default:
                        yield return StartCoroutine(Write("* YEEZUS - In blood and flesh \n* The king is dead, \nyet the crown still awaits."));
                        yield return new WaitForSeconds(1.5f);

                        break;
                }
                actnum++;
                yield return null;
            }
        }
        else
        {
            StartCoroutine(Act());
        }
    }
    public IEnumerator ActText()
    {
        acted = false;
        switch (actnum)
        {
            case 0:
                textBox.text = "  * Talk";
                break;
            case 1:
                textBox.text = "  * Bow";
                break;
            case 2:
                textBox.text = "  * Justice";
                break;
            case 3:
                textBox.text = "  * Cry";
                break;
            case 4:
                textBox.text = "  * Burn";
                break;
            case 5:
                textBox.text = "  * Agreement";
                break;
            default:
                textBox.text = "  * Check";
                break;
        }
        while (true)
        {
            if ((Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.Return)))
            {
                SoundsSource.clip = switch_sound;
                SoundsSource.Play();
                heartImgInv.SetActive(true);
                selectheart.SetActive(false);
                acted = true;
                yield break;
            }
            if (Input.GetKeyUp(KeyCode.X))
            {
                SoundsSource.clip = switch_sound;
                SoundsSource.Play();
                textBox.text = null;
                yield break;
            }
            yield return null;
        }
    }
    public IEnumerator Sellection()
    {
        while (oninventory)
        {
            if (Input.GetKeyUp(KeyCode.LeftArrow)) {
                UpdateIndex(-1);
                SoundsSource.clip = switch_sound;
                SoundsSource.Play();
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                UpdateIndex(1);
                SoundsSource.clip = switch_sound;
                SoundsSource.Play();
            }
            if ((Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.Return)))
            {
                if (isTyping)
                {
                    StopCoroutine(textManager);
                    textBox.text = message;
                    isTyping = false;
                }
                SoundsSource.clip = switch_sound;
                SoundsSource.Play();
                oninventory = false;
                yield return new WaitForSeconds(0.1f);
                switch (index)
                {
                    case 0:
                        yield return StartCoroutine(Attack());
                            break;
                    case 1:
                        yield return StartCoroutine(Act());
                        currentLine++;
                        StartCoroutine(DisplayNextLine());
                        break;
                    case 2:
                        yield return StartCoroutine(Item());
                        currentLine++;
                        StartCoroutine(DisplayNextLine());
                        break;
                    case 3:
                        yield return StartCoroutine(Spare());
                        currentLine++;
                        StartCoroutine(DisplayNextLine());
                        break;
                }
                Inventory.SetActive(false);

            }
            if(isTyping && Input.GetKey(KeyCode.X)) {
                StopCoroutine(textManager);
                textBox.text = message;
                isTyping = false;
            }
            yield return null;
        }

    }
}
