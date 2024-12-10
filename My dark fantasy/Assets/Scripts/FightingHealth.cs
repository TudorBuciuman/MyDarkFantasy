using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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
        if (Input.GetKey(KeyCode.S))
        {
            if(heart.rectTransform.anchoredPosition.y>-228)
            heart.rectTransform.anchoredPosition +=new Vector2(0,-4f)*sprintspeed;
        }
        else if(Input.GetKey(KeyCode.W))
        {
            if (heart.rectTransform.anchoredPosition.y <228)
                heart.rectTransform.anchoredPosition += new Vector2(0, 4f)*sprintspeed;

        }
        if (Input.GetKey(KeyCode.D))
        {
            if (heart.rectTransform.anchoredPosition.x < 228)
                heart.rectTransform.anchoredPosition += new Vector2(4f,0)*sprintspeed;

        }
        if (Input.GetKey(KeyCode.A))
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
                health -= damageAmount;
                timepassed = 2;
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
