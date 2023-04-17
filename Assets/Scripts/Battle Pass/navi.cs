using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class navi : MonoBehaviour
{
    public GameObject follower;
    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.GetFloat("xp", 0) >= 300)
        {
            follower.SetActive(true);
        }
        else
        {
            follower.SetActive(false);
        }
    }
}
