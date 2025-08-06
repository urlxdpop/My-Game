using UnityEngine;
using UnityEngine.AI;
using MyGame.Utils;
using System;

public class EnemyAI : MonoBehaviour {
    private const float MIN_SPEED = 0.1f;

    [SerializeField] private State _startingState;
    [SerializeField] private float _roamingDistanceMax = 7f;
    [SerializeField] private float _roamingDistanceMin = 3f;
    [SerializeField] private float _roamingTimerMax = 2f;

    [SerializeField] private bool _isChasingEnemy = false;
    [SerializeField] private float _chasingDistance = 4f;
    [SerializeField] private float _chasingSpeedBoost = 1.5f;

    [SerializeField] private bool _isAttackEnemy = false;
    [SerializeField] private float _attackDistance = 1f;
    [SerializeField] private float _attackRate = 2f;
    private float _nextAttackTime = 0f;

    private NavMeshAgent _navMeshAgent;
    private State _state;
    private float _roamingTime;
    private Vector3 _roamPosition;
    private Vector3 _startPosition;

    private float _roamingSpeed;
    private float _chasingSpeed;

    private float _nextCheckDirectionTime = 0f;
    private float _checkDirectionDuration = 0.1f;
    private Vector3 _lastPosition;

    public event EventHandler OnEnemyAttack;

    private enum State {
        IDLE,
        ROAMING,
        CHASING,
        ATTACKING,
        DEATH
    }

    public bool IsRunning {
        get {
            if (_navMeshAgent.velocity == Vector3.zero) return false;
            return true;
        }
    }

    private void Awake() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
        _state = _startingState;

        _roamingSpeed = _navMeshAgent.speed;
        _chasingSpeed = _navMeshAgent.speed * _chasingSpeedBoost;
    }

    private void Start() {
        _startPosition = transform.position;
    }

    private void Update() {
        StateHandler();
        MovmentDirectionHenddler();
    }

    public float GetRoamingAnimationSpeed() {
        return _navMeshAgent.speed / _roamingSpeed;
    }

    public void SetDeathState() {
        _navMeshAgent.ResetPath();
        _state = State.DEATH;
    }

    private void StateHandler() {
        switch (_state) {
            case State.ROAMING:
                _roamingTime -= Time.deltaTime;
                if (_roamingTime < 0) {
                    Roaming();
                    _roamingTime = _roamingTimerMax;
                }
                ChangeCurrentState();
                break;
            case State.CHASING:
                ChasingTarget();
                ChangeCurrentState();
                break;
            case State.ATTACKING:
                AttackingTarget();
                ChangeCurrentState();
                break;
            case State.DEATH:
                break;
            default:
            case State.IDLE:
                break;
        }
    }

    private void ChasingTarget() {
        _navMeshAgent.SetDestination(Player.Instance.transform.position);
    }

    private void AttackingTarget() {
        if (Time.time > _nextAttackTime) {
            OnEnemyAttack?.Invoke(this, EventArgs.Empty);

            _nextAttackTime = Time.time + _attackRate;
        }

    }

    private void ChangeCurrentState() {
        float distanceToTarget = Vector3.Distance(transform.position, Player.Instance.transform.position);
        State newState = State.ROAMING;

        if (_isChasingEnemy && distanceToTarget <= _chasingDistance) {
            newState = State.CHASING;
        }

        if (_isAttackEnemy && distanceToTarget <= _attackDistance) {
            if (Player.Instance.IsAlive()) {
                newState = State.ATTACKING;
            } else {
                newState = State.ROAMING;
            }
        }

        if (newState != _state) {
            if (newState == State.CHASING) {
                _navMeshAgent.ResetPath();
                _navMeshAgent.speed = _chasingSpeed;
            } else if (newState == State.ROAMING) {
                _roamingTime = 0;
                _navMeshAgent.speed = _roamingSpeed;
            } else if (newState == State.ATTACKING) {
                _navMeshAgent.ResetPath();
            }

            _state = newState;
        }
    }

    private void MovmentDirectionHenddler() {
        if (Time.time > _nextCheckDirectionTime) {
            if (IsRunning) {
                ChangeFacingDiraction(_lastPosition, transform.position);
            } else if (_state == State.CHASING) {
                ChangeFacingDiraction(transform.position, Player.Instance.transform.position);
            }

            _lastPosition = transform.position;
            _nextCheckDirectionTime = Time.time + _checkDirectionDuration;
        }
    }

    private void Roaming() {
        _roamPosition = GetRoamingPosition();
        _navMeshAgent.SetDestination(_roamPosition);
    }

    private Vector3 GetRoamingPosition() {
        return _startPosition + Utils.GetRandomDir() * UnityEngine.Random.Range(_roamingDistanceMin, _roamingDistanceMax);
    }

    private void ChangeFacingDiraction(Vector3 soursePosition, Vector3 targetPosition) {
        bool check = soursePosition.x < targetPosition.x;
        transform.rotation = Quaternion.Euler(0, check ? 0 : 180, 0);
    }
}
