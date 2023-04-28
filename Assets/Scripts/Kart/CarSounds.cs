using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSounds : MonoBehaviour
{
    public float minSpeed;
    public float maxSpeed;
    private float currentSpeed;

    public Rigidbody2D carRb;
    public AudioSource carAudio;

    public float minPitch;
    public float maxPitch;
    private float pitchFromCar;

    void Start()
    {
        //carAudio = GameObject.Find("Engine").GetComponent<AudioSource>();
        //carRb = GameObject.Find("Kart").GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (carRb == null) return;
        EngineSound();
    }

    void EngineSound()
    {
        currentSpeed = carRb.velocity.magnitude;
        pitchFromCar = carRb.velocity.magnitude/30f;

        if(currentSpeed < minSpeed)
        {
            carAudio.pitch = minPitch;
        }

        if(currentSpeed > minSpeed && currentSpeed < maxSpeed)
        {
            carAudio.pitch = minPitch + pitchFromCar;
        }

        if(currentSpeed > maxSpeed)
        {
            carAudio.pitch = maxPitch;
        }
    }

}
