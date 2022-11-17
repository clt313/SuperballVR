using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SingleplayerMenu : MonoBehaviour {
    public enum AiDifficulty {Easy, Normal, Hard};
    public AiDifficulty aiDifficulty;
    public Slider difficultySlider;
    public TMPro.TMP_Text difficultyText;

    public enum MapType {Map1, Map2, Map3};
    public MapType map;
    public Button map1;
    public Button map2;
    public Button map3;

    // Set default variables on start
    void Start() {
        SetAiDifficulty((float)AiDifficulty.Normal);
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
        this.aiDifficulty = (AiDifficulty)aiDifficulty;
        difficultySlider.value = (int)aiDifficulty;
        difficultyText.SetText(this.aiDifficulty.ToString());
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
        // TODO: implement this once game scene is implemented
        // SceneManager.LoadScene("...");
    }
}
