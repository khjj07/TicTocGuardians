using Default.Scripts.Util;
using TicTocGuardians.Scripts.Game;
using UnityEngine;

namespace TicTocGuardians.Scripts.Assets
{
    [CreateAssetMenu(fileName = "Global LevelObject Setting",menuName = "Level/Global LevelObject Setting")]
    public class GlobalLevelSetting : ScriptableSingleton<GlobalLevelSetting>
    {
        public LevelObject defaultTilePrefab;
        public Transform dummy;
        public Mesh playerMesh;
    }
}
