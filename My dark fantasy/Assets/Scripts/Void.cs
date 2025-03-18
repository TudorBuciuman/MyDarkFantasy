using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public static float xrot = -60;
    public static float yrot = 0;
    public AudioClip clip,clip1,clip2;
    public AudioSource audioSource,background;
    public Text text;
    public GameObject obj1,obj2;
    public void Awake()
    {
        //inputActions = new NewControls();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine(Inputter());
    }
    public IEnumerator Inputter()
    {
        audioSource.clip = clip1;
        audioSource.Play();
        yield return new WaitForSeconds(3);
        foreach(char c in "You really are an IDIOT")
        {
            text.text += c;
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(3);
        obj2.SetActive(false);
        audioSource.Pause();
        yield return new WaitForSeconds(3);
        obj1.SetActive(false);
        audioSource.clip = clip;
        while (true)
        {
            Manage();
            currentImput = new Vector2(speed * Input.GetAxis("Vertical"), speed * Input.GetAxis("Horizontal"));
            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
            {
                audioSource.loop = true;
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            else
            {
                audioSource.loop = false;
            }
            float movedir = moveDirr.y;
            moveDirr = ((transform.TransformDirection(Vector3.forward) * currentImput.x) + (transform.TransformDirection(Vector3.right) * currentImput.y));
            moveDirr.y = movedir;
            characterController.Move(moveDirr * Time.deltaTime);
            if (transform.position.x < -156)
            {
                audioSource.Stop();
                background.Stop();
                StartCoroutine(Idk());
                yield break;
            }
            yield return null;
        }
    }
    public IEnumerator Idk()
    {
        yield return new WaitForSeconds(2);
        audioSource.Play();
        audioSource.loop = false;
        audioSource.volume = 100;
        yield return new WaitForSeconds(4);
        audioSource.clip = clip2;
        audioSource.loop = true;
        audioSource.Play();
        obj1.SetActive(true);
        yield return new WaitForSeconds(10);
        BloodOnTheLeaves.SceneNum = 7;
        SceneManager.LoadScene("Blood");

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
