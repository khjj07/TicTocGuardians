using System.Collections.Generic;
using TicTocGuardians.Scripts.Game.ETC;
using UnityEngine;

namespace TicTocGuardians.Scripts.Assets
{
    [CreateAssetMenu(fileName = "New Stage Preset List Asset", menuName = "Stage Select UI/Stage Preset List Asset")]
    public class LevelPresetListAsset : ScriptableObject
    {
        [SerializeField] public List<LevelPreset> presets = new();

        public LevelPreset GetLevel(int index)
        {
            if(index<presets.Count) 
                return presets[index];
            return null;
        }
    }
}