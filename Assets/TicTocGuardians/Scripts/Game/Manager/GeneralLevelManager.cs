using System;
using System.Collections.Generic;
using Default.Scripts.Util;
using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Game.LevelObjects;
using TicTocGuardians.Scripts.Game.Player;
using TicTocGuardians.Scripts.Game.UI;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Action = TicTocGuardians.Scripts.Game.Player.Action;

namespace TicTocGuardians.Scripts.Game.Manager
{
    public class GeneralLevelManager : LevelManager
    {
        public enum Phase
        {
            None,
            Ready,
            Ordering,
            Player1,
            Player2,
            Player3,
            Success,
            Fail
        }

        [SerializeField] private PlayerClone[] clonePrefabs = new PlayerClone[3];

        [SerializeField] private List<PlayerType> playerOrder = new List<PlayerType>();
        [Header("UI")]
        [SerializeField] private Canvas readyUI;
        [SerializeField] private OrderingUI orderingUI;
        [SerializeField] private IngameUI ingameUI;
        [SerializeField] private SuccessUI successUI;
        [SerializeField] private FailUI failUI;

        [Header("ป๓ลย")]
        [SerializeField]
        private Phase currentState;

        private Subject<Phase> _stateSubject = new Subject<Phase>();


        private PlayerRecorder _recorder;
        [SerializeField]
        private List<PlayerCloneData> _cloneData = new List<PlayerCloneData>();
        private List<PlayerClone> _currentClones = new List<PlayerClone>();

        public void Awake()
        {
            _recorder = GetComponent<PlayerRecorder>();
        }

        public override void Start()
        {
            base.Start();
            GameManager.Instance.ActiveLevel(this);
            orderingUI.submitButton.onClick.AddListener(SubmitOrder);
            orderingUI.cancelButton.onClick.AddListener(CancelOrder);
            orderingUI.selectButtons[0].AddListener(() => { CharacterSelectButtonOnClick(0); });
            orderingUI.selectButtons[1].AddListener(() => { CharacterSelectButtonOnClick(1); });
            orderingUI.selectButtons[2].AddListener(() => { CharacterSelectButtonOnClick(2); });
            successUI.goToHomeButton.onClick.AddListener(() => { StartCoroutine(GlobalLoadingManager.Instance.Load("LobbyScene", 1.0f));});
            successUI.nextLevelButton.onClick.AddListener(() =>
            {
                GameManager.Instance.LoadLevel(GameManager.Instance.GetCurrentIndex()+1);
            });

            failUI.goToHomeButton.onClick.AddListener(() => { StartCoroutine(GlobalLoadingManager.Instance.Load("LobbyScene", 1.0f)); });
            failUI.retryButton.onClick.AddListener(() =>
            { 
                GameManager.Instance.LoadLevel(GameManager.Instance.GetCurrentIndex());
            });
            _stateSubject.Where(x => x == Phase.Ready).Subscribe(_ =>
            {
                EnableReadyUI();
                CreateReadyPhaseStream();
            });

            _stateSubject.Where(x => x == Phase.Ordering).Subscribe(_ =>
            {
                EnableOrderingUI();
            });

            _stateSubject.Where(x => x == Phase.Player1).Subscribe(_ =>
            {
                PlayerPhaseStart(0);
            });

            _stateSubject.Where(x => x == Phase.Player2).Subscribe(_ =>
            {
                PlayerPhaseStart(1);
            });

            _stateSubject.Where(x => x == Phase.Player3).Subscribe(_ =>
            {
                PlayerPhaseStart(2);
            });

            _stateSubject.Where(x => x == Phase.Success).Subscribe(_ =>
            {
                EnableSuccessUI();
            });

            _stateSubject.Where(x => x == Phase.Fail).Subscribe(_ =>
            {
                EnableFailUI();
            });
            ChangeState(Phase.Ready);
        }

        private void EnableFailUI()
        {
            readyUI.gameObject.SetActive(false);
            orderingUI.gameObject.SetActive(false);
            ingameUI.gameObject.SetActive(false);
            successUI.gameObject.SetActive(false);
            failUI.gameObject.SetActive(true);
            failUI.Enable();;
        }

        private void EnableSuccessUI()
        {
            readyUI.gameObject.SetActive(false);
            orderingUI.gameObject.SetActive(false);
            ingameUI.gameObject.SetActive(false);
            successUI.gameObject.SetActive(true);
            failUI.gameObject.SetActive(false);
            successUI.Enable(); ;
        }

        private void EnableReadyUI()
        {
            readyUI.gameObject.SetActive(true);
            orderingUI.gameObject.SetActive(false);
            ingameUI.gameObject.SetActive(false);
            successUI.gameObject.SetActive(false);
            failUI.gameObject.SetActive(false);
        }

