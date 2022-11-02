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
    public TMPro.TMP_Dropdown qualityDropdown;
    const string prefName = "optionvalue";

    void Awake() {
        qualityDropdown.onValueChanged.AddListener(new UnityAction<int>(index =>
            {
                PlayerPrefs.SetInt(prefName, qualityDropdown.value);
                PlayerPrefs.Save();
            }
        ));

    }

    void Start() {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        //SetMusicVolume(musicSlider.value);

        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 1f);
        //SetSoundVolume(soundSlider.value);

        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        SetMasterVolume(masterSlider.value);

        qualityDropdown.value = PlayerPrefs.GetInt(prefName, 3);
    }

    public void SetMasterVolume (float volume) {
        PlayerPrefs.SetFloat("MasterVolume",volume);
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        SetMusicVolume(musicSlider.value);
        SetSoundVolume(soundSlider.value);
    }

    public void SetMusicVolume (float volume) {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        volume = volume*masterSlider.value;
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSoundVolume (float volume) {
        PlayerPrefs.SetFloat("SoundVolume", volume);
        volume = volume*masterSlider.value;
        audioMixer.SetFloat("SoundVolume", Mathf.Log10(volume) * 20);
    }

    public void SetQuality(int qualityIndex) {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
}
