using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Fades the screen
// Some optimizations COULD be made, but it works as is (the software engineer grindset)
public class ScreenFader : MonoBehaviour {

    public GameObject screen;
    public GameObject cube;
    public float fadeDuration = 1.0f;
    
    // Calls the FadeIn couroutine
    public void FadeIn() {
        StartCoroutine(FadeInCoroutine());
    }

    // Fades in the screen
    public IEnumerator FadeInCoroutine() {
        screen.SetActive(true);
        Material cubeMaterial = cube.GetComponent<Renderer>().material;
        float timer = 0;
        Color c;
        // Interpolate alpha value across the fadeDuration
        while (cubeMaterial.color.a < 1) {
            c = cubeMaterial.color;
            c.a = Mathf.Lerp(0, 1, timer / fadeDuration);
            cubeMaterial.color = c;
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
        Material cubeMaterial = cube.GetComponent<Renderer>().material;
        float timer = 0;
        Color c;
        // Interpolate alpha value across the fadeDuration
        while (cubeMaterial.color.a > 0) {
            c = cubeMaterial.color;
            c.a = Mathf.Lerp(1, 0, timer / fadeDuration);
            cubeMaterial.color = c;
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
