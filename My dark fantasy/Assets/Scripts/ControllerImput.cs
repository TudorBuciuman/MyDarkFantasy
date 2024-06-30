using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Chunk voxelTerrain;
    public float reach = 5f;
    public float movementSpeed = 5f;
    public float lookSpeed = 400f;
    public float smoothSpeed = 100.0f;
    public float jumpForce = 5f;
    public float gravity = 98f;
    private Rigidbody rb;
    public Transform orientation;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        Application.targetFrameRate = 50;
        //   Cursor.lockState = CursorLockMode.Locked;
        //  Cursor.visible = false;
        characterController = gameObject.AddComponent<CharacterController>();
        characterController.transform.position = new Vector3(12.0f,60.0f,4.0f);
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        CheckViewDistance();
    }
  
    void HandleMovement()
    {
        
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            moveDirection = new Vector3(moveX, 0f, moveZ);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= movementSpeed;

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpForce;
            }
            if (Input.GetKey(KeyCode.W))
            {
                moveDirection.z += Time.deltaTime*lookSpeed;
            }
            if (Input.GetKey(KeyCode.S))
            {
                moveDirection.z -= Time.deltaTime * lookSpeed;
            }
            if (Input.GetKey(KeyCode.A))
            {
                moveDirection.x += Time.deltaTime * lookSpeed;
            }
            if (Input.GetKey(KeyCode.D))
            {
                moveDirection.x -= Time.deltaTime * lookSpeed;
            }

        }

        moveDirection.y -= gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);
    }

    public ChunkCoord GetPosition()
    {
        Vector3 position = transform.position;
        return new ChunkCoord(Mathf.FloorToInt(position.x) , Mathf.FloorToInt(position.z));
    }
    
    // Daca schimbi ceva se vor prabusi turnurile gemene
    void HandleMouseLook()
    {
        float x= Input.GetAxis("Mouse X") * Time.deltaTime * smoothSpeed;
        float y= Input.GetAxis("Mouse Y") * Time.deltaTime * smoothSpeed;
        rotationX -= y;
        rotationY += x;

        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        orientation.rotation = Quaternion.Euler(rotationX, rotationY, 0);
    }

    public void HandleBlockPlacement()
    {

        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, reach))
            {
                Vector3 hitPosition = hit.point - hit.normal * 0.5f;
                Debug.Log(hitPosition.x + ", " + hitPosition.y + ", " + hitPosition.z);
            }
        }

        if (Input.GetMouseButtonDown(1)) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, reach))
            {
                Vector3 hitPosition = hit.point + hit.normal * 0.5f;
               // voxelTerrain.PlaceBlock(hitPosition);
            }
        }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        transform.Rotate(Vector3.up * mouseX * 5f);
        transform.Rotate(Vector3.right * -mouseY * 5f);
    }

    public void CheckViewDistance()
    {

    }

}

