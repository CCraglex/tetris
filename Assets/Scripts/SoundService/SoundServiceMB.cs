using System;
using UnityEngine;

public class SoundServiceMB : MonoBehaviour
{
    public void PlaySound(AudioClip clip)
        => SoundService.PlaySound(clip,PlayerPrefs.GetFloat("Sfx"),-0.15f,0.15f);
}
