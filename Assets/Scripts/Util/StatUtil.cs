using UnityEngine;

namespace Utility
{
  static class StatUtil
  {
    // Based on the Box-Muller transform https://towardsdatascience.com/how-to-generate-random-variables-from-scratch-no-library-used-4b71eb3c8dc7
    public static float sampleGaussianDistribution(float mu, float sigma)
    {
      // create a new instance of the Random class
      System.Random random = new System.Random();

      // generate two random numbers that are uniformly distributed between 0 and 1
      float u1 = (float)random.NextDouble();
      float u2 = (float)random.NextDouble();

      // transform the uniformly distributed numbers to normally distributed numbers
      float z1 = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Cos(2f * Mathf.PI * u2);
      float z2 = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Sin(2f * Mathf.PI * u2);

      // use the first normally distributed number (z1) as the sample from the normal distribution
      float sample = mu + sigma * z1;

      return sample;
    }
  }
}
