using UnityEngine;

public class MainMenu : MonoBehaviour {
    
    // Exits the game, compatible with both the editor and player.
    // Help from https://www.youtube.com/watch?v=ZVfAbGa3obk&ab_channel=GameDevTraumUploads
    public void ExitGame() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

}
