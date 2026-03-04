using System;
using UnityEngine;

public class DestructiblrPlantVisual : MonoBehaviour
{
    [SerializeField] private DestructiblePlant destructiblePlant;
    [SerializeField] private GameObject bushDeathVFX;

    private void OnEnable()
    {
        destructiblePlant.OnDestructible += DestructiblePlant_OnDestructible;
    }

    private void DestructiblePlant_OnDestructible(object s, EventArgs a)
    {
        ShowDeathVFX();
    }

    private void ShowDeathVFX()
    {
        Instantiate(bushDeathVFX, transform.position, Quaternion.identity);
    }

    private void OnDisable()
    {
        destructiblePlant.OnDestructible -= DestructiblePlant_OnDestructible;
    }
}
