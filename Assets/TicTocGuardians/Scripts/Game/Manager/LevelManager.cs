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
using Action = TicTocGuardians.Scripts.Game.Player.Action;

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
        public OrderingUI orderingUI;
        public PauseUI pauseUI;

        public IDisposable timerHandler;
        public IDisposable movementWaitHandler;
        public IDisposable pauseStreamHandler;
        public bool isPlaying = false;
        public bool isSkip = false;
        public bool isPause = false;

        public virtual void Start()
        {
            GameManager.Instance.ActiveLevel(this);
            GlobalLoadingManager.Instance.ActiveScene();
            GlobalSoundManager.Instance.PlayBGM("BGM_Ingame");
            InitializeIngameUI();
            InitializeSuccessUI();
            InitializeFailUI();
            InitializePauseUI();
            CreateShortCutStream();
        }

        public void Continue()
        {
            if (isSkip)
            {
                Time.timeScale = 4;
            }
            else
            {
                Time.timeScale = 1;
            }
            pauseUI.gameObject.SetActive(false);
            isPause = false;
        }

        public void Restart()
        {
            Time.timeScale = 1;
            GameManager.Instance.LoadLevel(GameManager.Instance.GetCurrentIndex());
        }

        public void GoToLevel()
        {
            Time.timeScale = 1;
            StartCoroutine(GlobalLoadingManager.Instance.Load("LobbyScene", 1.0f));
        }


        public void InitializePauseUI()
        {
            pauseUI.continueButton.onClick.AddListener(Continue);
            pauseUI.restartButton.onClick.AddListener(Restart);
            pauseUI.levelButton.onClick.AddListener(GoToLevel);
            pauseUI.gameObject.SetActive(false);
        }
        
        public IDisposable CreatePauseStream()
        {
            return GlobalInputBinder.CreateGetKeyDownStream(KeyCode.Escape).Subscribe(_ =>
            {
            
                if (!isPause)
                {
                    Time.timeScale = 0;
                    pauseUI.gameObject.SetActive(true);
                    pauseUI.transform.localScale = Vector3.zero;
                    pauseUI.transform.DOScale(Vector3.one, 0.3f);
                    isPause = true;
                }
                else
                {
                    Continue();
                }
               
            }).AddTo(gameObject);
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
            pauseStreamHandler=CreatePauseStream();
            CreateMovementWaitStream(player);
        }

        public void ResetRepairing()
        {
            repairingDimensions.Clear();
        }

        public virtual void PlayPhaseEnd()
        {
            isPlaying = false;
            Time.timeScale = 1.0f;
            if (timerHandler != null)
            {
                timerHandler.Dispose();
                timerHandler = null;
            }
            if (movementWaitHandler != null)
            {
                movementWaitHandler.Dispose();
                movementWaitHandler = null;
            }
            if (pauseStreamHandler != null)
            {
                pauseStreamHandler.Dispose();
                pauseStreamHandler = null;
            }
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
            StartTimer();
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
            orderingUI.gameObject.SetActive(false);
        }

        public virtual void OnPlayPhaseActive()
        {
            readyUI.gameObject.SetActive(false);
            ingameUI.gameObject.SetActive(true);
            successUI.gameObject.SetActive(false);
            failUI.gameObject.SetActive(false);
            orderingUI.gameObject.SetActive(false);
            foreach (var portrait in ingameUI.portraits)
            {
                portrait.gameObject.SetActive(false);
            }
            ingameUI.changeOrderButton.gameObject.SetActive(false);
        }


        public virtual void OnFailPhaseActive()
        {
            readyUI.gameObject.SetActive(false);
            ingameUI.gameObject.SetActive(false);
            successUI.gameObject.SetActive(false);
            failUI.gameObject.SetActive(true);
            orderingUI.gameObject.SetActive(false);
            failUI.label.transform.position = failUI.labelStartPoint.position;
            failUI.label.transform.DOLocalMoveX(0, 0.2f);
            GlobalSoundManager.Instance.PlaySFX("SFX_FailNotice");
        }


        public virtual void OnSuccessPhaseActive()
        {
            readyUI.gameObject.SetActive(false);
            ingameUI.gameObject.SetActive(false);
            failUI.gameObject.SetActive(false);
            successUI.gameObject.SetActive(true);
            orderingUI.gameObject.SetActive(false);
            successUI.label.transform.position = successUI.labelStartPoint.position;
            successUI.label.transform.DOLocalMoveX(0, 0.2f);
            GlobalSoundManager.Instance.PlaySFX("SFX_ClearNotice");
        }

        public virtual void InitializeIngameUI()
        {
            ingameUI.skipButton.onClick.AddListener(Skip);
            ingameUI.replayButton.onClick.AddListener(RePlay);
        }

        public void InitializeFailUI()
        {
            failUI.goToHomeButton.onClick.AddListener(GoToLevel);
            failUI.retryButton.onClick.AddListener(Restart);
        }

        public void InitializeSuccessUI()
        {
            successUI.goToHomeButton.onClick.AddListener(GoToLevel);
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

            dimensionEnterStream.Subscribe(x=>
            {
                player.Act(new Action(Action.State.Repair));
                AddRepairDimension(x);
            }).AddTo(player.gameObject);
            dimensionExitStream.Subscribe(x=>
            {
                player.Act(new Action(Action.State.RepairRelease));
                RemoveRepairDimension(x);
            }).AddTo(player.gameObject);

        }

        public virtual void CreateShortCutStream()
        {
            GlobalInputBinder.CreateGetKeyDownStream(KeyCode.R).Subscribe(_ => RePlay()).AddTo(gameObject);
            GlobalInputBinder.CreateGetKeyDownStream(KeyCode.X).Subscribe(_ => Skip()).AddTo(gameObject);
        }

        public virtual void Skip()
        {
            if (isPlaying)
            {
                isSkip = true;
                Time.timeScale = 4.0f;
                playerController.DisposeAllStream();
                playerController.GetComponent<Player.Player>().Act(new Action(Action.State.Wait));
            }
        }

        public virtual void RePlay()
        {
          
        }

        public virtual void PlayPhaseForceEnd()
        {
            isPlaying = false;
            ResetRepairing();
            Time.timeScale = 1.0f;
            if (timerHandler != null)
            {
                timerHandler.Dispose();
                timerHandler = null;
            }
            if (movementWaitHandler != null)
            {
                movementWaitHandler.Dispose();
                movementWaitHandler = null;
            }
            if (pauseStreamHandler != null)
            {
                pauseStreamHandler.Dispose();
                pauseStreamHandler = null;
            }
        }
    }
}
