using UnityEngine;

/// <summary>
/// ScriptableObject що містить базові характеристики зброї.
/// Створіть екземпляр через Create -> Weapons -> WeaponStats
/// </summary>
[CreateAssetMenu(fileName = "WeaponStats", menuName = "Weapons/WeaponStats")]
public class WeaponStats : ScriptableObject
{
    [Header("Base stats")]
    [Tooltip("Дальність стрільби (одиниці юніта)")]
    public float Range = 50f;

    [Tooltip("Швидкість кулі (одиниці/сек)")]
    public float BulletSpeed = 20f;

    [Tooltip("Шкода однієї кулі")]
    public int Damage = 10;
}
