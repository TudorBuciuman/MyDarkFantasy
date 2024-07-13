using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public WorldManager wmanager;
    public PerlinGenerator perlin;
    public Toolbar toolbar;
    public Transform cam;
    public float reach = 5f;
    public float movementSpeed = 5f;
    public float sprintspeed = 2f;
    public float lookSpeed = 400f;
    public float smoothSpeed = 100.0f;
    public float gravity = -25.8f;
    public Transform orientation;
    public float speed = 1.0f;
    public float jump = 27f;
    public CharacterController controller;
    private bool grounded=true;
    private Vector3 Position;
    private Vector3 moveDirection = Vector3.zero;
    private float pozX = 0f;
    private float pozZ = 0f;
    private float verticalMomentum;
    private bool sprint = false;
    private bool cansprint=false;
    private bool jumpQm = false;
    public TMP_Text Pos;

    void Start()
    {
        this.transform.position = new Vector3(0,68,0);
        Application.targetFrameRate = 50;
        perlin = new PerlinGenerator();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
       // controller = gameObject.AddComponent<CharacterController>();
        controller.transform.position = new Vector3(12.0f,80f,4.0f);
    }

    void Update()
    {
        moveDirection.y -= gravity * Time.deltaTime;
        if (!toolbar.openedInv)
        {
            HandleMovement();
            HandleBlockPlacement();
        }
            CalculateVelocity();
            if (jumpQm)
                Jump();

            controller.Move(moveDirection * Time.deltaTime * speed);
        
    }
    private void FixedUpdate()
    {
        if (jumpQm)
            Jump();
        CalculateVelocity();
        if(sprint)
        controller.Move(moveDirection * Time.deltaTime * sprintspeed);
        else
        controller.Move(Position*Time.deltaTime * speed);
        Pos.text=((int)transform.position.x+"  "+(int)transform.position.y+"  "+(int)transform.position.z);
        if(time>0)
            time-= Time.deltaTime;
    }

    public ChunkCoord GetPosition()
    {
        Vector3 position = transform.position;
        return new ChunkCoord(Mathf.FloorToInt(position.x) , Mathf.RoundToInt(position.z));
    }
    void CalculateVelocity()
    {
        moveDirection = transform.right * pozX + transform.forward * pozZ;
        if ( verticalMomentum> gravity)
            verticalMomentum += Time.deltaTime * gravity;
        if ((moveDirection.z > 0 && front) || (moveDirection.z < 0 && back))
            moveDirection.z = 0;
        if ((moveDirection.x > 0 && right) || (moveDirection.x < 0 && left))
            moveDirection.x = 0;
        moveDirection += Vector3.up * verticalMomentum * Time.deltaTime;
        if (moveDirection.y < 0)
            moveDirection.y = checkDownSpeed(moveDirection.y);
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
                perlin.IsBlock(transform.position.x, transform.position.y, transform.position.z + 0.5f) ||
                perlin.IsBlock(transform.position.x, transform.position.y - 1, transform.position.z + 0.5f)
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
                perlin.IsBlock(transform.position.x, transform.position.y, transform.position.z-0.5f ) ||
                perlin.IsBlock(transform.position.x, transform.position.y - 1, transform.position.z -0.5f)
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
                perlin.IsBlock(transform.position.x -0.5f, transform.position.y, transform.position.z) ||
                perlin.IsBlock(transform.position.x -0.5f, transform.position.y - 1f, transform.position.z)
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
                perlin.IsBlock(transform.position.x + 0.5f, transform.position.y, transform.position.z) ||
                perlin.IsBlock(transform.position.x + 0.5f, transform.position.y - 1, transform.position.z)
                )
                return true;
            return false;
        }

    }
    private float checkDownSpeed(float downSpeed)
    {

        if (
            perlin.IsBlock(transform.position.x - 0.3f, transform.position.y + downSpeed-1, transform.position.z - 0.3f) ||
            perlin.IsBlock(transform.position.x + 0.3f, transform.position.y + downSpeed-1, transform.position.z - 0.3f) ||
            perlin.IsBlock(transform.position.x + 0.3f, transform.position.y + downSpeed-1, transform.position.z + 0.3f) ||
            perlin.IsBlock(transform.position.x - 0.3f, transform.position.y + downSpeed-1, transform.position.z + 0.3f)
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
            perlin.IsBlock(transform.position.x - 0.3f, transform.position.y + 1f+upSpeed, transform.position.z - 0.3f) ||
            perlin.IsBlock(transform.position.x + 0.3f, transform.position.y + 1f+upSpeed, transform.position.z - 0.3f) ||
            perlin.IsBlock(transform.position.x + 0.3f, transform.position.y + 1f+upSpeed, transform.position.z + 0.3f) ||
            perlin.IsBlock(transform.position.x - 0.3f, transform.position.y + 1f+upSpeed, transform.position.z + 0.3f)
           )
        {

            return 0;

        }
        else
        {

            return upSpeed;

        }

    }



    private float time = 0;
    void HandleBlockPlacement()
    {

        if (Input.GetMouseButtonDown(0) && time<=0) // Left mouse button/break
        {
            
            float step = 0.1f;
            while (step <= 5)
            {
                time = 0.4f;
                Vector3 pos = cam.position + (cam.forward * step);
                if (perlin.IsBlock(pos.x, pos.y, pos.z))
                {
                    wmanager.ModifyMesh(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z), 0);
                    break;
                }
                step += 0.1f;
            }

        }
        //place
        if (Input.GetMouseButtonDown(1) && toolbar.item[0,toolbar.slothIndex]>0 && time <= 0)
        {
            float step = 0.1f;
            Vector3 lastPos = new Vector3();
            while(step<=5)
            {
                Vector3 pos = cam.position + (cam.forward * step);
                if (perlin.IsBlock(pos.x, pos.y, pos.z))
                {
                    time = 0.3f;
                    wmanager.ModifyMesh(Mathf.RoundToInt(lastPos.x), Mathf.RoundToInt(lastPos.y), Mathf.RoundToInt(lastPos.z), wmanager.blockTypes[toolbar.item[0, toolbar.slothIndex]].place);
                    break;
                }
                lastPos = pos;
                step += 0.2f;
            }

        }
    }
}
