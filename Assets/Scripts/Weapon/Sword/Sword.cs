using System;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private int damage = 2;

    public event EventHandler OnSwordSwing;

    private PolygonCollider2D _polygonCollider2D;

    private void Awake() {
        _polygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    private void Start() {
        AttackColliderTurn(false);
    }

    public void Attack() {
        AttackColliderTurn(true, true);

        OnSwordSwing?.Invoke(this, EventArgs.Empty);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.transform.TryGetComponent(out EnemyEntity enemyEntity)) {
            enemyEntity.TakeDamage(damage);
        }
    }

    public void AttackColliderTurn(bool state, bool doubleActivated = false) {
        if (doubleActivated) _polygonCollider2D.enabled = !state;
        _polygonCollider2D.enabled = state;
    }
}
