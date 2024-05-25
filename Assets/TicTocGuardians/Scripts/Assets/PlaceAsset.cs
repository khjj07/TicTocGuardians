using TicTocGuardians.Scripts.Game;
using UnityEngine;

namespace TicTocGuardians.Scripts.Assets
{
    [CreateAssetMenu(fileName = "New Place", menuName = "Level/Place Asset")]
    public class PlaceAsset : ScriptableObject
    {
        [Header("Setting")] 
        public float timeLimit;
        public int chance;

        [Header("Objects")]
        public BaseObjectAsset spawnPoint;
        public LevelObjectAsset[] objects;
    }
}
