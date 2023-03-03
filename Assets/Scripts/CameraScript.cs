using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform playerObj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerObj != null)
        {
            transform.position = new Vector3(playerObj.position.x, playerObj.position.y, -10);
        }
    }

    public void setTarget(Transform target)
    {
        playerObj = target;
    }
}
