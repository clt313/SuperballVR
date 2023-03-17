using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Fades the screen
public class ScreenFader : MonoBehaviour {

    public GameObject screen;
    public GameObject[] objectsToFade;
    public float fadeDuration = 1.0f;

    private List<Material> materials;
    private List<Image> images;

    void Start() {
        materials = new List<Material>();
        images = new List<Image>();

        foreach (GameObject gameObject in objectsToFade) {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
                materials.Add(renderer?.material);
            images.AddRange(gameObject.GetComponentsInChildren<Image>());
        }
    }
    
    // Calls the FadeIn couroutine
    public void FadeIn() {
        StartCoroutine(FadeInCoroutine());
    }

    // Fades in the screen
    public IEnumerator FadeInCoroutine() {
        screen.SetActive(true);
        float timer = 0;
        float alpha = 0.0f;

        // Interpolate alpha value across the fadeDuration
        while (alpha < 1) {
            alpha = Mathf.Lerp(0, 1, timer / fadeDuration);

            foreach (Material material in materials) {
                Color temp = material.color;
                temp.a = alpha;
                material.color = temp;
            }

            foreach (Image image in images) {
                Color temp = image.color;
                temp.a = alpha;
                image.color = temp;
            }

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
        float timer = 0;
        float alpha = 1.0f;
        
        // Interpolate alpha value across the fadeDuration
        while (alpha > 0) {
            alpha = Mathf.Lerp(1, 0, timer / fadeDuration);

            foreach (Material material in materials) {
                Color temp = material.color;
                temp.a = alpha;
                material.color = temp;
            }

            foreach (Image image in images) {
                Color temp = image.color;
                temp.a = alpha;
                image.color = temp;
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }
}
