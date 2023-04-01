using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFPSController : MonoBehaviour
{   
    public Transform Orientation;

    public float
        MoveSpeed = 100F,
        GroundDrag = 2F,
        PlayerHeight = 2F,
        JumpForce = 50F,
        JumpCooldown = 0.25F,
        AirMultiplier = 0.4F;
    
    public LayerMask WhatIsGround;

    private Vector3
        InputVelocity = Vector3.zero,
        MoveDirection = Vector3.zero;
    
    private bool
        IsGrounded,
        IsReadyToJump = true;
    
    private Rigidbody Rigidbody;

    private void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        InputVelocity = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0,
            Input.GetAxisRaw("Vertical"));
        
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, PlayerHeight / 2F + 0.2F, WhatIsGround);

        Rigidbody.drag = IsGrounded ? GroundDrag : 0;
        
        Vector3 flatVelocity = Rigidbody.velocity;
        flatVelocity.y = 0F;

        if (flatVelocity.magnitude > MoveSpeed)
        {
            Rigidbody.velocity = flatVelocity.normalized * MoveSpeed * Time.fixedDeltaTime * 10F; 
        }

        if (IsReadyToJump && IsGrounded && Input.GetKey(KeyCode.Space))
        {
            IsReadyToJump = false;
            
            Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, 0F, Rigidbody.velocity.z);
            Rigidbody.AddForce(transform.up * JumpForce * Time.fixedDeltaTime * 10F, ForceMode.Impulse);

            Invoke(nameof(ResetJump), JumpCooldown);
        }
    }

    private void FixedUpdate()
    {   
        MoveDirection = Orientation.right * InputVelocity.x + Orientation.forward * InputVelocity.z;

        Rigidbody.AddForce(MoveDirection.normalized * MoveSpeed * Time.fixedDeltaTime * 10F * (!IsGrounded ? AirMultiplier : 1F), ForceMode.Force);
    }

    private void ResetJump() { IsReadyToJump = true; }
}