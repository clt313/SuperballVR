using UnityEngine;

public class QualityScaler : MonoBehaviour {
    // Public variables
    public int targetFrameRate = 90; // Target frame rate
    public int minQualityLevel = 0; // Minimum quality level
    public int maxQualityLevel = 5; // Maximum quality level

    // Private variables
    private int currentQualityLevel; // Current quality level

    // Start is called before the first frame update
    void Start()
    {
        // Get the current quality level
        currentQualityLevel = QualitySettings.GetQualityLevel();
    }

    // Update is called once per frame
    void Update()
    {
        // If the target frame rate is different from the current frame rate
        if (Application.targetFrameRate != targetFrameRate)
        {
            // Set the target frame rate to the specified value
            Application.targetFrameRate = targetFrameRate;
        }

        // If the frame count is a multiple of 30
        if (Time.frameCount % 30 == 0)
        {
            // Calculate the frame time and frame rate
            float frameTime = Time.deltaTime * 1000.0f;
            float frameRate = 1000.0f / frameTime;

            // If the frame rate is less than the target frame rate and the current quality level is greater than the minimum quality level
            if (frameRate < targetFrameRate && currentQualityLevel > minQualityLevel)
            {
                // Decrease the current quality level and apply the new quality level
                currentQualityLevel -= 1;
                QualitySettings.SetQualityLevel(currentQualityLevel);
            }
            // If the frame rate is greater than the target frame rate and the current quality level is less than the maximum quality level
            else if (frameRate > targetFrameRate && currentQualityLevel < maxQualityLevel)
            {
                // Increase the current quality level and apply the new quality level
                currentQualityLevel += 1;
                QualitySettings.SetQualityLevel(currentQualityLevel);
            }
        }
    }
}
