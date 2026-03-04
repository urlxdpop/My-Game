using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyVisual : MonoBehaviour {
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private EnemyEntity enemyEntity;
    [SerializeField] private GameObject enemyShadow;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private const string IsRunning = "IsRunning";
    private const string ChasingSpeedBoost = "ChasingSpeedBoost";
    private const string IsDie = "IsDie";
    private const string Attack = "Attack";
    private const string TakeHit = "TakeHit";
    
    private static readonly int Hit = Animator.StringToHash(TakeHit);
    private static readonly int Die = Animator.StringToHash(IsDie);
    private static readonly int Running = Animator.StringToHash(IsRunning);
    private static readonly int SpeedBoost = Animator.StringToHash(ChasingSpeedBoost);
    private static readonly int Attack1 = Animator.StringToHash(Attack);

    private void Awake() {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        enemyAI.OnEnemyAttack += EnemyAI_OnEnemyAttack;
        enemyEntity.OnTakeHit += EnemyEntity_OnTakeHit;
        enemyEntity.OnDie += EnemyEntity_OnDie;
    }

    public void TriggerAttackAnimationTurnOff() {
        enemyEntity.PolygonColliderTurn(false);
    }

    public void TriggerAttackAnimationTurnOn() {
        enemyEntity.PolygonColliderTurn(true);
    }

    private void EnemyEntity_OnTakeHit(object sender, System.EventArgs e) {
        _animator.SetTrigger(Hit);
    }

    private void EnemyEntity_OnDie(object sender, System.EventArgs e) {
        _animator.SetBool(Die, true);
        _spriteRenderer.sortingOrder = -1;
        enemyShadow.SetActive(false);
    }

    private void OnDestroy() {
        enemyAI.OnEnemyAttack -= EnemyAI_OnEnemyAttack;
        enemyEntity.OnTakeHit -= EnemyEntity_OnTakeHit;
        enemyEntity.OnDie -= EnemyEntity_OnDie;
    }

    private void Update() {
        _animator.SetBool(Running, enemyAI.IsRunning);
        _animator.SetFloat(SpeedBoost, enemyAI.GetRoamingAnimationSpeed());
    }

    private void EnemyAI_OnEnemyAttack(object sender, System.EventArgs e) {
        _animator.SetTrigger(Attack1);
    }
}
