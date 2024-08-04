using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public WorldManager wmanager;
  
    public Toolbar toolbar;
    public Transform cam;
    public float movementSpeed = 5f;
    public float sprintspeed = 8f;
    public float lookSpeed = 400f;
    public float smoothSpeed = 100.0f;
    public float gravity = -9.8f;
    public Transform orientation;
    public float speed = 50f;
    public float jump = 6f;
    public CharacterController controller;
    public GameObject box;

    private bool grounded=true;
    private Vector3 Position;
    private Vector3 moveDirection = Vector3.zero;
    private float pozX = 0f;
    private float pozZ = 0f;
    private float verticalMomentum=0;
    private bool sprint = false;
    private bool cansprint=false;
    private bool jumpQm = false;
    public Text Pos;

    void Start()
    {
        Application.targetFrameRate = 60;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        controller.transform.position = new Vector3(12.0f,80f,4.0f);
    }

    void Update()
    {
        PlayerControll();
    }
    public void PlayerControll()
    {
        if (!toolbar.escape)
        {
            moveDirection.y -= gravity * Time.deltaTime;
            if (!toolbar.openedInv)
            {
                HandleMovement();
                HandleBlockPlacement();
            }
            else
            {
                pozX = 0;
                pozZ = 0;
            }
            CalculateVelocity();
            if (jumpQm)
                Jump();
            controller.Move(new Vector3(0,moveDirection.y,0)*Time.deltaTime * speed);
            Pos.text = ((int)transform.position.x + "  " + (int)transform.position.y + "  " + (int)transform.position.z);
        }
    }
    public ChunkCoord GetPosition()
    {
        Vector3 position = transform.position;
        return new ChunkCoord(Mathf.FloorToInt(position.x) , Mathf.RoundToInt(position.z));
    }
    void CalculateVelocity()
    {
        moveDirection = transform.right * pozX + transform.forward * pozZ;
        if ((moveDirection.z > 0 && front) || (moveDirection.z < 0 && back))
            moveDirection.z = 0;
        if ((moveDirection.x > 0 && right) || (moveDirection.x < 0 && left))
            moveDirection.x = 0;
        if (sprint)
        {
            controller.Move(moveDirection * Time.fixedDeltaTime * sprintspeed);
        }
        else
            controller.Move(moveDirection * Time.fixedDeltaTime * movementSpeed);
            verticalMomentum += Time.fixedDeltaTime * gravity;
        moveDirection += Vector3.up * verticalMomentum * Time.fixedDeltaTime;
        if (moveDirection.y < 0)
        {
            moveDirection.y = checkDownSpeed(moveDirection.y);
        }
        else if (moveDirection.y > 0)
            moveDirection.y = checkUpSpeed(moveDirection.y);
    }
    void Jump()
    {

        verticalMomentum = jump;
        grounded = false;
        jumpQm = false;

    }
    void HandleMovement()
    {
        pozX = Input.GetAxis("Horizontal");
        pozZ = Input.GetAxis("Vertical");
        if(Input.GetKey(KeyCode.W))
        cansprint = true;
        else
        cansprint= false;
       
        if (cansprint && Input.GetKey(KeyCode.R))
        {
            sprint = true;
        }
        else if(!cansprint && sprint) 
            sprint = false;

        
        if (Input.GetButton("Jump") && grounded)
        {
            moveDirection.y = jump;
            grounded = false;
            jumpQm = true;
        }
       

    }
    public bool front
    {

        get
        {
            if (
                wmanager.IsBlock(transform.position.x, transform.position.y, transform.position.z + 0.5f) ||
                wmanager.IsBlock(transform.position.x, transform.position.y - 1, transform.position.z + 0.5f)
                )
                return true;
            return false;
        }

    }
    public bool back
    {
        
        get
        {
            if (
                wmanager.IsBlock(transform.position.x, transform.position.y, transform.position.z-0.5f ) ||
                wmanager.IsBlock(transform.position.x, transform.position.y - 1, transform.position.z -0.5f)
                )
                return true;
                return false;
        }

    }
    public bool left
    {

        get
        {
            if (
                wmanager.IsBlock(transform.position.x -0.5f, transform.position.y, transform.position.z) ||
                wmanager.IsBlock(transform.position.x -0.5f, transform.position.y - 1f, transform.position.z)
                )
                return true;
            return false;
        }

    }
    public bool right
    {

        get
        {
            if (
                wmanager.IsBlock(transform.position.x + 0.5f, transform.position.y, transform.position.z) ||
                wmanager.IsBlock(transform.position.x + 0.5f, transform.position.y - 1, transform.position.z)
                )
                return true;
            return false;
        }

    }
    private float checkDownSpeed(float downSpeed)
    {

        if (
            wmanager.IsBlock(transform.position.x - 0.3f, transform.position.y + downSpeed-1, transform.position.z - 0.3f) ||
            wmanager.IsBlock(transform.position.x + 0.3f, transform.position.y + downSpeed-1, transform.position.z - 0.3f) ||
            wmanager.IsBlock(transform.position.x + 0.3f, transform.position.y + downSpeed-1, transform.position.z + 0.3f) ||
            wmanager.IsBlock(transform.position.x - 0.3f, transform.position.y + downSpeed-1, transform.position.z + 0.3f)
           )
        {
            grounded = true;
            return 0;

        }
        else
        {
            grounded = false;
            return downSpeed;

        }

    }
    private float checkUpSpeed(float upSpeed)
    {

        if (
            wmanager.IsBlock(transform.position.x - 0.3f, transform.position.y + 1f+upSpeed, transform.position.z - 0.3f) ||
            wmanager.IsBlock(transform.position.x + 0.3f, transform.position.y + 1f+upSpeed, transform.position.z - 0.3f) ||
            wmanager.IsBlock(transform.position.x + 0.3f, transform.position.y + 1f+upSpeed, transform.position.z + 0.3f) ||
            wmanager.IsBlock(transform.position.x - 0.3f, transform.position.y + 1f+upSpeed, transform.position.z + 0.3f)
           )
        {

            return 0;

        }
        else
        {

            return upSpeed;

        }

    }

    private float time = 0,holdtme=0;
    int a, b, c;
    private bool breac=false;
    void HandleBlockPlacement()
    {

        if (Input.GetMouseButton(0)) // Left mouse button/break
        {
            if (toolbar.item[0, toolbar.slothIndex]>0 && wmanager.blockTypes[toolbar.item[0, toolbar.slothIndex]].utility == 1)
            {
                if (!breac)
                {
                    holdtme = 0;
                    breac = true;
                }
                float step = 0.1f;
                while (step <= 5)
                {

                    time = 0.1f;
                    Vector3 pos = cam.position + (cam.forward * step);
                    if (wmanager.IsBlock(pos.x, pos.y, pos.z))
                    {
                        int g= Mathf.RoundToInt(pos.x), h= Mathf.RoundToInt(pos.z);
                        int s=g, k=h;
                        if (g < 0 && g % 16 != 0)
                            s -= 16;
                        if (h < 0 && h % 16 != 0)
                        {
                            k -= 16;
                        }
                        if (g < 0)
                            g = 16 - (-g % 16);
                        if (h < 0)
                            h = 16 - (-h % 16);
                        
                        if (Mathf.RoundToInt(pos.x) == a && Mathf.RoundToInt(pos.y) == b && Mathf.RoundToInt(pos.z) == c)
                        {
                            holdtme += Time.deltaTime;
                        }
                        else
                        {
                            a = Mathf.RoundToInt(pos.x);
                            b = Mathf.RoundToInt(pos.y);
                            c = Mathf.RoundToInt(pos.z);
                            box.gameObject.SetActive(true);
                            box.transform.position = new Vector3(a, b, c);
                            holdtme = 0;
                        }
                        if (holdtme >= wmanager.blockTypes[WorldManager.chunks[(s / 16+100), (k/16+100)].Voxels[g%16, Mathf.RoundToInt(pos.y), h%16]].brktme)
                        {
                            wmanager.ModifyMesh(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z), 0);
                            box.gameObject.SetActive(false);
                            breac=false;
                            holdtme = 0;
                        }

                        break;
                    }

                    step += 0.1f;
                }
            }
        }
        else if (breac)
        {
            breac = false;
            holdtme=0;
            a --;
            box.gameObject.SetActive(false);
        }
        //right click
        if (Input.GetMouseButtonDown(1) && time <= 0)
        {
            if (toolbar.item[0, toolbar.slothIndex] > 0 && wmanager.blockTypes[toolbar.item[0, toolbar.slothIndex]].utility == 0)
            {
                float step = 0.1f;
                Vector3 lastPos = new Vector3();
                while (step <= 5)
                {
                    Vector3 pos = cam.position + (cam.forward * step);
                    if (wmanager.IsBlock(pos.x, pos.y, pos.z))
                    {
                        if (CanPlace(lastPos))
                        {
                            time = 0.2f;
                            wmanager.ModifyMesh(Mathf.RoundToInt(lastPos.x), Mathf.RoundToInt(lastPos.y), Mathf.RoundToInt(lastPos.z), toolbar.item[0, toolbar.slothIndex]);
                            toolbar.UpdateAnItem(toolbar.slothIndex);
                        }
                        break;
                    }
                    lastPos = pos;
                    step += 0.2f;
                }
            }
        }
        if(time>0)
            time-=Time.deltaTime;
    }

    public bool CanPlace(Vector3 pos)
    {
        pos.x= Mathf.RoundToInt(pos.x);
        pos.y= Mathf.RoundToInt(pos.y);
        pos.z= Mathf.RoundToInt(pos.z);
        float a = transform.position.x;
        float b = transform.position.y;
        float c = transform.position.z;
        for (int i=0; i<=1 ; i++)
        {
            if ((Mathf.RoundToInt(a +0.3f) == pos.x && Mathf.FloorToInt(b + i) == pos.y && Mathf.RoundToInt(c+0.3f ) == pos.z)||
            (Mathf.RoundToInt(a + 0.3f) == pos.x && Mathf.FloorToInt(b + i) == pos.y && Mathf.RoundToInt(c - 0.3f) == pos.z )||
            (Mathf.RoundToInt(a - 0.3f) == pos.x && Mathf.FloorToInt(b + i) == pos.y && Mathf.RoundToInt(c + 0.3f) == pos.z )||
            (Mathf.RoundToInt(a - 0.3f) == pos.x && Mathf.FloorToInt(b + i) == pos.y && Mathf.RoundToInt(c - 0.3f) == pos.z))
            return false;

        }
        return true;
    }
}
