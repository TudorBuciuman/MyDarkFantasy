using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FightingHealth : MonoBehaviour
{
    public float health=20;
    public GameObject ts; 
    public float damageAmount=3;
    public GameObject heart;
    public static float speed = 3f;
    public float timepassed = 0;
    public static float sprintspeed=1;

    void FixedUpdate()
    {
        if(timepassed > 0) 
        timepassed-= Time.deltaTime;
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            movement += Vector3.up; 
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            movement += Vector3.down; 
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            movement += Vector3.left; 
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            movement += Vector3.right; 
        }

        transform.position += speed * Time.deltaTime * movement.normalized;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("obstacle"))
        {
            if (timepassed<=0)
            {
                FightSistem.life-= damageAmount;
                FightSistem.instance.healthslider.value = FightSistem.life;
                FightSistem.instance.hlt.text = FightSistem.life.ToString();
                FightSistem.instance.PlayDamageSound();

                timepassed = 2;
                StartCoroutine(Heartdammage());
                if (FightSistem.life <= 0)
                {
                    timepassed = 100000000;
                    StartCoroutine(FightSistem.instance.PlayDeathMusic());
                }
            }
        }
    }
    public IEnumerator Heartdammage()
    {
        GetComponent<SpriteRenderer>().color= new Color32(118,0,0,255);
        yield return new WaitForSeconds(0.3f);
        GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        yield return new WaitForSeconds(0.3f);
        GetComponent<SpriteRenderer>().color = new Color32(118, 0, 0, 255);
        yield return new WaitForSeconds(0.3f);
        GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);

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
