using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
public class KnockBack : MonoBehaviour
{
    [SerializeField] private float _knockBackForce;
    [SerializeField] private float _knockBackMovingTimerMax;

    private float _knockBackMovingTimer;

    private Rigidbody2D _rb;

    public bool IsGettingKnockBack { get; private set; }

    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        _knockBackMovingTimer -= Time.deltaTime;
        if (_knockBackMovingTimer < 0) StopKnockBackMovment();
    }

    public void GetKnockedBack(Transform damageSource) {
        IsGettingKnockBack = true;
        _knockBackMovingTimer = _knockBackMovingTimerMax;
        Vector2 defference = (transform.position - damageSource.position).normalized * _knockBackForce / _rb.mass;
        _rb.AddForce (defference, ForceMode2D.Impulse);
    }

    public void StopKnockBackMovment() {
        _rb.linearVelocity = Vector2.zero;
        IsGettingKnockBack = false;
    }
}
