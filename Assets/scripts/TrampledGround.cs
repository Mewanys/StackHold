using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrampledGround : MonoBehaviour
{
    CameraFocus cameraController;

    void Start()
    {
        cameraController = Camera.main.GetComponent<CameraFocus>();
    }

    void OnMouseDown()
    {
        if (cameraController != null && !cameraController.IsInFocusMode())
        {
            cameraController.FocusOn(transform);
        }
    }
}
