using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private Animator _animator;
    private SpriteRenderer _sprite;

    private const string IsRun = "isRun";
    private const string TakeDamage = "TakeDamage";
    private const string IsDie = "IsDie";
    
    private static readonly int Die = Animator.StringToHash(IsDie);
    private static readonly int Damage = Animator.StringToHash(TakeDamage);
    private static readonly int Run = Animator.StringToHash(IsRun);

    private void Awake() {
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        Player.Instance.OnPlayerTakeDamage += Player_OnPlayerTakeDamage;
        Player.Instance.OnPlayerDeath += Player_OnPlayerDeath;
    }

    private void Player_OnPlayerDeath(object sender, System.EventArgs e) {
        _animator.SetBool(Die, true);
    }

    private void Player_OnPlayerTakeDamage(object sender, System.EventArgs e) {
        _animator.SetTrigger(Damage);
    }

    private void Update() {
        _animator.SetBool(Run, Player.Instance.IsRun());
        _sprite.flipX = Player.Instance.IsFlip();
    }

    private void OnDestroy()
    {
        Player.Instance.OnPlayerTakeDamage -= Player_OnPlayerTakeDamage;
        Player.Instance.OnPlayerDeath -= Player_OnPlayerDeath;
    }
}
