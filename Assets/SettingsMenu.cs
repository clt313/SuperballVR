using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public void SetMasterVolume (float volume) {
        audioMixer.SetFloat("MasterVolume", volume);
        audioMixer.SetFloat("MusicVolume", volume);
        audioMixer.SetFloat("SoundVolume", volume);
    }

    public void SetMusicVolume (float volume) {
        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void SetSoundVolume (float volume) {
        audioMixer.SetFloat("SoundVolume", volume);
    }

    public void SetQuality(int qualityIndex) {
        Debug.Log(qualityIndex);
        QualitySettings.SetQualityLevel(qualityIndex);
    }
}
