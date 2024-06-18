using UnityEditor;
using UnityEngine;

namespace TicTocGuardians.Scripts.LevelEditor
{
#if UNITY_EDITOR
    [CustomEditor(typeof(LevelEditor))]
    public class LevelEditorInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var tool = (LevelEditor)target;
            if (GUILayout.Button("Window Open")) LevelEditorWindow.Open(tool);
        }
    }
#endif
}