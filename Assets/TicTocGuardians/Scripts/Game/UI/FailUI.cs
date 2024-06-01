using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TicTocGuardians.Scripts.Game.UI
{
    public class FailUI : MonoBehaviour
    {
        public RectTransform labelStartPoint;
        [SerializeField] private Image label;
        public Button retryButton;
        public Button goToHomeButton;

        public void Enable()
        {
            label.transform.position = labelStartPoint.position;
            label.transform.DOLocalMoveX(0, 0.2f);
        }
    }
}
