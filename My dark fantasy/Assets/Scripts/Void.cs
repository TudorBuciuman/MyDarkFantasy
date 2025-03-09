using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Void : MonoBehaviour
{
    public static float sensivity=40;
    public static float speed=3;
    public Transform orientation,playerCamera;
    public NewControls inputActions;
    public FixedTouchField _FixedTouchField;
    public CharacterController characterController;
    private Vector2 currentImput;
    private Vector3 moveDirr;
    public static float xrot = 0;
    public static float yrot = 0;
    public void Awake()
    {
        //inputActions = new NewControls();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        Manage();
        currentImput = new Vector2(speed * Input.GetAxis("Vertical"), speed * Input.GetAxis("Horizontal"));
        float movedir = moveDirr.y;
        moveDirr=((transform.TransformDirection(Vector3.forward)*currentImput.x)+(transform.TransformDirection(Vector3.right)*currentImput.y));
        moveDirr.y=movedir;
        characterController.Move(moveDirr*Time.deltaTime);
    }
    void AndroidMovement()
    {
        Vector2 a = (Time.smoothDeltaTime * sensivity * 0.1f) * _FixedTouchField.TouchDist;
        xrot -= a.y;
        yrot += a.x;
        xrot = Mathf.Clamp(xrot, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xrot, 0, 0);
        orientation.Rotate(Vector3.up * a.x);
    }

    void Manage()
    {
        xrot -= Input.GetAxis("Mouse Y")*2;
        xrot = Mathf.Clamp(xrot, -90, 90);
        playerCamera.transform.localRotation = Quaternion.Euler(xrot, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X")*2, 0);
    }
}
