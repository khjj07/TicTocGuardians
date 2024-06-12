using System;
using DG.Tweening;
using TicTocGuardians.Scripts.Game.LevelObjects;
using TicTocGuardians.Scripts.Game.Manager;
using TicTocGuardians.Scripts.Interface;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace TicTocGuardians.Scripts.Game.Player
{
    public enum PlayerType
    {
        None,
        Beaver,
        Cat,
        Rabbit
    }

    [Serializable]
    public class Action
    {
        public enum State
        {
            MoveX,
            MoveZ,
            Push,
            PushReady,
            Jump,
            None
        }

        public Action(State state, object data = null)
        {
            this.state = state;
            this.data = data;
        }

        public State state;
        public object data;
    }

    [RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody))]
    public class Player : MonoBehaviour, IMovable
    {
        public enum AnimationState
        {
            Idle,
            Run,
            Jump,
            Falling,
            Landing,
            Push,
            PushReady,
            Repair
        }

        [SerializeField]
        protected float jumpForce;
        [SerializeField]
        protected AnimationState animationState;

        [SerializeField]
        protected float speedMax;
        [SerializeField]
        protected float acceleration;
        [SerializeField]
        protected float drag;

        [SerializeField] protected Transform footOrigin;
        [SerializeField] protected float checkGroundDistance;

        [SerializeField] protected SpriteRenderer repairImage;

        protected Vector3 _direction;
        protected CapsuleCollider _capsuleCollider;
        protected Rigidbody _rigidbody;
        protected Animator animator;
        protected Subject<Action> actionSubject;
        public DimensionLevelObject repairTarget;

        protected static readonly int Run = Animator.StringToHash("Run");
        protected static readonly int Jump1 = Animator.StringToHash("Jump");
        protected static readonly int Idle = Animator.StringToHash("Idle");
        protected static readonly int Push = Animator.StringToHash("Push");
        protected static readonly int PushReady = Animator.StringToHash("PushReady");
        protected static readonly int Repair = Animator.StringToHash("Repair");
        protected static readonly int Landing = Animator.StringToHash("Landing");
        protected static readonly int Falling = Animator.StringToHash("Falling");

        protected bool _movable = true;
        protected bool _isFalling = false;

        protected bool _isOperating = false;
        public float dimensionCheckDistance;

        public virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            animator = GetComponentInChildren<Animator>();
            _capsuleCollider = GetComponent<CapsuleCollider>();
            actionSubject = new Subject<Action>();
        }

        public virtual void Start()
        {
            CreateDefaultStream();
        }

        public virtual void Update()
        {
            MoveAnimation();

            if (_rigidbody.velocity.magnitude > 0.1)
            {
                var tmp = _rigidbody.velocity;
                tmp.y = 0;
                SetDirection(tmp.normalized);
            }

            if (repairTarget!=null)
            {
                Repairing();
            }
            else
            {
                UnRepairing();
            }

            Animate();
        }

        public virtual void CreateDefaultStream()
        {
            var baseStream = actionSubject.Where(_ => _ != null);
            var moveXStream = baseStream.Where(action => action.state == Action.State.MoveX);
            moveXStream.Where(_=> _movable).Subscribe(action =>
            {
                if (!_isOperating && IsContactGround())
                {
                    SetAnimationState(AnimationState.Run);
                }
                MoveX((float)action.data);
            }).AddTo(gameObject);

            var moveZStream = baseStream.Where(action => action.state == Action.State.MoveZ);
            moveZStream.Where(_ => _movable).Subscribe(action =>
            {
                if (!_isOperating && IsContactGround())
                {
                    SetAnimationState(AnimationState.Run);
                }
                MoveZ((float)action.data);
            }).AddTo(gameObject);

            var jumpStream = baseStream.Where(_ => IsContactGround())
                .Where(action => action.state == Action.State.Jump);
            jumpStream.Subscribe(action => Jump()).AddTo(gameObject);
            jumpStream.Subscribe(action =>
            {
                _isOperating = true;
                SetAnimationState(AnimationState.Jump);
                Observable.Timer(TimeSpan.FromMilliseconds(500))
                    .Subscribe(_ => _isOperating = false);
            }).AddTo(gameObject);



            var isFallingStream = this.UpdateAsObservable().Where(_ => !IsContactGround());

            isFallingStream.Subscribe(_ => _isFalling = true);
            isFallingStream.SelectMany(this.UpdateAsObservable()
                    .Where(_ => IsContactGround()))
                    .First().Repeat()

                .Subscribe(_ =>
                {
                    SetAnimationState(AnimationState.Landing);
                    Observable.Timer(TimeSpan.FromMilliseconds(500))
                        .Subscribe(_ => _isFalling = false);
                }).AddTo(gameObject);

            var dimensionEnterStream = this.OnTriggerEnterAsObservable()
                .Where(x => x.CompareTag("Dimension"))
                .Select(x => x.GetComponent<DimensionLevelObject>());

            var dimensionExitStream = this.OnTriggerExitAsObservable()
                .Where(x => x.CompareTag("Dimension"))
                .Select(x => x.GetComponent<DimensionLevelObject>());

            dimensionEnterStream.Subscribe(x =>
            {
                repairTarget = x;
            }).AddTo(gameObject);

            dimensionExitStream.Subscribe(x =>
            {
                repairTarget = null;
            }).AddTo(gameObject);
        }

        public void MoveAnimation()
        {
            if (IsContactGround())
            {
                if (!_isOperating)
                {
                    if (_rigidbody.velocity.magnitude < 1)
                    {
                        SetAnimationState(AnimationState.Idle);
                    }
                }
            }
            else
            {
                if (!_isOperating)
                {
                    SetAnimationState(AnimationState.Falling);
                }
            }
        }

        public void Repairing()
        {
            repairImage.gameObject.SetActive(true);
        }

        public void UnRepairing()
        {
            repairImage.gameObject.SetActive(false);
        }


        public bool IsContactGround()
        {
            return Physics.Raycast(footOrigin.position, Vector3.down, checkGroundDistance);
        }

        public void Act(Action action)
        {
            actionSubject.OnNext(action);
        }

        public void MoveTo(Vector3 position)
        {
            transform.DOMove(position, 0.2f).Rewind();
        }

        public void MoveX(float direction)
        {
            AccelerateX(direction);
        }

        public void MoveZ(float direction)
        {
            AccelerateZ(direction);
        }

        public void SetDirection(Vector3 direction)
        {
            _direction = direction;
            if (direction.magnitude > 0)
            {
                animator.transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        public void AccelerateX(float direction)
        {
            if (MathF.Abs(_rigidbody.velocity.x) < speedMax)
            {
                _rigidbody.AddForce(Vector3.right * acceleration * direction, ForceMode.VelocityChange);
            }
        }

        public void AccelerateZ(float direction)
        {
            if (MathF.Abs(_rigidbody.velocity.z) < speedMax)
            {
                _rigidbody.AddForce(Vector3.forward * acceleration * direction, ForceMode.VelocityChange);
            }
        }

        public void Jump()
        {
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        public void SetAnimationState(AnimationState state)
        {
            animationState = state;
        }

        public void Animate()
        {
            animator.SetBool(Idle, false);
            animator.SetBool(Run, false);
            animator.SetBool(Jump1, false);
            animator.SetBool(Push, false);
            animator.SetBool(PushReady, false);
            animator.SetBool(Repair, false);
            animator.SetBool(Landing, false);
            animator.SetBool(Falling, false);
            switch (animationState)
            {
                case AnimationState.Idle:
                    animator.SetBool(Idle, true);
                    break;
                case AnimationState.Run:
                    animator.SetBool(Run, true);
                    break;
                case AnimationState.Jump:
                    animator.SetBool(Jump1, true);
                    break;
                case AnimationState.Push:
                    animator.SetBool(Push, true);
                    break;
                case AnimationState.Repair:
                    animator.SetBool(Repair, true);
                    break;
                case AnimationState.Landing:
                    animator.SetBool(Landing, true);
                    break;
                case AnimationState.Falling:
                    animator.SetBool(Falling, true);
                    break;
                case AnimationState.PushReady:
                    animator.SetBool(PushReady, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}
