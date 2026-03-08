using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TransparencyDetection : MonoBehaviour
{
    [Range(0f, 1f)] [SerializeField] private float transparencyAmount = 0.8f;
    [SerializeField] private float fadeDuration = 0.5f;

    private const float FULL_NON_TRANSPARENCY = 1f;

    SpriteRenderer _spriteRenderer;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.GetComponent<Player>() ||
            collider is not CapsuleCollider2D) return;

        StartCoroutine(FadeRoutine(_spriteRenderer.color.a, transparencyAmount));
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (!collider.GetComponent<Player>() ||
            collider is not CapsuleCollider2D) return;

        StartCoroutine(FadeRoutine(_spriteRenderer.color.a, FULL_NON_TRANSPARENCY));
    }

    private IEnumerator FadeRoutine(float startTransparency, float endTransparency)
    {
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startTransparency, endTransparency, elapsedTime/fadeDuration);
            
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, newAlpha);
            yield return null;
        }
    }
}