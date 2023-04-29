using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;

    [SerializeField]
    private float scrollRate = 0.5f;

    private void Start() 
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
    }

    private void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        float ParallaxEffectMultiplier = scrollRate;
        transform.position += deltaMovement * ParallaxEffectMultiplier;
        lastCameraPosition = cameraTransform.position;
    }
}
