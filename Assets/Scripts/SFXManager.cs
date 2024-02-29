using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundEffect
{
    None,
    Success,
    Warning,
    Error,
    Info,
    CardSwipe,
    Click,
}

public class SFXManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] successSounds;
    public AudioClip[] warningSounds;
    public AudioClip[] errorSounds;
    public AudioClip[] InfoSounds;
    public AudioClip[] paperSwipeSounds;
    public AudioClip[] clickSounds;


    public void PlaySound(SoundEffect soundEffect)
    {
        switch(soundEffect)
        {
            case (SoundEffect.Success):
                audioSource.PlayOneShot(successSounds[Random.Range(0, successSounds.Length - 1)]);
                break;
            case (SoundEffect.Warning):
                audioSource.PlayOneShot(warningSounds[Random.Range(0, warningSounds.Length - 1)]);
                break;
            case (SoundEffect.Error):
                audioSource.PlayOneShot(errorSounds[Random.Range(0, errorSounds.Length - 1)]);
                break;

            case (SoundEffect.CardSwipe):
                audioSource.PlayOneShot(paperSwipeSounds[Random.Range(0, paperSwipeSounds.Length - 1)]);
                break;
            case (SoundEffect.Click):
                audioSource.PlayOneShot(clickSounds[Random.Range(0, clickSounds.Length - 1)]);
                break;
            case (SoundEffect.Info):
                audioSource.PlayOneShot(InfoSounds[Random.Range(0, InfoSounds.Length - 1)]);
                break;
            default:
                break;
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
