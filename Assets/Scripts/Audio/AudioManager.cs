using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public AudioMixerGroup am;
    // Start is called before the first frame update
    void Awake()
    {
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>(); //adds a audio source component to attached gameobject
            s.source.outputAudioMixerGroup = am; //set the audio mixer group to control volume levels through settings
            s.source.clip = s.clip; //set the audio clip
            s.source.volume = s.volume; //set the volume
            s.source.pitch = s.pitch; //set the pitch
            s.source.loop = s.loop; //set if loop
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name); //Finds the sound in the array
        if (s != null)
            s.source.Play();
    }
}
