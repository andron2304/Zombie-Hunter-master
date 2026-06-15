using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Positions")]
    public Transform locationLane1;
    public Transform locationLane2;
    public Transform locationLane3;
    public Transform gunpoint;

    [Header("Ammunition")]
    public GameObject[] Bullet;
    public GameObject currentBullet;

    [Header("Weapon Stats & Upgrades")]
    public int weaponLevel = 1;
    public float fireRate = 0.2f;        // Затримка між пострілами (менше = швидше)
    public int projectileCount = 1;      // Кількість куль за один постріл
    public float spreadAngle = 15f;      // Кут розльоту для кількох куль

    private int laneSelect = 2;
    private bool isFiring = false;       // Запобігає накладанню корутин вогню

    public enum FireMode
    {
        Single,
        Burst,
        Automatic
    }

    public FireMode fireMode;

    void Start()
    {
        transform.position = new Vector2(locationLane2.position.x, locationLane2.position.y);
        fireMode = FireMode.Single;
        
        // Ініціалізуємо кулю ДО того, як звертатися до її імені
        if (Bullet.Length > 0)
        {
            currentBullet = Bullet[0]; 
            Debug.Log("Current Bullet: " + currentBullet.name);
        }

        Debug.Log("Fire Mode: " + fireMode.ToString());
        laneSelect = 2; 
    }

    void Update()
    {
        MovementInput();
        CombatInput();

        // Тестова кнопка для перевірки прокачування зброї (Натисни 'U')
        if (Input.GetKeyDown(KeyCode.U))
        {
            UpgradeWeapon();
        }
    }

    void MovementInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            laneSelect--;
            if (laneSelect < 1) laneSelect = 1; 
            UpdateLanePosition();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            laneSelect++;
            if (laneSelect > 3) laneSelect = 3; 
            UpdateLanePosition();
        }
    }

    void UpdateLanePosition()
    {
        if (laneSelect == 1)
            transform.position = new Vector2(locationLane1.position.x, locationLane1.position.y);
        else if (laneSelect == 2)
            transform.position = new Vector2(locationLane2.position.x, locationLane2.position.y);
        else if (laneSelect == 3)
            transform.position = new Vector2(locationLane3.position.x, locationLane3.position.y);
    }

    void CombatInput()
    {
        if (Input.GetKeyDown(KeyCode.R)) fireMode = FireMode.Single;
        if (Input.GetKeyDown(KeyCode.E)) fireMode = FireMode.Automatic;
        if (Input.GetKeyDown(KeyCode.Q)) fireMode = FireMode.Burst;

        // Постріл
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && !isFiring)
        {
            Fire(fireMode);
        }
    }

    public void SetBullet(int index)
    {
        if (index >= 0 && index < Bullet.Length)
        {
            currentBullet = Bullet[index];
        }
    }

    // ВИПРАВЛЕНО: прибрано зайве слово enum
    public void Fire(FireMode mode)
    {
        switch (mode)
        {
            case FireMode.Single:
                SpawnBullet();
                break;
            case FireMode.Burst:
                StartCoroutine(BurstFire());
                break;
            case FireMode.Automatic:
                StartCoroutine(AutomaticFire());
                break;
        }
    }

    // Новий метод для створення куль (підтримує розліт/multishot)
    private void SpawnBullet()
    {
        if (projectileCount == 1)
        {
            Instantiate(currentBullet, gunpoint.position, gunpoint.rotation);
        }
        else
        {
            // Вираховуємо кути, щоб кулі летіли віялом
            float startAngle = -spreadAngle * (projectileCount - 1) / 2f;
            for (int i = 0; i < projectileCount; i++)
            {
                Quaternion rotation = gunpoint.rotation * Quaternion.Euler(0, 0, startAngle + (spreadAngle * i));
                Instantiate(currentBullet, gunpoint.position, rotation);
            }
        }
    }

    private IEnumerator BurstFire()
    {
        isFiring = true;
        for (int i = 0; i < 3; i++)
        {
            SpawnBullet();
            yield return new WaitForSeconds(fireRate); // Використовуємо змінну fireRate
        }
        isFiring = false;
    }

    private IEnumerator AutomaticFire()
    {
        isFiring = true;
        // Стріляємо, поки гравець тримає ЛКМ або Пробіл
        while (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
        {
            SpawnBullet();
            yield return new WaitForSeconds(fireRate); 
        }
        isFiring = false;
    }

    // ==========================================
    // СИСТЕМА ПОКРАЩЕННЯ ЗБРОЇ
    // ==========================================
    public void UpgradeWeapon()
    {
        weaponLevel++;
        Debug.Log("Weapon Upgraded to Level: " + weaponLevel);

        // 1. Збільшуємо швидкострільність (зменшуємо затримку, мінімум до 0.05 сек)
        fireRate = Mathf.Max(0.05f, fireRate - 0.03f);

        // 2. Додаємо додаткові кулі на певних рівнях прокачки (наприклад, на 3-му і 5-му рівні)
        if (weaponLevel == 3 || weaponLevel == 5)
        {
            projectileCount++;
            Debug.Log("Multishot activated! Projectiles: " + projectileCount);
        }

        // 3. Еволюція кулі: змінюємо префаб кулі кожні 2 рівні (якщо є в масиві)
        int newBulletIndex = (weaponLevel - 1) / 2;
        if (newBulletIndex < Bullet.Length)
        {
            SetBullet(newBulletIndex);
        }
    }
}