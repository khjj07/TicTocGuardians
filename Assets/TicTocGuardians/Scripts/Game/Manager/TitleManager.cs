using DG.Tweening;
using TicTocGuardians.Scripts.Game.UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace TicTocGuardians.Scripts.Game.Manager
{
    public class TitleManager : MonoBehaviour
    {
        [SerializeField] private CinemaPlayer cinemaPlayer;
        [SerializeField] private RectTransform tapToStart;

        // Start is called before the first frame update
        private void Start()
        {
            PlayerPrefs.DeleteAll();
            TitleBegin();
        }

        private void TitleBegin()
        {
            tapToStart.localScale = Vector3.zero;
            cinemaPlayer.AddCallback(OnCinemaEnd);
            GlobalSoundManager.Instance.PlaySFX("IntroSound", 1.0f);
            cinemaPlayer.Play();
        }

        private void OnCinemaEnd(VideoPlayer videoPlayer)
        {
            tapToStart.DOScale(Vector3.one, 1f).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                this.UpdateAsObservable().Where(_ => Input.anyKey).First()
                    .Subscribe(_ => SceneManager.LoadSceneAsync("LobbyScene")).AddTo(gameObject);
            });
        }
    }
}