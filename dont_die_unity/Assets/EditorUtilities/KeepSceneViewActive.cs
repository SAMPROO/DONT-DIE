using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class KeepSceneViewActive : MonoBehaviour
{
    public bool keepSceneViewActive = true;

    private void Awake()
    {
        if (this.keepSceneViewActive && Application.isEditor)
        {
            UnityEditor.EditorWindow.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
        }
    }
}
#endif