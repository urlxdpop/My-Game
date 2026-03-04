using System;
using UnityEngine;

public class DestructiblePlant : MonoBehaviour
{
    public EventHandler OnDestructible;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.GetComponent<Sword>()) return;
        OnDestructible?.Invoke(this, EventArgs.Empty);
        Destroy(gameObject);

        NavMeshSurfaceManagement.Instance.RebakeNavMeshSurface();
    }
}
