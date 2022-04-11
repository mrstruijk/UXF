using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace mrstruijk.Tools
{
    public static class BuildNewScene
    {
        private const string sceneGameObjectName = "Scene";

        public static void CreateNewScene()
        {
            SceneCreator();
            CleanScene();
            var gameObject = new GameObject(sceneGameObjectName);
            gameObject.transform.position = Vector3.zero;
        }


        private static void SceneCreator()
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            }
            else if (Application.isEditor && Application.isPlaying)
            {
                SceneManager.CreateScene("New Scene");
            }
        }


        private static void CleanScene()
        {
            var allObjects = Object.FindObjectsOfType<GameObject>();

            foreach (var go in allObjects)
            {
                Object.DestroyImmediate(go);
            }
        }
    }
}