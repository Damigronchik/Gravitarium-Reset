using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class GravitySystem : MonoBehaviour
{
    [Header("Gravity Settings")]
    [SerializeField] private float gravityStrength = 9.81f;
    [Header("Visual Effects (Deprecated)")]
    [Tooltip("Используется ObjectPoolManager для эффектов. Оставьте пустым.")]
    private Rigidbody rb;
    private Vector3 currentGravity = Vector3.down;
    private bool isGravityFlipped = false;
    private Vector3 gravityVelocity = Vector3.zero;
    public Vector3 CurrentGravity => currentGravity;
    public bool IsGravityFlipped => isGravityFlipped;
    public Vector3 GravityVelocity => gravityVelocity;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }
    private void FixedUpdate()
    {
        rb.AddForce(currentGravity * gravityStrength, ForceMode.Acceleration);
        gravityVelocity = rb.linearVelocity;
    }
    public void FlipGravity()
    {
        isGravityFlipped = !isGravityFlipped;
        currentGravity = isGravityFlipped ? Vector3.up : Vector3.down;
        if (ObjectPoolManager.Instance != null)
        {
            GameObject effect = ObjectPoolManager.Instance.SpawnFromPool("GravityFlip", transform.position, Quaternion.identity);
            if (effect == null)
            {
                Debug.LogWarning("GravitySystem: GravityFlip effect not found in ObjectPoolManager! Make sure the pool with tag 'GravityFlip' is configured.");
            }
        }
        EventBus.InvokeGravityFlipped(currentGravity);
    }
}
