using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
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
    private string GetMessage(int r)
    {
        switch (r)
        {
            case 0:return "Don't lose your determination";
            case 1:return "Stay safe";
            case 2:return "The future rests in your hands";
            case 3:return "You're refilled with bravery";
            case 4:return "Don't lose your faith";
            case 5:return "Don't sob, there is always tomorrow";
            case 6:return "Don't lose hope";
            case 7:return "*you're filled with COURAGE";
            case 9: return "Looser!";
            case 10: return "Imagine losing!";
            default:return "determination";
        }
    } 
    private IEnumerator TheWorldNeedsYou()
    {
        int r = Random.Range(0, 15);
        if (r >= 9)
        {
            writing.font = comicSans;
            audioSource.clip = getDunkedOn;
            audioSource.Play();
        }
        else
        {
            audioSource.clip = staydetermined;
            audioSource.Play();
        }
        Toolbar.instance.openedInv = true;
        yield return new WaitForSeconds(3);
        Logo.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.4f);
        writing.text = GetMessage(r);
        writing.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.8f);
        yield return StartCoroutine(WaitingForYou());
        writing.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.4f);
        Logo.gameObject.SetActive(false);
        yield return new WaitForSeconds(5f);
        Toolbar.escape = false;
        Toolbar.instance.openedInv = false;
        HealthSistem.health = 20;
        HealthSistem.istance.ReMakeHearts();
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
    }

}
