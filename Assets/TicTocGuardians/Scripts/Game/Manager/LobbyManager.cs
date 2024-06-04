using System.Threading.Tasks;
using DG.Tweening;
using TicTocGuardians.Scripts.Game.ETC;
using TicTocGuardians.Scripts.Game.LevelObjects;
using TicTocGuardians.Scripts.Game.Player;
using TicTocGuardians.Scripts.Game.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace TicTocGuardians.Scripts.Game.Manager
{
    public class LobbyManager : MonoBehaviour
    {
        [FormerlySerializedAs("levelCamera")] [SerializeField]
        private CameraLevelObject cameraLevelObject;
        [SerializeField]
        private ReactableArea normalUIArea;
        [SerializeField]
        private ReactableArea hardUIArea;

        [SerializeField] private PlayerController controller;

        [FormerlySerializedAs("difficultySelectUI")] [SerializeField] private DifficultySelectTV difficultySelectTV;
        [SerializeField] private Transform centerCameraPoint;
        [SerializeField] private Transform leftCameraPoint;
        [SerializeField] private Transform rightCameraPoint;

        [SerializeField] private StageSelectUI normalSelectUI;
        [SerializeField] private StageSelectUI hardSelectUI;

        [SerializeField] private RectTransform normalSelectUIStartPoint;
        [SerializeField] private RectTransform normalSelectUIEndPoint;
        [SerializeField] private RectTransform hardSelectUIStartPoint;
        [SerializeField] private RectTransform hardSelectUIEndPoint;
        void Start()
        {
            controller.CreateMovementStream();
            GlobalLoadingManager.Instance.ActiveScene();
            normalUIArea.stepInEvent.AddListener(() =>
            {
                cameraLevelObject.Move(rightCameraPoint.position, 1.0f);
                 difficultySelectTV.ChangeStateWithGlitch(DifficultySelectTV.State.Normal);
                normalSelectUI.transform.DOMove(normalSelectUIEndPoint.position, 1.0f);
                EventSystem.current.SetSelectedGameObject(normalSelectUI.frame.GetChild(0).gameObject);
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
                EventSystem.current.SetSelectedGameObject(hardSelectUI.frame.GetChild(0).gameObject);
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

    }
}
