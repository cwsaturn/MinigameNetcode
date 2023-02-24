using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class NetworkTransformTest : NetworkBehaviour
{
    void Update()
    {
        if (IsClient)
        {
            /*
            float theta = Time.frameCount / 10.0f;
            transform.position = new Vector3((float)Math.Cos(theta), 0.0f, (float)Math.Sin(theta));
            */
            transform.position += Vector3.right * Input.GetAxisRaw("Horizontal") * Time.deltaTime;
            transform.position += Vector3.up * Input.GetAxisRaw("Vertical") * Time.deltaTime;
        }
    }
}