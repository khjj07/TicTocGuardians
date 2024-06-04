using Default.Scripts.Util;
using TicTocGuardians.Scripts.Game;
using TicTocGuardians.Scripts.Game.LevelObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace TicTocGuardians.Scripts.Assets
{
    [CreateAssetMenu(fileName = "Global LevelObject Setting",menuName = "Level/Global LevelObject Setting")]
    public class GlobalLevelSetting : ScriptableSingleton<GlobalLevelSetting>
    {
        public ModelLevelObject defaultTilePrefab;
        [FormerlySerializedAs("unPassableModelLevelObject")] public UnPassableLevelObject unPassableLevelObject;
        [FormerlySerializedAs("dimensionPrefab")] public DimensionLevelObject dimensionLevelObjectPrefab;
        public Transform dummy;
        public Mesh playerMesh;
    }
}
