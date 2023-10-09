using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D CinemachineFollowTarget;
    [SerializeField]
    private PolygonCollider2D boundingBox;

    private Vector2 MovementInput;
    [SerializeField]
    private float CameraSpeed = 5f;

    private Vector2 Offset;
    Camera cam;
    private void Start()
    {
        cam = Camera.main;
        //gets screen width and height in world position
        Offset = new(cam.orthographicSize * cam.aspect, cam.orthographicSize);
    }
    public void CameraControls(InputAction.CallbackContext ctx)
    {
        MovementInput = ctx.ReadValue<Vector2>();
    }
    private void Update()
    {
        //changes offset if the viewport is resized, either from screen resize or a screen change
        if(Offset.x != cam.orthographicSize*cam.aspect || Offset.y != cam.orthographicSize)
        {
            Offset = new(cam.orthographicSize * cam.aspect, cam.orthographicSize);
        }
    }
    private void FixedUpdate()
    {
        //moves target
        CinemachineFollowTarget.velocity = MovementInput.normalized*CameraSpeed;
        //clamps target's position so it doesn't go outside of camera's movable range and induces some lag from having to come back to this very range
        Vector3 TargetPosition = CinemachineFollowTarget.position;
        TargetPosition.x = Mathf.Clamp(TargetPosition.x, boundingBox.bounds.min.x+Offset.x, boundingBox.bounds.max.x-Offset.x);
        TargetPosition.y = Mathf.Clamp(TargetPosition.y, boundingBox.bounds.min.y+Offset.y, boundingBox.bounds.max.y-Offset.y);
        CinemachineFollowTarget.position = TargetPosition;
    }
}
