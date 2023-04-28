using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hat : MonoBehaviour
{
    public Image hat;
    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.GetFloat("xp", 0) >= 200)
        {
            hat.enabled = true;
        }
        else
        {
            hat.enabled = false;
        }
    }

}
