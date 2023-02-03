using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Global manager for audio sources
// Inspired by: https://www.youtube.com/watch?v=6OT43pvUyfY&ab_channel=Brackeys
public class AudioManager : MonoBehaviour {

    // Array for easy use in inspector
    public SoundGroup[] soundGroups;

    // Dictionary for efficient lookup
    private Dictionary<string, Sound> d;

    public static AudioManager instance;

    // Initialize each Sound once
    void Awake() {
        // Only initialize once
        if (instance == null)
            instance = this;
        else {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        d = new Dictionary<string, Sound>();

        foreach (SoundGroup sg in soundGroups) {
            foreach (Sound s in sg.sounds) {

                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.outputAudioMixerGroup = s.output;

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.source.loop;

                bool added = d.TryAdd(s.name, s);
                if (!added)
                    Debug.LogWarning("Couldn't register sound with name \"" + s.name + "\"! Is it a duplicate?");

            }
        }
    }

    // On load, play the main theme
    IEnumerator Start() {
        // Setting the sliders produces a sound on load. So skip it!
        Sound s = FindSound("ButtonPress");
        s.source.mute = true;
        yield return new WaitForSeconds(0.1f);
        s.source.mute = false;

        Play("MainTheme");
    }

    // Play sound, looking up by its name
    public void Play(string name) {
        Sound s = FindSound(name);
        s.source.Play();
    }

    public void Stop(string name) {
        Sound s = FindSound(name);
        s.source.Stop();
    }

    public void Pause(string name) {
        Sound s = FindSound(name);
        s.source.Pause();
    }

    public void Unpause(string name) {
        Sound s = FindSound(name);
        s.source.UnPause();
    }

    // Attempts to find sound by name. Returns null if not found.
    private Sound FindSound(string name) {
        if (d.ContainsKey(name))
            return d[name];
        Debug.LogWarning("Couldn't find sound with name \"" + name + "\"!");
        return null;
    }
}
