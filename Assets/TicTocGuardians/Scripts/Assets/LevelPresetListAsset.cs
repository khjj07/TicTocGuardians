using System;
using System.Collections.Generic;
using TicTocGuardians.Scripts.Game.ETC;
using UnityEngine;
using UnityEngine.Serialization;

namespace TicTocGuardians.Scripts.Assets
{
    [CreateAssetMenu(fileName = "New Stage Preset List Asset", menuName = "Stage Select UI/Stage Preset List Asset")]
    public class LevelPresetListAsset : ScriptableObject
    {
        [SerializeField]
        public List<LevelPreset> presets = new List<LevelPreset>();

        public LevelPreset GetLevel(int index)
        {
            return presets[index];
        }
    }
}
