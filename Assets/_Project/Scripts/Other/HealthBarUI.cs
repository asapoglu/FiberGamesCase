using UnityEngine;
using UnityEngine.UI;
using System.Collections;
/// <summary>
/// Varlıkların sağlık durumunu gösteren UI bileşeni.
/// Can barının görüntülenmesi ve güncellenmesini sağlar.
/// </summary>
public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image healthSlider;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Fade Settings")]
    [SerializeField] private float showDuration = 2f;    // Görünür kalma süresi
    [SerializeField] private float fadeDuration = 0.5f;  // Kaybolma süresi

    private ICombatEntity _targetEntity;
    private Coroutine _fadeCoroutine;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0;
    }
    /// <summary>
    /// HealthBar'ı belirtilen varlık için başlatır.
    /// </summary>
    /// <param name="target">Sağlık barının bağlanacağı varlık</param>
    /// <param name="targetTransform">Varlığın transform bileşeni</param>
    public void Initialize(ICombatEntity target, Transform targetTransform)
    {
        _targetEntity = target;
        this.targetTransform = targetTransform;
        mainCamera = Camera.main;

        healthSlider.fillAmount = _targetEntity.MaxHealth;

        _targetEntity.OnDamageTaken += HandleDamageTaken;
    }

    private void HandleDamageTaken(float damage)
    {
        if (healthSlider != null)
        {
            float MaxHealth = _targetEntity.MaxHealth;
            float CurrentHealth = _targetEntity.CurrentHealth;
            float Percent = CurrentHealth / MaxHealth;
            healthSlider.fillAmount = Percent;
            ShowHealthBar();
        }
    }

    private void ShowHealthBar()
    {
        // Eğer önceki fade coroutine çalışıyorsa durdur
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        // Yeni fade coroutine başlat
        _fadeCoroutine = StartCoroutine(FadeRoutine());
    }
    
    /// <summary>
    /// Sağlık barının görünürlüğünü kontrol eder.
    /// Hasar alındığında görünür olur ve zamanla kaybolur.
    /// </summary>    
    private IEnumerator FadeRoutine()
    {
        // Hemen görünür yap
        canvasGroup.alpha = 1;

        // Belirlenen süre kadar bekle
        yield return new WaitForSeconds(showDuration);

        // Yavaşça kaybol
        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0;
    }

    private void LateUpdate()
    {
        if (targetTransform == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = targetTransform.position + offset;

        transform.rotation = mainCamera.transform.rotation;
    }

    private void OnDestroy()
    {
        if (_targetEntity != null)
        {
            _targetEntity.OnDamageTaken -= HandleDamageTaken;
        }
    }
}