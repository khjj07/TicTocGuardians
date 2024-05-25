using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace TicTocGuardians.Scripts.Game.UI
{
    public class TitleManager : MonoBehaviour
    {
        [SerializeField] private CinemaPlayer cinemaPlayer;
        [SerializeField] private Button tapToStart;

        private RectTransform _tapToStartButtonRect;
        // Start is called before the first frame update
        void Start()
        {
            tapToStart.onClick.AddListener(OnTapToStartButtonClick);
            _tapToStartButtonRect = tapToStart.GetComponent<RectTransform>();
            TitleBegin();
        }

        private void TitleBegin()
        {
             
             _tapToStartButtonRect.localScale = Vector3.zero;
             cinemaPlayer.AddCallback(OnCinemaEnd);
            cinemaPlayer.Play();
        }

        private void OnCinemaEnd(VideoPlayer videoPlayer)
        {
            _tapToStartButtonRect.DOScale(Vector3.one, 1f).SetEase(Ease.OutBounce);
        }

        public void OnTapToStartButtonClick()
        {
            StartCoroutine(GlobalLoadingManager.Instance.Loading("LobbyScene",1));
            Debug.Log("next");
        }

    }
}
