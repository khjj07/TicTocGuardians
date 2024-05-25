using UnityEditor;
using UnityEngine;

namespace TicTocGuardians.Scripts.LevelEditor
{
    [CustomEditor(typeof(LevelEditor))]
    public class LevelEditorInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            LevelEditor tool = (LevelEditor)target;
            if (GUILayout.Button("Window Open"))
            {
                LevelEditorWindow.Open(tool);
            }
        }
    }
}
