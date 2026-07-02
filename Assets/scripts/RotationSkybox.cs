using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSkybox : MonoBehaviour
{
    public float rotationSpeed = 1.0f; // Скорость вращения 
    public Material skyboxMaterial;
    void Start()
    {

    }
    void Update()
    {
        float angle = Time.time * rotationSpeed;
        skyboxMaterial.SetFloat("_Rotation", angle);
    }
}
