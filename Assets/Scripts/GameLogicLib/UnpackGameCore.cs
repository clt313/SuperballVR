using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnpackGameCore : MonoBehaviour {
  void Awake() {
    transform.DetachChildren();
    Destroy(gameObject);
  }
}
