using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obj4 : MonoBehaviour
{
    private float xpos,ypos,finx;
    public static int e = 1;
    void Start()
    {
        xpos = transform.position.x;
        ypos = transform.position.y;
        if (e == 1)
        {
            if (xpos < 0)
            {
                finx = 8.5f + xpos;
            }
            else
                finx = xpos - 8.5f;
        }
        else{
            if (ypos < 0)
            {
                finx = 8.5f + ypos;
            }
            else
                finx = ypos - 8.5f;
        }

    }

    public IEnumerator ToGo()
    {
        if (e == 1)
        {
            float t = 0;
            while (transform.position.x != finx)
            {
                t += Time.deltaTime;
                transform.position = new Vector2(Mathf.Lerp(xpos, finx, t / 3), Mathf.Lerp(ypos, -ypos, t / 3));
                yield return null;
            }
        }
        else
        {
            float t = 0;
            while (transform.position.y != finx)
            {
                t += Time.deltaTime;
                transform.position = new Vector2(Mathf.Lerp(xpos, -xpos, t / 3), Mathf.Lerp(ypos, finx, t / 3));
                yield return null;
            }
        }
    }
}
