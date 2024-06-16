using System;
using DG.Tweening;
using TicTocGuardians.Scripts.Game.LevelObjects;
using TicTocGuardians.Scripts.Game.Manager;
using TicTocGuardians.Scripts.Interface;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

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
            Wait,
            Repair,
            RepairRelease,
            Special,
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
            Wait,
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

        public bool _movable = true;
        public bool _isFalling = false;
        public bool _isRepairing = false;

        public bool _isOperating = false;
        public float dimensionCheckDistance;
        private bool _isWaiting;
        private static readonly int Wait = Animator.StringToHash("Wait");

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

        public void LateUpdate()
        {
            if (_rigidbody.velocity.magnitude > 0.1)
            {
                var tmp = _rigidbody.velocity;
                tmp.y = 0;
                SetDirection(tmp.normalized);
            }
        }

        public virtual void Update()
        {
            MoveAnimation();
            if (_isFalling || _isOperating)
            {
                _capsuleCollider.material.dynamicFriction = 0;
                _capsuleCollider.material.staticFriction = 0;
            }
            else
            {
                _capsuleCollider.material.dynamicFriction = 0.6f;
                _capsuleCollider.material.staticFriction = 0.6f;
            }
            if (repairTarget != null)
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
            moveXStream.Where(_ => _movable).Subscribe(action =>
            {
                if (!_isOperating && !_isFalling)
                {
                    SetAnimationState(AnimationState.Run);
                }
                MoveX((float)action.data);
            }).AddTo(gameObject);

            var moveZStream = baseStream.Where(action => action.state == Action.State.MoveZ);
            moveZStream.Where(_ => _movable).Subscribe(action =>
            {
                if (!_isOperating && !_isFalling)
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
                GlobalSoundManager.Instance.PlaySFX("SFX_JUMP_3");
                Observable.Timer(TimeSpan.FromMilliseconds(100))
                    .Subscribe(_ => _isOperating = false);
            }).AddTo(gameObject);

            baseStream.Where(action => action.state == Action.State.Repair)
                .Subscribe(_ => _isRepairing = true);

            var isFallingStream = this.UpdateAsObservable().Where(_ => !IsContactGround());

            isFallingStream.Subscribe(_ => _isFalling = true);
           this.LateUpdateAsObservable()
                    .Where(_ => _isFalling && IsContactGround())
                    .Subscribe(_ =>
                    {
                        SetAnimationState(AnimationState.Landing);
                        Observable.Timer(TimeSpan.FromMilliseconds(300))
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

            baseStream.Where(action => action.state == Action.State.Wait)
                .Subscribe(_ =>
                {
                    SetAnimationState(AnimationState.Wait);
                    _isWaiting = true;
                });
            
           
        }

        public void MoveAnimation()
        {
            if (IsContactGround())
            {
                if (_isRepairing)
                {
                    SetAnimationState(AnimationState.Repair);
                }
                else if (_isWaiting)
                {
                    SetAnimationState(AnimationState.Wait);
                }
                else if (!_isOperating && !_isFalling)
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
            Debug.DrawRay(footOrigin.position, Vector3.down * checkGroundDistance,Color.green);
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
            if (direction.magnitude > 0)
            {
                animator.transform.rotation = Quaternion.Slerp(animator.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5.0f);
            }
            _direction = direction;
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
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
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
            animator.SetBool(Wait, false);
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
                case AnimationState.Wait:
                    animator.SetBool(Wait, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}
