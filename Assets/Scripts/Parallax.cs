using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;

    private void Start() 
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
    }

    private void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        float ParallaxEffectMultiplier = .5f;
        transform.position += deltaMovement * ParallaxEffectMultiplier;
        lastCameraPosition = cameraTransform.position;
    }
}
