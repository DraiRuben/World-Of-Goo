using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D CinemachineFollowTarget;
    [SerializeField]
    private PolygonCollider2D boundingBox;
    [SerializeField]
    private CinemachineVirtualCamera vcam;

    private Vector2 MovementInput;
    [SerializeField]
    private float CameraSpeed = 5f;
    [SerializeField]
    private float m_zoomSpeed = 4f;
    [SerializeField]
    private float m_minZoom = 3f;
    [SerializeField]
    private float m_maxZoom = 15f;

    private CinemachineConfiner2D m_confiner;
    private Vector2 Offset;
    Camera cam;
    private void Awake()
    {
        cam = Camera.main;
        //gets screen width and height in world position
        Offset = new(cam.orthographicSize * cam.aspect, cam.orthographicSize);
    }
    private void Start()
    {
        m_confiner = vcam.GetComponent<CinemachineConfiner2D>();
    }
    public void Zoom(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            float modifier = ctx.ReadValue<float>()/120;
            vcam.m_Lens.OrthographicSize -= modifier * m_zoomSpeed/10f;
            vcam.m_Lens.OrthographicSize = Mathf.Clamp(vcam.m_Lens.OrthographicSize, m_minZoom, m_maxZoom);
        }
    }
    public void CameraControls(InputAction.CallbackContext ctx)
    {
        MovementInput = ctx.ReadValue<Vector2>();
    }
    private void Update()
    {
        //changes offset if the viewport is resized, either from screen resize or a screen change
        //for some fucking reason, the comparison doesn't work and returns the inverse of the condition,
        //which is why I need to reverse the conditions, ffs
        if (!(Offset.x != (cam.orthographicSize * cam.aspect) )
            || (Offset.y != cam.orthographicSize))
        {
            Offset = new(cam.orthographicSize * cam.aspect, cam.orthographicSize);
            m_confiner.InvalidateCache();
        }
    }
    private void FixedUpdate()
    {
        //moves target
        CinemachineFollowTarget.velocity = MovementInput.normalized * CameraSpeed;
        //clamps target's position so it doesn't go outside of camera's movable range and induces some lag from having to come back to this very range
        Vector3 TargetPosition = CinemachineFollowTarget.position;
        TargetPosition.x = Mathf.Clamp(TargetPosition.x, boundingBox.bounds.min.x + Offset.x, boundingBox.bounds.max.x - Offset.x);
        TargetPosition.y = Mathf.Clamp(TargetPosition.y, boundingBox.bounds.min.y + Offset.y, boundingBox.bounds.max.y - Offset.y);
        CinemachineFollowTarget.position = TargetPosition;
    }
}
