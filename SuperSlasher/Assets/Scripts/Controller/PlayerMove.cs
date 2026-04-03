using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("움직임")]
    public float runSpeed = 8f;
    public float sprintSpeed = 15f;
    public float accelerationTime = 1f;
    public float rotateSpeed = 10f;

    [Header("점프")]
    public float jumpForce = 7f;
    public float gravity = 1f;

    [Header("키")]
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;

    public float currentSpeed = 0f;
    public bool isRunning = false;
    public bool isGrounded = true;
    private bool isSprintAnim = false;

    [Header("지면 체크")]
    public LayerMask groundLayer;
    public float groundDistance = 0.3f;
    public Transform groundCheck;

    [Header("참조")]
    public Rigidbody rb;
    public Transform camPos;
    public Animator anim;

    private Vector2 moveInput;

    void Update()
    {
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        float inputMagnitude = moveInput.magnitude;

        isRunning = Input.GetKey(runKey);

        isGrounded = Physics.SphereCast(groundCheck.position, 0.2f, Vector3.down, out _, groundDistance, groundLayer);

        float targetSpeed = 0f;
        if (inputMagnitude > 0.1f)
        {
            targetSpeed = isRunning ? sprintSpeed : runSpeed;
        }

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime / accelerationTime);

        if (Input.GetKeyDown(jumpKey))
        {
            Jump();
        }

        rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        if (isRunning || currentSpeed > 10f)
        {
            isSprintAnim = true;
        }
        else
        {
            isSprintAnim = false;
        }

        float speedPercent = 0f;
        if (inputMagnitude > 0.1f)
        {
            speedPercent = isSprintAnim ? 1f : 0.6f;
        }

        anim.SetFloat("Speed", speedPercent, 0.1f, Time.deltaTime);
        anim.SetBool("IsRunning", isSprintAnim);
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("VelocityY", rb.velocity.y);

        float animSpeed = currentSpeed / runSpeed;

        if (isSprintAnim)
        {
            animSpeed *= 1f;
        }

        animSpeed = Mathf.Clamp(animSpeed, 0f, 1.2f);

        anim.SetFloat("AnimSpeed", animSpeed);
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

        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotateSpeed * Time.deltaTime
            );
        }

        rb.velocity = velocity;

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * gravity * 2f * Time.deltaTime;
        }
    }

    public void Jump()
    {
        if (!isGrounded) return;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;

        anim.SetTrigger("Jump");
    }
}