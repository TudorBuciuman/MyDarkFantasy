using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFire : MonoBehaviour
{
    public int a;
    float t=0;
    float px, py;
    float x, y;
    public void Start()
    {
        px = transform.position.x;
        py = transform.position.y;
        StartCoroutine(C_Fire());
    }

    public IEnumerator C_Fire()
    {
        float t = 0; 
        while (t < 2f)
        {
            t += Time.deltaTime; 
            transform.position = new Vector2(Mathf.Lerp(px, 0, t / 2f), Mathf.Lerp(py, 0, t / 2f));
            yield return null; 
        }
        Destroy(gameObject);
    }

}
