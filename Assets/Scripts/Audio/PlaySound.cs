using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlaySound : MonoBehaviour {
    public string soundName;

    // Attempts to play a sound specified by the sound name. Only works when an AudioManager is present in the scene.
    public void Play() {
        FindObjectOfType<AudioManager>().Play(soundName);
    }
}
