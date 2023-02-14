using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class PauseMenu : MonoBehaviour
{
    public static bool GamePaused = false;
    public GameObject pauseUI;
    public GameObject player;

    public GameObject leftController; 
    public GameObject rightController;

    private bool gameEnded = false;

    void Start() {
        GameEvents.gameEndEvent.AddListener(handleGameEnd);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("XRI_Left_SecondaryButton") || Input.GetButtonDown("XRI_Right_SecondaryButton")) {
            if (GamePaused) {
                Resume();
            }
            else if(!gameEnded) {
                Pause();
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
        Time.timeScale = 0f;
        GamePaused = true;
    }

    public void Restart() {
        Resume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit() {
        Resume();
        SceneManager.LoadScene("MainMenu");
    }

    ////////////////////////
    // EVENT HANDLES
    ////////////////////////
    void handleGameEnd() {
        gameEnded = true;
    }
}
