using System;
using UnityEditor;

namespace TicTocGuardians.Scripts.Assets
{
    [Serializable]
    public class BoolDataAsset : LevelDataAsset
    {
        public bool value;

        public static LevelDataAsset Create(string name, bool value)
        {
            var instance = CreateInstance<BoolDataAsset>();
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