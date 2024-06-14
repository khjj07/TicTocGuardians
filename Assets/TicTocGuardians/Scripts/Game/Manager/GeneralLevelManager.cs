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
            Play1,
            Play2,
            Play3,
            Success,
            Fail
        }

        [SerializeField] private PlayerClone[] clonePrefabs = new PlayerClone[3];

        [Header("UI")]
        [SerializeField] private OrderingUI orderingUI;


        [Header("ป๓ลย")]
        [SerializeField]
        private Phase currentState;

        private int _currentPlayPhaseIndex = 0;

        private Subject<Phase> _phaseSubject = new Subject<Phase>();
        public List<PlayerType> _playerOrder = new List<PlayerType>();
        private PlayerCloneData[] _cloneData = new PlayerCloneData[3];
        private List<PlayerClone> _currentClones = new List<PlayerClone>();
        private PlayerRecorder _recorder;
        
        public void Awake()
        {
            _recorder = GetComponent<PlayerRecorder>();
        }

        public override void Start()
        {
            base.Start();
            InitializeOrderingUI();
            InitializePhaseSubject();
            ChangeState(Phase.Ready);
        }

        private void InitializePhaseSubject()
        {
            _phaseSubject.Where(x => x == Phase.Ready).Subscribe(_ =>
            {
                OnReadyPhaseActive();
            });

            _phaseSubject.Where(x => x == Phase.Ordering).Subscribe(_ =>
            {
                OnOrderingPhaseActive();
            });

            _phaseSubject.Where(x => x == Phase.Play1).Subscribe(_ =>
            {
                OnPlay1PhaseActive();
            });

            _phaseSubject.Where(x => x == Phase.Play2).Subscribe(_ =>
            {
                OnPlay2PhaseActive();
            });

            _phaseSubject.Where(x => x == Phase.Play3).Subscribe(_ =>
            {
                OnPlay3PhaseActive();
            });

            _phaseSubject.Where(x => x == Phase.Success).Subscribe(_ =>
            {
                OnSuccessPhaseActive();
            });

            _phaseSubject.Where(x => x == Phase.Fail).Subscribe(_ =>
            {
                OnFailPhaseActive();
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

        public override void OnReadyPhaseActive()
        {
            base.OnReadyPhaseActive();
            orderingUI.gameObject.SetActive(false);
            this.UpdateAsObservable().Where(_ => Input.anyKey).First()
                .Subscribe(_ => ChangeState(Phase.Ordering)).AddTo(gameObject);
        }

        public override void OnPlayPhaseActive()
        {
            base.OnPlayPhaseActive();
            readyUI.gameObject.SetActive(false);
            orderingUI.gameObject.SetActive(false);
        }

        public void OnPlay1PhaseActive()
        {
            OnPlayPhaseActive();
            _currentPlayPhaseIndex = 0;
            var player = SpawnPlayer(_playerOrder[_currentPlayPhaseIndex]);
            PlayPhaseStart(playerController);
        }

        public void OnPlay2PhaseActive()
        {
            OnPlayPhaseActive();
            _currentPlayPhaseIndex = 1;
            var player = SpawnPlayer(_playerOrder[_currentPlayPhaseIndex]);
            PlayPhaseStart(playerController);
        }

        public void OnPlay3PhaseActive()
        {
            OnPlayPhaseActive();
            _currentPlayPhaseIndex = 2;
            SpawnPlayer(_playerOrder[_currentPlayPhaseIndex]);
            PlayPhaseStart(playerController);
            foreach (var instance in playerInstances)
            {
                CreateDimensionCheckStream(instance);
            }
        }
        public override void OnFailPhaseActive()
        {
            base.OnFailPhaseActive();
            orderingUI.gameObject.SetActive(false);
        }

        public override void OnSuccessPhaseActive()
        {
            base.OnSuccessPhaseActive();
            orderingUI.gameObject.SetActive(false);
        }

        private void CharacterSelectButtonOnClick(int i)
        {
            orderingUI.SubmitUnavilable();
            if (!orderingUI.selectButtons[i].IsSelected())
            {
                AddPlayerOrder(orderingUI.selectButtons[i].type);
                orderingUI.selectButtons[i].Select();
                if (_playerOrder.Count >= 3)
                {
                    orderingUI.SubmitAvailable();
                }
            }
            else
            {
                orderingUI.selectButtons[i].UnSelect();
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
            _playerOrder.Clear();
            orderingUI.ResetUI();
        }

        private void SubmitOrder()
        {
            NextState();
        }

        private void OnOrderingPhaseActive()
        {
            readyUI.gameObject.SetActive(false);
            orderingUI.gameObject.SetActive(true);
            ingameUI.gameObject.SetActive(false);
            successUI.gameObject.SetActive(false);
            failUI.gameObject.SetActive(false);
            _playerOrder.Clear();
            CancelOrder();
        }

        public override void PlayPhaseStart(PlayerController player)
        {
            base.PlayPhaseStart(playerController);
            CreateAllClones();
        }

        public void PlayPhaseForceEnd()
        {
            isPlaying = false;
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

            ResetRepairing();
            _recorder.RecordStop();
            DestroyAllPlayer();
            Time.timeScale = 1.0f;
        }

        public override void PlayPhaseEnd()
        {
            base.PlayPhaseEnd();
            Time.timeScale = 1.0f;
            if (_currentPlayPhaseIndex < _playerOrder.Count - 1)
            {
                DestroyAllPlayer();
                CreateCloneData(_playerOrder[_currentPlayPhaseIndex], _recorder.GetActionLists());
                ResetRepairing();
                NextState();
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
            _cloneData[_currentPlayPhaseIndex] = new PlayerCloneData(type, actions);
        }


        public override void InitializeIngameUI()
        {
            ingameUI.skipButton.onClick.AddListener(() =>
            {
                Time.timeScale = 4.0f;
                playerController.DisposeAllStream();
                playerController.GetComponent<Player.Player>().Act(new Action(Action.State.Wait));
                _recorder.Wait();
            });

            ingameUI.changeOrderButton.onClick.AddListener(() =>
            {
                PlayPhaseForceEnd();
                ChangeState(Phase.Ordering);
            });

            ingameUI.replayButton.onClick.AddListener(() =>
            {
                if (isPlaying)
                {
                    PlayPhaseForceEnd();
                    ChangeState(currentState);
                }
                else
                {
                    PlayPhaseForceEnd();
                    PreviousState();
                }
            });
            ingameUI.UpdateAsObservable().Select(_ => _playerOrder)
                .Subscribe(order =>
                {
                    for (int i = 0; i < order.Count; i++)
                    {
                        if (order[i] != PlayerType.None)
                        {
                            if (i == _currentPlayPhaseIndex)
                            {
                                ingameUI.portraits[i].sprite = ingameUI.highLightPortraitSprites[(int)order[i] - 1];
                            }
                            else
                            {
                                ingameUI.portraits[i].sprite = ingameUI.defaultPortraitSprites[(int)order[i] - 1];
                            }
                        }
                    }
                }).AddTo(gameObject);
        }

        public PlayerClone CreateClone(PlayerCloneData data)
        {
            var instance = Instantiate(clonePrefabs[(int)data.type - 1], origin);
            instance.transform.position = SpawnPointLevelObject.Instance.transform.position;
            instance.SetActions(data.actions);
            playerInstances.Add(instance.GetComponent<Player.Player>());
            _currentClones.Add(instance);
            return instance;
        }

        public void CreateAllClones()
        {
            for (int i = 0; i < _currentPlayPhaseIndex; i++)
            {
                CreateClone(_cloneData[i]);
            }
        }

        public void DestroyAllPlayer()
        {
            foreach (var player in playerInstances)
            {
                Destroy(player.gameObject);
            }

            playerInstances.Clear();
            playerController = null;
            _currentClones.Clear();
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
