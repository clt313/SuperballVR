using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SingleplayerMenu : MonoBehaviour {
    public StateController.AiDifficulty aiDifficulty;
    public Slider difficultySlider;
    public TMPro.TMP_Text difficultyText;

    public int matchLength;
    public Slider matchLengthSlider;
    public TMPro.TMP_Text matchLengthText;

    public enum MapType {Map1, Map2, Map3};
    public MapType map;
    public Button map1;
    public Button map2;
    public Button map3;

    // Set default variables on start
    void Start() {
        SetAiDifficulty((float)StateController.AiDifficulty.Normal);
        SetMatchLength(11);
        SetMap((int)MapType.Map1);
    }

    // Keep map selection loaded
    void Update() {
        if (this.isActiveAndEnabled) {
            SetMap((int)map);
        }
    }

    // Set the AI difficulty using a slider and display it on the text below
    public void SetAiDifficulty(float aiDifficulty) {
        this.aiDifficulty = (StateController.AiDifficulty)aiDifficulty;
        difficultySlider.value = (int)aiDifficulty;
        difficultyText.SetText(this.aiDifficulty.ToString());
    }

    public void SetMatchLength(float matchLength) {
        this.matchLength = (int)matchLength;
        matchLengthSlider.value = matchLength;
        matchLengthText.SetText(matchLength.ToString() + " points");
    }

    // Set the map by selecting one of the map buttons
    public void SetMap(int map) {
        this.map = (MapType)map;
        switch(map) {
            case 0: map1.Select(); break;
            case 1: map2.Select(); break;
            case 2: map3.Select(); break;
            default: map1.Select(); break;
        }
    }

    // Starts the game and moves to the game scene
    public void StartGame() {
        int buildIndex = SceneUtility.GetBuildIndexByScenePath(map.ToString());
        if (buildIndex == -1) {
            Debug.LogWarning("Could not find scene with name " + map.ToString() + "! Is it in the build? (File > Build Settings > Scenes in Build)");
            return;
        }
        AudioManager.instance.Stop("MainTheme");
        AudioManager.instance.Stop("Campfire");
        AudioManager.instance.Stop("Birds");
        StateController.matchLength = matchLength;
        StateController.aiDifficulty = aiDifficulty;
        SceneLoader.instance.LoadScene(map.ToString());
    }
}
