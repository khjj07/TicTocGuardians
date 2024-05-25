using UniRx;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game
{
    public class GameManager : MonoBehaviour
    {
        public enum State
        {
            None,
            Ordering,
            Player1,
            Player2,
            Player3,
            End
        }

        public enum PlayerType
        {
            None,
            Beaver,
            Cat,
            Rabbit
        }

        private PlayerType[] players = new PlayerType[3];
        private Subject<State> _stateSubject=new Subject<State>();

        private State _currentState;

        [SerializeField]
        private Canvas orderingUI;  
        [SerializeField]
        private Canvas ingameUI;


        public void SetPlayer(PlayerType type,int index)
        {
            players[index] = type;
        }

        public void Start()
        {
            _stateSubject.Where(x => x == State.Ordering).Subscribe(_ =>
            {
                orderingUI.gameObject.SetActive(true);
                ingameUI.gameObject.SetActive(false);
            });
            _stateSubject.Where(x => x == State.Player1).Subscribe(_ =>
            {
                orderingUI.gameObject.SetActive(false);
                ingameUI.gameObject.SetActive(true);
                //Spawn Player1 Controller
            });
            _stateSubject.Where(x => x == State.Player2).Subscribe(_ =>
            {
                orderingUI.gameObject.SetActive(false);
                ingameUI.gameObject.SetActive(true);
                //Spawn Player1 Clone
                //Spawn Player2 Controller
            });
            _stateSubject.Where(x => x == State.Player3).Subscribe(_ =>
            {
                orderingUI.gameObject.SetActive(false);
                ingameUI.gameObject.SetActive(true);
                //Spawn Player1 Clone
                //Spawn Player2 Clone
                //Spawn Player3 Controller
            });
        }

        // Update is called once per frame
        public void ChangeState(State state)
        {
            _stateSubject.OnNext(state);
            _currentState = state;
        }

        public void NextState()
        {
            ChangeState((State)(_currentState + 1));
        }

        public void PreviousState()
        {
            ChangeState((State)(_currentState - 1));
        }
    }
}
