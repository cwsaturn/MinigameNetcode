using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public AudioSource footsteps;
    public AudioSource jump;

    void Update()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        Vector3 vel = rb.velocity; 

        Debug.Log(Input.GetKey(KeyCode.A));
        if((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && vel.y < 0.1)
        {
            footsteps.enabled = true;
        }
        else
        {
            footsteps.enabled = false;
        }
        if((Input.GetKeyDown(KeyCode.Space)) && vel.y < 0.1)
        {
            jump.Play();
        }
    }
}
