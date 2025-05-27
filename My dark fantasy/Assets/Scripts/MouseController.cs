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
        if (!toolbar.openedInv && !Toolbar.escape)
           Manage(); 
        /*
        if (Input.GetKey(KeyCode.Alpha0))
        {
            orientation.transform.localEulerAngles=new Vector3(0,0,0);
        }
        else if(Input.GetKey(KeyCode.Alpha1))
        {
            orientation.transform.localEulerAngles = new Vector3(0, 90, 0);

        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            orientation.transform.localEulerAngles = new Vector3(0, 180, 0);

        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            orientation.transform.localEulerAngles = new Vector3(0, -90, 0);

        }
        */
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
        //float x = Input.GetAxis("Mouse X") * Time.smoothDeltaTime * sensivity;
        float x = Input.GetAxis("Mouse X") * Time.fixedDeltaTime * sensivity;
        //float y = Input.GetAxis("Mouse Y") * Time.smoothDeltaTime * sensivity;
        float y = Input.GetAxis("Mouse Y") * Time.fixedDeltaTime * sensivity;
        xrot -= y;
        yrot += x;

        xrot = Mathf.Clamp(xrot, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xrot, 0, 0);
        orientation.Rotate(Vector3.up * x);
    } 
}
