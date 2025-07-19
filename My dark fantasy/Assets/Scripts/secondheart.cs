using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class secondheart : MonoBehaviour
{
    public static secondheart instance;
    public static float speed = 90;
    private float x, y;

    void Start()
    {
        instance = this;
        x = GetComponent<Image>().rectTransform.position.x;
        y = GetComponent<Image>().rectTransform.position.y;
    }

    void FixedUpdate()
    {
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
        Vector3 p = GetComponent<Image>().rectTransform.position + speed * Time.deltaTime * movement.normalized;
        if(p.y<y+129 && p.y>y-129 && p.x>x-158 && p.x<x+158)
        transform.position += speed * Time.deltaTime * movement.normalized;
    }

    public IEnumerator Starting()
    {
        GetComponent<Image>().color = new Color32(118, 0, 0, 255);
        yield return new WaitForSeconds(0.1f);
        GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        yield return new WaitForSeconds(0.1f);
        GetComponent<Image>().color = new Color32(118, 0, 0, 255);
        yield return new WaitForSeconds(0.1f);
        GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    }
}
