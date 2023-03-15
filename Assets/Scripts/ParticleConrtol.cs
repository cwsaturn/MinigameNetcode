using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleConrtol : MonoBehaviour
{
    [SerializeField] private GameObject particles;
    // Start is called before the first frame update
    void Start()
    {
        particles.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
