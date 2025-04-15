using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectilesManager : MonoBehaviour
{
    public static float speed = 150f;
    public Vector3 direction; 

    void FixedUpdate()
    {
        transform.position+=direction * speed * Time.fixedDeltaTime;
    }
}
