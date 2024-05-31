using Default.Scripts.Util;
using TicTocGuardians.Scripts.Game;
using TicTocGuardians.Scripts.Game.LevelObjects;
using UnityEditor;
using UnityEngine;

namespace TicTocGuardians.Scripts.LevelEditor
{
    public class LevelEditor : Singleton<LevelEditor>
    {
        public DefaultAsset modelsFolder;
        public LevelSpawnPoint spawnPoint;
        public LevelCamera levelCamera;
        public LevelLight levelLight;
        public Transform origin;
    }
}