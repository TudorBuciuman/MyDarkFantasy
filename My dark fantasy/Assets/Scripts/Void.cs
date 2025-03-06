 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Void : MonoBehaviour
{
    public static float sensivity=40;
    public static float speed=5;
    public Image img,blk;
    public AudioClip clip;
    public AudioSource source;
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
    bool cor = false;
    void Update()
    {
        Manage();
        currentImput = new Vector2(speed * Input.GetAxis("Vertical"), speed * Input.GetAxis("Horizontal"));
        float movedir = moveDirr.y;
        moveDirr=((transform.TransformDirection(Vector3.forward)*currentImput.x)+(transform.TransformDirection(Vector3.right)*currentImput.y));
        moveDirr.y=movedir;
        characterController.Move(moveDirr*Time.deltaTime);
        if (transform.position.x < -188 && !cor)
        {
            cor=true;
            StartCoroutine(NextScene());
        }
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
    public IEnumerator NextScene()
    {
        source.clip=clip;
        source.Play();
        img.gameObject.SetActive(true);
        blk.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(MakeDark(10));
        BloodOnTheLeaves.SceneNum = 7;
        SceneManager.LoadScene("Blood");

    }
    public IEnumerator MakeDark(float time)
    {

        Color startColor = img.color;

        Color targetColor = Color.black;

        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            img.color = Color.Lerp(startColor, targetColor, elapsedTime / time);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        img.color = targetColor;
    }
}
