using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

// Loads scenes smoothly
// Inspired by https://www.youtube.com/watch?v=qYoec93bZx0&ab_channel=Unity3DSchool
public class SceneLoader : MonoBehaviour {

    public static SceneLoader instance;
    public GameObject loaderUI;
    public Slider progressSlider;
    public ScreenFader screenFader;
    private GameObject player;

    void Start() {
        // Only initialize once
         if (instance == null)
            instance = this;
        else {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        // Flip cube mesh inside out (so player sees from inside it)
        // With help from https://answers.unity.com/questions/476810/flip-a-mesh-inside-out.html
        GameObject cube = loaderUI.transform.Find("Cube").gameObject;
        Mesh mesh = cube.GetComponent<MeshFilter>().mesh;
        mesh.triangles = mesh.triangles.Reverse().ToArray();
    }

    // Reloads the current scene
    public void ReloadCurrent() {
        StartCoroutine(LoadSceneCoroutine(SceneManager.GetActiveScene().buildIndex));
    }

    // Searches scene by name, validates it, and begins load
    public void LoadScene(string name) {
        int buildIndex = SceneUtility.GetBuildIndexByScenePath(name);
        if (buildIndex == -1) {
            Debug.LogWarning("Could not load scene with name " + name + "! Is it in the build? (File > Build Settings > Scenes in Build)");
            return;
        }
        StartCoroutine(LoadSceneCoroutine(buildIndex));
    }
    
    // Loads a scene by buildIndex
    public IEnumerator LoadSceneCoroutine(int index) {
        // Temporarily disable player input
        SetPlayerInput(false);

        // Bring UI to player and wait for fadeout
        progressSlider.value = 0;
        player = GameObject.Find("XRRig/Camera Offset/Main Camera");
        SummonUI(loaderUI, player);
        yield return screenFader.FadeInCoroutine();
        GameObject slider = loaderUI.transform.Find("Slider").gameObject;
        slider.SetActive(true);

        // Wait for new scene to load
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

        // Fadeout UI (yes we need to bring it back to the player because the player location changes)
        slider.SetActive(false);
        player = GameObject.Find("XRRig/Camera Offset/Main Camera");
        SummonUI(loaderUI, player);
        yield return screenFader.FadeOutCoroutine();
        loaderUI.SetActive(false);

        // Re-enable player input
        SetPlayerInput(true);
    }

    // Summons UI directly in front of some target (usually the player)
    private void SummonUI(GameObject ui, GameObject target) {
        Vector3 pos = target.transform.position;
        pos.x += target.transform.forward.x * 0.5f;
        pos.z += target.transform.forward.z * 0.5f;
        ui.transform.position = pos;
        Vector3 angle = new Vector3(target.transform.forward.x, 0, target.transform.forward.z);
        ui.transform.forward = angle;
    }

    // Enables/disables player's hand controller inputs
    private void SetPlayerInput(bool enable) {
        GameObject xr = GameObject.Find("XRRig");

        // Toggle movement
        ActionBasedContinuousMoveProvider move = xr.GetComponent<ActionBasedContinuousMoveProvider>();
        move.enabled = enable;
        ActionBasedContinuousTurnProvider turn = xr.GetComponent<ActionBasedContinuousTurnProvider>();
        turn.enabled = enable;

        // Toggle input
        StateController.inputEnabled = enable;
    }
}
