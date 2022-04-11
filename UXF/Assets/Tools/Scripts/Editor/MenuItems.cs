using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace mrstruijk.Tools
{
    public static class MenuItems
    {
        [MenuItem("mrstruijk/Build New Scene")]
        private static void BuildScene()
        {
            BuildNewScene.CreateNewScene();
        }


        [MenuItem("mrstruijk/Start Application")]
        public static void StartApplication()
        {
            var introScenePath = EditorBuildSettings.scenes[0].path;

            if (Application.isEditor && !Application.isPlaying)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene(introScenePath);
                EditorApplication.isPlaying = true; // This forces the Editor to start playing
            }
            else if (Application.isEditor && Application.isPlaying)
            {
                SceneManager.LoadScene(introScenePath);
            }
        }
    }
}