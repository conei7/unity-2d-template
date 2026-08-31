using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity2DTemplate
{
    public sealed class SceneLoader : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogWarning("SceneLoader: Scene name is not configured.");
                return;
            }

            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }
    }
}
