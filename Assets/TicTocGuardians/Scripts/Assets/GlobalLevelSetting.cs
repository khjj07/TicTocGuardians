using Default.Scripts.Util;
using TicTocGuardians.Scripts.Game;
using TicTocGuardians.Scripts.Game.Abstract;
using TicTocGuardians.Scripts.Game.LevelObjects;
using UnityEngine;

namespace TicTocGuardians.Scripts.Assets
{
    [CreateAssetMenu(fileName = "Global LevelObject Setting",menuName = "Level/Global LevelObject Setting")]
    public class GlobalLevelSetting : ScriptableSingleton<GlobalLevelSetting>
    {
        public LevelObject defaultTilePrefab;
        public UnPassableLevelObject unPassableLevelObject;
        public Transform dummy;
        public Mesh playerMesh;
    }
}
