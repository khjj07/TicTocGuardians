using System.Collections.Generic;
using TicTocGuardians.Scripts.Game;
using TicTocGuardians.Scripts.Game.LevelObjects;
using UnityEditor;
using UnityEngine;

namespace TicTocGuardians.Scripts.Assets.LevelAsset
{
    public class LevelAsset : ScriptableObject
    {
        [Header("Setting")]
        public float timeLimit;

        [Header("Objects")]
        public LevelObjectAsset[] objects;

        public LevelObjectAsset spawnPoint;
        public LevelObjectAsset camera;
        public LevelObjectAsset light;
    }
}
