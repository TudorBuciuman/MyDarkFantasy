using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    public static bool world = true;
    public static bool active = false;
    public Text writing;
    public Text Logo;
    public Font comicSans;
    public AudioSource audioSource;
    public AudioClip staydetermined, getDunkedOn;

    public void Start()
    {
        audioSource.loop = true;
        StartCoroutine(TheWorldNeedsYou());
    }
    private IEnumerator GetMessage(int r)
    {
        switch (r)
        {
            case 1: 
                yield return StartCoroutine(Writer("You're not supposed\nto be here yet."));
                yield return StartCoroutine(WaitingForYou());
                yield return StartCoroutine(Writer("Please,\nwake up.."));
                yield return StartCoroutine(WaitingForYou());
                break;
            case 2:
                yield return StartCoroutine(Writer("The future rests \n upon you"));
                yield return StartCoroutine(WaitingForYou());
                yield return StartCoroutine(Writer("You are     \nour last hope"));
                yield return StartCoroutine(WaitingForYou());
                break; 
            case 3:
                yield return StartCoroutine(Writer("He's watching now. \nDon't turn around!"));
                yield return StartCoroutine(WaitingForYou());
                yield return StartCoroutine(Writer("[REDACTED] knows\nyou gave up."));
                yield return StartCoroutine(WaitingForYou());
                break; 
            case 4:
                yield return StartCoroutine(Writer("He's coming for you.\n[REDACTED] is angry"));
                yield return StartCoroutine(WaitingForYou());
                yield return StartCoroutine(Writer("... "));
                yield return StartCoroutine(WaitingForYou());
                yield return StartCoroutine(Writer("...can anyone hear me? \nHelp..."));
                yield return StartCoroutine(WaitingForYou());
                break;
            case 5:
                yield return StartCoroutine(Writer("He has died long ago. \nBy his own hand."));
                yield return StartCoroutine(WaitingForYou());
                yield return StartCoroutine(Writer("So will you"));
                yield return StartCoroutine(WaitingForYou());
                break;
            case 6:
                yield return StartCoroutine(Writer("All hope is lost."));
                yield return StartCoroutine(WaitingForYou());
                yield return StartCoroutine(Writer("We will never\nbe free.."));
                yield return StartCoroutine(WaitingForYou());
                break;
            case 7:
                yield return StartCoroutine(Writer("Imagine losing!"));
                yield return StartCoroutine(WaitingForYou());
                break;
            case 8:
                yield return StartCoroutine(Writer("You are the pawn"));
                yield return StartCoroutine(WaitingForYou());
                yield return StartCoroutine(Writer("worthless"));
                yield return StartCoroutine(WaitingForYou());
                break;
            default:
                yield return StartCoroutine(Writer("You're not supposed\nto be here yet."));
                yield return StartCoroutine(WaitingForYou());
                yield return StartCoroutine(Writer("Please,\nwake up.."));
                yield return StartCoroutine(WaitingForYou());
                break;

        }
    } 
    private IEnumerator Writer(string s)
    {
        foreach (char c in s)
        {
            writing.text += c;
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }
    private IEnumerator TheWorldNeedsYou()
    {
        active = true;
        int r = Random.Range(0, 15);
        if (r >= 9)
        {
            writing.font = comicSans;
            audioSource.clip = getDunkedOn;
        }
        else
        {
            audioSource.clip = staydetermined;
        }
        if (world)
        {
            Toolbar.instance.openedInv = true;
            audioSource.Play();
        }
            yield return new WaitForSeconds(3);
        Logo.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.4f);
        writing.gameObject.SetActive(true);
        yield return StartCoroutine(GetMessage(Voxeldata.PlayerData.deaths));
        writing.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.4f);
        Logo.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);
        if (world)
        {
            Toolbar.escape = false;
            Toolbar.instance.openedInv = false;
            HealthSistem.health = Voxeldata.PlayerData.love*4+16;
            HealthSistem.istance.ReMakeHearts();
            ControllerImput.Instance.currentVelocity = Vector3.zero;
        }
        active = false;
        SceneManager.UnloadSceneAsync("DeathScreen");
        }
    private IEnumerator WaitingForYou()
    {
        yield return new WaitForSeconds(1.7f); 
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                break;  
            yield return new WaitForSeconds(0.01f);  
        }

        yield return null;
    }

}
