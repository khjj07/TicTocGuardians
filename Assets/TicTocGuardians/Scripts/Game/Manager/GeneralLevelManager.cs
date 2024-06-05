using System;
using System.Collections.Generic;
using System.Linq;
using Default.Scripts.Util;
using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Game.LevelObjects;
using TicTocGuardians.Scripts.Game.Player;
using TicTocGuardians.Scripts.Game.UI;
using UniRx;
using UniRx.Triggers;

using UnityEngine;

using UnityEngine.Serialization;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;
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
            Play,
            Success,
            Fail
        }

        [SerializeField] private PlayerClone[] clonePrefabs = new PlayerClone[3];

        [Header("UI")]
        [SerializeField] private OrderingUI orderingUI;


        [Header("ป๓ลย")]
        [SerializeField]
        private Phase currentState;

        private int _currentPlayerIndex = 0;

        private Subject<Phase> _phaseSubject = new Subject<Phase>();
        private List<PlayerType> _playerOrder = new List<PlayerType>();
        private List<PlayerCloneData> _cloneData = new List<PlayerCloneData>();
        private List<PlayerClone> _currentClones = new List<PlayerClone>();
        private PlayerRecorder _recorder;

        public void Awake()
        {
            _recorder = GetComponent<PlayerRecorder>();
        }

        public override void Start()
        {
            base.Start();
            GameManager.Instance.ActiveLevel(this);
            InitializeOrderingUI();
            InitializeSuccessUI();
            InitializeFailUI();
            InitializePhaseSubject();
            ChangeState(Phase.Ready);
        }

        private void InitializePhaseSubject()
        {
            _phaseSubject.Where(x => x == Phase.Ready).Subscribe(_ =>
            {
                EnableReadyUI();
                CreateReadyPhaseStream();
            });

            _phaseSubject.Where(x => x == Phase.Ordering).Subscribe(_ =>
            {
                EnableOrderingUI();
            });

            _phaseSubject.Where(x => x == Phase.Play).Subscribe(_ =>
            {
                var player = SpawnPlayer(_playerOrder[_currentPlayerIndex]);
                PlayPhaseStart(player);
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

        private void InitializeOrderingUI()
        {
            orderingUI.submitButton.onClick.AddListener(SubmitOrder);
            orderingUI.cancelButton.onClick.AddListener(CancelOrder);
            orderingUI.selectButtons[0].AddListener(() => { CharacterSelectButtonOnClick(0); });
            orderingUI.selectButtons[1].AddListener(() => { CharacterSelectButtonOnClick(1); });
            orderingUI.selectButtons[2].AddListener(() => { CharacterSelectButtonOnClick(2); });
        }

        public override void EnableReadyUI()
        {
            base.EnableReadyUI();
            orderingUI.gameObject.SetActive(false);
        }

        public override void EnableIngameUI()
        {
            base.EnableIngameUI();
            readyUI.gameObject.SetActive(false);
            orderingUI.gameObject.SetActive(false);
        }

        public override void EnableFailUI()
        {
            base.EnableFailUI();
            orderingUI.gameObject.SetActive(false);
        }

        public override void EnableSuccessUI()
        {
            base.EnableSuccessUI();
            orderingUI.gameObject.SetActive(false);
        }

        private void CharacterSelectButtonOnClick(int i)
        {
            orderingUI.SubmitUnavilable();
            if (!orderingUI.selectButtons[i].IsSelected())
            {
                AddPlayerOrder(orderingUI.selectButtons[i].type);

                if (_playerOrder.Count >= 3)
                {
                    orderingUI.SubmitAvailable();
                }
            }
            else
            {
                for (int j = 0; j < _playerOrder.Count; j++)
                {
                    if (orderingUI.selectButtons[i].type == _playerOrder[j])
                    {
                        RemovePlayerOrder(j);
                        break;
                    }
                }
            }

            foreach (var button in orderingUI.selectButtons)
            {
                for (int j = 0; j < _playerOrder.Count; j++)
                {
                    if (button.type == _playerOrder[j])
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

        public override void PlayPhaseStart(Player.Player player)
        {
            base.PlayPhaseStart(player);
            CreateAllClones();
        }

        public override void PlayPhaseEnd()
        {
            _recorder.RecordStop();
            if (_currentPlayerIndex<_playerOrder.Count-1)
            {
                DestroyAllPlayer();
                CreateCloneData(_playerOrder[_currentPlayerIndex], _recorder.GetActionLists());
                _currentPlayerIndex++;
                ChangeState(Phase.Play);
            }
            else
            {
                if (repairingDimensions.Count == _playerOrder.Count)
                {
                    ChangeState(Phase.Success);
                }
                else
                {
                    ChangeState(Phase.Fail);
                }
            }
        }

        private void CreateReadyPhaseStream()
        {
            this.UpdateAsObservable().Where(_ => Input.anyKey).First().Subscribe(_ => ChangeState(Phase.Ordering)).AddTo(gameObject);
        }

        public override void ActiveLevel(PlayerController controller)
        {
            base.ActiveLevel(controller);
            foreach (var clone in _currentClones)
            {
                clone.CreateMovementStream();
            }
            _recorder.RecordStart(controller.GetComponent<Player.Player>());
            StartTimer();
        }

        public void CreateCloneData(PlayerType type, List<List<Action>> actions)
        {
            _cloneData.Add(new PlayerCloneData(type, actions));
        }

        public PlayerClone CreateClone(PlayerCloneData data)
        {
            var instance = Instantiate(clonePrefabs[(int)data.type - 1], origin);
            instance.transform.position = SpawnPointLevelObject.Instance.transform.position;
            instance.SetActions(data.actions);
            instance.GetComponent<Player.Player>().CreateDimensionCheckStream();
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
            _playerOrder.Add(type);
        }

        public void RemovePlayerOrder(int index)
        {
            _playerOrder.RemoveAt(index);
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
    }
}
