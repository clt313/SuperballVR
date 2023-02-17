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
    private GameObject player;

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
        // TODO: fade out screen
        StartCoroutine(LoadSceneCoroutine(buildIndex));
    }
    
    // Loads a scene by name
    public IEnumerator LoadSceneCoroutine(int index) {
        progressSlider.value = 0;
        loaderUI.SetActive(true);
        player = GameObject.Find("XRRig/Camera Offset/Main Camera");
        SummonUI(loaderUI, player);

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

        loaderUI.SetActive(false);
    }

    // Summons UI in front of some target (usually the player)
    private void SummonUI(GameObject ui, GameObject target) {
        Vector3 pos = target.transform.position;
        pos.x += target.transform.forward.x * 2;
        pos.z += target.transform.forward.z * 2;
        ui.transform.position = pos;
        Vector3 angle = new Vector3(target.transform.forward.x, 0, target.transform.forward.z);
        ui.transform.forward = angle;
    }
}
