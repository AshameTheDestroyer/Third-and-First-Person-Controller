using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAnimationState { Idle, Walking, Running, Sprinting, Falling, Jumping }

public class PlayerTPSController : MonoBehaviour
{
    public Transform
        CameraTransform,
        GroundCheckerTransform;

    public float
        WalkingSpeed = 200F,
        RunningSpeed = 400F,
        SprintingSpeed = 600F,
        RotationSpeed = 10F,
        FallingSpeed = 200F,
        LeapingOffset = 200F,
        GravityIntensity = 25F,
        JumpingHeight = 30F;
    public PlayerAnimationState AnimationState
    {
        get => animationState;
        set => OnPlayerAnimationStateChange(animationState = value);
    }
    public Action<PlayerAnimationState> OnPlayerAnimationStateChange = playerAnimationState => {};
    public KeyCode
        WalkingKey = KeyCode.LeftShift,
        SprintingKey = KeyCode.LeftControl,
        JumpingKey = KeyCode.Space;
    public LayerMask WhatIsGround;

    private Vector3 moveDirection;
    private float currentSpeed, inAirTimer;
    private bool isGrounded, isJumping;

    private new Rigidbody rigidbody;
    private PlayerAnimationState animationState = PlayerAnimationState.Idle;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
        HandleFalling();
        HandleJumping();
    }

    private void HandleInput()
    {
        moveDirection =
            CameraTransform.forward * Input.GetAxisRaw("Vertical") +
            CameraTransform.right * Input.GetAxisRaw("Horizontal");
        moveDirection.Normalize();

        HandleMovingState();
    }

    private void HandleMovingState()
    {
        if (!isGrounded || isJumping) { return; }

        if (moveDirection == Vector3.zero) { AnimationState = PlayerAnimationState.Idle; return; }
        if (Input.GetKey(SprintingKey)) { AnimationState = PlayerAnimationState.Sprinting; return; }
        if (Input.GetKey(WalkingKey)) { AnimationState = PlayerAnimationState.Walking; return; }

        AnimationState = PlayerAnimationState.Running;
    }

    private void HandleMovement()
    {
        switch (AnimationState)
        {
            case PlayerAnimationState.Idle: currentSpeed = 0F; break;
            case PlayerAnimationState.Walking: currentSpeed = WalkingSpeed; break;
            case PlayerAnimationState.Running: currentSpeed = RunningSpeed; break;
            case PlayerAnimationState.Sprinting: currentSpeed = SprintingSpeed; break;
            default: break;
        }

        Vector3 velocity = moveDirection * currentSpeed * Time.fixedDeltaTime;
        rigidbody.velocity = new Vector3(velocity.x, rigidbody.velocity.y, velocity.z);
    }

    private void HandleRotation()
    { 
        Quaternion targetRotation = Quaternion.LookRotation(
            moveDirection != Vector3.zero ? moveDirection : transform.forward);
        
        transform.rotation = Quaternion.Slerp(
            transform.rotation, targetRotation, RotationSpeed * Time.fixedDeltaTime);
    }

    private void HandleFalling()
    {
        RaycastHit hit;
        isGrounded =
            Physics.SphereCast(GroundCheckerTransform.position, 0.2F, Vector3.down, out hit, 0.2F, WhatIsGround);

        if (isGrounded)
        {
            inAirTimer = 0F;
            isJumping = false;

            if (AnimationState == PlayerAnimationState.Jumping)
            { AnimationState = PlayerAnimationState.Idle; }

            return;
        }

        inAirTimer += Time.fixedDeltaTime;
        rigidbody.AddForce(Vector3.down * FallingSpeed * inAirTimer * Time.fixedDeltaTime, ForceMode.Impulse);

        if (isJumping) { return; }

        rigidbody.AddForce(transform.forward * LeapingOffset * Time.fixedDeltaTime, ForceMode.Impulse);

        AnimationState = PlayerAnimationState.Falling;
    }

    private void HandleJumping()
    {
        if (!Input.GetKey(JumpingKey) || !isGrounded) { return; }

        const float SQRT_OF_TWO = 1.141F;
        float jumpingVelocity = SQRT_OF_TWO * GravityIntensity * JumpingHeight;

        Vector3 velocity =
            new Vector3(moveDirection.x, jumpingVelocity * Time.fixedDeltaTime, moveDirection.z);
        rigidbody.velocity = velocity;
        
        isJumping = true;
        AnimationState = PlayerAnimationState.Jumping;
    }
}