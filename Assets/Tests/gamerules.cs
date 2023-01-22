using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class gamerules
{

  public string testSceneName = "Map1";

  [OneTimeSetUp]
  public void Setup()
  {
    SceneManager.LoadScene(testSceneName);
  }

  // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
  // `yield return null;` to skip a frame.
  [UnityTest]
  public IEnumerator gamerulesWithEnumeratorPasses()
  {
    LogAssert.ignoreFailingMessages = true; // Only fail tests on failed assertions here. Not other errors in game.
    float startTime = Time.time;

    while (Time.time - startTime < 10)
    {

      // Advances a frame
      yield return null;
    }
    Debug.Log("Finished running test!");
  }
}
