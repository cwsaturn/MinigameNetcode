using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public AudioSource increasingVolume;
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
        int res_index = PlayerPrefs.GetInt("res_index", 0);
        Screen.SetResolution(resolutions[res_index].horizontal, resolutions[res_index].vertical, false);
        dropdown.value = res_index;

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
            increasingVolume.Play();
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
        Screen.SetResolution(resolutions[index].horizontal, resolutions[index].vertical, false);
        PlayerPrefs.SetInt("res_index", index);
    }
}

[System.Serializable]
public class ResItem
{
    public int horizontal;
    public int vertical;
}
