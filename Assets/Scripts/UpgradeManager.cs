using UnityEngine;

/// <summary>
/// Менеджер апгрейдів — веде облік вбивств (валюти) та дозволяє купувати покращення.
/// Кожен апгрейд коштує `upgradeCost` вбивств. При купівлі знімає вбивства і застосовує апгрейд до `WeaponSystem`.
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    [Header("Currency")]
    [Tooltip("Поточна кількість вбивств/валюти")]
    public int currentKills = 0;

    [Tooltip("Вартість кожного апгрейду в вбивствах")]
    public int upgradeCost = 3;

    [Header("References")]
    [Tooltip("Цільовий компонент зброї, до якого застосовуються апгрейди")]
    public WeaponSystem weaponSystem;

    /// <summary>
    /// Додає одне вбивство (можна викликати при смерті ворога).
    /// </summary>
    public void AddKill()
    {
        currentKills += 1;
        Debug.Log($"AddKill -> currentKills: {currentKills}");
    }

    /// <summary>
    /// Перевіряє, чи вистачає вбивств для покупки.
    /// </summary>
    public bool CanAfford()
    {
        return currentKills >= upgradeCost;
    }

    /// <summary>
    /// Купує апгрейд дальності (+5). Повертає true, якщо покупка успішна.
    /// </summary>
    public bool UpgradeRange()
    {
        if (weaponSystem == null)
        {
            Debug.LogWarning("UpgradeRange failed: weaponSystem not assigned.");
            return false;
        }

        if (!CanAfford())
        {
            Debug.Log("Not enough kills to purchase Range upgrade.");
            return false;
        }

        currentKills -= upgradeCost;
        weaponSystem.ApplyRangeUpgrade(5f); // +5 одиниць дальності
        Debug.Log($"Range upgrade purchased. Remaining kills: {currentKills}");
        return true;
    }

    /// <summary>
    /// Купує апгрейд швидкості кулі (+10%). Повертає true, якщо покупка успішна.
    /// </summary>
    public bool UpgradeBulletSpeed()
    {
        if (weaponSystem == null)
        {
            Debug.LogWarning("UpgradeBulletSpeed failed: weaponSystem not assigned.");
            return false;
        }

        if (!CanAfford())
        {
            Debug.Log("Not enough kills to purchase BulletSpeed upgrade.");
            return false;
        }

        currentKills -= upgradeCost;
        weaponSystem.ApplyBulletSpeedUpgradePercent(0.1f); // +10%
        Debug.Log($"BulletSpeed upgrade purchased. Remaining kills: {currentKills}");
        return true;
    }
}
