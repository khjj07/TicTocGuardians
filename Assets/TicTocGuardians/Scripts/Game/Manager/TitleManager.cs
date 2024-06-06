using DG.Tweening;
using TicTocGuardians.Scripts.Game.UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace TicTocGuardians.Scripts.Game.Manager
{
    public class TitleManager : MonoBehaviour
    {
        [SerializeField] private CinemaPlayer cinemaPlayer;
        [SerializeField] private Button tapToStart;

        private RectTransform _tapToStartButtonRect;
        // Start is called before the first frame update
        void Start()
        {
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
            _tapToStartButtonRect.DOScale(Vector3.one, 1f).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                this.UpdateAsObservable().Where(_ => Input.anyKey).First()
                    .Subscribe(_ => StartCoroutine(GlobalLoadingManager.Instance.Load("LobbyScene",1.0f))).AddTo(gameObject);
            });
        }


    }
}
