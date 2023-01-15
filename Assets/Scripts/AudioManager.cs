using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public AudioSource buttonPressAudio;

    IEnumerator Start() {
        // Prevent button press audio from playing on load
        // That happens due to sliders (with their own audio) having their values set on load
        yield return new WaitForSeconds(0.1f);
        buttonPressAudio.mute = false;
    }
}
