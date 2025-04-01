using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Atk4 : MonoBehaviour
{
    public GameObject obj;
    public static Atk4 ts;
    public GameObject parent;
    public GameObject[] set = new GameObject[20];
    int e = 1;
    public void Start()
    {
        ts = this;
    }
    public IEnumerator upd()
    {
        e = Random.Range(1, 3);
        Obj4.e = e;
        StartCoroutine(pt1());
        StartCoroutine(pt2());

        yield return new WaitForSeconds(2.5f);
        yield return StartCoroutine(StartTheFire());
    }
    public IEnumerator pt1()
    {
        if (e == 1)
        {
            float y = 4, x = 3;
            for (int i = 0; i < 10; i++)
            {
                x = (i - 5) * (i - 5) * 0.05f + 3;
                set[i] = Instantiate(obj, new Vector3(x, y, 0), Quaternion.identity, parent.transform);
                y--;
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            float y = 4, x = 3;
            for (int i = 0; i < 10; i++)
            {
                x = (i - 5) * (i - 5) * 0.05f + 3;
                set[i] = Instantiate(obj, new Vector3(y, x, 0), Quaternion.identity, parent.transform);
                y--;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    public IEnumerator pt2()
    {
        if (e == 1)
        {
            float y = -4f, x = -3;
            for (int i = 0; i < 10; i++)
            {
                x = -(i - 5) * (i - 5) * 0.05f - 3;
                set[i + 10] = Instantiate(obj, new Vector3(x, y, 0), Quaternion.identity, parent.transform);

                y++;
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            float y = -4f, x = -3;
            for (int i = 0; i < 10; i++)
            {
                x = -(i - 5) * (i - 5) * 0.05f - 3;
                set[i + 10] = Instantiate(obj, new Vector3(y, x, 0), Quaternion.identity, parent.transform);

                y++;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    public IEnumerator StartTheFire()
    {
        foreach(GameObject g in set)
        {
            StartCoroutine(g.GetComponent<Obj4>().ToGo());
        }
        yield return new WaitForSeconds(3);
    }
}
