using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores static variables between scenes
public class StateController : MonoBehaviour {
    public enum AiDifficulty {Easy, Normal, Hard};
    public static AiDifficulty aiDifficulty = AiDifficulty.Normal;
    public static int matchLength = 11;
    public static bool inputEnabled = true;
}
