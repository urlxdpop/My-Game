using System;
using System.Collections;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(KnockBack))]
[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour {

    public static Player Instance { get; private set; }
    public event EventHandler OnPlayerTakeDamage;
    public event EventHandler OnPlayerDeath;

    private const float MIN_SPEED = 0.1f;


    [SerializeField] private float _speed = 5f;
    [SerializeField] private int _maxHP = 6;
    [SerializeField] private float _damageRecoveryTime = 0.5f;

    private Rigidbody2D _rg;
    private KnockBack _knockBack;

    private bool _isRun = false;
    private bool _isFlip = false;
    private Vector2 _inputVector;
    private int _hp;
    private bool _canTakeDamage = true;
    private bool _isAlive = true;

    private void Awake() {
        Instance = this;
        _rg = GetComponent<Rigidbody2D>();
        _knockBack = GetComponent<KnockBack>();
        _hp = _maxHP;
    }

    private void Start() {
        GameInput.Instance.OnPlayerAttack += GameInput_OnPlayerAttack;
    }

    private void Update() {
        _inputVector = GameInput.Instance.GetMovmentAction();
    }

    private void FixedUpdate() {
        if (!_knockBack.IsGettingKnockBack) _isRun = HandleMovement();
    }

    public bool IsRun() {
        return _isRun;
    }

    public bool IsFlip() {
        return _isFlip;
    }

    public bool IsAlive() {
        return _isAlive;
    }

    public void TakeDamage(Transform damageSource, int damage) {
        if (_canTakeDamage && _isAlive) {
            _canTakeDamage = false;
            _hp = Mathf.Max(0, _hp -= damage);
            _knockBack.GetKnockedBack(damageSource);

            OnPlayerTakeDamage?.Invoke(this, EventArgs.Empty);

            StartCoroutine(DamageRecoveryRoutine());
        }

        DetectDeath();
    }

    public int GetHP() {
        return _hp;
    }
    
    public int GetMaxHP() {
        return _maxHP;
    }

    private void DetectDeath() {
        if (_hp == 0 && _isAlive) {
            _isAlive = false;
            _knockBack.StopKnockBackMovment();
            GameInput.Instance.DisableMovement();

            OnPlayerDeath?.Invoke(this, EventArgs.Empty);
        }
    }

    private IEnumerator DamageRecoveryRoutine() {
        yield return new WaitForSeconds(_damageRecoveryTime);
        _canTakeDamage = true;
    }

    private bool HandleMovement() {
        _rg.MovePosition(_rg.position + _inputVector * (_speed * Time.fixedDeltaTime));

        if (_inputVector.x > MIN_SPEED) _isFlip = false;
        else if (_inputVector.x < -MIN_SPEED) _isFlip = true;

        if (Mathf.Abs(_inputVector.x) > MIN_SPEED || Mathf.Abs(_inputVector.y) > MIN_SPEED) return true;
        return false;
    }

    private void GameInput_OnPlayerAttack(object sender, EventArgs e) {
        ActiveWeapon.Instance.GetActiveWeapon().Attack();
    }
}