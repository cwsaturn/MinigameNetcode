using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using UnityEngine.UI;
using TMPro;
using System.Reflection;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public AudioSource decreasingVolume;

    private float prev_vol;
    private float init_time;
    private float volumeUpdate;

    public Slider music;
    public Slider announcer;
    public Slider sfx;

    public TMPro.TMP_Dropdown dropdown;

    public List<ResItem> resolutions = new List<ResItem>();
    private int selectedResolution;
    private bool fullscreen;

    //Monitors
    public Toggle fpsToggle;
    public Toggle pingToggle;
    public Toggle fullscreenToggle;


    void Start() {
        //glados variables
        prev_vol = 0;
        init_time = Time.deltaTime;
        volumeUpdate = Time.deltaTime - init_time;

        //audio mixer values
        audioMixer.SetFloat("Music Volume", PlayerPrefs.GetFloat("music_vol", 0));
        audioMixer.SetFloat("Announcer Volume", PlayerPrefs.GetFloat("announcer_vol", 0));
        audioMixer.SetFloat("SFX Volume", PlayerPrefs.GetFloat("sfx_vol", 0));

        //slider values
        music.value = PlayerPrefs.GetFloat("music_vol", 0);
        announcer.value = PlayerPrefs.GetFloat("announcer_vol", 0);
        sfx.value = PlayerPrefs.GetFloat("sfx_vol", 0);

        //resolution
        selectedResolution = PlayerPrefs.GetInt("res_index", 3);
        fullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1 ? true : false;
        fullscreenToggle.isOn = fullscreen;
        setScreen();
        dropdown.value = selectedResolution;

        fpsToggle.isOn = PlayerPrefs.GetInt("fps")  == 1 ? true : false;
        pingToggle.isOn = PlayerPrefs.GetInt("ping")  == 1 ? true : false;

    }

    public void SetMusicVolume (float volume)
    {
        audioMixer.SetFloat("Music Volume", volume);
    }

    public void SetAnnouncerVolume (float volume)
    {
        audioMixer.SetFloat("Announcer Volume", volume);

        if ((volume > prev_vol) && (volumeUpdate > 0.02))
        {
            volumeUpdate = 0;
        } 
        else if ((volume < prev_vol) && (volumeUpdate > 0.02))
        {
            volumeUpdate = 0;
            decreasingVolume.Play();
        }

        volumeUpdate = Time.deltaTime + volumeUpdate;
        prev_vol = volume;
    }

    public void SetSFXVolume (float volume)
    {
        audioMixer.SetFloat("SFX Volume", volume);
    }

    public void ResDropdown(int index)
    {
        selectedResolution = index;
        PlayerPrefs.SetInt("res_index", index);
        setScreen();
    }

    public void fpsCounter()
    {
        PlayerPrefs.SetInt("fps", fpsToggle.isOn?1:0);
    }

    public void pingCounter()
    {
        PlayerPrefs.SetInt("ping", pingToggle.isOn?1:0);
    }

    public void fullscreenToggled()
    {
        fullscreen = fullscreenToggle.isOn ? true : false;
        PlayerPrefs.SetInt("fullscreen", fullscreen ? 1 : 0);
        setScreen();
    }

    private void setScreen()
    {
        Screen.SetResolution(resolutions[selectedResolution].horizontal, resolutions[selectedResolution].vertical, fullscreen);
    }
}

[System.Serializable]
public class ResItem
{
    public int horizontal;
    public int vertical;
}