        private void CharacterSelectButtonOnClick(int i)
        {
            orderingUI.SubmitUnavilable();
            if (!orderingUI.selectButtons[i].IsSelected())
            {
                AddPlayerOrder(orderingUI.selectButtons[i].type);

                if (playerOrder.Count >= 3)
                {
                    orderingUI.SubmitAvailable();
                }
            }
            else
            {
                for (int j = 0; j < playerOrder.Count; j++)
                {
                    if (orderingUI.selectButtons[i].type == playerOrder[j])
                    {
                        RemovePlayerOrder(j);
                        break;
                    }
                }
            }

            foreach (var button in orderingUI.selectButtons)
            {
                for (int j = 0; j < playerOrder.Count; j++)
                {
                    if (button.type == playerOrder[j])
                    {
                        button.order.sprite = orderingUI.orderSprites[j];
                    }
                }
            }
        }

        private void CancelOrder()
        {
            orderingUI.ResetUI();
        }

        private void SubmitOrder()
        {
            EnableIngameUI();
            NextState();
        }

        private void EnableOrderingUI()
        {
            readyUI.gameObject.SetActive(false);
            orderingUI.gameObject.SetActive(true);
            ingameUI.gameObject.SetActive(false);
            successUI.gameObject.SetActive(false);
            failUI.gameObject.SetActive(false);
        }

        private void EnableIngameUI()
        {
            readyUI.gameObject.SetActive(false);
            orderingUI.gameObject.SetActive(false);
            ingameUI.gameObject.SetActive(true);
            successUI.gameObject.SetActive(false);
            failUI.gameObject.SetActive(false);
        }

        public void PlayerPhaseStart(int order)
        {
            Debug.Log("Player Phase Start");
            EnableIngameUI();
            var player = SpawnPlayer(playerOrder[order]);
            CreateAllClones();
            SetTimer(timeLimit);
            CreateMovementWaitStream(playerInstances[0].GetComponent<PlayerController>(), order);
        }

        public void PlayerPhaseEnd(int order)
        {
            Debug.Log("Player Phase End");
            _recorder.RecordStop();
            if (order != 2)
            {
                DestroyAllPlayer();
                CreateCloneData(playerOrder[order], _recorder.GetActionLists());
            }
            NextState();
        }

        public void SetTimer(double time)
        {
            currentTime = time;
            timerText.SetText(currentTime.ToString("N2"));
        }

        public void CreateReadyPhaseStream()
        {
            this.UpdateAsObservable().Where(_ => Input.anyKey).First().Subscribe(_ => ChangeState(Phase.Ordering)).AddTo(gameObject);
        }

        public void CreateMovementWaitStream(PlayerController controller, int order)
        {
            Observable.Amb(GlobalInputBinder.CreateGetAxisStreamOptimize("Horizontal").Select(x => Math.Abs(x) != 0),
                GlobalInputBinder.CreateGetAxisStreamOptimize("Vertical").Select(x => Math.Abs(x) != 0),
                GlobalInputBinder.CreateGetKeyDownStream(KeyCode.Space)).First().Subscribe(_ =>
             {
                 controller.CreateMovementStream();
                 foreach (var clone in _currentClones)
                 {
                     clone.CreateMovementStream();
                 }
                 _recorder.RecordStart(controller.GetComponent<Player.Player>());
                 StartTimer(order);
             }).AddTo(gameObject);
        }

        public void StartTimer(int order)
        {

            Observable.Interval(TimeSpan.FromSeconds(timeStep)).TakeWhile(_ => currentTime > 0).Subscribe(_ =>
            {
                SetTimer(currentTime - timeStep);
            }, null, () =>
            {
                PlayerPhaseEnd(order);
            });
        }

        public void CreateCloneData(PlayerType type, List<List<Action>> actions)
        {
            _cloneData.Add(new PlayerCloneData(type, actions));
        }

        public PlayerClone CreateClone(PlayerCloneData data)
        {
            var instance = Instantiate(clonePrefabs[(int)data.type - 1], origin);
            instance.transform.position = spawnPoint.transform.position;
            instance.SetActions(data.actions);
            playerInstances.Add(instance.GetComponent<Player.Player>());
            _currentClones.Add(instance);
            return instance;
        }

        public void CreateAllClones()
        {
            foreach (var data in _cloneData)
            {
                CreateClone(data);
            }
        }

        public void DestroyAllPlayer()
        {
            foreach (var player in playerInstances)
            {
                Destroy(player.gameObject);
            }

            playerInstances = new List<Player.Player>();
            _currentClones = new List<PlayerClone>();
        }

        public void AddPlayerOrder(PlayerType type)
        {
            playerOrder.Add(type);
        }

        public void RemovePlayerOrder(int index)
        {
            playerOrder.RemoveAt(index);
        }

        public void ChangeState(Phase state)
        {
            _stateSubject.OnNext(state);
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
    }
}
