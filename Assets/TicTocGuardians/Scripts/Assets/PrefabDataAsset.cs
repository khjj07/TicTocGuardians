using UnityEditor;
using UnityEngine;

namespace TicTocGuardians.Scripts.Assets
{
    public class PrefabDataAsset : LevelDataAsset
    {
        public GameObject value;

        public static LevelDataAsset Create(string name, GameObject value)
        {
            var instance = CreateInstance<PrefabDataAsset>();
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