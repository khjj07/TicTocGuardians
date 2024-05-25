using Default.Scripts.Util;
using TicTocGuardians.Scripts.Game;
using UnityEditor;
using UnityEngine;

namespace TicTocGuardians.Scripts.LevelEditor
{
    public class LevelEditor : Singleton<LevelEditor>
    {
        public DefaultAsset modelsFolder;
        public LevelSpawnPoint spawnPoint;
        public Transform origin;
    }
}