using System.Collections;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;

public class SoundsManager : MonoBehaviour
{
    public AudioSource moveSource;
    public AudioMixer soundsMixer;
    public AudioMixer musicMixer;
    public AudioSource mouseSource;
    public AudioSource pickup;
    public AudioSource songs;
    public AudioClip[] clip=new AudioClip[10];
    public static string Master = "sounds";
    public static string Music = "soundtrack";
    public byte nrsongs=10;
    public void Start()
    {
        string settingsPath = Path.Combine(Application.persistentDataPath, "Settings/settings.json");
        string json = File.ReadAllText(settingsPath);
        SettingsData data = JsonUtility.FromJson<SettingsData>(json);
        if (!data.totalsound)
        {
            musicMixer.SetFloat(Music, -80f);
            soundsMixer.SetFloat(Master, -80f);
        }
        else
        {
            float musicVolume = Mathf.Lerp(-80f, 0f, data.musiclevel / 100f); 
            float soundsVolume = Mathf.Lerp(-80f, 0f, data.movementlevel / 100f); 

            musicMixer.SetFloat(Music, musicVolume);
            soundsMixer.SetFloat(Master, soundsVolume);
        }
    }
    public void PlaySong(byte id)
    {
        if (!songs.isPlaying)
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Songs");
            StartCoroutine(LoadAndPlayMusic(path, id));
        }
    }
    public void PlaySceneSong(byte id)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Songs");
        StartCoroutine(LoadAndPlayMusic(path, id));
    }
    public void PlayRandomSound(byte id)
    {
        switch(id)
        {
            case 0:
                moveSource.clip = clip[Random.Range(2,6)];
                moveSource.Play();
                break;
            case 1:
                mouseSource.clip = clip[Random.Range(7,9)];
                moveSource.Play();
                break;
            default:
                break;
        }
        
    }
    public void Move(byte id)
    {
        moveSource.clip = clip[id];
        moveSource.Play();
    }
    public void PlayRandom()
    {
        if(songs.isPlaying)
        songs.Stop();
        else
        PlaySong((byte)(Random.Range(1,nrsongs)));
    }
    public void Placement(byte id)
    {
        mouseSource.clip = clip[id];
        mouseSource.Play();
    }
    public void PickUp()
    {
        pickup.clip = clip[10];
        pickup.Play();
    }
    public void Stopmove()
    {
        moveSource.Stop();
    }
    public void Stopbreak()
    {
        mouseSource.Stop();
    }
    public void StopSong()
    {
        songs.Stop();
    }

    private IEnumerator LoadAndPlayMusic(string path, byte id)
    {
        string musicFilePath;

    #if UNITY_STANDALONE_WIN
        string[] musicFiles = Directory.GetFiles(path, "*.ogg");
        musicFilePath = System.IO.Path.Combine(Application.streamingAssetsPath, $"Songs/song{id}.ogg");
#elif UNITY_ANDROID
        musicFilePath = Path.Combine(Application.streamingAssetsPath, $"song{id}.ogg");
        Debug.Log(musicFilePath);
#endif
        using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(musicFilePath, AudioType.OGGVORBIS);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error loading audio file: " + www.error);
        }
        else
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            if (clip != null)
            {
                songs.clip = clip;
                songs.Play();
            }
        }
    }

}
