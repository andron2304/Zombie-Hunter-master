using UnityEngine;

// Перелік типів зброї (це дуже зручний підхід в програмуванні, щоб не плутатись)
public enum WeaponType 
{ 
    Pistol,     // Звичайний пістолет (повільно)
    MachineGun, // Автомат (швидко)
    Shotgun     // Дрібовик (багато куль врізнобіч)
}

public class Weapon : MonoBehaviour
{
    [Header("🔫 Основні налаштування")]
    public WeaponType currentWeapon; // Обирай тип зброї прямо в Unity
    public Transform firePoint;      // Точка на дулі, звідки вилітає куля
    public GameObject bulletPrefab;  // Твій префаб кулі

    [Header("⚡ Характеристики стрільби")]
    public float fireRate = 0.5f;    // Час між пострілами (чим менше — тим швидше стріляє)
    private float nextFireTime = 0f; // Внутрішній таймер

    [Header("💥 Налаштування Дрібовика")]
    public int pelletsCount = 5;     // Кількість кульок за один постріл
    public float spreadAngle = 15f;  // Кут розкиду (наскільки широко летять кулі)


    void Update()
    {
        // Якщо гравець затискає ліву кнопку миші (Fire1)
        if (Input.GetButton("Fire1"))
        {
            // Перевіряємо, чи пройшов час затримки (щоб не стріляти як лазер)
            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + fireRate; // Оновлюємо таймер
                Shoot(); // Викликаємо команду пострілу
            }
        }
    }

    // Логіка самого пострілу
    void Shoot()
    {
        // Конструкція switch визначає, як стріляти залежно від обраної зброї
        switch (currentWeapon)
        {
            case WeaponType.Pistol:
            case WeaponType.MachineGun:
                // Звичайна стрільба однією кулею рівно вперед
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                break;

            case WeaponType.Shotgun:
                // Стрільба дробом (спавнимо кілька куль у циклі)
                for (int i = 0; i < pelletsCount; i++)
                {
                    // Вираховуємо випадковий кут для кожної дробинки
                    float randomSpread = Random.Range(-spreadAngle, spreadAngle);
                    
                    // Повертаємо кулю на цей випадковий кут
                    Quaternion rotation = firePoint.rotation * Quaternion.Euler(0, 0, randomSpread);
                    
                    // Створюємо кулю
                    Instantiate(bulletPrefab, firePoint.position, rotation);
                }
                break;
        }
    }
}