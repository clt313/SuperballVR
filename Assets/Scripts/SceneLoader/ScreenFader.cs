using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Fades the screen
public class ScreenFader : MonoBehaviour {

    public GameObject screen;
    public float fadeDuration = 1.0f;
    
    // Calls the FadeIn couroutine
    public void FadeIn() {
        StartCoroutine(FadeInCoroutine());
    }

    // Fades in the screen
    public IEnumerator FadeInCoroutine() {
        screen.SetActive(true);
        Image image = screen.GetComponent<Image>();
        float timer = 0;
        Color c;
        // Interpolate alpha value across the fadeDuration
        while (image.color.a < 1) {
            c = image.color;
            c.a = Mathf.Lerp(0, 1, timer / fadeDuration);
            image.color = c;
            timer += Time.deltaTime;
            yield return null;
        }
    }

    // Calls the FadeOut couroutine
    public void FadeOut() {
        StartCoroutine(FadeOutCoroutine());
    }

    // Fades out the screen
    public IEnumerator FadeOutCoroutine() {
        screen.SetActive(true);
        Image image = screen.GetComponent<Image>();
        float timer = 0;
        Color c;
        // Interpolate alpha value across the fadeDuration
        while (image.color.a > 0) {
            c = image.color;
            c.a = Mathf.Lerp(1, 0, timer / fadeDuration);
            image.color = c;
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
