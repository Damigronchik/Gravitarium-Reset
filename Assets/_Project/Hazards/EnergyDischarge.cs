using UnityEngine;
using System.Collections.Generic;
public class EnergyDischarge : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damagePerSecond = 10f;
    [SerializeField] private float damageInterval = 0.5f;
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem dischargeEffect;
    [SerializeField] private Light dischargeLight;
    [SerializeField] private float lightIntensity = 2f;
    [Header("Audio")]
    [SerializeField] private AudioClip dischargeSound;
    [Header("State")]
    [SerializeField] private bool isActive = true;
    private float lastDamageTime = 0f;
    private HashSet<PlayerController> playersInRange = new HashSet<PlayerController>();
    private void Start()
    {
        UpdateVisualState();
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError($"EnergyDischarge: No Collider found on {gameObject.name}! Add a Collider component.");
        }
        else if (!col.isTrigger)
        {
            Debug.LogWarning($"EnergyDischarge: Collider on {gameObject.name} is not set as Trigger! Set 'Is Trigger' to true.");
        }
        if (!isActive)
        {
            Debug.LogWarning($"EnergyDischarge: {gameObject.name} is not active! Set 'Is Active' to true.");
        }
    }
    private void Update()
    {
        if (isActive)
        {
            ApplyDamage();
        }
        else
        {
            if (playersInRange.Count > 0)
            {
                playersInRange.Clear();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player == null && other.CompareTag("Player"))
        {
            player = other.GetComponentInParent<PlayerController>();
            if (player == null)
            {
                player = FindObjectOfType<PlayerController>();
            }
        }
        if (player != null)
        {
            playersInRange.Add(player);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player == null && other.CompareTag("Player"))
        {
            player = other.GetComponentInParent<PlayerController>();
            if (player == null)
            {
                player = FindObjectOfType<PlayerController>();
            }
        }
        if (player != null)
        {
            playersInRange.Remove(player);
        }
    }
    private void ApplyDamage()
    {
        if (!isActive)
            return;
        if (Time.time - lastDamageTime < damageInterval)
            return;
        if (playersInRange.Count == 0)
            return;
        foreach (var player in playersInRange)
        {
            if (player != null)
            {
                var stats = player.GetComponent<PlayerStatsComponent>();
                if (stats != null)
                {
                    float damage = damagePerSecond * damageInterval;
                    stats.TakeDamage(damage);
                }
                else
                {
                    Debug.LogWarning($"EnergyDischarge: PlayerStatsComponent not found on player!");
                }
            }
        }
        lastDamageTime = Time.time;
    }
    private void UpdateVisualState()
    {
        if (dischargeEffect != null)
        {
            if (isActive)
            {
                dischargeEffect.Play();
            }
            else
            {
                dischargeEffect.Stop();
            }
        }
        if (dischargeLight != null)
        {
            dischargeLight.enabled = isActive;
            dischargeLight.intensity = isActive ? lightIntensity : 0f;
        }
    }
    public void SetActive(bool active)
    {
        isActive = active;
        UpdateVisualState();
    }
    public bool IsActive => isActive;
}
