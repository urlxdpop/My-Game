using UnityEngine;
using UnityEngine.AI;
using MyGame.Utils;
using System;

public class EnemyAI : MonoBehaviour {
    private const float MinSpeed = 0.1f;

    [SerializeField] private State startingState;
    [SerializeField] private float roamingDistanceMax = 7f;
    [SerializeField] private float roamingDistanceMin = 3f;
    [SerializeField] private float roamingTimerMax = 2f;

    [SerializeField] private bool isChasingEnemy;
    [SerializeField] private float chasingDistance = 4f;
    [SerializeField] private float chasingSpeedBoost = 1.5f;

    [SerializeField] private bool isAttackEnemy;
    [SerializeField] private float attackDistance = 1f;
    [SerializeField] private float attackRate = 2f;
    private float _nextAttackTime;

    private NavMeshAgent _navMeshAgent;
    private State _state;
    private float _roamingTime;
    private Vector3 _roamPosition;
    private Vector3 _startPosition;

    private float _roamingSpeed;
    private float _chasingSpeed;

    private float _nextCheckDirectionTime;
    private Vector3 _lastPosition;
    
    private const float CheckDirectionDuration = 0.1f;

    public event EventHandler OnEnemyAttack;

    private enum State {
        Idle,
        Roaming,
        Chasing,
        Attacking,
        Death
    }

    public bool IsRunning => _navMeshAgent.velocity != Vector3.zero;

    private void Awake() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
        _state = startingState;

        _roamingSpeed = _navMeshAgent.speed;
        _chasingSpeed = _navMeshAgent.speed * chasingSpeedBoost;
    }

    private void Start() {
        _startPosition = transform.position;
    }

    private void Update() {
        StateHandler();
        MovementDirectionHandler();
    }

    public float GetRoamingAnimationSpeed() {
        return _navMeshAgent.speed / _roamingSpeed;
    }

    public void SetDeathState() {
        _navMeshAgent.ResetPath();
        _state = State.Death;
    }

    private void StateHandler() {
        switch (_state) {
            case State.Roaming:
                _roamingTime -= Time.deltaTime;
                if (_roamingTime < 0) {
                    Roaming();
                    _roamingTime = roamingTimerMax;
                }
                ChangeCurrentState();
                break;
            case State.Chasing:
                ChasingTarget();
                ChangeCurrentState();
                break;
            case State.Attacking:
                AttackingTarget();
                ChangeCurrentState();
                break;
            case State.Death:
                break;
            default:
            case State.Idle:
                break;
        }
    }

    private void ChasingTarget() {
        _navMeshAgent.SetDestination(Player.Instance.transform.position);
    }

    private void AttackingTarget()
    {
        if (!(Time.time > _nextAttackTime)) return;
        OnEnemyAttack?.Invoke(this, EventArgs.Empty);

        _nextAttackTime = Time.time + attackRate;

    }

    private void ChangeCurrentState() {
        float distanceToTarget = Vector3.Distance(transform.position, Player.Instance.transform.position);
        State newState = State.Roaming;

        if (isChasingEnemy && distanceToTarget <= chasingDistance) {
            newState = State.Chasing;
        }

        if (isAttackEnemy && distanceToTarget <= attackDistance)
        {
            newState = Player.Instance.IsAlive() ? State.Attacking : State.Roaming;
        }

        if (newState == _state) return;
        switch (newState)
        {
            case State.Chasing:
            {
                _navMeshAgent.ResetPath();
                _navMeshAgent.speed = _chasingSpeed;
                break;
            }
            case State.Roaming:
            {
                _roamingTime = 0;
                _navMeshAgent.speed = _roamingSpeed;
                break;
            }
            case State.Attacking:
            {
                _navMeshAgent.ResetPath();
                break;
            }
            case State.Idle:
            case State.Death:
            {
                break;
            }
            default:
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        _state = newState;
    }

    private void MovementDirectionHandler()
    {
        if (!(Time.time > _nextCheckDirectionTime)) return;
        if (IsRunning) {
            ChangeFacingDirection(_lastPosition, transform.position);
        } else if (_state == State.Chasing) {
            ChangeFacingDirection(transform.position, Player.Instance.transform.position);
        }

        _lastPosition = transform.position;
        _nextCheckDirectionTime = Time.time + CheckDirectionDuration;
    }

    private void Roaming() {
        _roamPosition = GetRoamingPosition();
        _navMeshAgent.SetDestination(_roamPosition);
    }

    private Vector3 GetRoamingPosition() {
        return _startPosition + Utils.GetRandomDir() * UnityEngine.Random.Range(roamingDistanceMin, roamingDistanceMax);
    }

    private void ChangeFacingDirection(Vector3 sourcePosition, Vector3 targetPosition) {
        bool check = sourcePosition.x < targetPosition.x;
        transform.rotation = Quaternion.Euler(0, check ? 0 : 180, 0);
    }
}
