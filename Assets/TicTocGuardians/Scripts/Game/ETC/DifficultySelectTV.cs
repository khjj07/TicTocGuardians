using System.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.ETC
{
    public class DifficultySelectTV : MonoBehaviour
    {
        public enum State
        {
            Select,
            Normal,
            Hard,
            Glitch
        }

        [SerializeField] private Transform model;
        [SerializeField] private Transform endPoint;
        [SerializeField] private MeshRenderer screen;

        [SerializeField] private Material selectScreen;
        [SerializeField] private Material normalScreen;
        [SerializeField] private Material hardScreen;
        [SerializeField] private Material glitchScreen;

        private readonly Subject<State> _stateSubject = new();

        private void Start()
        {
            model.DOLocalMoveY(endPoint.localPosition.y, 1.0f);

            _stateSubject.Where(x => x == State.Select)
                .Subscribe(_ => screen.sharedMaterial = selectScreen);
            _stateSubject.Where(x => x == State.Normal)
                .Subscribe(_ => screen.sharedMaterial = normalScreen);
            _stateSubject.Where(x => x == State.Hard)
                .Subscribe(_ => screen.sharedMaterial = hardScreen);
            _stateSubject.Where(x => x == State.Glitch)
                .Subscribe(_ => screen.sharedMaterial = glitchScreen);
        }

        public void ChangeState(State state)
        {
            _stateSubject.OnNext(state);
        }

        public async Task ChangeStateWithGlitch(State state)
        {
            _stateSubject.OnNext(State.Glitch);
            await Task.Delay(500);
            _stateSubject.OnNext(state);
        }
    }
}