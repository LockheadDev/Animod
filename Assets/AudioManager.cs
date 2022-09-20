using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField]
    private AudioLowPassFilter audioLPF;
    public List<SoundBase> sounds = new List<SoundBase>();



    private void Awake()
    {
        foreach (SoundBase s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;   
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
            s.source.panStereo = s.pan;
        }
    }
    private void Start()
    {
        //Helps audio play accordingly
        foreach (SoundBase s in sounds)
        {
            s.source.enabled = false;
            s.source.enabled = true;

        }
    }
    public void SetlpfValues(float value)
    {
        audioLPF.cutoffFrequency = value;
    }

    public void SetAudioVolume(string name, float volume)
    {
        foreach (SoundBase s in sounds)
        {
            if (s.name.ToLower() == name.ToLower())
            {
                s.source.volume = volume;
            }
        }
    }

    public void SetAudioPitch(string name,float pitch)
    {
        foreach (SoundBase s in sounds)
        {
            if (s.name.ToLower() == name.ToLower())
            {
                s.source.pitch = pitch;
            }
        }
    }
    public void SetAudioPan(string name, float pan)
    {
        foreach (SoundBase s in sounds)
        {
            if (s.name.ToLower() == name.ToLower())
            {
                s.source.panStereo = pan;
            }
        }
    }
}
