using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePreferencesManager : MonoBehaviour
{
    void OnApplicationQuit()
    {
        SavePrefs();
    }

    public void SavePrefs()
    {
        PlayerPrefs.SetFloat("music_vol", GameObject.Find("Music Slider").GetComponent<Slider>().value);
        PlayerPrefs.SetFloat("announcer_vol", GameObject.Find("Announcer Slider").GetComponent<Slider>().value);
        PlayerPrefs.SetFloat("sfx_vol", GameObject.Find("SFX Slider").GetComponent<Slider>().value);
    }
}
