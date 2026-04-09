using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("움직임")]
    public float runSpeed = 15f;
    public float hyperSprintSpeed = 30f;
    public float accelerationTime = 1f;
    public float rotateSpeed = 10f;

    [Header("전투 회전")]
    public bool useCombatRotate = true;
    public KeyCode combatKey = KeyCode.Mouse1; // 우클릭 누르면 카메라 방향 바라봄

    [Header("스킬")]
    public float hyperSprintGauge = 0.0f;
    public float maxHyperSprintGauge = 100.0f;
    public float hyperSprintCost = 5.0f;
    public bool isSkillReady = false;

    [Header("에프터 이미지")]
    public MonoBehaviour[] afterImage;

    [Header("점프")]
    public float jumpForce = 7f;
    public float fallGravityMultiplier = 2f;

    [Header("키")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode hyperSprintKey = KeyCode.LeftShift;

    public float currentSpeed = 0f;
    public bool isGrounded = true;
    private bool isHyperSprinting = false;

    [Header("지면 체크")]
    public LayerMask groundLayer;
    public float groundDistance = 0.3f;
    public Transform groundCheck;

    [Header("참조")]
    public Rigidbody rb;
    public Transform camPos; // 여기엔 MainCamera 말고 FollowPos 넣기
    public Animator anim;
    public PlayerManager pm;

    private Vector2 moveInput;

    void Start()
    {
        hyperSprintGauge = maxHyperSprintGauge;
        isSkillReady = hyperSprintGauge > 0f;

        if (rb != null)
        {
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    void Update()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveInput = Vector2.ClampMagnitude(moveInput, 1f);

        float inputMagnitude = moveInput.magnitude;

        isGrounded = Physics.SphereCast(
            groundCheck.position,
            0.2f,
            Vector3.down,
            out _,
            groundDistance,
            groundLayer
        );

        isHyperSprinting =
            Input.GetKey(hyperSprintKey) &&
            isSkillReady &&
            inputMagnitude > 0.1f;

        foreach (var script in afterImage)
        {
            if (script != null)
                script.enabled = isHyperSprinting;
        }

        float targetSpeed = 0f;

        if (inputMagnitude > 0.1f)
        {
            targetSpeed = isHyperSprinting ? hyperSprintSpeed : runSpeed;
        }

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime / accelerationTime);

        if (isHyperSprinting)
        {
            hyperSprintGauge -= hyperSprintCost * Time.deltaTime;

            if (hyperSprintGauge <= 0f)
            {
                hyperSprintGauge = 0f;
                isSkillReady = false;
                isHyperSprinting = false;
            }
        }

        float speedPercent = 0f;
        if (inputMagnitude > 0.1f)
        {
            speedPercent = isHyperSprinting ? 1f : 0.6f;
        }

        anim.SetFloat("Speed", speedPercent, 0.1f, Time.deltaTime);
        anim.SetBool("IsRunning", isHyperSprinting);
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("VelocityY", rb.velocity.y);

        float animSpeed = currentSpeed / runSpeed;
        animSpeed = Mathf.Clamp(animSpeed, 0f, 0.8f);
        anim.SetFloat("AnimSpeed", animSpeed);

        if (Input.GetKeyDown(jumpKey))
        {
            Jump();
        }
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
        move.Normalize();

        Vector3 velocity = move * currentSpeed;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;

        RotatePlayer(move);

        if (rb.velocity.y < 0f)
        {
            rb.velocity += Physics.gravity * (fallGravityMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    void RotatePlayer(Vector3 move)
    {
        bool isCombatRotating = useCombatRotate && Input.GetKey(combatKey);

        if (isCombatRotating)
        {
            Vector3 faceDir = camPos.forward;
            faceDir.y = 0f;
            faceDir.Normalize();

            if (faceDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(faceDir);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotateSpeed * Time.fixedDeltaTime
                );
            }
        }
        else
        {
            if (move.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotateSpeed * Time.fixedDeltaTime
                );
            }
        }
    }

    public void Jump()
    {
        if (!isGrounded) return;

        Vector3 velocity = rb.velocity;
        velocity.y = 0f;
        rb.velocity = velocity;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;

        anim.SetTrigger("Jump");
    }
}