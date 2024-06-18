using System;
using UnityEngine;

namespace TicTocGuardians.Scripts.Assets
{
    [Serializable]
    public class LevelDataAsset : ScriptableObject
    {
        public new string name;

        public virtual object GetValue()
        {
            return null;
        }
    }
}