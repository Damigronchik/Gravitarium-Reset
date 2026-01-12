using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameHUD : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [Header("Gravity Indicator")]
    [SerializeField] private Image gravityIndicator;
    [SerializeField] private TextMeshProUGUI gravityText;
    [SerializeField] private Color normalGravityColor = Color.blue;
    [SerializeField] private Color flippedGravityColor = Color.red;
    [Header("Energy Cores")]
    [SerializeField] private TextMeshProUGUI energyCoresText;
    [Header("Key Cards")]
    [SerializeField] private TextMeshProUGUI keyCardsText;
    [Header("Interaction Hint")]
    [SerializeField] private GameObject interactionHint;
    [SerializeField] private TextMeshProUGUI interactionHintText;
    private PlayerController playerController;
    private GravitySystem gravitySystem;
    private PlayerStatsComponent playerStatsComponent;
    private InventoryManager inventoryManager;
    private void Start()
    {
        FindPlayerComponents();
        inventoryManager = InventoryManager.Instance;
        SubscribeToEvents();
        if (interactionHint != null)
        {
            interactionHint.SetActive(false);
        }
    }
    private void FindPlayerComponents()
    {
        playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            gravitySystem = playerController.GetComponent<GravitySystem>();
            playerStatsComponent = playerController.GetComponent<PlayerStatsComponent>();
            if (playerStatsComponent == null)
            {
                Debug.LogWarning("GameHUD: PlayerStatsComponent not found on player! Health UI will not work.");
            }
            if (gravitySystem == null)
            {
                Debug.LogWarning("GameHUD: GravitySystem not found on player! Gravity indicator will not work.");
            }
        }
        else
        {
            Debug.LogWarning("GameHUD: PlayerController not found! Trying again in LateStart...");
            Invoke(nameof(FindPlayerComponents), 0.5f);
        }
    }
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    private void Update()
    {
        if (playerController == null || gravitySystem == null || playerStatsComponent == null)
        {
            if (Time.frameCount % 60 == 0)
            {
                FindPlayerComponents();
            }
        }
        UpdateHealth();
        UpdateGravityIndicator();
        UpdateEnergyCores();
        UpdateKeyCards();
    }
    private void SubscribeToEvents()
    {
        EventBus.OnGravityFlipped += OnGravityFlipped;
        EventBus.OnKeyCardCollected += OnKeyCardCollected;
        EventBus.OnEnergyCoreCollected += OnEnergyCoreCollected;
    }
    private void UnsubscribeFromEvents()
    {
        EventBus.OnGravityFlipped -= OnGravityFlipped;
        EventBus.OnKeyCardCollected -= OnKeyCardCollected;
        EventBus.OnEnergyCoreCollected -= OnEnergyCoreCollected;
    }
    private void OnGravityFlipped(Vector3 newGravity)
    {
        UpdateGravityIndicator();
    }
    private void OnKeyCardCollected(GameObject keyCard, string keyCardId)
    {
        UpdateKeyCards();
    }
    private void OnEnergyCoreCollected(GameObject energyCore)
    {
        UpdateEnergyCores();
    }
    private void UpdateHealth()
    {
        if (playerStatsComponent == null)
            return;
        float health = playerStatsComponent.CurrentHealth;
        float maxHealth = playerStatsComponent.MaxHealth;
        float healthPercentage = playerStatsComponent.HealthPercentage;
        if (healthBar != null)
        {
            healthBar.value = healthPercentage;
        }
        if (healthText != null)
        {
            healthText.text = $"{Mathf.RoundToInt(health)} / {Mathf.RoundToInt(maxHealth)}";
        }
    }
    private void UpdateGravityIndicator()
    {
        if (gravitySystem == null)
            return;
        bool isFlipped = gravitySystem.IsGravityFlipped;
        if (gravityIndicator != null)
        {
            gravityIndicator.color = isFlipped ? flippedGravityColor : normalGravityColor;
        }
        if (gravityText != null)
        {
            gravityText.text = isFlipped ? "Gravity: Flipped" : "Gravity: Normal";
        }
    }
    private void UpdateEnergyCores()
    {
        if (inventoryManager == null)
            return;
        int cores = inventoryManager.GetEnergyCoreCount();
        if (energyCoresText != null)
        {
            energyCoresText.text = $"Energy Cores: {cores}";
        }
    }
    private void UpdateKeyCards()
    {
        if (inventoryManager == null)
            return;
        int keyCards = inventoryManager.GetCollectedKeyCards().Count;
        if (keyCardsText != null)
        {
            keyCardsText.text = $"Key Cards: {keyCards}";
        }
    }
    public void ShowInteractionHint(string hintText)
    {
        if (interactionHint != null)
        {
            interactionHint.SetActive(true);
            if (interactionHintText != null)
            {
                interactionHintText.text = hintText;
            }
        }
    }
    public void HideInteractionHint()
    {
        if (interactionHint != null)
        {
            interactionHint.SetActive(false);
        }
    }
}
