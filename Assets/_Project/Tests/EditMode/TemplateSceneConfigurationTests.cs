using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity2DTemplate.Tests.EditMode
{
    public sealed class TemplateSceneConfigurationTests
    {
        private static readonly string[] ExpectedScenePaths =
        {
            "Assets/_Project/Scenes/Title.unity",
            "Assets/_Project/Scenes/Game.unity",
            "Assets/_Project/Scenes/Result.unity",
            "Assets/_Project/Scenes/Extras.unity",
            "Assets/_Project/Scenes/Credits.unity",
            "Assets/_Project/Scenes/Statistics.unity",
            "Assets/_Project/Scenes/Achievements.unity",
            "Assets/_Project/Scenes/Gallery.unity"
        };

        [Test]
        public void BuildSettingsContainExpectedEnabledScenesInOrder()
        {
            var enabledScenePaths = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            CollectionAssert.AreEqual(ExpectedScenePaths, enabledScenePaths);
        }

        [Test]
        public void TemplateScenesExistAndContainNoMissingScripts()
        {
            var previousSceneSetup = EditorSceneManager.GetSceneManagerSetup();

            try
            {
                foreach (var scenePath in ExpectedScenePaths)
                {
                    var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                    Assert.That(sceneAsset, Is.Not.Null, $"Scene asset was not found: {scenePath}");

                    var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                    foreach (var rootObject in scene.GetRootGameObjects())
                    {
                        AssertNoMissingScriptsRecursively(rootObject, scenePath);
                    }
                }
            }
            finally
            {
                if (previousSceneSetup.Any(sceneSetup => sceneSetup.isLoaded))
                {
                    EditorSceneManager.RestoreSceneManagerSetup(previousSceneSetup);
                }
                else
                {
                    EditorSceneManager.OpenScene(ExpectedScenePaths[0], OpenSceneMode.Single);
                }
            }
        }

        private static void AssertNoMissingScriptsRecursively(GameObject gameObject, string scenePath)
        {
            var missingScriptCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(gameObject);
            Assert.That(
                missingScriptCount,
                Is.Zero,
                $"Missing Script found on '{GetHierarchyPath(gameObject.transform)}' in {scenePath}");

            foreach (Transform child in gameObject.transform)
            {
                AssertNoMissingScriptsRecursively(child.gameObject, scenePath);
            }
        }

        private static string GetHierarchyPath(Transform transform)
        {
            return transform.parent == null
                ? transform.name
                : $"{GetHierarchyPath(transform.parent)}/{transform.name}";
        }
    }
}
