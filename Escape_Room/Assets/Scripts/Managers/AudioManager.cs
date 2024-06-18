using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("# BGM")]
    public AudioSource bgmAudio;
    public AudioClip[] bgmClips;

    [Header("# BGM")]
    public GameObject sfxObject;
    public AudioSource[] sfxAudios;
    public AudioClip[] sfxClips;

    private void Awake()
    {
        sfxAudios = sfxObject.GetComponentsInChildren<AudioSource>();
    }

    public void SFX(int index)
    {
        foreach (AudioSource audio in sfxAudios)
        {
            if (!audio.isPlaying)
            {
                audio.clip = sfxClips[index];
                audio.Play();

                break;
            }
        }
    }

    public void SetBGMVolume(float volume)
    {
        bgmAudio.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        foreach(AudioSource sfxAudio in sfxAudios)
        {
            sfxAudio.volume = volume;
        }
    }
}
