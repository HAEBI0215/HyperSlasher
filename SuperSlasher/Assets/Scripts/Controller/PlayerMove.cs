using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("움직임")]
    public float runSpeed = 12f;
    public float runMag = 1.5f;
    public float accelerationTime = 0.5f;
    public float walkSpeed = 6f;
    public float rotateSpeed = 10f;

    [Header("점프")]
    public float jumpForce = 7f;
    public float gravity = 3.6f;

    [Header("키")]
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;

    public float currentSpeed = 0.0f;
    public bool isRunning = false;
    public bool isGrounded = true;

    public LayerMask groundLayer;
    public float groundDistance = 10.2f;
    private Vector2 moveInput;
    public Transform groundCheck;
    public Rigidbody rb;
    public Transform camPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        isGrounded = Physics.SphereCast(groundCheck.position, 0.2f, Vector3.down, out RaycastHit hit, groundDistance, groundLayer);

        isRunning = Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = isRunning ? runSpeed : walkSpeed;

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime / accelerationTime);
        
        if (Input.GetKeyDown(jumpKey))
        {
            Jump();
        }
        rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
    }

    void Move(Vector2 input)
    {
        moveInput = input;
    }

    void FixedUpdate()
    {
        Vector3 camForward = camPos.forward;
        Vector3 camRight = camPos.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camRight * moveInput.x + camForward * moveInput.y;

        Vector3 velocity = move * currentSpeed;
        velocity.y = rb.velocity.y;

        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 
            rotateSpeed * Time.deltaTime);
        }

        rb.velocity = velocity;
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * gravity * 2f * Time.deltaTime;
        }
    }

    public void SetRunSpeed(float mag)
    {
        runMag += mag;
        runSpeed = walkSpeed * runMag;
    }

    public void Jump()
    {
        if (isGrounded == false)
            return;
        
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }
}
