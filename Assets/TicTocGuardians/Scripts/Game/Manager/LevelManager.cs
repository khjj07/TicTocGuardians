using System;
using System.Collections.Generic;
using Default.Scripts.Util;
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
        public List<DimensionLevelObject> repairingDimensions = new List<DimensionLevelObject>();

        [HideInInspector]
        public double timeStep = 0.01d;

        public Canvas readyUI;
        public IngameUI ingameUI;
        public SuccessUI successUI;
        public FailUI failUI;

        public virtual void Start()
        {
            GlobalLoadingManager.Instance.ActiveScene();
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
            instance.CreateDimensionCheckStream();
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

            Observable.Interval(TimeSpan.FromSeconds(timeStep)).TakeWhile(_ => currentTime > 0).Subscribe(_ =>
            {
                SetTimer(currentTime - timeStep);
            }, null, PlayPhaseEnd);
        }
        public virtual void PlayPhaseStart(Player.Player player)
        {
            ResetRepairing();
            EnableIngameUI();
            SetTimer(timeLimit);
            CreateMovementWaitStream(player.GetComponent<PlayerController>());
        }

        private void ResetRepairing()
        {
            repairingDimensions.Clear();
        }

        public virtual void PlayPhaseEnd()
        {
            
        }

        public void CreateMovementWaitStream(PlayerController controller)
        {
            Observable.Amb(GlobalInputBinder.CreateGetAxisStreamOptimize("Horizontal").Select(x => Math.Abs(x) != 0),
                GlobalInputBinder.CreateGetAxisStreamOptimize("Vertical").Select(x => Math.Abs(x) != 0),
                GlobalInputBinder.CreateGetKeyDownStream(KeyCode.Space)).First().Subscribe(_ =>
            {
                ActiveLevel(controller);
            }).AddTo(gameObject);
        }

        public virtual void ActiveLevel(PlayerController controller)
        {
            controller.CreateMovementStream();
        }

        public void AddRepairDimension(DimensionLevelObject repairTarget)
        {
            repairingDimensions.Add(repairTarget);
        }

        public void RemoveRepairDimension(DimensionLevelObject repairTarget)
        {
            repairingDimensions.Remove(repairTarget);
        }

        public virtual void EnableReadyUI()
        {
            readyUI.gameObject.SetActive(true);
            ingameUI.gameObject.SetActive(false);
            successUI.gameObject.SetActive(false);
            failUI.gameObject.SetActive(false);
        }

        public virtual void EnableIngameUI()
        {
            readyUI.gameObject.SetActive(false);
            ingameUI.gameObject.SetActive(true);
            successUI.gameObject.SetActive(false);
            failUI.gameObject.SetActive(false);
        }


        public virtual void EnableFailUI()
        {
            readyUI.gameObject.SetActive(false);
            ingameUI.gameObject.SetActive(false);
            successUI.gameObject.SetActive(false);
            failUI.gameObject.SetActive(true);
            failUI.Enable(); ;
        }


        public virtual void EnableSuccessUI()
        {
            readyUI.gameObject.SetActive(false);
            ingameUI.gameObject.SetActive(false);
            successUI.gameObject.SetActive(true);
            failUI.gameObject.SetActive(false);
            successUI.Enable(); ;
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
    }
}
