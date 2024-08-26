using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{

    public static float sensivity = 400;
    public Transform orientation;
    public Toolbar toolbar;
    public static float xrot = 0;
    public static float yrot = 0;
    void Update()
    {
        if(!toolbar.openedInv)
        Manage();   
    }
    void Manage()
    {
        float x = Input.GetAxis("Mouse X") * Time.smoothDeltaTime * sensivity;
        float y = Input.GetAxis("Mouse Y") * Time.smoothDeltaTime * sensivity;
        xrot -= y;
        yrot += x;

        xrot = Mathf.Clamp(xrot, -90f, 90f);
        transform.localRotation=Quaternion.Euler(xrot,0,0);
        orientation.Rotate(Vector3.up*x);
    } 
}
