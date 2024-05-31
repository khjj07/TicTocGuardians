using UnityEngine;

namespace TicTocGuardians.Scripts.Assets.LevelAsset
{
    [CreateAssetMenu(fileName = "New Tutorial Level Asset", menuName = "Level/Tutorial Level Asset")]
    public class TutorialLevelAsset : LevelAsset
    {
        public enum Character
        {
            Rabbit,
            Cat,
            Beaver
        }
        [Space]
        public Character character;
    }
}
