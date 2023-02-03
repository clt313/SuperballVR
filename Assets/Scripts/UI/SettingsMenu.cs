using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour {
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider soundSlider;
    public TMPro.TMP_Dropdown qualityDropdown;

    const string PrefQuality = "QualityValue";
    const string PrefMaster = "MasterVolume";
    const string PrefMusic = "MusicVolume";
    const string PrefSound = "SoundVolume";

    void Start() {
        // Master must be set after other two sliders due to conversion timing
        musicSlider.value = PlayerPrefs.GetFloat(PrefMusic, 1f);
        soundSlider.value = PlayerPrefs.GetFloat(PrefSound, 1f);
        masterSlider.value = PlayerPrefs.GetFloat(PrefMaster, 1f);
        SetMasterVolume(masterSlider.value);
        qualityDropdown.value = PlayerPrefs.GetInt(PrefQuality, 3);
        SetQuality(qualityDropdown.value);
        FindObjectOfType<AudioManager>().Play("MainTheme");
    }

    public void SetMasterVolume (float volume) {
        PlayerPrefs.SetFloat(PrefMaster,volume);
        audioMixer.SetFloat(PrefMaster, Mathf.Log10(volume) * 20);
        SetMusicVolume(musicSlider.value);
        SetSoundVolume(soundSlider.value);
    }

    public void SetMusicVolume (float volume) {
        PlayerPrefs.SetFloat(PrefMusic, volume);
        volume = volume*masterSlider.value;
        audioMixer.SetFloat(PrefMusic, Mathf.Log10(volume) * 20);
    }

    public void SetSoundVolume (float volume) {
        PlayerPrefs.SetFloat(PrefSound, volume);
        volume = volume*masterSlider.value;
        audioMixer.SetFloat(PrefSound, Mathf.Log10(volume) * 20);
    }

    public void SetQuality(int qualityIndex) {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt(PrefQuality, qualityDropdown.value);
    }
}
