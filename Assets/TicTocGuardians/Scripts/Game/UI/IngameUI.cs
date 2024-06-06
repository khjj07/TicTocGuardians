using UnityEngine;
using UnityEngine.UI;

namespace TicTocGuardians.Scripts.Game.UI
{
    public class IngameUI : MonoBehaviour
    {
        public Button changeOrderButton;
        public Button replayButton;
        public Button skipButton;
        public Sprite[] defaultPortraitSprites = new Sprite[3];
        public Sprite[] highLightPortraitSprites = new Sprite[3];
        public Image[] portraits = new Image[3];
    }
}