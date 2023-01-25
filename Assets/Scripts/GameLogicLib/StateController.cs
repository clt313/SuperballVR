using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores static variables between scenes
public class StateController : MonoBehaviour {
    public enum AiDifficulty {Easy, Normal, Hard};
    public static AiDifficulty aiDifficulty;
    public static int matchLength;
}
