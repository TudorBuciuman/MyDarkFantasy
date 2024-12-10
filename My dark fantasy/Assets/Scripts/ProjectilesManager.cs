using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectilesManager : MonoBehaviour
{
    public static float speed = 150f;
    public Vector2 direction; 
    public Image dis;
    public void Start()
    {
        dis = GetComponent<Image>();
    }

    void FixedUpdate()
    {
        dis.rectTransform.anchoredPosition+=direction * speed * Time.fixedDeltaTime;
    }
}
