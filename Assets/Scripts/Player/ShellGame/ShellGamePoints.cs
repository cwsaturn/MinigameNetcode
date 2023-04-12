using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellGamePoints : MonoBehaviour

{
    [SerializeField]
    Collider2D boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        boxCollider.isTrigger = false;
    }

}
