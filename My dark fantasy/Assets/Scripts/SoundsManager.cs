using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public class SoundsManager : MonoBehaviour
{
    public AudioSource moveSource;
    public AudioMixer soundsMixer;
    public AudioMixer musicMixer;
    public AudioSource mouseSource;
    public AudioSource songs;
    [SerializeField]
    public AudioClip[] clip=new AudioClip[10];
    public static string Master = "sounds";
    public static string Music = "soundtrack";
    
    public void Start()
    {
        string settingsPath = Path.Combine(Application.persistentDataPath, "Settings/settings.json");
        string json = File.ReadAllText(settingsPath);
        SettingsData data = JsonUtility.FromJson<SettingsData>(json);
        if (!data.totalsound)
        {
            musicMixer.SetFloat("Master", -80f);
            soundsMixer.SetFloat("Master", -80f);
        }
        else
        {
            musicMixer.SetFloat(Music, data.musiclevel-80);
            soundsMixer.SetFloat(Master, data.movementlevel - 80);
        }
    }

    public void PlaySong(byte id)
    {
        if (!mouseSource.isPlaying)
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Songs");
            StartCoroutine(LoadAndPlayMusic(path, id));
        }
    }

    public void Move(byte id)
    {
        moveSource.clip = clip[id];
        moveSource.Play();
    }
    public void Placement(byte id)
    {
        mouseSource.clip = clip[id];
        mouseSource.Play();
    }
    public void stopmove()
    {
        moveSource.Stop();
    }
    public void stopbreak()
    {
        mouseSource.Stop();
    }
    public void StopSong()
    {
        songs.Stop();
    }

    private IEnumerator LoadAndPlayMusic(string path,byte id)
    {
        string[] musicFiles = Directory.GetFiles(path, "*.ogg");
        using (WWW www = new WWW("file://" + musicFiles[id]))
        {
            yield return www;

            AudioClip clip = www.GetAudioClip();
            if (clip != null)
            {
                songs.clip = clip;
                songs.Play();
            }
        }
    }
}
