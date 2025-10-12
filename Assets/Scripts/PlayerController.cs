using UnityEngine;

[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField, Tooltip("CrowdSystem used to compute dynamic crowd radius for clamping.")]
    private CrowdSystem _crowdSystem;

    [SerializeField, Min(0f), Tooltip("Road width in world units (X-axis).")]
    private float _roadWidth = 8f;

    [Header("Movement Settings")]
    [SerializeField, Min(0f), Tooltip("Forward movement speed (world Z per second).")]
    private float _moveSpeed = 3f;

    [Header("Control Settings")]
    [SerializeField, Min(0f), Tooltip("Horizontal sensitivity: how many road-widths per full-screen swipe.")]
    private float _slideSensitivity = 1.0f;

    [SerializeField, Range(0f, 0.3f), Tooltip("Smoothing time for horizontal movement (seconds). 0 = instant.")]
    private float _slideSmoothTime = 0.08f;

    private Vector3 _pointerDownScreenPos = Vector3.zero;
    private Vector3 _playerPosAtPointerDown = Vector3.zero;
    private float _xVelocity;
    private bool _dragging;

    private void Update()
    {
        MoveForward();
        ManageControl();
    }

    private void ManageControl()
    {
        // Mouse (Editor/Desktop) + Touch (Mobile) unified
        bool pointerDown = Input.GetMouseButtonDown(0) || TouchBegan();
        bool pointerHeld = Input.GetMouseButton(0) || TouchHeld();

        if(pointerDown)
        {
            _dragging = true;
            _pointerDownScreenPos = GetPointerScreenPosition();
            _playerPosAtPointerDown = transform.position;
        }
        else if(_dragging && pointerHeld)
        {
            float dxScreen = GetPointerScreenPosition().x - _pointerDownScreenPos.x; // pixels
            float dxNormalized = dxScreen / Mathf.Max(1f, Screen.width);             // 0..1
            float deltaX = dxNormalized * _slideSensitivity * _roadWidth;            // world units (proportional to road width)

            float targetX = _playerPosAtPointerDown.x + deltaX;

            // Clamp with current crowd radius
            float radius = GetCrowdRadiusSafe(); // 0 if _crowdSystem is null
            float half = Mathf.Max(0f, _roadWidth * 0.5f - radius); // ensure non-negative half
            targetX = Mathf.Clamp(targetX, -half, half);

            // Smooth horizontal movement (X only)
            Vector3 pos = transform.position;
            pos.x = (_slideSmoothTime > 0f)
                ? Mathf.SmoothDamp(pos.x, targetX, ref _xVelocity, _slideSmoothTime)
                : targetX;

            transform.position = pos;
        }
        else
        {
            _dragging = false;
            _xVelocity = 0f;
        }
    }

    public void MoveForward()
    {
        transform.position += Vector3.forward * (_moveSpeed * Time.deltaTime);
    }

    /// <summary>Safe accessor to the current crowd radius. Falls back to 0 if no CrowdSystem.</summary>
    private float GetCrowdRadiusSafe()
    {
        if(_crowdSystem == null) return 0f;
        return _crowdSystem.GetBoundingRadius();
    }

    private static bool TouchBegan()
    {
        return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
    }

    private static bool TouchHeld()
    {
        if(Input.touchCount == 0) return false;
        var p = Input.GetTouch(0).phase;
        return p == TouchPhase.Moved || p == TouchPhase.Stationary;
    }

    private static Vector3 GetPointerScreenPosition()
    {
        if(Input.touchCount > 0) return Input.GetTouch(0).position;
        return Input.mousePosition;
    }

    /// <summary>
    /// Optional helper if you change road width at runtime.
    /// </summary>
    public void SetRoadWidth(float width)
    {
        _roadWidth = Mathf.Max(0f, width);
    }
}
