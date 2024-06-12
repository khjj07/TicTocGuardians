using System;
using System.Collections.Generic;
using Default.Scripts.Util;
using DG.Tweening;
using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using TicTocGuardians.Scripts.Game.LevelObjects;
using TicTocGuardians.Scripts.Game.Player;
using TicTocGuardians.Scripts.Game.UI;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Serialization;
using static UnityEngine.UI.Image;

namespace TicTocGuardians.Scripts.Game.Manager
{
    public class LevelManager : Singleton<LevelManager>
    {
        public Transform world;
        public Transform origin;
        public TMP_Text timerText;
        public float timeLimit;
        public double currentTime;

        public Player.Player[] playerPrefabs = new Player.Player[3];
        public List<Player.Player> playerInstances = new List<Player.Player>();
        public PlayerController playerController;
        public List<DimensionLevelObject> repairingDimensions = new List<DimensionLevelObject>();

        [HideInInspector]
        public double timeStep = 0.01d;

        public Canvas readyUI;
        public IngameUI ingameUI;
        public SuccessUI successUI;
        public FailUI failUI;

        public IDisposable timerHandler;
        public IDisposable movementWaitHandler;
        public bool isPlaying = false;

        public virtual void Start()
        {
            GameManager.Instance.ActiveLevel(this);
            GlobalLoadingManager.Instance.ActiveScene();
            InitializeIngameUI();
            InitializeSuccessUI();
            InitializeFailUI();
        }

        public virtual void LoadLevel(LevelAsset asset)
        {
            timeLimit = asset.timeLimit;
            foreach (var obj in asset.objects)
            {
                var instance = Instantiate(obj.prefab, origin);
                instance.Deserialize(obj);
            }
            SpawnPointLevelObject.Instance.Deserialize(asset.spawnPoint);
            CameraLevelObject.Instance.Deserialize(asset.camera);
            LightLevelObject.Instance.Deserialize(asset.light);
        }

        public Player.Player SpawnPlayer(PlayerType type)
        {
            var instance = Instantiate(playerPrefabs[(int)type - 1], origin);
            instance.transform.position = SpawnPointLevelObject.Instance.transform.position;
            playerController = instance.GetComponent<PlayerController>();
            playerInstances.Add(instance);
            return instance;
        }

        public void SetTimer(double time)
        {
            currentTime = time;
            timerText.SetText(currentTime.ToString("N2"));
        }

        public void StartTimer()
        {
            timerHandler = this.UpdateAsObservable().TakeWhile(_ => currentTime > 0).Subscribe(_ =>
            {
                SetTimer(currentTime - Time.deltaTime);
            }, null, PlayPhaseEnd).AddTo(gameObject);
        }
        public virtual void PlayPhaseStart(PlayerController player)
        {
            SetTimer(timeLimit);
            CreateMovementWaitStream(player);
        }

        public void ResetRepairing()
        {
            repairingDimensions.Clear();
        }

        public virtual void PlayPhaseEnd()
        {
            isPlaying = false;
        }

        public void CreateMovementWaitStream(PlayerController controller)
        {
            movementWaitHandler = Observable.Amb(GlobalInputBinder.CreateGetAxisStreamOptimize("Horizontal").Select(x => Math.Abs(x) != 0),
                GlobalInputBinder.CreateGetAxisStreamOptimize("Vertical").Select(x => Math.Abs(x) != 0),
                GlobalInputBinder.CreateGetKeyDownStream(KeyCode.Space).Select(_=>true)).First().Subscribe(_ =>
            {
                isPlaying = true;
                ActiveLevel(controller);
            }).AddTo(gameObject);
        }

        public virtual void ActiveLevel(PlayerController controller)
        {
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
        }

        public void AddRepairDimension(DimensionLevelObject repairTarget)
        {
            repairingDimensions.Add(repairTarget);
        }

        public void RemoveRepairDimension(DimensionLevelObject repairTarget)
        {
            repairingDimensions.Remove(repairTarget);
        }

        public virtual void OnReadyPhaseActive()
        {
            readyUI.gameObject.SetActive(true);
            ingameUI.gameObject.SetActive(false);
            successUI.gameObject.SetActive(false);
            failUI.gameObject.SetActive(false);
        }

        public virtual void OnPlayPhaseActive()
        {
            readyUI.gameObject.SetActive(false);
            ingameUI.gameObject.SetActive(true);
            successUI.gameObject.SetActive(false);
            failUI.gameObject.SetActive(false);
        }


        public virtual void OnFailPhaseActive()
        {
            readyUI.gameObject.SetActive(false);
            ingameUI.gameObject.SetActive(false);
            successUI.gameObject.SetActive(false);
            failUI.gameObject.SetActive(true);
            failUI.label.transform.position = failUI.labelStartPoint.position;
            failUI.label.transform.DOLocalMoveX(0, 0.2f);
        }


        public virtual void OnSuccessPhaseActive()
        {
            readyUI.gameObject.SetActive(false);
            ingameUI.gameObject.SetActive(false);
            failUI.gameObject.SetActive(false);
            successUI.gameObject.SetActive(true);
            successUI.label.transform.position = successUI.labelStartPoint.position;
            successUI.label.transform.DOLocalMoveX(0, 0.2f);
        }

        public virtual void InitializeIngameUI()
        {
            ingameUI.skipButton.onClick.AddListener(() =>
            {
                Time.timeScale = 4.0f;
                playerController.DisposeAllStream();
            });
        }

        public void InitializeFailUI()
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

        public void InitializeSuccessUI()
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

        public void CreateDimensionCheckStream(Player.Player player)
        {
            var dimensionEnterStream = player.OnTriggerEnterAsObservable()
                .Where(x => x.CompareTag("Dimension"))
                .Select(x => x.GetComponent<DimensionLevelObject>());

            var dimensionExitStream = player.OnTriggerExitAsObservable()
                .Where(x => x.CompareTag("Dimension"))
                .Select(x => x.GetComponent<DimensionLevelObject>());

            dimensionEnterStream.Subscribe(AddRepairDimension).AddTo(player.gameObject);
            dimensionExitStream.Subscribe(RemoveRepairDimension).AddTo(player.gameObject);

        }
    }
}
