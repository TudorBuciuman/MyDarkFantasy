using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Atk3 : MonoBehaviour
{
    public GameObject obj;
    public Sprite red, yellow;
    public GameObject warning;
    public GameObject parent;
    public SpriteRenderer warnsign;
    private float t;
    private byte e=0;
    public static Atk3 ts;
    private void Start()
    {
        ts = this;
    }
    public IEnumerator upd()
    {
        float h = 1.3f;
        e = 0;
        t = 1.3f;
        while (t < 8)
        {
            if (h > 2) {
                h = 0;
                StartCoroutine(Warning());
            }
            h += Time.deltaTime;
            t += Time.deltaTime;
            yield return null;
        }
        StopAllCoroutines();
        yield break;
    }
    public IEnumerator Warning()
    {
        StartCoroutine(Sinusator());
        float xpos = -3.9f;
        if (e % 2 == 0)
        {
            warning.transform.position = new Vector2(xpos,0);
        }
        else
        {
            warning.transform.position = new Vector2(0, 0);
        }
        e++;
        yield return Switching();

            float posx = e % 2 ==1 ? -2.5f : 2.4f;
            for (float i = -1.2f; i <= 1.2; i+=0.4f)
            {
                GameObject fire = Instantiate(obj, new Vector3(posx +i+ Random.Range(-0.9f, 0.9f), 2.8f +Random.Range(-0.8f,0.7f), 0), Quaternion.identity, parent.transform);
                fire.GetComponent<DownWave>().sinusoid = false;
            }
        yield return new WaitForSeconds(0.2f);
        for (float i = -1.2f; i <= 1.2; i += 0.4f)
        {
            GameObject fire = Instantiate(obj, new Vector3(posx + i+Random.Range(-0.9f, 0.9f)+1, 2.8f+ Random.Range(-0.7f, 0.8f), 0), Quaternion.identity, parent.transform);
            fire.GetComponent<DownWave>().sinusoid = false;

        }

        for (float i = -1.2f; i <= 1.2; i += 0.4f)
        {
            GameObject fire = Instantiate(obj, new Vector3(posx + i+ Random.Range(-0.9f, 0.9f) , -2.79f+ Random.Range(-0.8f, 0.7f), 0), Quaternion.identity, parent.transform);
            fire.GetComponent<DownWave>().sinusoid = false;
            fire.GetComponent<DownWave>().up = false;
        }
        yield return new WaitForSeconds(0.2f);
        for (float i = -1.2f; i <= 1.2; i += 0.4f)
        {
            GameObject fire = Instantiate(obj, new Vector3(posx + i + Random.Range(-0.9f, 0.9f)-1, -2.79f + Random.Range(-0.8f, 0.7f), 0), Quaternion.identity, parent.transform);
            fire.GetComponent<DownWave>().sinusoid = false;
            fire.GetComponent<DownWave>().up = false;
        }
        yield break;
    }
    public IEnumerator Sinusator()
    {
        byte q = 0;
        while (q < 3)
        {
            GameObject fire = Instantiate(obj, new Vector3(Random.Range(-0.3f, 0.3f), 2.8f, 0), Quaternion.identity, parent.transform);
            fire.GetComponent<DownWave>().sinusoid = true;
            q++;
            yield return new WaitForSeconds(0.5f);
        }

        yield break;
    }
    public IEnumerator Switching()
    {
        warnsign.sprite= red;
        warning.SetActive(true);
        float q = 0,h=0f;
        bool l = true;
        while (q < 0.4)
        {
            if (h > 0.1f)
            {
                if (l)
                {
                    warnsign.sprite = yellow;
                    l=false;
                }
                else
                {
                    warnsign.sprite = red;
                    l = true;
                }
                h = 0;
            }
            h += Time.deltaTime;
            q += Time.deltaTime;
            yield return null;
        }
        warning.SetActive(false);
    }
}
