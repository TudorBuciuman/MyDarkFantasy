using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    public Image logo,TTG;
    public Sprite logoBlackndWhite;
    public Text Text;
    public AudioClip boomsound,fightsound;
    public AudioSource AudioSource;
    public void Awake()
    {
        Application.targetFrameRate = 30;
        OnAppOpened.pressed = false;
    }
    public void Starting()
    {
        if (Voxeldata.PlayerData.scene==2)
            TTG.sprite = logoBlackndWhite;
        Voxeldata.PlayerData.deaths=1;
        Voxeldata.PlayerData.special=0;
        PlayerDataData.SavePlayerFile();
        switch (Voxeldata.PlayerData.special)
        {
            case 1:
                BloodOnTheLeaves.SceneNum = 0;
                SceneManager.LoadScene("Blood");
                break;
            case 2:
                BloodOnTheLeaves.SceneNum = 1;
                SceneManager.LoadScene("Blood");
                break;
            case 3:
                BloodOnTheLeaves.SceneNum = 2;
                SceneManager.LoadScene("Blood");
                break;
            case 4:
                BloodOnTheLeaves.SceneNum = 3;
                SceneManager.LoadScene("Blood");
                break;
            case 5:
                BloodOnTheLeaves.SceneNum = 4;
                SceneManager.LoadScene("Blood");
                break;
            case 6:
                BloodOnTheLeaves.SceneNum = 5;
                SceneManager.LoadScene("Blood");
                break;
            case 7:
                BloodOnTheLeaves.SceneNum = 6;
                SceneManager.LoadScene("Blood");
                break;
            case 8:
                BloodOnTheLeaves.SceneNum = 7;
                SceneManager.LoadScene("Blood");
                break;
            case 9:
                BloodOnTheLeaves.SceneNum = 8;
                SceneManager.LoadScene("Blood");
                //Hell of a life
                break;
            case 10:
                BloodOnTheLeaves.SceneNum = 9;
                SceneManager.LoadScene("Blood");
                //Blame Game
                break;
            case 11:
                SceneManager.LoadScene("Waterfall");
                break;

            //case 12 is runaway
            //case 13 is used for displaying the castle coords
            case 100:
                SceneManager.LoadScene("Fight");
                break;
            default:
                StartCoroutine(PlayOnSight());
                break;
        }
    }
    private IEnumerator PlayOnSight()
    {
        yield return Waiting(2.3f);
        TTG.gameObject.SetActive(true);
        yield return Waiting(2f);
        TTG.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlaySound();
        yield return Waiting(2.6f);
        Text.text = "presents";
        Text.gameObject.SetActive(true);
        yield return Waiting(2f);
        Text.gameObject.SetActive(false);
        yield return Waiting(2.5f);
        logo.gameObject.SetActive(true);
        yield return Waiting(4f);
        logo.gameObject.SetActive(false);
        PlaySound();
        yield return Waiting(2f);
        Text.text = "made by B.Tudor";
        Text.gameObject.SetActive(true);
        yield return Waiting(2.5f);
        Text.gameObject.SetActive(false);
        PlaySound();
        yield return Waiting(6f);
        PlayerDataData.Intro_Fighting();
    }

    private IEnumerator Waiting(float n)
    {
        yield return new WaitForSeconds(n);
    }

    public void PlaySound()
    {
        AudioSource.clip = boomsound;
        AudioSource.Play();
    }
    public void PlayWarSound()
    {
        AudioSource.clip = fightsound;
        AudioSource.Play();
    }
}
