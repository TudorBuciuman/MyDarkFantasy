using UnityEngine;
using System.Collections; 
public class SineFire : MonoBehaviour
{
    private float vspeed=-9f; 
    private float s=0;
    public bool add;
    float x=0, y=0;
    void Update()
    {
        s += Time.deltaTime;
        y = vspeed * Time.smoothDeltaTime;
        if (add)
            x =-Mathf.Sin(s/(0.157f)) * 0.11f;
        else
            x=Mathf.Sin(s/(0.157f)) * 0.11f;
        transform.Translate(new Vector3(x, y, 0));
        if (s > 4)
            Destroy(this);
    }
}
