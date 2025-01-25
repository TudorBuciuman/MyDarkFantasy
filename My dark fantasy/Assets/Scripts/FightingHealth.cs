using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FightingHealth : MonoBehaviour
{
    // Start is called before the first frame update
    public float health=20;
    public float damageAmount=3;
    public Image heart;
    public float timepassed = 0;
    public static float sprintspeed=1;
    public void FixedUpdate()
    {
        if (timepassed > 0)
            timepassed-= Time.fixedDeltaTime;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            if(heart.rectTransform.anchoredPosition.y>-228)
            heart.rectTransform.anchoredPosition +=new Vector2(0,-4f)*sprintspeed;
        }
        else if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            if (heart.rectTransform.anchoredPosition.y <228)
                heart.rectTransform.anchoredPosition += new Vector2(0, 4f)*sprintspeed;

        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (heart.rectTransform.anchoredPosition.x < 228)
                heart.rectTransform.anchoredPosition += new Vector2(4f,0)*sprintspeed;

        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (heart.rectTransform.anchoredPosition.x >- 228)
                heart.rectTransform.anchoredPosition += new Vector2(-4f,0)*sprintspeed;
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("obstacle"))
        {
            if (timepassed<=0)
            {
                FightSistem.life-= damageAmount;
                timepassed = 0;
                if (FightSistem.life <= 0)
                {
                    SceneManager.LoadSceneAsync("DeathScreen",LoadSceneMode.Additive);
                    DeathScreen.world = false;
                    FightSistem.life = 20;
                }
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("obstacle"))
        {
            if (timepassed <= 0)
            {
                health -= damageAmount;
                timepassed = 2;
            }
        }
    }

}
