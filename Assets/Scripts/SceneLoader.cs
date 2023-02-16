using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Loads scenes smoothly
// Inspired by https://www.youtube.com/watch?v=qYoec93bZx0&ab_channel=Unity3DSchool
public class SceneLoader : MonoBehaviour {

    public static SceneLoader instance;
    public GameObject loaderUI;
    public Slider progressSlider;

    void Awake() {
        // Only initialize once
         if (instance == null)
            instance = this;
        else {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    // Searches scene by name, validates it, and begins load
    public void LoadScene(string name) {
        int buildIndex = SceneUtility.GetBuildIndexByScenePath(name);
        if (buildIndex == -1) {
            Debug.LogWarning("Could not load scene with name " + name + "! Is it in the build? (File > Build Settings > Scenes in Build)");
            return;
        }
        StartCoroutine(LoadSceneCoroutine(buildIndex));
        // TODO: fade out screen
    }
    
    // Loads a scene by name
    public IEnumerator LoadSceneCoroutine(int index) {
        progressSlider.value = 0;
        loaderUI.SetActive(true);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index);
        asyncOperation.allowSceneActivation = false;
        float progress = 0;
        while (!asyncOperation.isDone) {
            progress = Mathf.MoveTowards(progress, asyncOperation.progress, Time.deltaTime);
            progressSlider.value = progress;
            if (progressSlider.value >= 0.9f) {
                progressSlider.value = 1;
                asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
