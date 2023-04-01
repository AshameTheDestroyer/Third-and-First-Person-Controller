using System;
using System.Collections;
using UnityEngine;

public class CameraTPSController : MonoBehaviour
{
    public Transform
        TargetTransform,
        PivotTransform,
        CameraTransform;
    public float
        FollowSpeed = 5F,
        Sensitivity = 130F,
        ClampYAngle = 45F,
        CollisionRadius = 0.4F,
        MinimumCollisionOffset = 0.2F;
    
    public LayerMask CollisionLayers;
    
    private float lookAngle, pivotAngle, defaultZPosition;
    private Vector3
        cameraFollowVelocity = Vector3.zero;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        defaultZPosition = CameraTransform.position.z;
    }

    private void LateUpdate()
    {
        FollowTarget();
        RotateCamera();
        HandleCollisions();
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(
            transform.position, TargetTransform.position,
            ref cameraFollowVelocity, FollowSpeed * Time.fixedDeltaTime);
        
        transform.position = targetPosition;
    }

    private void RotateCamera()
    {
        lookAngle += Input.GetAxisRaw("Mouse X") * Sensitivity * Time.fixedDeltaTime;
        pivotAngle -= Input.GetAxisRaw("Mouse Y") * Sensitivity * Time.fixedDeltaTime;

        pivotAngle = Mathf.Clamp(pivotAngle, -ClampYAngle, ClampYAngle);

        transform.eulerAngles = new Vector3(0F, lookAngle, 0F);
        PivotTransform.localEulerAngles = new Vector3(pivotAngle, 0F, 0F);
    }
    
    private void HandleCollisions()
    {
        float targetPosition = defaultZPosition;
        RaycastHit hit;
        Vector3 direction = CameraTransform.position - PivotTransform.position;
        direction.Normalize();

        if (Physics.SphereCast(PivotTransform.position, CollisionRadius,
            direction, out hit, Mathf.Abs(targetPosition), CollisionLayers))
        {
            float distance = Vector3.Distance(PivotTransform.position, hit.point);
            targetPosition += distance - CollisionRadius;
        }

        if (Mathf.Abs(targetPosition) < MinimumCollisionOffset)
        {
            targetPosition -= MinimumCollisionOffset;
        }

        Vector3 cameraPosition = CameraTransform.localPosition;
        cameraPosition.z = Mathf.Lerp(
            CameraTransform.localPosition.z, targetPosition, FollowSpeed * Time.fixedDeltaTime);
        CameraTransform.localPosition = cameraPosition;
    }
}