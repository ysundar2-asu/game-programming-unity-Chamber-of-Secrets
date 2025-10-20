using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 6.0f;
    public float sprintSpeed = 9.0f;
    public float jumpHeight = 1.4f;
    public float gravity = -20f;

    [Header("Mouse Look")]
    public Transform cameraRoot;   
    public float mouseSensitivity = 120f;
    public float minPitch = -85f;
    public float maxPitch = 85f;

    [Header("Ground Check")]
    public Transform groundCheck;          
    public float groundRadius = 0.25f;
    public LayerMask groundMask;           
    public float coyoteTime = 0.1f;        
    public float jumpBuffer = 0.1f;

    [Header("Interaction (optional)")]
    public bool enableInteraction = false; 
    public float interactDistance = 3f;
    public LayerMask interactableMask;
    public KeyCode interactKey = KeyCode.Mouse0;

    private CharacterController controller;
    private Vector3 velocity;              
    private float yaw, pitch;

    private float lastGroundedTime = -999f;
    private float lastJumpPressedTime = -999f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cameraRoot == null) cameraRoot = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Look();
        HandleJumpInput();
        Move();

        if (enableInteraction) InteractCheck();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked) ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = (Cursor.lockState != CursorLockMode.Locked);
        }
    }

    void Look()
    {
        float mx = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float my = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw   += mx;
        pitch -= my;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        cameraRoot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            lastJumpPressedTime = Time.time;
    }

    bool DoGroundCheck()
    {
        bool groundedPhysics = false;
        if (groundCheck != null)
            groundedPhysics = Physics.CheckSphere(groundCheck.position, groundRadius, (groundMask.value == 0 ? ~0 : groundMask), QueryTriggerInteraction.Ignore);

        bool grounded = controller.isGrounded || groundedPhysics;

        if (grounded) lastGroundedTime = Time.time;
        return grounded;
    }

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(x, 0f, z).normalized;

        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        Vector3 move = (transform.right * input.x + transform.forward * input.z) * speed;

        bool grounded = DoGroundCheck();
        if (grounded && velocity.y < 0f)
            velocity.y = -2f; 

        bool canCoyote = (Time.time - lastGroundedTime) <= coyoteTime;
        bool buffered  = (Time.time - lastJumpPressedTime) <= jumpBuffer;

        if (canCoyote && buffered)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            lastJumpPressedTime = -999f; 
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 total = move + new Vector3(0f, velocity.y, 0f);
        controller.Move(total * Time.deltaTime);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }

    void InteractCheck()
    {
        if (Input.GetKeyDown(interactKey))
        {
            Ray ray = new Ray(cameraRoot.position, cameraRoot.forward);
            LayerMask mask = (interactableMask.value == 0) ? ~0 : interactableMask;
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, mask))
            {
                var interactable = hit.collider.GetComponentInParent<IInteractable>();
                if (interactable != null) interactable.Interact();
            }
        }
    }
}

public interface IInteractable
{
    void Interact();
    string Prompt { get; }
}