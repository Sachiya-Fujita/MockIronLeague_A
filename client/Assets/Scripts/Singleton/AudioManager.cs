using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonMonoBehaviour<AudioManager>{
    private AudioSource AudioSourceBGM;
    private AudioSource AudioSourceSE;
    [SerializeField] private AudioClip[] bgmClips;
    [SerializeField] private AudioClip[] seClips;

    private new void Awake()
    {
        base.Awake();
        AudioSource[] audioSources = GetComponents<AudioSource>();
        AudioSourceBGM = audioSources[0];
        AudioSourceSE = audioSources[1];
    }

    public void ChangeBGM(int playBGMindex,float soundScale)
    {
        StopBGM();
        AudioSourceBGM.volume = soundScale;
        AudioSourceBGM.clip = bgmClips[playBGMindex];
        AudioSourceBGM.Play();
    }

    public void StopBGM()
    {
        AudioSourceBGM.Stop();
    }

    public void PlayBGMClip(AudioClip clip,float soundScale)
    {
        AudioSourceBGM.PlayOneShot(clip, soundScale);
    }

    public void PlayBGMClipFromIndex(int index,float soundScale)
    {
        AudioSourceBGM.PlayOneShot(bgmClips[index], soundScale);
    }

    public void PlaySEClip(AudioClip clip,float soundScale)
    {
        AudioSourceSE.PlayOneShot(clip, soundScale);
    }

    public void PlaySEClipFromIndex(int index,float soundScale)
    {
        AudioSourceSE.PlayOneShot(seClips[index], soundScale);
    }

    public void StopAllsound()
    {
        AudioSourceBGM.Stop();
        AudioSourceSE.Stop();
    }
}
