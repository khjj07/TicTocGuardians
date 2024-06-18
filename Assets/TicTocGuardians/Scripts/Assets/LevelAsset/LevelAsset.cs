using TicTocGuardians.Scripts.Game.LevelObjects;
using UnityEngine;

namespace TicTocGuardians.Scripts.Assets.LevelAsset
{
    public class LevelAsset : ScriptableObject
    {
        [Header("Setting")] public float timeLimit;

        [Header("Objects")] public LevelObjectAsset[] objects;

        public LevelObjectAsset camera;
        public LevelObjectAsset light;
    }
}