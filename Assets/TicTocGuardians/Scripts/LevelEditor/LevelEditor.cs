using Default.Scripts.Util;
using UnityEditor;
using UnityEngine;

namespace TicTocGuardians.Scripts.LevelEditor
{
    public class LevelEditor : Singleton<LevelEditor>
    {
#if UNITY_EDITOR
        public DefaultAsset modelsFolder;
        public Transform origin;
#endif
    }
}