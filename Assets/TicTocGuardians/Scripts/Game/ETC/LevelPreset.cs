using System;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.ETC
{
    [Serializable]
    public class LevelPreset
    {
        public string name;
        public LevelAsset levelAsset;
        public Sprite previewImage;
        public string explain;
    }
}