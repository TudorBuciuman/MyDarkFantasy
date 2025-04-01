using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownWave : MonoBehaviour
{
    float t = 0;
    public bool sinusoid = false;
    public bool up = true;
    private float k;

    public void Start()
    {
        k = Random.Range(0, 3.14f);
        t = 0;
    }
    public void Update()
    {
        t += Time.deltaTime;
        float r = -t * 0.6f;
        if (!up)
            r = -r;

        if(sinusoid)
            transform.Translate(new Vector2(Mathf.Sin(t+k)/100,-t*0.05f));
        else
            transform.Translate(new Vector2(Mathf.Cos(t * 2 + 2) / 200, r));

        if (t > 4f)
            Destroy(gameObject);
    }
}
