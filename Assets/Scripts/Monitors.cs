using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Monitors : MonoBehaviour
{
    public TextMeshProUGUI FpsText;
    public TextMeshProUGUI PingText;
    private float pollingTime = 1f;
    private float time;
    private int frameCount = 0;

    // Update is called once per frame
    void Update()
    {
        if(PlayerPrefs.GetInt("fps")  == 1 ? true : false)
        {
            FpsText.enabled = true;
            time += Time.deltaTime;
            frameCount++;

            if(time >= pollingTime)
            {
                int frameRate = Mathf.RoundToInt(frameCount/time);
                FpsText.text = "FPS: " + frameRate.ToString();

                time -= pollingTime;
                frameCount = 0;
            }
        }
        else
        {
            FpsText.enabled = false;
        }

        if(PlayerPrefs.GetInt("ping")  == 1 ? true : false)
        {
            PingText.enabled = true;

        }
        else
        {
            PingText.enabled = false;
        }
    }
}
