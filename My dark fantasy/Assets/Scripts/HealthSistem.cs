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
    private float love;
    public Slider healthslider;
    public Text healthLabel;  
    public SoundsManager soundsManager;
    //love= level of violence
    private float regeneration;
    public void Start()
    {
        istance = this;
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
        Toolbar.escape = true;
        SceneManager.LoadScene("DeathScreen", LoadSceneMode.Additive);
        yield return new WaitForSeconds(1);

    }
    public void UpdateHealth(float amount)
    {
        health += amount;
        if (health < 0)
        {
            health = 0;
            ReMakeHearts();
            Toolbar.escape = true;
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
        healthLabel.text=$"{hlt}/{maxHealth}".ToString();
    }
}