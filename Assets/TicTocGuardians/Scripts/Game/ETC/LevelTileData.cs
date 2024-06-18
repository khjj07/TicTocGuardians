using System;

namespace TicTocGuardians.Scripts.Game.ETC
{
    [Serializable]
    public class LevelTileData
    {
        public BoolList[] value = new BoolList[100];

        public void Set(int i, int j, bool val)
        {
            value[i].value[j] = val;
        }

        public bool Get(int i, int j)
        {
            return value[i].value[j];
        }

        public bool[,] ToArray()
        {
            var result = new bool[value.Length, value[0].value.Length];
            for (var i = 0; i < value.Length; i++)
            for (var j = 0; j < value[i].value.Length; j++)
                result[i, j] = value[i].value[j];
            return result;
        }

        public void FromArray(bool[,] array, int row, int column)
        {
            for (var i = 0; i < row; i++)
            for (var j = 0; j < column; j++)
                value[i].value[j] = array[i, j];
        }

        [Serializable]
        public class BoolList
        {
            public bool[] value;

            public BoolList()
            {
                value = new bool[100];
            }
        }
    }
}