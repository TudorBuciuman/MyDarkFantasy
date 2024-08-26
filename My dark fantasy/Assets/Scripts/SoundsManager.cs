using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

public class SoundsManager : MonoBehaviour
{
    public AudioMixer audioSource;
    [SerializeField]
    public AudioClip[] clip=new AudioClip[10];
    public Scrollbar scrollbar;
    public string Master = "MusicVolume";
    public void Start()
    {
        audioSource.SetFloat(Master, PlayerPrefs.GetFloat(Master));
        GameObject.DontDestroyOnLoad(gameObject);
    }
    public void Change()
    {
        PlayerPrefs.SetFloat(Master,scrollbar.value);
        audioSource.SetFloat(Master, PlayerPrefs.GetFloat(Master));
    }

    public void Play(byte id)
    {
        GetComponent<AudioSource>().clip = clip[id];
        GetComponent<AudioSource>().Play();
    }
    public void stop()
    {
        GetComponent<AudioSource>().Stop();
    }
}
