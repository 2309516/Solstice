using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float rotationSpeed = 10f;

    [Header("Jump Settings")]
    public float jumpForce = 10f;
    public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter = 0f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.4f;
    public LayerMask groundMask;

    [Header("Air Control Settings")]
    [Tooltip("Dampens player movement in air")]
    public float airControlMultiplier = 0.3f;

    [Header("Dash Settings")]
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public KeyCode dashKey = KeyCode.LeftControl;
    private bool isDashing = false;

    [Header("Air Dash Settings")]
    public bool canAirDash = true;
    private bool hasDashedInAir = false;

    private Rigidbody rb;
    private Camera mainCamera;
    private bool isGrounded;
    private Vector3 inputDir;

    [Header("Camera Shake Settings")]
    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.1f;
    private Vector3 cameraInitialPos;

    [Header("Dash VFX")]
    public ParticleSystem dashSpeedLines;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        cameraInitialPos = mainCamera.transform.localPosition;

    }

    private void FixedUpdate()
    {
        GroundCheck();

        if (!isDashing)
        {
            Move();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        if (Input.GetKeyDown(dashKey) && !isDashing && inputDir.magnitude > 0f && isGrounded)
        {
            StartCoroutine(PerformDash());
        }
            if (Input.GetKeyDown(KeyCode.LeftShift) && !isGrounded && canAirDash && !hasDashedInAir)
        {
            StartCoroutine(PerformDash());
        }

        if (jumpBufferCounter > 0f && isGrounded)
        {
            Jump();
            jumpBufferCounter = 0f;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }

        if (jumpBufferCounter > 0f)
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        inputDir = new Vector3(horizontal, 0f, vertical).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            float baseSpeed = (Input.GetKey(KeyCode.LeftShift) && isGrounded) ? sprintSpeed : walkSpeed;
            float targetSpeed = isGrounded ? baseSpeed : baseSpeed * airControlMultiplier;

            Vector3 camForward = mainCamera.transform.forward;
            camForward.y = 0f;
            camForward.Normalize();
            Vector3 camRight = mainCamera.transform.right;
            camRight.y = 0f;
            camRight.Normalize();
            Vector3 moveDir = (camForward * inputDir.z + camRight * inputDir.x).normalized;

            rb.MovePosition(rb.position + moveDir * targetSpeed * Time.fixedDeltaTime);

            if (isGrounded)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        if (jumpBufferCounter > 0f && isGrounded)
        {
            Jump();
            jumpBufferCounter = 0f;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }

        if (jumpBufferCounter > 0f)
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void GroundCheck()
{
    isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

    if (isGrounded)
    {
        hasDashedInAir = false;
    }
}

    IEnumerator PerformDash()
    {
    if (!canAirDash && !isGrounded)
        yield break;

    if (isGrounded)
    {
        hasDashedInAir = false;
    }
    else if (hasDashedInAir)
    {
        yield break;
    }

    isDashing = true;

    Vector3 camForward = mainCamera.transform.forward;
    camForward.y = 0f;
    camForward.Normalize();
    Vector3 camRight = mainCamera.transform.right;
    camRight.y = 0f;
    camRight.Normalize();

    Vector3 dashDir = (camForward * inputDir.z + camRight * inputDir.x).normalized;

    rb.velocity = Vector3.zero;
    rb.AddForce(dashDir * dashForce, ForceMode.VelocityChange);

    if (dashSpeedLines != null) dashSpeedLines.Play();
    StartCoroutine(CameraShake());


    hasDashedInAir = true;

    yield return new WaitForSeconds(dashDuration);

    if (dashSpeedLines != null) dashSpeedLines.Stop();

    rb.velocity = new Vector3(0f, rb.velocity.y, 0f);

    isDashing = false;
    }

    IEnumerator CameraShake()
    {
    float elapsed = 0f;

    while (elapsed < shakeDuration)
    {
        float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
        float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;

        mainCamera.transform.localPosition = cameraInitialPos + new Vector3(offsetX, offsetY, 0);

        elapsed += Time.deltaTime;
        yield return null;
    }
    mainCamera.transform.localPosition = cameraInitialPos;
    }


}
