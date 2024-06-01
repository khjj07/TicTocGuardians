using System;
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
        private float jumpForce;
        [SerializeField]
        private AnimationState animationState;

        [SerializeField]
        private float speedMax;
        [SerializeField]
        private float acceleration;
        [SerializeField]
        private float drag;

        [SerializeField] private Transform footOrigin;
        [SerializeField] private float checkGroundDistance;

        private Vector3 _direction;
        private Rigidbody _rigidbody;
        private Animator _animator;
        private Subject<Action> _actionSubject = new Subject<Action>();

        private static readonly int Run = Animator.StringToHash("Run");
        private static readonly int Jump1 = Animator.StringToHash("Jump");
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int Push = Animator.StringToHash("Push");
        private static readonly int Repair = Animator.StringToHash("Repair");
        private static readonly int Landing = Animator.StringToHash("Landing");

        private bool _isJumping = false;
        private bool _isFalling = false;
        private static readonly int Falling = Animator.StringToHash("Falling");

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponentInChildren<Animator>();
        }

        private void CreateDefaultStream()
        {
            var baseStream = _actionSubject.Where(_ => _ != null);
            var moveXStream = baseStream.Where(action => action.state == Action.State.MoveX);
            moveXStream.Subscribe(action => MoveX(action.direction)).AddTo(gameObject);

            var moveZStream = baseStream.Where(action => action.state == Action.State.MoveZ);
            moveZStream.Subscribe(action => MoveZ(action.direction)).AddTo(gameObject);

            var jumpStream = baseStream.Where(_ => IsContactGround()).Where(action => action.state == Action.State.Jump);
            jumpStream.Subscribe(action => Jump()).AddTo(gameObject);
            jumpStream.Subscribe(action =>
            {
                _isJumping = true;
                SetAnimationState(AnimationState.Jump);
            }).AddTo(gameObject);
        }

        public virtual void Start()
        {
            CreateDefaultStream();
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

        private bool IsContactGround()
        {
            return Physics.Raycast(footOrigin.position, Vector3.down, checkGroundDistance);
        }

        public void Act(Action action)
        {
            _actionSubject.OnNext(action);
        }

        private void MoveX(float direction)
        {
            AccelerateX(direction);
        }

        private void MoveZ(float direction)
        {
            AccelerateZ(direction);
        }

        private void SetDirection(Vector3 direction)
        {
            if (direction.magnitude > 0)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }


        private void AccelerateX(float direction)
        {
            if (MathF.Abs(_rigidbody.velocity.x) < speedMax)
            {
                _rigidbody.AddForce(Vector3.right * acceleration * direction, ForceMode.VelocityChange);
            }
        }

        private void AccelerateZ(float direction)
        {
            if (MathF.Abs(_rigidbody.velocity.z) < speedMax)
            {
                _rigidbody.AddForce(Vector3.forward * acceleration * direction, ForceMode.VelocityChange);
            }
        }

        private void Jump()
        {
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        }

        public void SetAnimationState(AnimationState state)
        {
            animationState = state;
        }

        private void Animate()
        {
            _animator.SetBool(Idle, false);
            _animator.SetBool(Run, false);
            _animator.SetBool(Jump1, false);
            _animator.SetBool(Push, false);
            _animator.SetBool(Repair, false);
            _animator.SetBool(Landing, false);
            _animator.SetBool(Falling, false);
            switch (animationState)
            {
                case AnimationState.Idle:
                    _animator.SetBool(Idle, true);
                    break;
                case AnimationState.Run:
                    _animator.SetBool(Run, true);
                    break;
                case AnimationState.Jump:
                    _animator.SetBool(Jump1, true);
                    break;
                case AnimationState.Push:
                    _animator.SetBool(Push, true);
                    break;
                case AnimationState.Repair:
                    _animator.SetBool(Repair, true);
                    break;
                case AnimationState.Landing:
                    _animator.SetBool(Landing, true);
                    break;
                case AnimationState.Falling:
                    _animator.SetBool(Falling, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


    }
}
