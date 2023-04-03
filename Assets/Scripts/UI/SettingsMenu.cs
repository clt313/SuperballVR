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
    public Toggle ballTrajectory;

    public Toggle ballTrail;

    const string PrefQuality = "QualityValue";
    const string PrefMaster = "MasterVolume";
    const string PrefMusic = "MusicVolume";
    const string PrefSound = "SoundVolume";
    const string PrefTrajectory = "BallTrajectory";
    const string PrefTrail = "BallTrail";

    void Start() {
        // Master must be set after other two sliders due to conversion timing
        musicSlider.value = PlayerPrefs.GetFloat(PrefMusic, 1f);
        soundSlider.value = PlayerPrefs.GetFloat(PrefSound, 1f);
        masterSlider.value = PlayerPrefs.GetFloat(PrefMaster, 1f);
        SetMasterVolume(masterSlider.value);
        qualityDropdown.value = PlayerPrefs.GetInt(PrefQuality, 3);
        SetQuality(qualityDropdown.value);
        ballTrajectory.isOn = PlayerPrefs.GetInt(PrefTrajectory, 1) != 0;
        ballTrail.isOn = PlayerPrefs.GetInt(PrefTrail, 1) != 0;
        AudioManager.instance.Play("MainTheme");
        AudioManager.instance.Play("Campfire");
        AudioManager.instance.Play("Birds");
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

    public void SetBallTrajectoryAssist(bool enabled) {
        StateController.ballTrajectory = enabled;
        PlayerPrefs.SetInt(PrefTrajectory, ballTrajectory.isOn ? 1 : 0);
    }

    public void SetBallTrail(bool enabled) {
        StateController.ballTrail = enabled;
        PlayerPrefs.SetInt(PrefTrail, ballTrail.isOn ? 1 : 0);
    }
}
