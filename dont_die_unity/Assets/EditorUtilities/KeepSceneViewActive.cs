using UnityEngine;

public class KeepSceneViewActive : MonoBehaviour
{
    public bool keepSceneViewActive = true;

    private void Awake()
    {
        if (this.keepSceneViewActive && Application.isEditor)
        {
            UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
        }
    }
}