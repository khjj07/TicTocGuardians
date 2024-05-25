using System;
using UnityEditor;

namespace TicTocGuardians.Scripts.Assets
{
    [Serializable]
    public class IntegerDataAsset : LevelDataAsset
    {
        public static LevelDataAsset Create(string name, int value)
        {
            var instance = CreateInstance<IntegerDataAsset>();
            EditorUtility.SetDirty(instance);
            instance.name = name;
            instance.value = value;
            return instance;
        }
        public int value;

        public override object GetValue()
        {
            return (object)value;
        }
    }
}
