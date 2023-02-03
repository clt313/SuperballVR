using UnityEngine;
using UnityEngine.Audio;

// Stores sound information
[System.Serializable]
public class Sound {
    [Tooltip("NOTE: Sound name must be unique across entire AudioManager.")]
    public string name = "New Sound";
    public AudioClip clip;
    public AudioMixerGroup output;

    public bool loop = false;

    [Range(0, 1f)]
    public float volume = 1;
    [Range(.1f, 3f)]
    public float pitch = 1f;

    [HideInInspector]
    public AudioSource source;
}
