using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using UnityEngine.UI;

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

    public List<ResItem> resolutions = new List<ResItem>();
    private int selectedResolution;

    void Start() {
        prev_vol = 0;
        init_time = Time.deltaTime;
        volumeUpdate = Time.deltaTime - init_time;

        audioMixer.SetFloat("Music Volume", PlayerPrefs.GetFloat("music_vol", 0));
        audioMixer.SetFloat("Announcer Volume", PlayerPrefs.GetFloat("announcer_vol", 0));
        audioMixer.SetFloat("SFX Volume", PlayerPrefs.GetFloat("sfx_vol", 0));

        music.value = PlayerPrefs.GetFloat("music_vol", 0);
        announcer.value = PlayerPrefs.GetFloat("announcer_vol", 0);
        sfx.value = PlayerPrefs.GetFloat("sfx_vol", 0);
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
        Screen.SetResolution(resolutions[index].horizontal, resolutions[index].vertical, true);
    }
}

[System.Serializable]
public class ResItem
{
    public int horizontal;
    public int vertical;
}
