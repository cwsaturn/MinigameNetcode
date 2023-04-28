using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CupCosmetics : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer forrest;

    [SerializeField]
    private SpriteRenderer city;

    // Start is called before the first frame update
    void Start()
    {
        if(forrest != null)
        {
            Debug.Log(PlayerPrefs.GetFloat("xp", 0));
            if(PlayerPrefs.GetFloat("xp", 0) >= 400)
            {
                forrest.enabled = true;
                city.enabled = false;
            }
            else
            {
                forrest.enabled = false;
                city.enabled = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
