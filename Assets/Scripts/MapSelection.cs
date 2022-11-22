using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelection : MonoBehaviour
{
    public void Map1() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
