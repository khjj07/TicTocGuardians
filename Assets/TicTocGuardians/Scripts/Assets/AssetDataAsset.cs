using UnityEditor;
using UnityEngine;

namespace TicTocGuardians.Scripts.Assets
{
    public class AssetDataAsset : LevelDataAsset
    {
        public ScriptableObject value;

        public static LevelDataAsset Create(string name, ScriptableObject value)
        {
            var instance = CreateInstance<AssetDataAsset>();
#if UNITY_EDITOR
            EditorUtility.SetDirty(instance);
#endif
            instance.name = name;
            instance.value = value;
            return instance;
        }

        public override object GetValue()
        {
            return value;
        }
    }
}