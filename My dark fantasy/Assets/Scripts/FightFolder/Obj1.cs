using UnityEngine;
public class SineFire : MonoBehaviour
{
    public float vspeed; 
    public float s;
    public bool add;
    float x=0, y=0;
    void Update()
    {
            s += Time.deltaTime;
            y= vspeed*Time.deltaTime;
        if (add)
            x =-Mathf.Sin(s/(0.157f)) * 0.11f;
        else
            x=Mathf.Sin(s/(0.157f)) * 0.11f;
        transform.Translate(new Vector3(x, y, 0));
        if (s > 3)
            Destroy(this);
    }
}
