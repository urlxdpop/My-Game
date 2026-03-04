using System;
using System.Collections;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(KnockBack))]
[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    public event EventHandler OnPlayerTakeDamage;
    public event EventHandler OnPlayerDeath;

    private const float MinSpeed = 0.1f;

    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    
    [Header("HP")]
    [SerializeField] private int maxHp = 6;
    [SerializeField] private float damageRecoveryTime = 0.5f;
    
    [Header("Dash")]
    [SerializeField] private float dashSpeed = 4;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private float dashCoolDownTime = 0.5f;

    private Rigidbody2D _rg;
    private KnockBack _knockBack;

    private bool _isRun;
    private bool _isFlip;
    private Vector2 _inputVector;
    private int _hp;
    private bool _canTakeDamage = true;
    private bool _isAlive = true;
    private bool _isDashing;
    private float _initialSpeed;

    private void Awake()
    {
        Instance = this;
        _rg = GetComponent<Rigidbody2D>();
        _knockBack = GetComponent<KnockBack>();
        _hp = maxHp;
        _initialSpeed = speed;
    }

    private void Start()
    {
        GameInput.Instance.OnPlayerAttack += GameInput_OnPlayerAttack;
        GameInput.Instance.OnPlayerDash += GameInput_OnPlayerDash;
    }

    private void Update()
    {
        _inputVector = GameInput.Instance.GetMovementAction();
    }

    private void FixedUpdate()
    {
        if (!_knockBack.IsGettingKnockBack) _isRun = HandleMovement();
    }

    public bool IsRun()
    {
        return _isRun;
    }

    public bool IsFlip()
    {
        return _isFlip;
    }

    public bool IsAlive()
    {
        return _isAlive;
    }

    public void TakeDamage(Transform damageSource, int damage)
    {
        if (_canTakeDamage && _isAlive)
        {
            _canTakeDamage = false;
            _hp = Mathf.Max(0, _hp -= damage);
            _knockBack.GetKnockedBack(damageSource);

            OnPlayerTakeDamage?.Invoke(this, EventArgs.Empty);

            StartCoroutine(DamageRecoveryRoutine());
        }

        DetectDeath();
    }

    public int GetHp()
    {
        return _hp;
    }

    public int GetMaxHp()
    {
        return maxHp;
    }

    private void DetectDeath()
    {
        if (_hp != 0 || !_isAlive) return;
        _isAlive = false;
        _knockBack.StopKnockBackMovement();
        GameInput.Instance.DisableMovement();

        OnPlayerDeath?.Invoke(this, EventArgs.Empty);
    }

    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        _canTakeDamage = true;
    }

    private bool HandleMovement()
    {
        _rg.MovePosition(_rg.position + _inputVector * (speed * Time.fixedDeltaTime));

        _isFlip = _inputVector.x switch
        {
            > MinSpeed => false,
            < -MinSpeed => true,
            _ => _isFlip
        };

        return Mathf.Abs(_inputVector.x) > MinSpeed || Mathf.Abs(_inputVector.y) > MinSpeed;
    }

    private void Dash()
    {
        if (!_isDashing) StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        _isDashing = true;
        speed *= dashSpeed;
        trailRenderer.enabled = true;

        yield return new WaitForSeconds(dashTime);

        trailRenderer.enabled = false;
        speed = _initialSpeed;

        yield return new WaitForSeconds(dashCoolDownTime);

        _isDashing = false;
    }

    private static void GameInput_OnPlayerAttack(object sender, EventArgs e)
    {
        ActiveWeapon.Instance.GetActiveWeapon().Attack();
    }

    private void GameInput_OnPlayerDash(object sender, EventArgs e)
    {
        Dash();
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnPlayerAttack -= GameInput_OnPlayerAttack;
        GameInput.Instance.OnPlayerDash -= GameInput_OnPlayerDash;
    }
}