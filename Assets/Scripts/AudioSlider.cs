using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSlider : MonoBehaviour
{
    [SerializeField] 
    private AudioMixer Mixer;
    [SerializeField]
    private AudioSource AudioSource;
    //[SerializeField]
    //private TMPro.TextMeshProUGUI ValueText;
    [SerializeField]
    private AudioMixMode MixMode;

    public void OnChangeSlider(float Value)
    {
        switch (MixMode)
        {
            case AudioMixMode.LinearAudioSourceVolume:
                AudioSource.volume = Value;
                break;
            case AudioMixMode.LinearMixerVolume:
                Mixer.SetFloat("Volume", (-80 + Value * 100));
                break;
            case AudioMixMode.LogrithmicMixerVolume:
                Mixer.SetFloat("Volume", Mathf.Log10(Value) * 20);
                break;
        }
    }

    public enum AudioMixMode
    {
        LinearAudioSourceVolume,
        LinearMixerVolume,
        LogrithmicMixerVolume
    }
}
