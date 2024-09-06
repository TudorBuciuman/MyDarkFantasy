using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ControllerImput : MonoBehaviour
{

    //Stiu ca e input - sunt doar silly :)
    public WorldManager wmanager;
    public Crafting craft;
    public itemsManager itemsManager;
    public SoundsManager soundTrack;
    public Toolbar toolbar;

    public GameObject Hud1;
    public GameObject Hud2;
    public GameObject Hud3;
    public Transform cam;
    public float movementSpeed = 5f;
    public float sprintspeed = 8f;
    public float lookSpeed = 400f;
    public float smoothSpeed = 100.0f;
    public float gravity = -8f;
    public Transform orientation;
    public float speed = 50f;
    public float jump = 6f;
    public CharacterController controller;
    public GameObject box;

    private bool grounded=true;
    private Vector3 moveDirection = Vector3.zero;

    public  Quaternion Rotation;
    public  Vector3 Posi=Vector3.zero;
    private float pozX = 0f;
    private float pozZ = 0f;
    private float verticalMomentum=0;
    private bool sprint = false;
    private bool cansprint=false;
    private bool jumpQm = false;
    public Text Pos;
    public float wtime=0,brktime=0;

    void Start()
    {
        bool esc=Toolbar.escape;
        Toolbar.escape = true;
        QualitySettings.vSyncCount = 1;
        UiManager ui = new();
        ui.ReadSet();
        Application.targetFrameRate = 60;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (ChunkSerializer.seed == -1)
        {
            SceneManager.LoadScene(1);
            return;
        }
        else
        {
            while (!ChunkSerializer.pret)
                ;
            
        }
        if (ChunkSerializer.pos != Vector3.zero)
        {
            controller.enabled = false;
            transform.position = ChunkSerializer.pos;
            transform.rotation = ChunkSerializer.rot;
        }
        else
        {
            transform.position = new Vector3(0,80,0);
        }
        wmanager.GenerateWorld();
        controller.enabled = true;
        if (!UiManager.hud)
        {
            Hud1.SetActive(false);
            Hud2.SetActive(false);
            Hud3.GetComponent<Image>().color=Color.clear;
        }
        if (esc)
        {
            toolbar.Escape();
        }
        else
        {
            Toolbar.escape=false;
        }
        //soundTrack.PlaySong((byte)Random.Range(0, 6));
    }

    private void FixedUpdate()
    {
        PlayerControll();
    }
    public void PlayerControll()
    {
        if (!Toolbar.escape)
        {
           // if(moveDirection.y>gravity)
         //   moveDirection.y -= gravity * Time.deltaTime;
            if (!toolbar.openedInv)
            {
                HandleMovement();
                HandleMouseInput();
            }
            else
            {
                pozX = 0;
                pozZ = 0;
            }
            CalculateVelocity();
            if (jumpQm)
                Jump();
            controller.Move(new Vector3(0,moveDirection.y,0)*Time.smoothDeltaTime * speed);
            Pos.text = ((int)transform.position.x + "  " + (int)transform.position.y + "  " + (int)transform.position.z);
        }
        else if (Posi == Vector3.zero)
        {
            Rotation=transform.rotation;
            Posi = transform.position;
        }
    }
    public Vector3 playerPos()
    {
        return Posi;
    }
    public Quaternion playerRot()
    {
        return Rotation;
    }
    public ChunkCoord GetPosition()
    {
        Vector3 position = controller.transform.position;
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
            controller.Move(moveDirection * Time.deltaTime * sprintspeed);
        }
        else
            controller.Move(moveDirection * Time.deltaTime * movementSpeed);
        
        verticalMomentum += Time.fixedDeltaTime * gravity;
        moveDirection += Vector3.up * verticalMomentum * Time.deltaTime;
        if (moveDirection.y < 0)
        {
            moveDirection.y = checkDownSpeed(moveDirection.y);
        }
        else if (moveDirection.y > 0)
            moveDirection.y = checkUpSpeed(moveDirection.y);
        if(moveDirection.y==0)
            verticalMomentum = 0;
        if ((moveDirection.x != 0 || moveDirection.y != 0))
        {
            if (wtime <= 0 && grounded)
            {
                wtime = 1.2f;
                soundTrack.Move(0);
            }
            else if (!grounded)
            {
                wtime = 0;
                soundTrack.stopmove();
            }
        }
        else
        {
            if (wtime>0)
            {
                wtime = 0;
                soundTrack.stopmove();
            }
        }
        if (wtime > 0)
        {
            wtime-=Time.fixedDeltaTime;
        }
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
        
        if (Input.GetKey(KeyCode.W))
        {
            cansprint = true;
        }
        else
            cansprint = false;
       
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
                wmanager.blockTypes[wmanager.Block(transform.position.x, transform.position.y, transform.position.z + 0.5f)].isblock ||
                wmanager.blockTypes[wmanager.Block(transform.position.x, transform.position.y - 1, transform.position.z + 0.5f)].isblock
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
                wmanager.blockTypes[wmanager.Block(transform.position.x, transform.position.y, transform.position.z - 0.5f)].isblock ||
                wmanager.blockTypes[wmanager.Block(transform.position.x, transform.position.y - 1, transform.position.z - 0.5f)].isblock
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
                wmanager.blockTypes[wmanager.Block(transform.position.x - 0.5f, transform.position.y, transform.position.z)].isblock ||
                wmanager.blockTypes[wmanager.Block(transform.position.x - 0.5f, transform.position.y - 1f, transform.position.z)].isblock
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
                wmanager.blockTypes[wmanager.Block(transform.position.x + 0.5f, transform.position.y, transform.position.z)].isblock ||
                wmanager.blockTypes[wmanager.Block(transform.position.x + 0.5f, transform.position.y - 1, transform.position.z)].isblock
                )
                return true;
            return false;
        }

    }
    private float checkDownSpeed(float downSpeed)
    {
        if (transform.position.y <= 1)
        {
            verticalMomentum = 0;
            controller.enabled = false;
            //animatie cu dark screen, gaseste tu ceva
            transform.position = ChunkSerializer.pos+new Vector3(0,1,0);
            controller.enabled = true;
            return 0;
        }
        if (transform.position.y > 160)
        {
            return downSpeed;
        }
        if (
            wmanager.blockTypes[wmanager.Block(transform.position.x - 0.3f, transform.position.y + downSpeed-1, transform.position.z - 0.3f)].isblock ||
            wmanager.blockTypes[wmanager.Block(transform.position.x + 0.3f, transform.position.y + downSpeed - 1, transform.position.z - 0.3f)].isblock ||
            wmanager.blockTypes[wmanager.Block(transform.position.x + 0.3f, transform.position.y + downSpeed - 1, transform.position.z + 0.3f)].isblock ||
            wmanager.blockTypes[wmanager.Block(transform.position.x - 0.3f, transform.position.y + downSpeed - 1, transform.position.z + 0.3f)].isblock 
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
        if (transform.position.y > 158)
        {
            return upSpeed;
        }
        if (
            wmanager.blockTypes[wmanager.Block(transform.position.x - 0.3f, transform.position.y + 0.7f + upSpeed, transform.position.z - 0.3f)].isblock ||
            wmanager.blockTypes[wmanager.Block(transform.position.x + 0.3f, transform.position.y + 0.7f + upSpeed, transform.position.z - 0.3f)].isblock ||
            wmanager.blockTypes[wmanager.Block(transform.position.x + 0.3f, transform.position.y + 0.7f + upSpeed, transform.position.z + 0.3f)].isblock ||
            wmanager.blockTypes[wmanager.Block(transform.position.x - 0.3f, transform.position.y + 0.7f + upSpeed, transform.position.z + 0.3f)].isblock
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
    public static int a, b, c;
    private bool breac=false;
    void HandleMouseInput()
    {

        if (Input.GetMouseButton(0)) // Left mouse button/break
        {
            if (toolbar.item[0, toolbar.slothIndex]>0 && wmanager.blockTypes[toolbar.item[0, toolbar.slothIndex]].utility == 10)
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
                        if ((wmanager.blockTypes[wmanager.Block(pos.x, pos.y, pos.z)].utility == 0 || wmanager.blockTypes[wmanager.Block(pos.x, pos.y, pos.z)].utility > 1))
                        {
                            if (brktime <= 0)
                            {
                                soundTrack.Placement(1);

                                brktime = 0.3f;
                            }
                            int g = Mathf.RoundToInt(pos.x), h = Mathf.RoundToInt(pos.z);
                            int s = g, k = h;
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
                            if (holdtme >= wmanager.blockTypes[WorldManager.chunks[(s / 16 + 100), (k / 16 + 100)].Voxels[g % 16, Mathf.RoundToInt(pos.y), h % 16]].brktme)
                            {
                                box.gameObject.SetActive(false);
                                itemsManager.SetItem(wmanager.Block(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z)), 1, new Vector3(pos.x, Mathf.RoundToInt(pos.y) - 0.3f, pos.z));
                                wmanager.ModifyMesh(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z), 0);

                                breac = false;
                                holdtme = 0;
                            }
                        }
                        break;
                    }
                    step += 0.1f;
                }
            }
            else if(toolbar.item[0, toolbar.slothIndex] > 0 && wmanager.blockTypes[toolbar.item[0, toolbar.slothIndex]].utility == 11)
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
                        if (wmanager.blockTypes[wmanager.Block(pos.x, pos.y, pos.z)].utility == 1)
                        {
                            if (brktime <= 0)
                            {
                                soundTrack.Placement(1);

                                brktime = 0.3f;
                            }
                            int g = Mathf.RoundToInt(pos.x), h = Mathf.RoundToInt(pos.z);
                            int s = g, k = h;
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
                            if (holdtme >= wmanager.blockTypes[WorldManager.chunks[(s / 16 + 100), (k / 16 + 100)].Voxels[g % 16, Mathf.RoundToInt(pos.y), h % 16]].brktme)
                            {
                                box.gameObject.SetActive(false);
                                itemsManager.SetItem(wmanager.Block(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z)), 1, new Vector3(pos.x, Mathf.RoundToInt(pos.y) - 0.3f, pos.z));
                                wmanager.ModifyMesh(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z), 0);

                                breac = false;
                                holdtme = 0;
                            }
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
            
            brktime = 0;
            holdtme=0;
            a --;
            box.gameObject.SetActive(false);
            soundTrack.stopbreak();
        }
        if(brktime > 0)
        {
            brktime-=Time.deltaTime;
        }
        //right click
        if (Input.GetMouseButton(1) && time<=0)
        {
            if (toolbar.item[0, toolbar.slothIndex] > 0 && wmanager.blockTypes[toolbar.item[0, toolbar.slothIndex]].utility <10)
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
            else
            {
                float step = 0.1f;
                Vector3 lastPos = new Vector3();
                while (step <= 5)
                {
                    Vector3 pos = cam.position + (cam.forward * step);
                    if (wmanager.IsBlock(pos.x, pos.y, pos.z))
                    {
                        if (wmanager.blockTypes[wmanager.Block(pos.x,pos.y,pos.z)].utility>=2)
                        {
                            switch(wmanager.blockTypes[wmanager.Block(pos.x, pos.y, pos.z)].utility)
                            {
                                case 2: toolbar.OpenInventory(1);
                                    break;
                                case 3: toolbar.OpenInventory(2);
                                    break;
                                case 4: toolbar.OpenInventory(3);
                                    break;
                            }
                       }
                        time = 0.2f;
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
