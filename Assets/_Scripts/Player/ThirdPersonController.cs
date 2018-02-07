using UnityEngine;
using System.Collections;

public class ThirdPersonController : MonoBehaviour
{

    public float walkSpeed = 3.0f;
    [SerializeField]
    float trotSpeed = 4.0f;
    [SerializeField]
    float runSpeed = 6.0f;
    [SerializeField]
    float inAirControlAcceleration = 3.0f;
    [SerializeField]
    float jumpHeight = 0.5f;
    [SerializeField]
    float extraJumpHeight = 2.5f;
    [SerializeField]
    float gravity = 20.0f;
    [SerializeField]
    float controlledDescentGravity = 2.0f;
    [SerializeField]
    float speedSmoothing = 10.0f;
    [SerializeField]
    float rotateSpeed = 500.0f;
    [SerializeField]
    float trotAfterSeconds = 3.0f;

    [SerializeField]
    bool canJump = true;
    [SerializeField]
    bool canControlDescent = true;
    [SerializeField]
    bool canWallJump = false;

    private float jumpRepeatTime = 0.05f;
    private float wallJumpTimeout = 0.15f;
    private float jumpTimeout = 0.15f;
    private float groundedTimeout = 0.25f;
    private float lockCameraTimer = 0.0f;
    private Vector3 moveDirection = Vector3.zero;
    private float verticalSpeed = 0.0f;
    private float moveSpeed = 0.0f;
    private CollisionFlags collisionFlags;
    private bool jumping = false;
    private bool jumpingReachedApex = false;
    private bool movingBack = false;
    private bool isMoving = false;
    private float walkTimeStart = 0.0f;
    private float lastJumpButtonTime = -10.0f;
    private float lastJumpTime = -1.0f;
    private Vector3 wallJumpContactNormal;
    private float wallJumpContactNormalHeight;
    private float lastJumpStartHeight = 0.0f;
    private float touchWallJumpTime = -1.0f;
    private Vector3 inAirVelocity = Vector3.zero;
    private float lastGroundedTime = 0.0f;
    private float lean = 0.0f;
    private bool slammed = false;
    private bool isControllable = true;

    void Awake()
    {
        moveDirection = transform.TransformDirection(Vector3.forward);
    }

    void HidePlayer()
    {
        GameObject.Find("rootJoint").GetComponent<SkinnedMeshRenderer>().enabled = false;
        isControllable = false;
    }

    void ShowPlayer()
    {
        GameObject.Find("rootJoint").GetComponent<SkinnedMeshRenderer>().enabled = true;
        isControllable = true;
    }

    void UpdateSmoothedMovementDirection()
    {
        Transform cameraTransform = Camera.main.transform;
        bool grounded = IsGrounded();
        Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0;
        forward = forward.normalized;

        Vector3 right = new Vector3(forward.z, 0, -forward.x);

        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");

        if (v < -0.2f) movingBack = true;
        else movingBack = false;

        bool wasMoving = isMoving;
        isMoving = Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;

        Vector3 targetDirection = h * right + v * forward;

        if (grounded)
        {

            lockCameraTimer += Time.deltaTime;
            if (isMoving != wasMoving) lockCameraTimer = 0.0f;

            if (targetDirection != Vector3.zero)
            {

                if (moveSpeed < walkSpeed * 0.9f && grounded)
                {
                    moveDirection = targetDirection.normalized;
                }
                else
                {
                    moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
                    moveDirection = moveDirection.normalized;
                }
            }

            float curSmooth = speedSmoothing * Time.deltaTime;
            float targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);

            if (Input.GetButton("Fire3"))
            {
                targetSpeed *= runSpeed;
            }
            else if (Time.time - trotAfterSeconds > walkTimeStart)
            {
                targetSpeed *= trotSpeed;
            }
            else
            {
                targetSpeed *= walkSpeed;
            }

            moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);

