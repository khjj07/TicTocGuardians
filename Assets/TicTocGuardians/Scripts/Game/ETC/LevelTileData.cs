using System;

namespace TicTocGuardians.Scripts.Game.ETC
{
    [Serializable]
    public class LevelTileData
    {
        [Serializable]
        public class BoolList
        {
            public bool[] value;

            public BoolList()
            {
                value = new bool[100];
            }
        }

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
            bool[,] result=new bool[value.Length,value[0].value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                for (int j = 0; j < value[i].value.Length; j++)
                {
                    result[i,j] = value[i].value[j];
                }
            }
            return result;
        }

        public void FromArray(bool[,] array,int row, int column)
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                { 
                    value[i].value[j]= array[i,j];
                }
            }
        }
    }
}
