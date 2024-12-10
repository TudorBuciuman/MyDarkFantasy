using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthSistem : MonoBehaviour
{
    public static float health;
    public static HealthSistem istance;
    public GameObject hearts;
    public GameObject semihearts;
    public Transform heartParent;
    private float maxHealth=20;
    private float love;
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
        foreach (Transform child in heartParent)  
            Destroy(child.gameObject);
        for (int i = 1; i <= health; i++) 
            Instantiate(hearts, heartParent);
        if (Mathf.RoundToInt(health) > health)
        {
            Instantiate(semihearts, heartParent);
        }
        if((int)health==0 && health > 0)
        {
            Instantiate(semihearts, heartParent);
        }
    }
}
