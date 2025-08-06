using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyVisual : MonoBehaviour {
    [SerializeField] private EnemyAI _enemyAI;
    [SerializeField] private EnemyEntity _enemyEntity;
    [SerializeField] private GameObject _enemyShadow;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private const string IS_RUNNING = "IsRunning";
    private const string CHASING_SPEED_BOOST = "ChasingSpeedBoost";
    private const string IS_DIE = "IsDie";
    private const string ATTACK = "Attack";
    private const string TAKE_HIT = "TakeHit";

    private void Awake() {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        _enemyAI.OnEnemyAttack += EnemyAI_OnEnemyAttack;
        _enemyEntity.OnTakeHit += EnemyEntity_OnTakeHit;
        _enemyEntity.OnDie += EnemyEntity_OnDie;
    }

    public void TriggerAttackAnimationTurnOff() {
        _enemyEntity.PolygonColliderTurn(false);
    }

    public void TriggerAttackAnimationTurnOn() {
        _enemyEntity.PolygonColliderTurn(true);
    }

    private void EnemyEntity_OnTakeHit(object sender, System.EventArgs e) {
        _animator.SetTrigger(TAKE_HIT);
    }

    private void EnemyEntity_OnDie(object sender, System.EventArgs e) {
        _animator.SetBool(IS_DIE, true);
        _spriteRenderer.sortingOrder = -1;
        _enemyShadow.SetActive(false);
    }

    private void OnDestroy() {
        _enemyAI.OnEnemyAttack -= EnemyAI_OnEnemyAttack;
        _enemyEntity.OnTakeHit -= EnemyEntity_OnTakeHit;
        _enemyEntity.OnDie -= EnemyEntity_OnDie;
    }

    private void Update() {
        _animator.SetBool(IS_RUNNING, _enemyAI.IsRunning);
        _animator.SetFloat(CHASING_SPEED_BOOST, _enemyAI.GetRoamingAnimationSpeed());
    }

    private void EnemyAI_OnEnemyAttack(object sender, System.EventArgs e) {
        _animator.SetTrigger(ATTACK);
    }
}
