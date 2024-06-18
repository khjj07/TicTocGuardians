using System;
using UnityEditor;

namespace TicTocGuardians.Scripts.Assets
{
    [Serializable]
    public class FloatDataAsset : LevelDataAsset
    {
        public float value;

        public override object GetValue()
        {
            return value;
        }

        public static LevelDataAsset Create(string name, float value)
        {
            var instance = CreateInstance<FloatDataAsset>();
#if UNITY_EDITOR
            EditorUtility.SetDirty(instance);
#endif
            instance.name = name;
            instance.value = value;
            return instance;
        }
    }
}