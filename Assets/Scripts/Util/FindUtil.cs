using UnityEngine;

namespace Utility
{
  static class FindUtil
  {
    public static Transform RecursiveFindChild(Transform parent, string childName)
    {
      foreach (Transform child in parent)
      {
        if (child.name == childName)
        {
          return child;
        }
        else
        {
          Transform found = RecursiveFindChild(child, childName);
          if (found != null)
          {
            return found;
          }
        }
      }
      return null;
    }
  }
}
