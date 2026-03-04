using UnityEngine;

public class SelfDestroyVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSustem;

    private void Awake()
    {
        particleSustem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (!particleSustem) return;
        if (!particleSustem.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
