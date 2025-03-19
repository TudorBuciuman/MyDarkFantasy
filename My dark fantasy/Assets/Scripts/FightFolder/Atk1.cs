using System.Collections;
using UnityEngine;

public class Atk1 : MonoBehaviour
{
    float[] alarm = new float[3];
    float s = 0;
    float off = 0;
    public GameObject sineFirePrefab;
    public GameObject parent;
    public static Atk1 ts;
    public GameObject sideDamPrefab; 
    public float globalTurntimer=5; 
    public float[] idealBorder = new float[2];

    void Start()
    {
        ts = this;
        alarm[0] = 0;
        alarm[1] = 0;
        s = Random.Range(0, 360); 
    }
    float posx=-2,posy=0,px=3f,py=0.5f;
    public IEnumerator upd()
    {
        float t = 0,k=0;
        while (t < 15)
        {
            k += Time.deltaTime;
            alarm[0] += Time.deltaTime;
            alarm[1] += Time.deltaTime;
            alarm[2] += Time.deltaTime;
            if (alarm[0] > 0)
            {
                if (k>0 && k < 1.5f)
                {
                    alarm[0] = -0.05f;
                    OnAlarm0(posx,posy);
                }
                else if(k>1.5)
                {
                    posx =Random.Range(-0.3f,0.2f)-2;
                    posy += 0.3f;
                    py += 0.3f;
                    px = posx+5;
                    k = -0.7f;
                    s += 17.8f;
                }
            }

            if (alarm[1] > 0)
            {
                if (k>(-0.1f) && k < 1.4f)
                {
                    alarm[1] = -0.05f;
                    OnAlarm0(px, py);
                }
            }
            if (alarm[2] > 0)
            {
                if (k > (0.2f) && k < 1.6f)
                {
                    alarm[2] = -0.05f;
                    OnAlarm0(px-2, py+0.3f);
                }
            }
            t += Time.deltaTime;
            yield return null;
        }
    }

    void OnAlarm0(float posx,float posy)
    {
        s += Time.deltaTime;
        for (int i = 1; i <= 2; i++)
        {
            float xPos = posx;
            float yPos = posy+5 +Mathf.Sin(off / 17) + Mathf.Sin(off / 17) ;
            if (i == 2)
            {
                xPos = posx-2;
            }
            GameObject fire = Instantiate(sineFirePrefab, new Vector3(xPos, yPos, 0), Quaternion.identity, parent.transform);
            fire.GetComponent<SineFire>().add = (i == 1);
        }
        off+=Time.deltaTime;

    }
}
