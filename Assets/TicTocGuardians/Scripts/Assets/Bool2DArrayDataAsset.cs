using System;
using System.Collections.Generic;
using UnityEditor;

namespace TicTocGuardians.Scripts.Assets
{
    [Serializable]
    public class Bool2DArrayDataAsset : LevelDataAsset
    {
        public List<BoolList> value = new();
        public int row, column;

        public override object GetValue()
        {
            return value;
        }

        public bool[,] GetDataToArray()
        {
            var result = new bool[row, column];
            for (var i = 0; i < row; i++)
            for (var j = 0; j < column; j++)
                result[i, j] = value[i].data[j];
            return result;
        }

        public static LevelDataAsset Create(string name, bool[,] value, int row, int column)
        {
            var instance = CreateInstance<Bool2DArrayDataAsset>();
#if UNITY_EDITOR
            EditorUtility.SetDirty(instance);
#endif
            instance.name = name;
            instance.row = row;
            instance.column = column;
            for (var i = 0; i < row; i++)
            {
                instance.value.Add(new BoolList());
                for (var j = 0; j < column; j++) instance.value[i].data.Add(value[i, j]);
            }

            return instance;
        }

        [Serializable]
        public class BoolList
        {
            public List<bool> data = new();
        }
    }
}