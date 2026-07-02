using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPinch : MonoBehaviour
{
    [Header("SettingsZoom")]
    public float zoomSpeed = 0.01f;
    public float minZoom = 3f;
    public float maxZoom = 10f;
    public float returnSpeed = 5f;

    [Header("SwipeRotateSettings")]
    public Transform target;
    public float rotationSpeed = 300f;
    public float swipeThreshold = 50f;

    float defaultZoom;
    Camera cam;

    private Vector2 startTouchPos;
    private bool isRotating = false;
    private float targetAngle;
    private float currentAngle;
    CameraFocus cameraController;


    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
        defaultZoom = cam.orthographicSize;
        currentAngle = transform.eulerAngles.y;
        targetAngle = currentAngle;
        cameraController = Camera.main.GetComponent<CameraFocus>();

    }

    void Update()
    {
        // PINCH (2 Ô‡Î¸ˆ‡)
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 prev0 = t0.position - t0.deltaPosition;
            Vector2 prev1 = t1.position - t1.deltaPosition;

            float prevDist = Vector2.Distance(prev0, prev1);
            float currDist = Vector2.Distance(t0.position, t1.position);

            float diff = currDist - prevDist;

            cam.orthographicSize -= diff * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
        // ¬Œ«¬–¿“ œ–» Œ“œ”— ¿Õ»»
        else
        {
            cam.orthographicSize = Mathf.Lerp(
                cam.orthographicSize,
                defaultZoom,
                Time.deltaTime * returnSpeed
            );
        }

        if (cameraController != null && !cameraController.IsInFocusMode())
        {
            HandleSwipe();
        }
        RotateCamera();
    }

    void HandleSwipe()
    {
        if (Input.touchCount != 1 || isRotating)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            startTouchPos = touch.position;
        }

        if (touch.phase == TouchPhase.Ended)
        {
            float deltaX = touch.position.x - startTouchPos.x;

            if (Mathf.Abs(deltaX) > swipeThreshold)
            {
                if (deltaX > 0)
                    targetAngle += 90f;   // Ò‚‡ÈÔ ‚Ô‡‚Ó
                else
                    targetAngle -= 90f;   // Ò‚‡ÈÔ ‚ÎÂ‚Ó

                isRotating = true;
            }
        }
    }

    void RotateCamera()
    {
        if (!isRotating)
            return;

        currentAngle = Mathf.MoveTowardsAngle(
            currentAngle,
            targetAngle,
            rotationSpeed * Time.deltaTime
        );

        transform.RotateAround(
            target.position,
            Vector3.up,
            Mathf.DeltaAngle(transform.eulerAngles.y, currentAngle)
        );

        if (Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) < 0.1f)
        {
            currentAngle = targetAngle;
            isRotating = false;
        }
    }
}
