using UnityEngine;
using Zenject;

public class HealthBarManager : MonoBehaviour
{
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private Transform healthBarsParent; // Canvas transform

    private DiContainer _container;

    [Inject]
    public void Construct(DiContainer container)
    {
        _container = container;
    }

    public void CreateHealthBar(ICombatEntity entity, Transform targetTransform)
    {
        if (entity == null || targetTransform == null) return;

        var healthBarInstance = _container.InstantiatePrefab(healthBarPrefab, healthBarsParent);
        var healthBarUI = healthBarInstance.GetComponent<HealthBarUI>();
        
        if (healthBarUI != null)
        {
            healthBarUI.Initialize(entity, targetTransform);
        }
    }
}