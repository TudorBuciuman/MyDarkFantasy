using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class ControllerImput : MonoBehaviour
{

    //Stiu ca e input - sunt doar silly :}
    public WorldManager wmanager;
    public HealthSistem healthSistem;
    public Crafting craft;
    public itemsManager itemsManager;
    public SoundsManager soundTrack;
    public Toolbar toolbar;
    public GameObject Hud1,Hud2,Hud3;
    public Transform cam;
    public static ControllerImput Instance;

    public float movementSpeed = 5f;
    public const float sprintspeed = 6f;
    public const float shiftSpeed= 3f;
    public const float normalSpeed= 5f;
    public float movementAcceleration = 4f;
    public float sprintAcceleration = 5f;
    public Vector3 currentVelocity = Vector3.zero;
    private float verticalMomentum = 0f;
    public static float lookSpeed = 400f;
    public static float smoothSpeed = 100.0f;
    public const float gravity = -10f;
    public const float waterSpeed = 2f;
    public const float waterAcceleration = 5f;
    public const float waterDeceleration = 5f;

    public Transform orientation;
    public static float speed = 50f;
    public static float jump = 4.8f;
    private bool inWater=false;
    private float timeinWater = 0;
    public CharacterController controller;
    public GameObject box;
    public FixedJoystick joystick;

    public InputActionAsset inputActions;
    private InputAction androidclick;
    private InputAction androidPress;
    private InputActionMap androidActionMap;
    private InputAction moveAction;
    private InputAction shiftAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction escapeAction;
    private readonly InputAction ANDRmoveact;
    private InputAction middleAction;

    private Coroutine showfps, pos;

    private bool grounded=false;
    private bool shift=false;
    private Vector3 moveDirection = Vector3.zero;
    public Camera cameracontrol;
    public  Quaternion Rotation;
    public  Vector3 Posi=Vector3.zero;
    private float pozX = 0f;
    private float pozZ = 0f;
    private bool sprint = false;
    private bool jumpQm = false;
    public Text Pos;
    public Text framerate;

    public Image itemimg,mapImg,mapImg2;
    public Image watersprite;

    public short fps=0;
    public static float wtime=0,brktime=0;
    private void Awake()
    {
        Instance = this;
        toolbar.openedInv = true;
        androidActionMap = inputActions.FindActionMap("Android");
        androidActionMap.Enable();
        moveAction = androidActionMap.FindAction("Movement");
        moveAction.Enable();
        shiftAction = androidActionMap.FindAction("Shifting");
        shiftAction.Enable();

#if UNITY_ANDROID

        androidclick= new InputAction(binding: "<Touchscreen>/primaryTouch/tap");
        androidPress = new InputAction(binding: "<Touchscreen>/press");
        
#endif
        androidActionMap.FindAction("Sprint").Enable();
         escapeAction = androidActionMap.FindAction("Escape");
        escapeAction.Enable();
         sprintAction = inputActions.FindAction("Sprint");
        jumpAction = androidActionMap.FindAction("Jump");
        jumpAction.Enable();
        middleAction = new InputAction(binding: "<Mouse>/middleButton");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Start()
    {
        bool esc =Toolbar.escape;
        Toolbar.escape = true;
        QualitySettings.vSyncCount = 1;
        UiManager.ReadSet();
        Application.targetFrameRate = 60;
        if (ChunkSerializer.seed == -1)
        {
            SceneManager.LoadScene(0);
            return;
        }
        else
        {
            while (!ChunkSerializer.pret)
                ;
            
        }
        bool newworld=false;
        controller.enabled = false;
        if (ChunkSerializer.pos != Vector3.zero)
        {
            transform.SetPositionAndRotation(ChunkSerializer.pos, ChunkSerializer.rot);
            wmanager.GenerateWorld();
        }
        else
        {
            WorldManager.AddChunk(0,0, new Chunk(new ChunkCoord(100,100), wmanager));
            wmanager.BakeNewWorld();
            newworld = true;
            transform.position = new Vector3(0,100,0);
        }
        
        if(newworld)
        {
            for(int i=120; i>50; i--)
            {
                if (WorldManager.GetChunk(0, 0).Voxels[0, i, 0].Value1 != 0)
                {
                    transform.position=new Vector3(0,i+2,0);
                    break;
                }
            }

        }
        ReRead();
        if (esc)
        {
            toolbar.Escape();
        }
        else
        {
            Toolbar.escape=false;
        }

        ItemsFunctions.ItemsStart(wmanager,itemsManager,itemimg,mapImg,mapImg2);
        if(Voxeldata.showfps)
        InvokeRepeating(nameof(PlayScary), 20f, 45f);
        toolbar.openedInv = false;
        controller.enabled = true;
        //soundTrack.PlaySong((byte)Random.Range(0, 6));
    }
    private void FixedUpdate()
    {
        PlayerControll();
    }
    public void GetFps() { 
        fps = ((short)(1f / Time.unscaledDeltaTime));
        framerate.text = fps.ToString();
    }
    public void ReRead()
    {
        if (!UiManager.hud)
        {
            Hud1.SetActive(false);
            Hud2.GetComponent<CanvasGroup>().alpha = 0;
            Hud3.GetComponent<Image>().enabled = false;
            CancelInvoke(nameof(PosOut));
            Pos.text = null;
            framerate.text = null;
            Pos.gameObject.SetActive(false);
        }
        else
        {
            InvokeRepeating(nameof(PosOut), 0, 0.05f);
            Hud1.SetActive(true);
            Hud2.GetComponent<CanvasGroup>().alpha = 1;
            Pos.gameObject.SetActive(true);
            Hud3.GetComponent<Image>().enabled = true;
        }
        if(UiManager.fps && UiManager.hud)
        InvokeRepeating(nameof(GetFps), 0, 0.3f);
        else
        {
            CancelInvoke(nameof(GetFps));
            framerate.text=null;
        }

    }
    public void PlayScary()
    {
        if (transform.position.y < 60)
        {
            soundTrack.PlayRandomSound(0);
        }
        else if(transform.position.y>80){
            soundTrack.PlayRandomSound(1);
        }
        else if (WorldManager.currenttime > 900)
        {
            soundTrack.PlayRandomSound(2);
        }
    }
    public void PlayerControll()
    {
        if (!Toolbar.escape)
        {
            
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
            controller.Move(speed * Time.smoothDeltaTime * new Vector3(0,moveDirection.y,0));
        }
        else if (Posi == Vector3.zero)
        {
            Rotation=transform.rotation;
            Posi = transform.position;
        }
    }
    public Vector3 PlayerPos()
    {
        return Posi;
    }
    public Quaternion PlayerRot()
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
        // === Determine if in water ===
        bool inWater = IsInWater();

        // === Movement input direction ===
        moveDirection = transform.right * pozX + transform.forward * pozZ;

        // === Collision blocking ===
        if ((moveDirection.z > 0 && Front) || (moveDirection.z < 0 && Back))
        {
            moveDirection.z = 0;
            currentVelocity.z = 0;
        }
        if ((moveDirection.x > 0 && Right) || (moveDirection.x < 0 && Left))
        {
            moveDirection.x = 0;
            currentVelocity.x = 0;
        }

        // === Movement values ===
        float baseSpeed = inWater ? waterSpeed : (sprint ? sprintspeed : movementSpeed);
        float baseAccel = inWater ? waterAcceleration : (sprint ? sprintAcceleration : movementAcceleration);
        float baseDecel = inWater ? waterDeceleration : (sprint ? 25 : 13);

        // === Horizontal movement ===
        Vector3 targetVelocity = moveDirection * baseSpeed;

        if (moveDirection.magnitude > 0)
        {
            currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, targetVelocity.x, baseAccel * Time.fixedDeltaTime);
            currentVelocity.z = Mathf.MoveTowards(currentVelocity.z, targetVelocity.z, baseAccel * Time.fixedDeltaTime);
        }
        else
        {
            currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, 0, baseDecel * Time.fixedDeltaTime);
            currentVelocity.z = Mathf.MoveTowards(currentVelocity.z, 0, baseDecel * Time.fixedDeltaTime);
        }

        // === Vertical ===
        //DO NOT TOUCH!!!
        if (!grounded && !inWater)
        {
            verticalMomentum += gravity * Time.fixedDeltaTime;
            verticalMomentum = Mathf.Clamp(verticalMomentum, -500, 10);
        }
        else if (inWater)
        {
            float waterGravity = -2f; 
            verticalMomentum += waterGravity * Time.fixedDeltaTime;

            verticalMomentum = Mathf.Clamp(verticalMomentum, -1.5f, 2f);
        }
        else
        {
            verticalMomentum = -0.1f; 
        }


        currentVelocity.y = verticalMomentum * Time.fixedDeltaTime;

        if (currentVelocity.y > 0)
        {
            currentVelocity.y = CheckUpSpeed(currentVelocity.y);
        }
        else if (currentVelocity.y < 0)
        {
            float r = CheckDownSpeed(currentVelocity.y);
            if (r == 0)
            {
                if (currentVelocity.y < -0.25f)
                {
                    float c = currentVelocity.y;
                    currentVelocity.y = 0;
                    grounded = true;
                    verticalMomentum = -0.1f;
                    healthSistem.UpdateHealth(c * 10);
                }
                grounded = true;
            }
            currentVelocity.y = r;
        }
        else
        {
            grounded = true;
        }

        currentVelocity.y *= speed;

        // === Apply movement ===
        controller.Move(currentVelocity * Time.fixedDeltaTime);

        // === Footstep sounds ===
        if (moveDirection.magnitude > 0 && grounded)
        {
            if (wtime <= 0)
            {
                wtime = 1.2f;
                soundTrack.Move(0);
            }
        }
        else
        {
            soundTrack.Stopmove();
            wtime = 0;
        }

        if (wtime > 0)
            wtime -= Time.fixedDeltaTime;
    }

    public void PosOut()
    {
        Pos.text = $"{(int)transform.position.x}  {(int)transform.position.y}  {(int)transform.position.z}";
    }
    public void Jump()
    {
        if (grounded)
        {
            verticalMomentum = jump;
            grounded = false;
            jumpQm = false;
        }
        if (inWater)
        {
            verticalMomentum = 2f;
        }

    }
    public void OnMovement(InputAction.CallbackContext context)
    {
        Vector2 movementInput = context.ReadValue<Vector2>();
        pozX = movementInput.x;
        pozZ = movementInput.y;
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        sprint =!sprint;
        float targetFOV = sprint ? 65f : 60f;
        StartCoroutine(SmoothFOVTransition(targetFOV, 0.3f));
    }
    private IEnumerator SmoothFOVTransition(float targetFOV, float duration)
    {
        Camera camera = cameracontrol.GetComponent<Camera>();
        float startFOV = camera.fieldOfView;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            camera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        camera.fieldOfView = targetFOV;
    }
    void OnEnable()
    {
        moveAction.Enable();
        middleAction.Enable();
        sprintAction.Enable();
        escapeAction.Enable();
        shiftAction.Enable();

        shiftAction.performed += OnShift;
        shiftAction.canceled -= OnShift;

        middleAction.performed += OnMiddleClicking;
        middleAction.canceled += OnMiddleClicking;
        sprintAction.performed += OnSprint;
        sprintAction.canceled -= OnSprint;
        escapeAction.performed += OnEscape;
        escapeAction.canceled -= OnEscape;
        moveAction.performed += OnMovement;
        moveAction.canceled += OnMovement;

        if (androidclick!=null)
        {
            androidclick.Enable();
            androidPress.Enable();
            ANDRmoveact.Enable();

            androidclick.performed += OnClicking1;
            androidclick.canceled += OnClicking1;

            androidPress.performed += OnClicking2;
            androidPress.canceled += OnClicking2;

            ANDRmoveact.performed += OnJoystickMovement;
            ANDRmoveact.canceled += OnJoystickMovement;
        }

    }
    void OnDisable()
    {
        shiftAction.performed -= OnShift;
        shiftAction.canceled -= OnShift;
        middleAction.performed -= OnMiddleClicking;
        middleAction.canceled -= OnMiddleClicking;
        sprintAction.performed -= OnSprint;
        sprintAction.canceled -= OnSprint;
        escapeAction.performed -= OnEscape;
        escapeAction.canceled -= OnEscape;
        moveAction.performed -= OnMovement;
        moveAction.canceled -= OnMovement;

        shiftAction.Disable();
        moveAction.Disable();
        middleAction.Disable();
        sprintAction.Disable();
        escapeAction.Disable();

        if (androidclick != null)
        {
            androidclick.performed -= OnClicking1;
            androidclick.canceled -= OnClicking1;

            androidPress.performed -= OnClicking2;
            androidPress.canceled -= OnClicking2;

            ANDRmoveact.performed -= OnJoystickMovement;
            ANDRmoveact.canceled -= OnJoystickMovement;

            androidclick.Disable();
            androidPress.Disable();
            ANDRmoveact.Disable();
        }
    }
    public void OnMiddleClicking(InputAction.CallbackContext context)
    {
        float step = 0.1f;

        while (step <= 5)
        {

            time = 0.1f;
            Vector3 pos = cam.position + (cam.forward * step);
            if (wmanager.IsBlock(pos.x, pos.y, pos.z))
            {
                toolbar.SearchInInventory(wmanager.Block(pos.x, pos.y, pos.z));
                break;
            }
            step += 0.2f;
        }
    }
    void HandleMovement()
    {
        if(jumpAction.IsPressed())
        {
            if (grounded)
            {
                moveDirection.y = jump;
                grounded = false;
                jumpQm = true;
            }
            else if (inWater)
            {
                verticalMomentum = 2;
            }
        }
        //pozX = joystick.Horizontal;
        //pozZ = joystick.Vertical;
    }
    public void OnJoystickMovement(InputAction.CallbackContext context)
    {
        Vector2 movementInput = context.ReadValue<Vector2>();
        pozX = movementInput.x;
        pozZ = movementInput.y;
    }
    public void OnEscape(InputAction.CallbackContext context)
    {
        if (!BookManager.readingBook && Toolbar.CanEsc)
        {
            if (!Toolbar.escape)
                toolbar.Escape();
            else
                toolbar.Again();
        }
    }
    public void OnClicking1(InputAction.CallbackContext context)
    {
        if (time <= 0) {
            if (toolbar.item[0, Toolbar.slothIndex] > 0 && wmanager.blockTypes[toolbar.item[0, Toolbar.slothIndex]].Items.isblock )
            {
                float step = 0.1f;
                Vector3 lastPos = new();
                while (step <= 5)
                {
                    Vector3 pos = cam.position + (cam.forward * step);
                    if (wmanager.IsBlock(pos.x, pos.y, pos.z))
                    {
                        if (CanPlace(lastPos))
                        {
                            time = 0.2f;
                            wmanager.ModifyMesh(Mathf.RoundToInt(lastPos.x), Mathf.RoundToInt(lastPos.y), Mathf.RoundToInt(lastPos.z),new Chunk.VoxelStruct(toolbar.item[0, Toolbar.slothIndex],(byte)Random.Range(0,2)));
                            toolbar.UpdateAnItem(Toolbar.slothIndex);
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
                while (step <= 5)
                {
                    Vector3 pos = cam.position + (cam.forward * step);
                    if (wmanager.IsBlock(pos.x, pos.y, pos.z))
                    {
                        if (wmanager.blockTypes[wmanager.Block(pos.x, pos.y, pos.z)].Items.blocks.special >= 2)
                        {
                            switch (wmanager.blockTypes[wmanager.Block(pos.x, pos.y, pos.z)].Items.blocks.special)
                            {
                                case 2:
                                    toolbar.OpenInventory(1);
                                    break;
                                case 3:
                                    toolbar.OpenInventory(2);
                                    break;
                                case 4:
                                    Chest.Instance.OpenChest(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
                                    break;
                                case 5:
                                    if (step < 2)
                                        Sleep(new Vector3Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z)));
                                    break;
                            }
                        }
                        time = 0.2f;
                        break;
                    }
                    step += 0.2f;
                
            }
        }
    }
    }
    public void OnClicking2(InputAction.CallbackContext context)
    {
        Debug.Log(2);
    }

    public void OnShift(InputAction.CallbackContext context)
    {
        movementSpeed= shift? normalSpeed: shiftSpeed;
        shift = !shift;
        float targetFOV = shift ? 50f : 60f;
        StartCoroutine(SmoothFOVTransition(targetFOV, 0.3f));
    }
    public bool Front
    {
        get
        {
            if(transform.position.y>159)
                return false;
            if (
                wmanager.blockTypes[wmanager.Block(transform.position.x, transform.position.y, transform.position.z + 0.5f)].Items.isblock ||
                wmanager.blockTypes[wmanager.Block(transform.position.x, transform.position.y - 1, transform.position.z + 0.5f)].Items.isblock
                )
                return true;
            return false;
        }

    }
    public bool Back
    {
        
        get
        {
            if (transform.position.y > 159)
                return false;
            if (
                wmanager.blockTypes[wmanager.Block(transform.position.x, transform.position.y, transform.position.z - 0.5f)].Items.isblock ||
                wmanager.blockTypes[wmanager.Block(transform.position.x, transform.position.y - 1, transform.position.z - 0.5f)].Items.isblock
                )
                return true;
                return false;
        }

    }
    public bool Left
    {

        get
        {
            if (transform.position.y > 159)
                return false;
            if (
                wmanager.blockTypes[wmanager.Block(transform.position.x - 0.5f, transform.position.y, transform.position.z)].Items.isblock ||
                wmanager.blockTypes[wmanager.Block(transform.position.x - 0.5f, transform.position.y - 1f, transform.position.z)].Items.isblock
                )
                return true;
            return false;
        }

    }
    public bool Right
    {

        get
        {
            if (transform.position.y > 159)
                return false;
            if (
                wmanager.blockTypes[wmanager.Block(transform.position.x + 0.5f, transform.position.y, transform.position.z)].Items.isblock ||
                wmanager.blockTypes[wmanager.Block(transform.position.x + 0.5f, transform.position.y - 1, transform.position.z)].Items.isblock
                )
                return true;
            return false;
        }

    }
    private float CheckDownSpeed(float downSpeed)
    {
        if (transform.position.y <= 1)
        {
            verticalMomentum = 0;
            controller.enabled = false;
            healthSistem.UpdateHealth(-100);
            transform.position = ChunkSerializer.pos+new Vector3(0,1,0);
            controller.enabled = true;
            return 0;
        }
        if (transform.position.y > 159)
        {
            return downSpeed;
        }
        if (
            wmanager.blockTypes[wmanager.Block(transform.position.x - 0.3f, transform.position.y + downSpeed-1, transform.position.z - 0.3f)].Items.isblock ||
            wmanager.blockTypes[wmanager.Block(transform.position.x + 0.3f, transform.position.y + downSpeed - 1, transform.position.z - 0.3f)].Items.isblock ||
            wmanager.blockTypes[wmanager.Block(transform.position.x + 0.3f, transform.position.y + downSpeed - 1, transform.position.z + 0.3f)].Items.isblock ||
            wmanager.blockTypes[wmanager.Block(transform.position.x - 0.3f, transform.position.y + downSpeed - 1, transform.position.z + 0.3f)].Items.isblock
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
    private float CheckUpSpeed(float upSpeed)
    {
        if (transform.position.y > 156)
        {
            return upSpeed;
        }
        if (
            wmanager.blockTypes[wmanager.Block(transform.position.x - 0.3f, transform.position.y + 0.7f + upSpeed, transform.position.z - 0.3f)].Items.isblock ||
            wmanager.blockTypes[wmanager.Block(transform.position.x + 0.3f, transform.position.y + 0.7f + upSpeed, transform.position.z - 0.3f)].Items.isblock ||
            wmanager.blockTypes[wmanager.Block(transform.position.x + 0.3f, transform.position.y + 0.7f + upSpeed, transform.position.z + 0.3f)].Items.isblock ||
            wmanager.blockTypes[wmanager.Block(transform.position.x - 0.3f, transform.position.y + 0.7f + upSpeed, transform.position.z + 0.3f)].Items.isblock
           )
        {

            return 0;

        }
        else
        {

            return upSpeed;

        }

    }

    private bool IsInWater()
    {
        Vector3[] checkPositions = new Vector3[]
        {
        transform.position, 
        transform.position + new Vector3(0, 0, -0.5f), 
        transform.position + new Vector3(0, 0, 0.5f), 
        transform.position + new Vector3(0.5f, 0, 0), 
        transform.position + new Vector3(0.5f, 0, -0.5f), 
        transform.position + new Vector3(-0.5f, 0, 0),
        transform.position + new Vector3(-0.5f, 0, +0.5f) 
        };

        bool isInWaterAtAnyPosition = false;

        foreach (Vector3 pos in checkPositions)
        {
            Vector3Int blockPos = Vector3Int.FloorToInt(pos);
            byte b = wmanager.Block(blockPos.x, blockPos.y, blockPos.z);

            if (b == 21) 
            {
                isInWaterAtAnyPosition = true;
                break; 
            }
        }

        if (isInWaterAtAnyPosition)
        {
            if (!inWater)
            {
                inWater = true;
                watersprite.gameObject.SetActive(true);
                RenderSettings.fogDensity = 0.04f;
                RenderSettings.fogColor = new Color(12f / 256f, 0f, 156f / 256f, 1);
                timeinWater = 0;
            }
            else
            {
                timeinWater += Time.deltaTime;
                if (timeinWater >= 10)
                {
                    Shake();
                    healthSistem.UpdateHealth(-0.5f);
                    timeinWater -= 1.2f;
                }
            }
        }
        else if (inWater)
        {
            inWater = false;
            watersprite.gameObject.SetActive(false);
            RenderSettings.fogDensity = 0.01f;
            RenderSettings.fogColor = new Color(0.5f, 0.5f, 0.5f, 1);
        }
        else if (timeinWater > 0)
        {
            timeinWater -= Time.deltaTime;
        }

        return isInWaterAtAnyPosition;
    }



    public float time = 0,holdtme=0;
    public static int a, b, c;
    public bool breac=false;
    public float duration = 0.2f; // How long the shake lasts
    public float magnitude = 0.005f; // How strong the shake is

    public void Shake()
    {
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        Vector3 originalPosition = cam.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float xOffset = Random.Range(-1f, 1f) * magnitude;
            float yOffset = Random.Range(-1f, 1f) * magnitude;
            cam.position = originalPosition + new Vector3(xOffset, yOffset, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.position = originalPosition;
    }
    void HandleMouseInput()
    {

        if (Input.GetMouseButton(0)) // Left mouse button/break
        {
            if (toolbar.item[0, Toolbar.slothIndex]>0 && wmanager.blockTypes[toolbar.item[0, Toolbar.slothIndex]].Items.tool.type == 1)
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
                        if ((wmanager.blockTypes[wmanager.Block(pos.x, pos.y, pos.z)].Items.blocks.durability == 0))
                        {
                            if (brktime <= 0)
                            {
                                soundTrack.Placement(1);
                                brktime = 1.47f;
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
                                box.SetActive(true);
                                box.transform.position = new Vector3(a, b, c);
                                holdtme = 0;
                            }
                            if (holdtme >= wmanager.blockTypes[WorldManager.GetChunk(s/16,k/16).Voxels[g % 16, Mathf.RoundToInt(pos.y), h % 16].Value1].Items.blocks.breakTime)
                            {
                                box.SetActive(false);
                                itemsManager.SetItem(wmanager.Block(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z)), 1, new Vector3(pos.x, Mathf.RoundToInt(pos.y) - 0.3f, pos.z));
                                wmanager.ModifyMesh(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z), new Chunk.VoxelStruct(0,0));

                                breac = false;
                                holdtme = 0;
                            }
                        }
                        break;
                    }
                    step += 0.1f;
                }
            }
            else if(toolbar.item[0, Toolbar.slothIndex] > 0 && wmanager.blockTypes[toolbar.item[0, Toolbar.slothIndex]].Items.tool.type == 2)
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
                        if (wmanager.blockTypes[wmanager.Block(pos.x, pos.y, pos.z)].Items.blocks.durability == 1)
                        {
                            if (brktime <= 0)
                            {
                                soundTrack.Placement(1);
                                brktime = 1f;
                            }
                            if (Mathf.RoundToInt(pos.x) == a && Mathf.RoundToInt(pos.y) == b && Mathf.RoundToInt(pos.z) == c)
                            {
                                holdtme += Time.deltaTime;
                            }
                            else
                            {
                                a = Mathf.RoundToInt(pos.x);
                                b = Mathf.RoundToInt(pos.y);
                                c = Mathf.RoundToInt(pos.z);
                                box.SetActive(true);
                                box.transform.position = new Vector3(a, b, c);
                                holdtme = 0;
                            }
                            if (wmanager.Block(pos.x + 1, pos.y + 1, pos.z)!=7 && wmanager.Block(pos.x + 1, pos.y + 1, pos.z + 1)!=7 && wmanager.Block(pos.x - 1, pos.y + 1, pos.z)!=7 && wmanager.Block(pos.x - 1, pos.y + 1, pos.z - 1)!= 7 && wmanager.Block(pos.x + 1, pos.y + 1, pos.z - 1) != 7 && wmanager.Block(pos.x, pos.y + 1, pos.z - 1) != 7 && wmanager.Block(pos.x, pos.y + 1, pos.z + 1) != 7)
                            {
                                if (holdtme >= 1.4f)
                                {
                                    box.SetActive(false);
                                    ItemsFunctions.CutDownTree(pos, wmanager.Block(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z)));
                                    breac = false;
                                    holdtme = 0;
                                }
                            }
                            else
                            {
                                if (holdtme >= 1.1f)
                                {
                                    box.SetActive(false);
                                    itemsManager.SetItem(wmanager.Block(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z)), 1, new Vector3(pos.x, Mathf.RoundToInt(pos.y) - 0.3f, pos.z));
                                    wmanager.ModifyMesh(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z), new Chunk.VoxelStruct(0, 0));

                                    breac = false;
                                    holdtme = 0;
                                }
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
            box.SetActive(false);
            soundTrack.Stopbreak();
        }
        if(brktime > 0)
        {
            brktime-=Time.deltaTime;
        }
        //right click
        if (Input.GetMouseButton(1) && time<=0)
        {
            if(toolbar.item[0, Toolbar.slothIndex] > 0 && wmanager.blockTypes[toolbar.item[0, Toolbar.slothIndex]].Items.tool.type==4)
            {
                ItemsFunctions.MakeMap();
                toolbar.UpdateAnItem(Toolbar.slothIndex);
                toolbar.item[0,Toolbar.slothIndex] = 23;
                toolbar.itemsize[0,Toolbar.slothIndex] = 1;
                toolbar.itemSlots[Toolbar.slothIndex].image.sprite = wmanager.blockTypes[23].itemSprite;
            }
            else if (toolbar.item[0, Toolbar.slothIndex] == 38)
            {
                if(!BookManager.readingBook)
                BookManager.instance.OpenABook();
            }
            else if (toolbar.item[0, Toolbar.slothIndex] == 55 && brktime<=0)
            {
                ItemsFunctions.RealKnife();
                brktime = 2;
            }
            else if (wmanager.blockTypes[toolbar.item[0, Toolbar.slothIndex]].Items.item.coolfunction == 5 && brktime<=0)
            {
                healthSistem.Heal(wmanager.blockTypes[toolbar.item[0, Toolbar.slothIndex]].Items.item.secondfunction);
                toolbar.UpdateAnItem(Toolbar.slothIndex);
                brktime = 2;
            }
            else if (toolbar.item[0, Toolbar.slothIndex] > 0 && wmanager.blockTypes[toolbar.item[0, Toolbar.slothIndex]].Items.isblock)
            {
                float step = 0.1f;
                Vector3 lastPos = new();
                while (step <= 5)
                {
                    Vector3 pos = cam.position + (cam.forward * step);
                    if (wmanager.IsBlock(pos.x, pos.y, pos.z))
                    {
                        if (CanPlace(lastPos))
                        {
                            time = 0.2f;
                            wmanager.ModifyMesh(Mathf.RoundToInt(lastPos.x), Mathf.RoundToInt(lastPos.y), Mathf.RoundToInt(lastPos.z), new Chunk.VoxelStruct(toolbar.item[0, Toolbar.slothIndex], (byte)(Random.Range(0, 2))));
                            //toolbar.UpdateAnItem(Toolbar.slothIndex);
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
                while (step <= 5)
                {
                    Vector3 pos = cam.position + (cam.forward * step);
                    if (wmanager.IsBlock(pos.x, pos.y, pos.z))
                    {
                        if (wmanager.blockTypes[wmanager.Block(pos.x, pos.y, pos.z)].Items.blocks.special >= 2)
                        {
                            switch (wmanager.blockTypes[wmanager.Block(pos.x, pos.y, pos.z)].Items.blocks.special)
                            {
                                case 2: toolbar.OpenInventory(1);
                                    break;
                                case 3: toolbar.OpenInventory(2);
                                    break;
                                case 4: Chest.Instance.OpenChest(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
                                    break;
                            }
                        }
                        time = 0.2f;
                        break;
                    }

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

    public void CloseMap()
    {
        mapImg.gameObject.SetActive(false);
    }

    public void Sleep(Vector3Int v)
    {
        Debug.Log("sleeping");

        if (WorldManager.currenttime > 550)
        {
            BookManager.readingBook = true;

            StartCoroutine(Sleeping(v));
        }

    }
    public IEnumerator Sleeping(Vector3Int v)
    {
        BookManager.readingBook = true;
        Toolbar.escape = true;
        toolbar.openedInv = true;
        Vector3[] w = new Vector3[3];
        w[0] = transform.position;
        w[1] = transform.eulerAngles;
        w[2] = cam.eulerAngles;
        Quaternion q = transform.rotation;
        if (wmanager.Block2(v.x, v.y, v.z) == 1)
        {
            transform.position = new Vector3(v.x, v.y + 0.7f, v.z);
        }
        else
        {
            transform.position = new Vector3(v.x, v.y + 0.7f, v.z + 1);
        }
        transform.localRotation = Quaternion.Euler(0, -180, 0);
        cam.localRotation = Quaternion.Euler(-75, 0, 0);
        yield return new WaitForSeconds(10);
        toolbar.GoToSleepNSave(w[0], q);
        //save
        Voxeldata.PlayerData.timesSlept++;
        PlayerDataData.SavePlayerFile();
        if (Voxeldata.PlayerData.timesSlept == 1)
            SceneManager.LoadScene("Waterfall");
        else
        {
            WorldManager.currenttime = 300;
            transform.position = w[0];
            transform.eulerAngles = w[1];
            cam.eulerAngles = w[2];
            Toolbar.escape = false;
            BookManager.readingBook = false;
            toolbar.openedInv = false;

        }
    }
}