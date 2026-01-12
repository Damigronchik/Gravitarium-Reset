using UnityEngine;
public class KeyCard : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string keyCardId = "keycard_001";
    [SerializeField] private bool isCollected = false;
    [Header("Visual Effects (Deprecated)")]
    [Tooltip("Используется ObjectPoolManager для эффектов. Оставьте пустым.")]
    [SerializeField] private ParticleSystem collectEffect;
    [Header("Animation")]
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float floatAmplitude = 0.5f;
    [SerializeField] private float floatSpeed = 2f;
    private Vector3 startPosition;
    private float floatOffset;
    private void Start()
    {
        if (InventoryManager.Instance != null && InventoryManager.Instance.HasKeyCard(keyCardId))
        {
            gameObject.SetActive(false);
            isCollected = true;
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }
            return;
        }
        startPosition = transform.position;
        floatOffset = Random.Range(0f, Mathf.PI * 2f);
    }
    private void Update()
    {
        if (!isCollected)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed + floatOffset) * floatAmplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
    public void Collect()
    {
        if (isCollected)
            return;
        isCollected = true;
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.SpawnFromPool("ItemCollect", transform.position, Quaternion.identity);
        }
        else if (collectEffect != null)
        {
            collectEffect.Play();
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.GetItemCollectSound(), transform.position);
        }
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddKeyCard(keyCardId);
        }
        gameObject.SetActive(false);
        EventBus.InvokeKeyCardCollected(gameObject, keyCardId);
        EventBus.InvokeItemCollected(gameObject);
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
        Destroy(gameObject, 2f);
    }
    public string KeyCardId => keyCardId;
    public bool IsCollected => isCollected;
}
