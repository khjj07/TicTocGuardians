using System;
using TicTocGuardians.Scripts.Assets.LevelAsset;

namespace TicTocGuardians.Scripts.Game.ETC
{
    [Serializable]
    public class LevelPreset
    {
        public string name;
        public LevelAsset levelAsset;
    }
}