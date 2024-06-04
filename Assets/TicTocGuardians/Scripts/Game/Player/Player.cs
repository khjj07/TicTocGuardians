using System;
using TicTocGuardians.Scripts.Game.LevelObjects;
using TicTocGuardians.Scripts.Game.Manager;
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
            Jump,
            None
        }
        public Action(State state, float direction = 0)
        {
            this.state = state;
            this.direction = direction;
        }

        public State state;
        public float direction;
    }

    [RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody))]
    public class Player : MonoBehaviour
    {
        public enum AnimationState
        {
            Idle,
            Run,
            Jump,
            Falling,
            Landing,
            Push,
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
        protected static readonly int Repair = Animator.StringToHash("Repair");
        protected static readonly int Landing = Animator.StringToHash("Landing");

        protected bool _isJumping = false;
        protected bool _isFalling = false;
        protected static readonly int Falling = Animator.StringToHash("Falling");

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

            Animate();
            Debug.DrawRay(footOrigin.position, Vector3.down * checkGroundDistance, Color.red);
        }

        public void CreateDefaultStream()
        {
            var baseStream = actionSubject.Where(_ => _ != null);
            var moveXStream = baseStream.Where(action => action.state == Action.State.MoveX);
            moveXStream.Subscribe(action => MoveX(action.direction)).AddTo(gameObject);

            var moveZStream = baseStream.Where(action => action.state == Action.State.MoveZ);
            moveZStream.Subscribe(action => MoveZ(action.direction)).AddTo(gameObject);

            var jumpStream = baseStream.Where(_ => IsContactGround())
                .Where(action => action.state == Action.State.Jump);
            jumpStream.Subscribe(action => Jump()).AddTo(gameObject);
            jumpStream.Subscribe(action =>
            {
                _isJumping = true;
                SetAnimationState(AnimationState.Jump);
            }).AddTo(gameObject);
        }

        public void MoveAnimation()
        {
            if (IsContactGround() && !_isFalling && !_isJumping)
            {
                SetAnimationState(AnimationState.Idle);
                if (_rigidbody.velocity.magnitude >= 1)
                {
                    SetAnimationState(AnimationState.Run);
                }
            }
            else
            {
                if (!_isJumping)
                {
                    _isFalling = true;
                    SetAnimationState(AnimationState.Falling);
                }

                Observable.Timer(TimeSpan.FromSeconds(0.1f))
                    .Subscribe(_ =>
                    {
                        this.UpdateAsObservable().Where(_ => IsContactGround()).Take(1).Subscribe(_ =>
                        {
                            SetAnimationState(AnimationState.Landing);
                            Observable.Timer(TimeSpan.FromSeconds(0.2f)).Subscribe(_ =>
                            {
                                _isFalling = false;
                                _isJumping = false;
                            }).AddTo(gameObject);
                        }).AddTo(gameObject);
                    }).AddTo(gameObject);


            }
        }

        public bool IsContactGround()
        {
            return Physics.Raycast(footOrigin.position, Vector3.down, checkGroundDistance);
        }

        public void Act(Action action)
        {
            actionSubject.OnNext(action);
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
            if (direction.magnitude > 0)
            {
                transform.rotation = Quaternion.LookRotation(direction);
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}
