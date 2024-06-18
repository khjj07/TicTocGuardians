using System;
using UnityEditor;

namespace TicTocGuardians.Scripts.Assets
{
    [Serializable]
    public class IntegerDataAsset : LevelDataAsset
    {
        public int value;

        public static LevelDataAsset Create(string name, int value)
        {
            var instance = CreateInstance<IntegerDataAsset>();
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