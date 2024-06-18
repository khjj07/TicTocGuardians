using TicTocGuardians.Scripts.Game.Player;
using UnityEngine;

namespace TicTocGuardians.Scripts.Assets.LevelAsset
{
    [CreateAssetMenu(fileName = "New Tutorial Level Asset", menuName = "Level/Tutorial Level Asset")]
    public class TutorialLevelAsset : LevelAsset
    {
        [Space] public PlayerType character;
    }
}