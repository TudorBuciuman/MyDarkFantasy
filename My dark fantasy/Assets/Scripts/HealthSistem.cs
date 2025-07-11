using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthSistem : MonoBehaviour
{
    public static float health;
    public static HealthSistem istance;
    private float maxHealth=20;
    public Slider healthslider;
    public Text healthLabel,love;  
    public SoundsManager soundsManager;
    public AudioClip heal, dammage;
    public AudioSource source;
    //love= level of violence
    private float regeneration;
    public void Start()
    {
        istance = this;
        if (Voxeldata.PlayerData.scene == 0)
        {
            love.text = (1).ToString();
            Voxeldata.PlayerData.love = 1;
        }
        else
            love.text = Voxeldata.PlayerData.love.ToString();
        maxHealth = 16 + Voxeldata.PlayerData.love * 4;
    }
    public void ScreenOfDeath()
    {
        soundsManager.StopSong();
        soundsManager.Stopmove();
        soundsManager.Stopbreak();
        StartCoroutine(TheWorldNeedsYou());
    }

    private IEnumerator TheWorldNeedsYou()
    {
        if (Voxeldata.PlayerData.deaths == 0)
        {
            health=maxHealth;
            Toolbar.instance.SaveProgress();
            Voxeldata.PlayerData.deaths++;
            BloodOnTheLeaves.SceneNum = 0;
            SceneManager.LoadScene("Blood");
        }
        else
        {
            Voxeldata.PlayerData.deaths++;
            PlayerDataData.SavePlayerFile();
            Toolbar.escape = true;
            SceneManager.LoadScene("DeathScreen", LoadSceneMode.Additive);
            yield return new WaitForSeconds(1);
        }
    }
    public void Heal(float amount)
    {
        UpdateHealth(amount);
    }
    public void UpdateHealth(float amount)
    {
        health += amount;
        if (amount > 0)
        {
            source.clip = heal;
            source.Play();
        }
        else
        {
            source.clip = dammage;
            source.Play();
        }
        if (health <= 0)
        {
            health = 0;
            ReMakeHearts();
            Toolbar.escape = true;
            if(!DeathScreen.active)
            ScreenOfDeath();
        }
        else
            ReMakeHearts();
    }
    public void ReMakeHearts()
    {
        int hlt = Mathf.RoundToInt( (float)(health / maxHealth)*20);
        if(hlt==0 && health > 0)
        {
            hlt = 1;
        }
        healthslider.value = hlt;
        healthLabel.text=$"{hlt} / {maxHealth}".ToString();
    }
    public void Protocol()
    {
        health = 0;
        ReMakeHearts();
        StartCoroutine(KnifeDeath());
    }
    public IEnumerator SlowlyDeath()
    {
        while (health > 0)
        {
            UpdateHealth(-1);
            yield return new WaitForSeconds(5);
        }
    }
    private IEnumerator KnifeDeath()
    {
        health = maxHealth;
        Toolbar.instance.SaveProgress();
        Voxeldata.PlayerData.special = 100;
        PlayerDataData.SavePlayerFile();
        yield return new WaitForSeconds(1);
        soundsManager.StopSong();
        SceneManager.LoadScene("Fight");
        yield return null;
    }
}
