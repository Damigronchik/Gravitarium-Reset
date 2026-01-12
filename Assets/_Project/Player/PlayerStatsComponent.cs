using UnityEngine;
public class PlayerStatsComponent : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private PlayerStats stats = new PlayerStats();
    public float MaxHealth => stats.MaxHealth;
    public float CurrentHealth
    {
        get => stats.CurrentHealth;
        set => stats.CurrentHealth = value;
    }
    public float MaxEnergy => stats.MaxEnergy;
    public float CurrentEnergy
    {
        get => stats.CurrentEnergy;
        set => stats.CurrentEnergy = value;
    }
    public float HealthPercentage => stats.HealthPercentage;
    public float EnergyPercentage => stats.EnergyPercentage;
    public void TakeDamage(float damage)
    {
        stats.TakeDamage(damage);
    }
    public void Heal(float amount)
    {
        stats.Heal(amount);
    }
    public void ConsumeEnergy(float amount)
    {
        stats.ConsumeEnergy(amount);
    }
    public void RestoreEnergy(float amount)
    {
        stats.RestoreEnergy(amount);
    }
    public PlayerStats GetStats()
    {
        return stats;
    }
    public void SetStats(PlayerStats newStats)
    {
        stats = newStats;
    }
}