            if (moveSpeed < walkSpeed * 0.3f) walkTimeStart = Time.time;

        }
        else
        {

            if (jumping) lockCameraTimer = 0.0f;

            if (isMoving) inAirVelocity += targetDirection.normalized * Time.deltaTime * inAirControlAcceleration;
        }
    }

    void ApplyWallJump()
    {

        if (!jumping) return;

        if (collisionFlags == CollisionFlags.CollidedSides)
        {
            touchWallJumpTime = Time.time;
        }

        bool mayJump = lastJumpButtonTime > touchWallJumpTime - wallJumpTimeout && lastJumpButtonTime < touchWallJumpTime + wallJumpTimeout;
        if (!mayJump) return;

        if (lastJumpTime + jumpRepeatTime > Time.time) return;


        if (Mathf.Abs(wallJumpContactNormal.y) < 0.2f)
        {
            wallJumpContactNormal.y = 0;
            moveDirection = wallJumpContactNormal.normalized;
            moveSpeed = Mathf.Clamp(moveSpeed * 1.5f, trotSpeed, runSpeed);
        }
        else
        {
            moveSpeed = 0;
        }

        verticalSpeed = CalculateJumpVerticalSpeed(jumpHeight);
        DidJump();
        SendMessage("DidWallJump", null, SendMessageOptions.DontRequireReceiver);
    }

    void ApplyJumping()
    {

        if (lastJumpTime + jumpRepeatTime > Time.time) return;

        if (IsGrounded())
        {
            if (canJump && Time.time < lastJumpButtonTime + jumpTimeout)
            {
                verticalSpeed = CalculateJumpVerticalSpeed(jumpHeight);
                SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    void ApplyGravity()
    {
        if (isControllable)
        {

            bool jumpButton = Input.GetButton("Jump");
            bool controlledDescent = canControlDescent && verticalSpeed <= 0.0f && jumpButton && jumping;

            if (jumping && !jumpingReachedApex && verticalSpeed <= 0.0f)
            {
                jumpingReachedApex = true;
                SendMessage("DidJumpReachApex", SendMessageOptions.DontRequireReceiver);
            }

            bool extraPowerJump = IsJumping() && verticalSpeed > 0.0f && jumpButton && transform.position.y < lastJumpStartHeight + extraJumpHeight;

            if (controlledDescent)
                verticalSpeed -= controlledDescentGravity * Time.deltaTime;
            else if (extraPowerJump)
                return;
            else if (IsGrounded())
                verticalSpeed = 0.0f;
            else
                verticalSpeed -= gravity * Time.deltaTime;
        }
    }

    float CalculateJumpVerticalSpeed(float targetJumpHeight)
    {
        return Mathf.Sqrt(2 * targetJumpHeight * gravity);
    }

    void DidJump()
    {
        jumping = true;
        jumpingReachedApex = false;
        lastJumpTime = Time.time;
        lastJumpStartHeight = transform.position.y;
        touchWallJumpTime = -1;
        lastJumpButtonTime = -10;
    }

    void Update()
    {

        if (!isControllable)
        {
            Input.ResetInputAxes();
        }

        if (Input.GetButtonDown("Jump"))
        {
            lastJumpButtonTime = Time.time;
        }

        UpdateSmoothedMovementDirection();
        ApplyGravity();

        if (canWallJump) ApplyWallJump();

        ApplyJumping();

        Vector3 movement = moveDirection * moveSpeed + new Vector3(0, verticalSpeed, 0) + inAirVelocity;
        movement *= Time.deltaTime;

        CharacterController controller = GetComponent<CharacterController>();
        wallJumpContactNormal = Vector3.zero;
        collisionFlags = controller.Move(movement);

        if (IsGrounded())
        {
            if (slammed)
            {
                slammed = false;
                controller.height = 2;
                transform.position = transform.position + new Vector3(0, 0.75f, 0);
            }

            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
        else
        {
            if (!slammed)
            {
                Vector3 xzMove = movement;
                xzMove.y = 0;
                if (xzMove.sqrMagnitude > 0.001f)
                {
                    transform.rotation = Quaternion.LookRotation(xzMove);
                }
            }
        }

        if (IsGrounded())
        {
            lastGroundedTime = Time.time;
            inAirVelocity = Vector3.zero;
            if (jumping)
            {
                jumping = false;
                SendMessage("DidLand", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.moveDirection.y > 0.01f) return;
        wallJumpContactNormal = hit.normal;
    }

    public float GetSpeed()
    {
        return moveSpeed;
    }

    public bool IsJumping()
    {
        return jumping && !slammed;
    }

    public bool IsGrounded()
    {
        return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
    }

    public void SuperJump(float height)
    {
        verticalSpeed = CalculateJumpVerticalSpeed(height);
        collisionFlags = CollisionFlags.None;
        SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
    }

    void SuperJump(float height, Vector3 jumpVelocity)
    {
        verticalSpeed = CalculateJumpVerticalSpeed(height);
        inAirVelocity = jumpVelocity;

        collisionFlags = CollisionFlags.None;
        SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
    }

    void Slam(Vector3 direction)
    {
        verticalSpeed = CalculateJumpVerticalSpeed(1);
        inAirVelocity = direction * 6;
        direction.y = 0.6f;
        Quaternion.LookRotation(-direction);
        CharacterController controller = GetComponent<CharacterController>();
        controller.height = 0.5f;
        slammed = true;
        collisionFlags = CollisionFlags.None;
        SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
    }

    Vector3 GetDirection()
    {
        return moveDirection;
    }

    bool IsMovingBackwards()
    {
        return movingBack;
    }

    float GetLockCameraTimer()
    {
        return lockCameraTimer;
    }

    public bool IsMoving()
    {
        return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5f;
    }

    public bool HasJumpReachedApex()
    {
        return jumpingReachedApex;
    }

    public bool IsGroundedWithTimeout()
    {
        return lastGroundedTime + groundedTimeout > Time.time;
    }

    public bool IsControlledDescent()
    {
        bool jumpButton = Input.GetButton("Jump");
        return canControlDescent && verticalSpeed <= 0.0f && jumpButton && jumping;
    }

    void Reset()
    {
        gameObject.tag = "Player";
    }

}