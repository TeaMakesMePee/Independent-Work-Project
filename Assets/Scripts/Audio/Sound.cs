using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[System.Serializable]
public class Sound
{
    public AudioClip clip;
    public string name;
    [Range(0.1f, 3f)]
    public float pitch;
    [Range(0f, 1f)]
    public float volume;
    public bool loop;

    [HideInInspector]
    public AudioSource source;
}
