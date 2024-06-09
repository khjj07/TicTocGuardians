using DG.Tweening;
using TicTocGuardians.Scripts.Interface;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.ETC
{
    public class BeaverBox : MonoBehaviour, IMovable, IPushable
    {
        public BeaverBoxPushPoint[] pushPoints;

        public Vector3 contactDirection;
        public float offset =2.0f;

        private Rigidbody _rigidbody;

        public void Awake()
        {
            _rigidbody=GetComponent<Rigidbody>();
        }
        public void Start()
        {
            pushPoints = GetComponentsInChildren<BeaverBoxPushPoint>();
            foreach (var pushPoint in pushPoints)
            {
                pushPoint.OnTriggerEnterAsObservable().Where(x => x.CompareTag("Player")).Subscribe(_ =>
                {
                    contactDirection = pushPoint.direction.normalized;
                });
                pushPoint.OnTriggerExitAsObservable().Where(x=>x.CompareTag("Player")).Subscribe(_ =>
                {
                    contactDirection = Vector3.zero;
                });
            }
        }

        public void Update()
        {
            if (!Physics.Raycast(transform.position, -contactDirection, offset))
            {

                Debug.DrawRay(transform.position, -contactDirection * offset);
            }
            else
            {

                Debug.DrawRay(transform.position , -contactDirection * offset, Color.red);
            }

        }

        public void MoveX(float speed)
        {
            transform.Translate(Vector3.right* speed*Time.deltaTime,Space.World);
        }

        public void MoveZ(float speed)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.World);
        }

        public void OnPush()
        {
            if (!Physics.Raycast(transform.position, -contactDirection, offset))
            {
                transform.DOMove(-contactDirection * offset, 0.5f).SetRelative(true);
            }
        }
    }
}