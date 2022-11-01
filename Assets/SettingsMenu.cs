using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider soundSlider;
    public Dropdown qualityDropdown;
    const string prefName = "optionvalue";

    void Awake() {
        // qualityDropdown.onValueChanged.AddListener(new UnityAction<int>(index =>
        //     {
        //         PlayerPrefs.SetInt(prefName, qualityDropdown.value);
        //         PlayerPrefs.Save();
        //     }
        // ));

    }

    void Start() {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        audioMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume"));

        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        audioMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume"));

        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 1f);
        audioMixer.SetFloat("SoundVolume", PlayerPrefs.GetFloat("SoundVolume"));

        //qualityDropdown.value = PlayerPrefs.GetInt(prefName, 3);
    }

    public void SetMasterVolume (float volume) {
        PlayerPrefs.SetFloat("MasterVolume", volume);
        audioMixer.SetFloat("MasterVolume", volume);

        PlayerPrefs.SetFloat("MusicVolume", volume);
        audioMixer.SetFloat("MusicVolume", volume);

        PlayerPrefs.SetFloat("SoundVolume", volume);
        audioMixer.SetFloat("SoundVolume", volume);
    }

    public void SetMusicVolume (float volume) {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void SetSoundVolume (float volume) {
        PlayerPrefs.SetFloat("SoundVolume", volume);
        audioMixer.SetFloat("SoundVolume", volume);
    }

    public void SetQuality(int qualityIndex) {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
}
