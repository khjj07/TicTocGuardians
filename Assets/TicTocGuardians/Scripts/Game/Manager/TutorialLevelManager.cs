using System;
using Default.Scripts.Util;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using TicTocGuardians.Scripts.Game.Player;
using TicTocGuardians.Scripts.Game.UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Profiling;

namespace TicTocGuardians.Scripts.Game.Manager
{
    public class TutorialLevelManager : LevelManager
    {
        public enum Phase
        {
            None,
            Ready,
            Player,
            Success,
            Fail
        }

        [Header("UI")]
        [SerializeField] private Canvas readyUI;
        [SerializeField] private IngameUI ingameUI;
        [SerializeField] private SuccessUI successUI;
        [SerializeField] private FailUI failUI;

        private Subject<Phase> _phaseSubject = new Subject<Phase>();

        [SerializeField] private PlayerType playerType;

        [Header("상태")]
        [SerializeField]
        private Phase currentState;

        public override void Start()
        {
            base.Start();
            GameManager.Instance.ActiveLevel(this);
            InitializeSuccessUI();
            InitializeFailUI();
            ChangeState(Phase.Ready);
        }

        public override void LoadLevel(LevelAsset asset)
        {
            base.LoadLevel(asset);
            var tutorialAsset = asset as TutorialLevelAsset;
            playerType = tutorialAsset.character;
        }

        private void InitializeFailUI()
        {
            failUI.goToHomeButton.onClick.AddListener(() =>
            {
                StartCoroutine(GlobalLoadingManager.Instance.Load("LobbyScene", 1.0f));
            });
            failUI.retryButton.onClick.AddListener(() =>
            {
                GameManager.Instance.LoadLevel(GameManager.Instance.GetCurrentIndex());
            });
        }

        private void InitializeSuccessUI()
        {
            successUI.goToHomeButton.onClick.AddListener(() =>
            {
                StartCoroutine(GlobalLoadingManager.Instance.Load("LobbyScene", 1.0f));
            });
            successUI.nextLevelButton.onClick.AddListener(() =>
            {
                GameManager.Instance.LoadLevel(GameManager.Instance.GetCurrentIndex() + 1);
            });
        }

        private void EnableReadyUI()
        {
            readyUI.gameObject.SetActive(true);
            ingameUI.gameObject.SetActive(false);
            successUI.gameObject.SetActive(false);
            failUI.gameObject.SetActive(false);
        }
        private void EnableIngameUI()
        {
            readyUI.gameObject.SetActive(false);
            ingameUI.gameObject.SetActive(true);
            successUI.gameObject.SetActive(false);
            failUI.gameObject.SetActive(false);
        }
        public void CreateReadyPhaseStream()
        {
            this.UpdateAsObservable().Where(_ => Input.anyKey).First().Subscribe(_ => ChangeState(Phase.Player)).AddTo(gameObject);
        }

        public void PlayerPhaseStart()
        {
            EnableIngameUI();
            var player = SpawnPlayer(playerType);
            SetTimer(timeLimit);
            CreateMovementWaitStream(playerInstances[0].GetComponent<PlayerController>());
        }
        public void PlayerPhaseEnd()
        {
            
            NextState();
        }


        public void CreateMovementWaitStream(PlayerController controller)
        {
            Observable.Amb(GlobalInputBinder.CreateGetAxisStreamOptimize("Horizontal").Select(x => Math.Abs(x) != 0),
                GlobalInputBinder.CreateGetAxisStreamOptimize("Vertical").Select(x => Math.Abs(x) != 0),
                GlobalInputBinder.CreateGetKeyDownStream(KeyCode.Space)).First().Subscribe(_ =>
            {
                controller.CreateMovementStream();
                StartTimer();
            }).AddTo(gameObject);
        }

        public void SetTimer(double time)
        {
            currentTime = time;
            timerText.SetText(currentTime.ToString("N2"));
        }
        public void StartTimer()
        {

            Observable.Interval(TimeSpan.FromSeconds(timeStep)).TakeWhile(_ => currentTime > 0).Subscribe(_ =>
            {
                SetTimer(currentTime - timeStep);
            }, null, () =>
            {
                PlayerPhaseEnd();
            });
        }

        private void InitializePhaseSubject()
        {
            _phaseSubject.Where(x => x == Phase.Ready).Subscribe(_ =>
            {
                EnableReadyUI();
                CreateReadyPhaseStream();
            });

            _phaseSubject.Where(x => x == Phase.Player).Subscribe(_ =>
            {
                PlayerPhaseStart();
            });

            _phaseSubject.Where(x => x == Phase.Success).Subscribe(_ =>
            {
                EnableSuccessUI();
            });

            _phaseSubject.Where(x => x == Phase.Fail).Subscribe(_ =>
            {
                EnableFailUI();
            });
        }
        public void ChangeState(Phase state)
        {
            _phaseSubject.OnNext(state);
            currentState = state;
        }

        public void NextState()
        {
            ChangeState((Phase)(currentState + 1));
        }

        public void PreviousState()
        {
            ChangeState((Phase)(currentState - 1));
        }

        private void EnableFailUI()
        {
            readyUI.gameObject.SetActive(false);
            ingameUI.gameObject.SetActive(false);
            successUI.gameObject.SetActive(false);
            failUI.gameObject.SetActive(true);
            failUI.Enable(); ;
        }

        private void EnableSuccessUI()
        {
            readyUI.gameObject.SetActive(false);
            ingameUI.gameObject.SetActive(false);
            successUI.gameObject.SetActive(true);
            failUI.gameObject.SetActive(false);
            successUI.Enable(); ;
        }
    }
}