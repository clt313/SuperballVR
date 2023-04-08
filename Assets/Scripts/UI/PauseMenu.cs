using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class PauseMenu : MonoBehaviour
{
    public static bool GamePaused = false;
    public GameObject pauseUI;
    public GameObject pauseButtonGroup;
    public GameObject settingsMenu;
    public GameObject player;

    public GameObject leftController; 
    public GameObject rightController;

    private bool gameEnded = false;

    void Start() {
        GameEvents.gameEndEvent.AddListener(handleGameEnd);
    }

    // Update is called once per frame
    void Update() {
        if (StateController.inputEnabled
                && (Input.GetButtonDown("XRI_Left_SecondaryButton") || Input.GetButtonDown("XRI_Right_SecondaryButton")
                || Input.GetButtonDown("XRI_Left_MenuButton") || Input.GetButtonDown("XRI_Right_MenuButton"))) {
            if (GamePaused) {
                Resume();
                AudioManager.instance.Play("Unpause");
            }
            else if(!gameEnded) {
                Pause();
                AudioManager.instance.Play("Pause");
            }
        }
    }

    public void Resume() {
        // Disable interaction rays
        leftController.GetComponent<XRInteractorLineVisual>().enabled = false;
        rightController.GetComponent<XRInteractorLineVisual>().enabled = false;

        // Remove pause menu
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        GamePaused = false;
    }

    void Pause() {
        // Enable interaction rays
        leftController.GetComponent<XRInteractorLineVisual>().enabled = true;
        rightController.GetComponent<XRInteractorLineVisual>().enabled = true;

        // Summon pause menu in front of player camera
        pauseUI.SetActive(true);
        Vector3 pos = player.transform.position;
        pos.x += player.transform.forward.x * 2;
        pos.z += player.transform.forward.z * 2;
        pauseUI.transform.position = pos;
        Vector3 angle = new Vector3(player.transform.forward.x, 0, player.transform.forward.z);
        pauseUI.transform.forward = angle;

        pauseButtonGroup.SetActive(true);
        settingsMenu.SetActive(false);

        Time.timeScale = 0f;
        GamePaused = true;
    }

    public void Restart() {
        Resume();
        SceneLoader.instance.ReloadCurrent();
    }

    public void Quit() {
        Resume();
        SceneLoader.instance.LoadScene("MainMenu");
    }

    ////////////////////////
    // EVENT HANDLES
    ////////////////////////
    void handleGameEnd() {
        gameEnded = true;
    }
}