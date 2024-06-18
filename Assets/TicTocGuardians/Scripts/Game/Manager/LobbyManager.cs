using System;
using Default.Scripts.Util;
using DG.Tweening;
using TicTocGuardians.Scripts.Game.ETC;
using TicTocGuardians.Scripts.Game.LevelObjects;
using TicTocGuardians.Scripts.Game.Player;
using TicTocGuardians.Scripts.Game.UI;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace TicTocGuardians.Scripts.Game.Manager
{
    public class LobbyManager : Singleton<LobbyManager>
    {
        [FormerlySerializedAs("levelCamera")] [SerializeField]
        private CameraLevelObject cameraLevelObject;

        [SerializeField] private ReactableArea normalUIArea;

        [SerializeField] private ReactableArea hardUIArea;

        public PlayerController controller;

        [FormerlySerializedAs("difficultySelectUI")] [SerializeField]
        private DifficultySelectTV difficultySelectTV;

        [SerializeField] private Transform centerCameraPoint;
        [SerializeField] private Transform leftCameraPoint;
        [SerializeField] private Transform rightCameraPoint;

        [SerializeField] private StageSelectUI normalSelectUI;
        [SerializeField] private StageSelectUI hardSelectUI;

        [SerializeField] private RectTransform normalSelectUIStartPoint;
        [SerializeField] private RectTransform normalSelectUIEndPoint;
        [SerializeField] private RectTransform hardSelectUIStartPoint;
        [SerializeField] private RectTransform hardSelectUIEndPoint;

        [SerializeField] private SynopsisUI synopsisUI;
        [SerializeField] private CreditUI creditUI;

        private void Start()
        {
            //PlayerPrefs.DeleteAll();
            controller.gameObject.SetActive(false);
            synopsisUI.gameObject.SetActive(false);
            creditUI.gameObject.SetActive(false);
            if (!PlayerPrefs.HasKey("FirstTimeFlag"))
            {
                synopsisUI.gameObject.SetActive(true);
                synopsisUI.transform.localScale = Vector3.zero;
                synopsisUI.transform.DOScale(1, 1f);
                PlayerPrefs.SetInt("FirstTimeFlag", 1);
            }
            else
            {
                PlayerActive();
            }

            GlobalSoundManager.Instance.PlayBGM("BGM_Main");
            GlobalLoadingManager.Instance.ActiveScene();
            normalUIArea.stepInEvent.AddListener(() =>
            {
                cameraLevelObject.Move(rightCameraPoint.position, 1.0f);
                difficultySelectTV.ChangeStateWithGlitch(DifficultySelectTV.State.Normal);
                normalSelectUI.transform.DOMove(normalSelectUIEndPoint.position, 1.0f);
                normalSelectUI.gameObject.SetActive(true);
            });

            normalUIArea.stepOutEvent.AddListener(() =>
            {
                cameraLevelObject.Move(centerCameraPoint.position, 1.0f);

                difficultySelectTV.ChangeStateWithGlitch(DifficultySelectTV.State.Select);
                normalSelectUI.transform.DOMove(normalSelectUIStartPoint.position, 1.0f).OnComplete(() =>
                {
                    normalSelectUI.gameObject.SetActive(false);
                });
            });

            hardUIArea.stepInEvent.AddListener(() =>
            {
                cameraLevelObject.Move(leftCameraPoint.position, 1.0f);
                difficultySelectTV.ChangeStateWithGlitch(DifficultySelectTV.State.Hard);
                hardSelectUI.transform.DOMove(hardSelectUIEndPoint.position, 1.0f);
                hardSelectUI.gameObject.SetActive(true);
            });

            hardUIArea.stepOutEvent.AddListener(() =>
            {
                cameraLevelObject.Move(centerCameraPoint.position, 1.0f);
                difficultySelectTV.ChangeStateWithGlitch(DifficultySelectTV.State.Select);
                hardSelectUI.transform.DOMove(hardSelectUIStartPoint.position, 1.0f).OnComplete(() =>
                {
                    hardSelectUI.gameObject.SetActive(false);
                });
            });
        }

        public void PlayerActive()
        {
            controller.gameObject.SetActive(true);
            controller.transform.localScale = Vector3.zero;
            controller.transform.DOScale(1, 0.5f);
            switch (controller.type)
            {
                case PlayerType.None:
                    break;
                case PlayerType.Beaver:
                    controller.CreateMovementStream();
                    controller.CreateBeaverStream();
                    break;
                case PlayerType.Cat:
                    controller.CreateMovementStream();
                    break;
                case PlayerType.Rabbit:
                    controller.CreateMovementStream();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            GlobalInputBinder.CreateGetKeyDownStream(KeyCode.F7).Where(_ => !creditUI.isActiveAndEnabled).Subscribe(_ =>
            {
                creditUI.gameObject.SetActive(true);
                controller.gameObject.SetActive(false);
                GlobalInputBinder.CreateGetMouseButtonDownStream(0).Take(3)
                    .Subscribe(_ => { creditUI.NextSprite(); });
            });
        }
    }
}