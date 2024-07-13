using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{

    private float sensivity = 300;
    public Transform orientation;
    public Toolbar toolbar;
    public float xrot = 0;
    public float yrot = 0;
    void Update()
    {
        if(!toolbar.openedInv)
        Manage();   
    }

    void Manage()
    {
        float x = Input.GetAxis("Mouse X") * Time.deltaTime * sensivity;
        float y = Input.GetAxis("Mouse Y") * Time.deltaTime * sensivity;
        xrot -= y;
        yrot += x;

        xrot = Mathf.Clamp(xrot, -90f, 90f);
        transform.localRotation=Quaternion.Euler(xrot,0,0);
        orientation.Rotate(Vector3.up*x);
    } 
}
