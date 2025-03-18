using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Atk2 : MonoBehaviour
{
    float s = 0;
    float off = 0;
    public GameObject C_fire;
    public GameObject parent;
    public static Atk2 ts;
    public float globalTurntimer = 5;
    public float[] idealBorder = new float[2];

    void Start()
    {
        ts = this;
    }
    int q = 0;
    public IEnumerator upd()
    {
        q = Random.Range(0, 36);
        float t = 0, k = 0;
        while (t < 6)
        {
            if (k > 1)
            {
                Fire();
                k = 0;
            }
            k += Time.deltaTime;
            t += Time.deltaTime;
            yield return null;
        }
    }
    public void Fire()
    {
        int r = q+Random.Range(-10,10);
        for (int i = 0; i < 36; i++)
        {
            float angle = 8 * (i+r%36); 
            float radians = angle * Mathf.Deg2Rad;
            GameObject fire = Instantiate(
                C_fire,
                new Vector3(Mathf.Cos(radians) * 6.6f, Mathf.Sin(radians) * 6.6f, 0),
                Quaternion.identity,
                parent.transform
            );
            
        }

    }
}
