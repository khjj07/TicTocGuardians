using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;

namespace TicTocGuardians.Scripts.Game.LevelObjects
{
    public class ScaffoldingModelLevelObject : StaticModelLevelObject
    {
        public UnityEvent stepInEvent;
        public UnityEvent stepOutEvent;
        public void Start()
        {
            GetComponentInChildren<MeshCollider>().OnCollisionStayAsObservable()
                .Where(collision => collision.contacts[0].normal.y < -0.7)
                .Subscribe(_=> stepInEvent.Invoke());

            GetComponentInChildren<MeshCollider>().OnCollisionExitAsObservable()
                .Subscribe(_ => stepOutEvent.Invoke());
        }
    }
}
