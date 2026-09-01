using System.Collections;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Unity2DTemplate.Tests.PlayMode
{
    public sealed class TemplateSceneLoadingTests
    {
        [UnityTest]
        public IEnumerator EveryEnabledBuildSceneCanBeLoaded()
        {
            var buildSceneCount = SceneManager.sceneCountInBuildSettings;
            Assert.That(buildSceneCount, Is.GreaterThan(0), "Build Settings contains no enabled scenes.");

            for (var buildIndex = 0; buildIndex < buildSceneCount; buildIndex++)
            {
                var expectedPath = NormalizePath(SceneUtility.GetScenePathByBuildIndex(buildIndex));
                var loadOperation = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Single);

                Assert.That(loadOperation, Is.Not.Null, $"Failed to start loading scene: {expectedPath}");

                while (!loadOperation.isDone)
                {
                    yield return null;
                }

                yield return null;

                var activeScenePath = NormalizePath(SceneManager.GetActiveScene().path);
                Assert.That(activeScenePath, Is.EqualTo(expectedPath));
            }
        }

        private static string NormalizePath(string path)
        {
            return path.Replace('\\', '/');
        }
    }
}
