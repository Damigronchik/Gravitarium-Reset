using System;
using UnityEngine;
[Serializable]
public class PlayerStats
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float currentEnergy = 100f;
    [SerializeField] private int collectedEnergyCores = 0;
    public float MaxHealth => maxHealth;
    public float CurrentHealth
    {
        get => currentHealth;
        set => currentHealth = Mathf.Clamp(value, 0f, maxHealth);
    }
    public float MaxEnergy => maxEnergy;
    public float CurrentEnergy
    {
        get => currentEnergy;
        set => currentEnergy = Mathf.Clamp(value, 0f, maxEnergy);
    }
    public int CollectedEnergyCores
    {
        get => collectedEnergyCores;
        set => collectedEnergyCores = Mathf.Max(0, value);
    }
    public float HealthPercentage => currentHealth / maxHealth;
    public float EnergyPercentage => currentEnergy / maxEnergy;
    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0f)
        {
            EventBus.InvokePlayerDeath();
        }
    }
    public void Heal(float amount)
    {
        CurrentHealth += amount;
    }
    public void ConsumeEnergy(float amount)
    {
        CurrentEnergy -= amount;
    }
    public void RestoreEnergy(float amount)
    {
        CurrentEnergy += amount;
    }
    public void AddEnergyCore()
    {
        CollectedEnergyCores++;
    }
}
