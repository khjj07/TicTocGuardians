using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;

namespace TicTocGuardians.Scripts.Game.ETC
{
    [RequireComponent(typeof(BoxCollider))]
    public class ReactableArea : MonoBehaviour
    {
        public UnityEvent stepInEvent;
        public UnityEvent stepOutEvent;

        public void Start()
        {
            GetComponent<BoxCollider>().OnTriggerEnterAsObservable()
                .Subscribe(_ => stepInEvent.Invoke());

            GetComponent<BoxCollider>().OnTriggerExitAsObservable()
                .Subscribe(_ => stepOutEvent.Invoke());
        }
    }
}