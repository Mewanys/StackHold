using System.Collections;
using UnityEngine;
using UnityEngine.Events; // Добавлено для событий

public class CameraFocus : MonoBehaviour
{
    [Header("Focus Move")]
    public float moveDuration = 0.6f;
    public float focusDistance = 10f; // Чуть дальше для обзора стройки
    public float focusHeight = 5f;

    [Header("Perspective Settings")]
    public float targetFOV = 60f;

    [Header("Return Swipe")]
    public float minSwipeUpDistance = 120f;

    [Header("UI")]
    public GameObject focusUIPanel;

    // СОБЫТИЕ: Срабатывает, когда камера сфокусировалась
    public UnityEvent onFocusComplete;
    public UnityEvent onReturnComplete;

    Camera cam;
    Vector3 defaultPos;
    Quaternion defaultRot;
    float defaultOrthoSize;
    bool isFocusing = false;
    bool inFocusMode = false;
    Vector2 swipeStart;

    void Awake() => cam = GetComponent<Camera>();

    void Start()
    {
        SaveDefaultState();
        if (focusUIPanel != null) focusUIPanel.SetActive(false);
    }
    public bool IsInFocusMode()
    {
        return inFocusMode || isFocusing;
    }

    void Update()
    {
        if (!inFocusMode) return;

        // Логика свайпа для выхода
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began) swipeStart = t.position;
            if (t.phase == TouchPhase.Ended)
            {
                if ((t.position.y - swipeStart.y) >= minSwipeUpDistance)
                    ReturnToDefault();
            }
        }
    }

    void SaveDefaultState()
    {
        defaultPos = transform.position;
        defaultRot = transform.rotation;
        defaultOrthoSize = cam.orthographicSize;
    }

    public void FocusOn(Transform target)
    {
        if (isFocusing) return;
        SaveDefaultState();
        StartCoroutine(FocusRoutine(target));
    }

    IEnumerator FocusRoutine(Transform target)
    {
        isFocusing = true;
        inFocusMode = true;

        Vector3 dir = (transform.position - target.position).normalized;
        Vector3 targetPos = target.position + dir * focusDistance + Vector3.up * focusHeight;
        Quaternion targetRot = Quaternion.LookRotation(target.position - targetPos);

        float aspect = cam.aspect;
        Matrix4x4 orthoMat = Matrix4x4.Ortho(-defaultOrthoSize * aspect, defaultOrthoSize * aspect, -defaultOrthoSize, defaultOrthoSize, cam.nearClipPlane, cam.farClipPlane);
        Matrix4x4 perspMat = Matrix4x4.Perspective(targetFOV, aspect, cam.nearClipPlane, cam.farClipPlane);

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / moveDuration);

            transform.position = Vector3.Lerp(defaultPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(defaultRot, targetRot, t);
            cam.projectionMatrix = MatrixLerp(orthoMat, perspMat, t);

            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
        cam.orthographic = false;
        cam.fieldOfView = targetFOV;
        cam.ResetProjectionMatrix();

        if (focusUIPanel != null) focusUIPanel.SetActive(true);

        isFocusing = false;

        // Уведомляем подписчиков (мини-игру), что мы на месте
        onFocusComplete?.Invoke();
    }

    public void ReturnToDefault()
    {
        if (isFocusing) return;
        StartCoroutine(ReturnRoutine());
    }

    IEnumerator ReturnRoutine()
    {
        isFocusing = true;
        if (focusUIPanel != null) focusUIPanel.SetActive(false);

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float aspect = cam.aspect;
        Matrix4x4 orthoMat = Matrix4x4.Ortho(-defaultOrthoSize * aspect, defaultOrthoSize * aspect, -defaultOrthoSize, defaultOrthoSize, cam.nearClipPlane, cam.farClipPlane);
        Matrix4x4 perspMat = Matrix4x4.Perspective(targetFOV, aspect, cam.nearClipPlane, cam.farClipPlane);

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / moveDuration);

            transform.position = Vector3.Lerp(startPos, defaultPos, t);
            transform.rotation = Quaternion.Slerp(startRot, defaultRot, t);
            cam.projectionMatrix = MatrixLerp(perspMat, orthoMat, t);

            yield return null;
        }

        cam.orthographic = true;
        cam.orthographicSize = defaultOrthoSize;
        cam.ResetProjectionMatrix();
        transform.position = defaultPos;
        transform.rotation = defaultRot;

        inFocusMode = false;
        isFocusing = false;
        onReturnComplete?.Invoke();
    }

    public static Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float time)
    {
        Matrix4x4 ret = new Matrix4x4();
        for (int i = 0; i < 16; i++) ret[i] = Mathf.Lerp(from[i], to[i], time);
        return ret;
    }
}