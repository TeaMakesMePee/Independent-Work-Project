using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/*
 * This scripts manages the settings and values
 * Updates the player preferences
*/

public class SliderScript : MonoBehaviour
{
    public bool Sens, Sfx;
    public Slider s;
    private float val;
    public AudioMixer am;
    // Update is called once per frame
    private void Start()
    {
        val = s.value;
        if (Sfx)
        {
            if (PlayerPrefs.HasKey("Sfx"))
            {
                s.value = PlayerPrefs.GetFloat("Sfx");
            }
            else
            {
                PlayerPrefs.SetFloat("Sfx", s.value);
            }
            am.SetFloat("volume", PlayerPrefs.GetFloat("Sfx"));
        }

        if (Sens)
        {
            if (PlayerPrefs.HasKey("Sens"))
            {
                s.value = PlayerPrefs.GetFloat("Sens");
            }
            else
            {
                PlayerPrefs.SetFloat("Sens", s.value);
            }
        }
    }

    void Update()
    {
        if (Sens)
        {
            if (val != s.value)
            {
                PlayerPrefs.SetFloat("Sens", s.value);
                val = s.value;
            }
        }

        if (Sfx)
        {
            if (val != s.value)
            {
                PlayerPrefs.SetFloat("Sfx", s.value);
                val = s.value;
                am.SetFloat("volume", PlayerPrefs.GetFloat("Sfx"));
            }
        }
    }
}
