using UnityEngine;

[System.Serializable]
public class SoundBase
{
    public string name;
    public AudioClip clip;
    public bool loop;
    public bool playOnAwake;
    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;
    [HideInInspector]
    public AudioSource source;
    [Range (-1f, 1f)]
    public float pan;
}