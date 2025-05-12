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
    }
    public void Starting()
    {
        if(Voxeldata.PlayerData.scene==2)
            TTG.sprite = logoBlackndWhite;
        StartCoroutine(PlayOnSight());
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
