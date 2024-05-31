using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace TicTocGuardians.Scripts.Assets
{
    [Serializable]
    public class StageSelectButtonPreset
    {
        public string text;
        public LevelAsset.LevelAsset levelAsset;
    }

    [CreateAssetMenu(fileName = "New StageSelectUIAsset",menuName = "Stage Select UI/Stage Select UI Asset")]
    public class StageSelectUIAsset : ScriptableObject
    {
        public List<StageSelectButtonPreset> presets;
    }
}
