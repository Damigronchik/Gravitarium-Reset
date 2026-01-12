using UnityEngine;
[RequireComponent(typeof(PlayerController))]
public class PlayerFootsteps : MonoBehaviour
{
    [Header("Footstep Settings")]
    [SerializeField] private float footstepInterval = 0.5f;
    [SerializeField] private float surfaceCheckDistance = 0.2f;
    [SerializeField] private float pitchVariation = 0.15f;
    [Header("Surface Tags")]
    [SerializeField] private string metalTag = "Metal";
    [SerializeField] private string concreteTag = "Concrete";
    private PlayerController playerController;
    private GravitySystem gravitySystem;
    private float lastFootstepTime;
    private SurfaceType currentSurface = SurfaceType.Concrete;
    private SurfaceType previousSurface = SurfaceType.Concrete;
    private enum SurfaceType
    {
        Metal,
        Concrete
    }
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        gravitySystem = GetComponent<GravitySystem>();
    }
    private void Update()
    {
        if (playerController == null)
        {
            Debug.LogWarning("PlayerFootsteps: PlayerController not found!");
            return;
        }
        if (!playerController.IsGrounded)
            return;
        CheckSurfaceType();
        Vector3 velocity = playerController.Velocity;
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        float horizontalSpeed = horizontalVelocity.magnitude;
        if (horizontalSpeed > 0.1f && Time.time - lastFootstepTime >= footstepInterval)
        {
            PlayFootstep();
            lastFootstepTime = Time.time;
        }
    }
    private void CheckSurfaceType()
    {
        RaycastHit hit;
        Vector3 checkDirection = (gravitySystem != null && gravitySystem.IsGravityFlipped) ? Vector3.up : Vector3.down;
        Vector3 origin = transform.position + (checkDirection * -1) * 0.1f;
        float checkDistance = surfaceCheckDistance + 0.2f;
        if (Physics.Raycast(origin, checkDirection, out hit, checkDistance))
        {
            string hitTag = hit.collider.tag;
            SurfaceType newSurface = SurfaceType.Metal;
            if (hitTag == concreteTag)
            {
                newSurface = SurfaceType.Concrete;
            }
            if (newSurface != currentSurface)
            {
                previousSurface = currentSurface;
                currentSurface = newSurface;
            }
        }
        else
        {
            if (currentSurface != SurfaceType.Concrete)
            {
                previousSurface = currentSurface;
                currentSurface = SurfaceType.Concrete;
            }
        }
    }
    private void PlayFootstep()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("PlayerFootsteps: AudioManager not found!");
            return;
        }
        string surfaceType = currentSurface switch
        {
            SurfaceType.Metal => "Metal",
            _ => "Concrete"
        };
        AudioClip clip = AudioManager.Instance.GetFootstepClip(surfaceType);
        if (clip != null)
        {
            AudioManager.Instance.PlayFootstepSFX(clip, pitchVariation);
        }
        else
        {
            Debug.LogWarning($"PlayerFootsteps: No footstep clip found for surface type: {surfaceType}. Make sure AudioManager has footstep sounds configured!");
        }
    }
}
