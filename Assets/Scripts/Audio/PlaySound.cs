using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlaySound : MonoBehaviour {
    public string soundName;

    public void Play() {
        FindObjectOfType<AudioManager>().Play(soundName);
    }
}
