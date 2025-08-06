using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]
public class FlashBlink : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _damagebleObject;
    [SerializeField] private Material _blinkMaterial;
    [SerializeField] private float _blinkDuration = 0.2f;

    private float _blinkTimer;
    private Material _defaultMaterial;
    private SpriteRenderer _spriteRenderer;
    private bool _isBlinking;

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _blinkMaterial = _spriteRenderer.material;

        _isBlinking = true;
    }

    private void Update() {
        if (_isBlinking && _blinkTimer != -999) {
            if (_blinkTimer < 0) {
                SetDefaultMaterial();
            } else {
                _blinkTimer -= Time.deltaTime;
            }
        }
    }

    public void StopBlink() {
        SetDefaultMaterial();
        _isBlinking=false;
    }

    private void SetBlinkingMaterial() {
        _blinkTimer = _blinkDuration;
        _spriteRenderer.material = _blinkMaterial;
    }

    private void SetDefaultMaterial() {
        _spriteRenderer.material = _defaultMaterial;
        _blinkTimer = -999;
    }
}
