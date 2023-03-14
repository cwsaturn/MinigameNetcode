using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public AudioSource increasingVolume;
    public AudioSource decreasingVolume;

    private float prev_vol;
    private float init_time;
    private float volumeUpdate;

    void Start() {
        prev_vol = 0;
        init_time = Time.deltaTime;
        volumeUpdate = Time.deltaTime - init_time;
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
        else if ((volume < prev_vol) && ((volumeUpdate > 0.02) || (volumeUpdate == 0)))
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
}
