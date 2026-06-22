using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TopDownShooter.Editor
{
    public static class TopDownSurvivalScenePopulator
    {
        private const string ScenePath = "Assets/Scenes/TopDownSurvival.unity";

        [MenuItem("Tools/TopDown Survival/Build Scene")]
        public static void BuildSceneFromMenu()
        {
            BuildAndSave();
        }

        public static void BuildAndSave()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            var bootstrap = Object.FindObjectOfType<TopDownSurvivalSetup>();
            if (bootstrap == null)
            {
                var bootstrapGo = new GameObject("TopDownSurvivalBootstrap");
                bootstrap = bootstrapGo.AddComponent<TopDownSurvivalSetup>();
            }

            bootstrap.BuildContent();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            Debug.Log("TopDown Survival scene populated and saved.");
        }
    }
}
