using UnityEngine;
using System.Collections;

public class GravitySystem : MonoBehaviour
{
    [Header("Gravity Settings")]
    [SerializeField] private float gravityStrength = 9.81f;
    [Header("Flip Target")]
    [Tooltip("Объект, который переворачивается при смене гравитации. Поворот по X и Y приводится к 0, по Z — 0° или 180°.")]
    [SerializeField] private Transform flipTarget;
    [Tooltip("Длительность обнуления осей X и Y перед переворотом")]
    [SerializeField] private float normalizeDuration = 0.5f;
    [Tooltip("Длительность плавного поворота по оси Z (0° или 180°)")]
    [SerializeField] private float flipDuration = 1f;
    [Header("Visual Effects (Deprecated)")]
    [Tooltip("Используется ObjectPoolManager для эффектов. Оставьте пустым.")]
    [SerializeField] private Rigidbody _rigidbody;
    private Vector3 currentGravity = Vector3.down;
    private bool isGravityFlipped = false;
    private bool isFlipping = false;
    private Vector3 gravityVelocity = Vector3.zero;
    public Vector3 CurrentGravity => currentGravity;
    public bool IsGravityFlipped => isGravityFlipped;
    public bool IsFlipping => isFlipping;
    public Vector3 GravityVelocity => gravityVelocity;
    private void Awake()
    {
        _rigidbody.useGravity = false;
    }
    private void FixedUpdate()
    {
        _rigidbody.AddForce(currentGravity * gravityStrength, ForceMode.Acceleration);
        gravityVelocity = _rigidbody.linearVelocity;
    }
    public void FlipGravity()
    {
        if (isFlipping) return;
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
        if (flipTarget != null && (normalizeDuration > 0f || flipDuration > 0f))
        {
            StartCoroutine(SmoothFlip());
        }
    }

    private static float NormalizeEuler(float euler)
    {
        while (euler > 180f) euler -= 360f;
        while (euler < -180f) euler += 360f;
        return euler;
    }

    private IEnumerator SmoothFlip()
    {
        isFlipping = true;
        float targetZ = isGravityFlipped ? 180f : 0f;
        Vector3 startEuler = flipTarget.localEulerAngles;
        float startZ = NormalizeEuler(startEuler.z);
        Quaternion startRot = flipTarget.localRotation;

        if (normalizeDuration > 0f)
        {
            Quaternion toZeroXY = Quaternion.Euler(0f, 0f, startZ);
            float elapsed = 0f;
            while (elapsed < normalizeDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / normalizeDuration);
                t = t * t * (3f - 2f * t);
                flipTarget.localRotation = Quaternion.Slerp(startRot, toZeroXY, t);
                yield return null;
            }
            flipTarget.localRotation = toZeroXY;
        }

        if (flipDuration > 0f)
        {
            Quaternion fromRot = normalizeDuration > 0f ? Quaternion.Euler(0f, 0f, startZ) : flipTarget.localRotation;
            Quaternion toRot = Quaternion.Euler(0f, 0f, targetZ);
            float elapsed = 0f;
            while (elapsed < flipDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / flipDuration);
                t = t * t * (3f - 2f * t);
                flipTarget.localRotation = Quaternion.Slerp(fromRot, toRot, t);
                yield return null;
            }
            flipTarget.localRotation = toRot;
        }
        else
        {
            flipTarget.localRotation = Quaternion.Euler(0f, 0f, targetZ);
        }

        isFlipping = false;
    }
}
