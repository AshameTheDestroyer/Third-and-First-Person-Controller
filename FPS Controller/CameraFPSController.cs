using System;
using System.Collections;
using UnityEngine;

public class CameraFPSController : MonoBehaviour
{
    public float Sensitivity = 200F;
    public Vector2 Rotation = Vector2.zero;

    public Transform Orientation, CameraPosition;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 mouse = new Vector2(
            Input.GetAxisRaw("Mouse X") * Time.deltaTime * Sensitivity,
            Input.GetAxisRaw("Mouse Y") * Time.deltaTime * Sensitivity);
        
        Rotation.x -= mouse.y;
        Rotation.y += mouse.x;

        Rotation.x = Mathf.Clamp(Rotation.x, -90F, +90F);

        transform.eulerAngles = Rotation;
        Orientation.eulerAngles = new Vector3(0, Rotation.y);

        transform.position = CameraPosition.position;
    }
}
