using System;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using TicTocGuardians.Scripts.Game.Player;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.Manager
{
    public class TutorialLevelManager : LevelManager
    {
        public enum Phase
        {
            None,
            Ready,
            Play,
            Success,
            Fail
        }

        [SerializeField] private PlayerType playerType;

        [Header("상태")] [SerializeField] private Phase currentState;

        private readonly Subject<Phase> _phaseSubject = new();

        public override void Start()
        {
            base.Start();
            InitializePhaseSubject();
            ChangeState(Phase.Ready);
        }

        public override void LoadLevel(LevelAsset asset)
        {
            base.LoadLevel(asset);
            var tutorialAsset = asset as TutorialLevelAsset;
            playerType = tutorialAsset.character;
        }

        public void CreateReadyPhaseStream()
        {
            this.UpdateAsObservable().Where(_ => Input.anyKey).Take(1).Subscribe(_ => ChangeState(Phase.Play))
                .AddTo(gameObject);
        }

        public override void PlayPhaseEnd()
        {
            base.PlayPhaseEnd();
            _isEnd = true;
            Observable.Timer(TimeSpan.FromSeconds(2.0f)).Subscribe(_ =>
            {
                Destroy(playerController.gameObject);
                if (repairingDimensions.Count == 1)
                    ChangeState(Phase.Success);
                else
                    ChangeState(Phase.Fail);
            }).AddTo(gameObject);
        }

        public override void RePlay()
        {
            base.RePlay();
            if (isPlaying)
            {
                PlayPhaseForceEnd();
                ChangeState(currentState);
                if (playCount > 0) playCount--;
            }
            else
            {
                PlayPhaseForceEnd();
                PreviousState();
                if (playCount > 1)
                    playCount -= 2;
                else if (playCount > 0) playCount--;
            }
        }

        private void InitializePhaseSubject()
        {
            _phaseSubject.Where(x => x == Phase.Ready).Subscribe(_ =>
            {
                OnReadyPhaseActive();
                CreateReadyPhaseStream();
            });

            _phaseSubject.Where(x => x == Phase.Play).Subscribe(_ => { OnPlayPhaseActive(); });

            _phaseSubject.Where(x => x == Phase.Success).Subscribe(_ => { OnSuccessPhaseActive(); });

            _phaseSubject.Where(x => x == Phase.Fail).Subscribe(_ => { OnFailPhaseActive(); });
        }

        public override void PlayPhaseForceEnd()
        {
            base.PlayPhaseForceEnd();
            Destroy(playerController.gameObject);
        }

        public override void OnPlayPhaseActive()
        {
            base.OnPlayPhaseActive();
            var player = SpawnPlayer(playerType, 0);
            CreateDimensionCheckStream(player);
            PlayPhaseStart(playerController);
        }

        public void ChangeState(Phase state)
        {
            _phaseSubject.OnNext(state);
            currentState = state;
        }

        public void NextState()
        {
            ChangeState(currentState + 1);
        }

        public void PreviousState()
        {
            ChangeState(currentState - 1);
        }
    }
}