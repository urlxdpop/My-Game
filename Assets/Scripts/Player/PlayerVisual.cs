using UnityEngine;

public class PlayerVisual : MonoBehaviour
{

    private Animator animator;
    private SpriteRenderer sprite;

    private const string IS_RUN = "isRun";
    private const string TAKE_DAMAGE = "TakeDamage";
    private const string IS_DIE = "IsDie";

    private void Awake() {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        Player.Instance.OnPlayerTakeDamage += Player_OnPlayerTakeDamage;
        Player.Instance.OnPlayerDeath += Player_OnPlayerDeath;
    }

    private void Player_OnPlayerDeath(object sender, System.EventArgs e) {
        animator.SetBool(IS_DIE, true);
    }

    private void Player_OnPlayerTakeDamage(object sender, System.EventArgs e) {
        animator.SetTrigger(TAKE_DAMAGE);
    }

    private void Update() {
        animator.SetBool(IS_RUN, Player.Instance.IsRun());
        sprite.flipX = Player.Instance.IsFlip();
    }
}
