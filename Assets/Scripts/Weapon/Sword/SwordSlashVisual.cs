using UnityEngine;

public class SwordSlashVisual : MonoBehaviour
{
    [SerializeField] private Sword sword;
    
    private static readonly int Attack1 = Animator.StringToHash(Attack);

    private Animator _animator;
    private const string Attack = "Attack";

    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    private void Start() {
        sword.OnSwordSwing += Sword_OnSwordSwing;
    }

    private void Sword_OnSwordSwing(object sender, System.EventArgs e) {
        _animator.SetTrigger(Attack1);
    }

    public void TriggerEndAttackAnimation() {
        sword.AttackColliderTurn(false);
    }

    private void OnDestroy() {
        sword.OnSwordSwing -= Sword_OnSwordSwing;
    }
}
