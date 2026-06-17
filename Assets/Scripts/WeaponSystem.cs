using UnityEngine;

/// <summary>
/// Компонент, що керує стрільбою та поточними характеристиками зброї.
/// Ініціалізує runtime-значення з `WeaponStats` і надає метод `Shoot()`.
/// </summary>
public class WeaponSystem : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Базові стат-дані (префаб ScriptableObject)")]
    public WeaponStats baseStats;

    [Header("Runtime stats (modifiable)")]
    [Tooltip("Поточна дальність, використовується при стрільбі")]
    public float currentRange;

    [Tooltip("Поточна швидкість кулі")]
    public float currentBulletSpeed;

    [Tooltip("Поточна шкода")]
    public int currentDamage;

    [Header("Optional (bullet)")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Ініціалізує поточні характеристики із `baseStats`.
    /// Викликається в Start(), але можна викликати вручну при потребі.
    /// </summary>
    public void Initialize()
    {
        if (baseStats != null)
        {
            currentRange = baseStats.Range;
            currentBulletSpeed = baseStats.BulletSpeed;
            currentDamage = baseStats.Damage;
        }
    }

    /// <summary>
    /// Демонстраційний метод стрільби, що використовує currentRange та currentBulletSpeed.
    /// Якщо задано `bulletPrefab` і `firePoint`, створює об'єкт кулі і задає йому швидкість.
    /// Інакше просто лог-повідомлення з використовуваними значеннями.
    /// </summary>
    public void Shoot()
    {
        Debug.Log($"Shoot: Range={currentRange}, BulletSpeed={currentBulletSpeed}, Damage={currentDamage}");

        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // Підтримуємо як 3D, так і 2D Rigidbody (якщо присутні)
            Rigidbody rb3 = bullet.GetComponent<Rigidbody>();
            if (rb3 != null)
            {
                rb3.linearVelocity = firePoint.forward * currentBulletSpeed;
            }
            else
            {
                Rigidbody2D rb2 = bullet.GetComponent<Rigidbody2D>();
                if (rb2 != null)
                {
                    // Для 2D вважаємо, що право (local right) - напрям стрільби
                    rb2.linearVelocity = firePoint.right * currentBulletSpeed;
                }
            }

            // Автоматично знищити кулю через час, приблизно відповідний дальності
            Destroy(bullet, currentRange / Mathf.Max(0.01f, currentBulletSpeed));
        }
    }

    /// <summary>
    /// Збільшує дальність на вказане значення.
    /// </summary>
    public void ApplyRangeUpgrade(float amount)
    {
        currentRange += amount;
        Debug.Log($"Range upgraded by {amount}. New Range: {currentRange}");
    }

    /// <summary>
    /// Збільшує швидкість кулі на відсоток (наприклад, 0.1f = +10%).
    /// </summary>
    public void ApplyBulletSpeedUpgradePercent(float percent)
    {
        currentBulletSpeed *= (1f + percent);
        Debug.Log($"BulletSpeed increased by {percent * 100f}% . New BulletSpeed: {currentBulletSpeed}");
    }
}
