using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteMusic : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Object manager = GameObject.Find("MusicManager");
        Destroy(manager);
    }
}
