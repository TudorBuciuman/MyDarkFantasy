using UnityEngine;

public class MouseController : MonoBehaviour
{

    public static float sensivity;
    public Transform orientation;
    public NewControls inputActions;
    public Toolbar toolbar;
    public FixedTouchField _FixedTouchField;

    public static float xrot = 0;
    public static float yrot = 0;
    public void Awake()
    {
        inputActions = new NewControls();
    }
    void Update()
    {
        if (!toolbar.openedInv)
        Manage();
        //AndroidMovement();
        //pentru android trebuie folosita prima si pentru pc a doua
    }
    void AndroidMovement()
    {
        Vector2 a = (Time.smoothDeltaTime * sensivity*0.1f) * _FixedTouchField.TouchDist;
        xrot -= a.y;
        yrot += a.x;
        xrot = Mathf.Clamp(xrot, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xrot, 0, 0);
        orientation.Rotate(Vector3.up * a.x);
    }

    void Manage()
    {
        float x = Input.GetAxis("Mouse X") * Time.smoothDeltaTime * sensivity;
        float y = Input.GetAxis("Mouse Y") * Time.smoothDeltaTime * sensivity;
        xrot -= y;
        yrot += x;

        xrot = Mathf.Clamp(xrot, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xrot, 0, 0);
        orientation.Rotate(Vector3.up * x);
    } 
}
