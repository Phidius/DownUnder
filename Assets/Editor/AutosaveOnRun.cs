using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class AutosaveOnRun
{
    static AutosaveOnRun()
    {
        EditorApplication.playmodeStateChanged = () =>
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
            {
                Debug.Log("Auto-Saving scenes before entering Play mode");

                //EditorApplication.SaveScene();
                for (var index = 0; index < UnityEngine.SceneManagement.SceneManager.sceneCount - 1; index++)
                {
                    EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetSceneAt(index));
                }
                EditorApplication.SaveAssets();
            }
        };
    }
}
