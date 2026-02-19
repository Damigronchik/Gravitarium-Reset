using UnityEngine;
#if CINEMACHINE_EXISTS
using Cinemachine;
#endif

// Для условной компиляции добавьте символ CINEMACHINE_EXISTS в Player Settings > Other Settings > Scripting Define Symbols
// Или установите пакет Cinemachine через Package Manager

/// <summary>
/// Контроллер камеры для работы с Cinemachine Virtual Camera.
/// Управляет вертикальным вращением камеры и переворотом при изменении гравитации.
/// </summary>
public class CinemachineCameraController : MonoBehaviour
{
    [Header("Cinemachine Settings")]
    [Tooltip("Cinemachine Virtual Camera. Если не указан, будет найден автоматически.")]
    [SerializeField] private MonoBehaviour virtualCameraComponent;
    
    /// <summary>
    /// Устанавливает Virtual Camera (для автоматической настройки)
    /// </summary>
    public void SetVirtualCamera(MonoBehaviour vcam)
    {
        virtualCameraComponent = vcam;
#if CINEMACHINE_EXISTS
        if (vcam != null)
        {
            virtualCamera = vcam as CinemachineVirtualCamera;
            if (virtualCamera != null)
            {
                povComponent = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
                if (povComponent == null)
                {
                    povComponent = virtualCamera.AddCinemachineComponent<CinemachinePOV>();
                }
                if (povComponent != null)
                {
                    verticalRotation = povComponent.m_VerticalAxis.Value;
                }
            }
        }
#endif
    }
    
    [Header("Camera Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float verticalLookLimit = 80f;
    
    private float verticalRotation = 0f;
    private Vector2 lookInput;
    private bool isFlippingGravity = false;
    
#if CINEMACHINE_EXISTS
    private CinemachineVirtualCamera virtualCamera;
    private CinemachinePOV povComponent;
#endif

    private void Awake()
    {
#if CINEMACHINE_EXISTS
        // Ищем Virtual Camera, если не указан
        if (virtualCameraComponent == null)
        {
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        }
        else
        {
            virtualCamera = virtualCameraComponent as CinemachineVirtualCamera;
        }
        
        if (virtualCamera != null)
        {
            povComponent = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
            if (povComponent == null)
            {
                Debug.LogWarning("CinemachineCameraController: POV component not found on Virtual Camera. Adding it...");
                povComponent = virtualCamera.AddCinemachineComponent<CinemachinePOV>();
            }
            
            // Инициализируем вертикальное вращение
            if (povComponent != null)
            {
                verticalRotation = povComponent.m_VerticalAxis.Value;
            }
        }
        else
        {
            Debug.LogWarning("CinemachineCameraController: Cinemachine Virtual Camera not found!");
        }
#else
        Debug.LogWarning("CinemachineCameraController: Cinemachine package is not installed!");
#endif
    }

    /// <summary>
    /// Устанавливает ввод мыши для управления камерой
    /// </summary>
    public void SetLookInput(Vector2 input)
    {
        lookInput = input;
    }

    /// <summary>
    /// Обновляет вращение камеры на основе ввода мыши
    /// </summary>
    public void UpdateCameraRotation()
    {
        if (isFlippingGravity) return;
        
#if CINEMACHINE_EXISTS
        if (povComponent != null && lookInput.magnitude > 0.01f)
        {
            // Горизонтальное вращение управляется через Virtual Camera (Follow target)
            // Вертикальное вращение управляется через POV
            float mouseY = lookInput.y * mouseSensitivity;
            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -verticalLookLimit, verticalLookLimit);
            
            povComponent.m_VerticalAxis.Value = verticalRotation;
        }
#endif
    }

    /// <summary>
    /// Переворачивает камеру после переворота гравитации
    /// </summary>
    public void FlipCamera()
    {
#if CINEMACHINE_EXISTS
        if (povComponent != null)
        {
            verticalRotation = -verticalRotation;
            povComponent.m_VerticalAxis.Value = verticalRotation;
        }
#endif
    }

    /// <summary>
    /// Устанавливает флаг переворота гравитации (блокирует управление камерой)
    /// </summary>
    public void SetFlippingGravity(bool flipping)
    {
        isFlippingGravity = flipping;
    }

    /// <summary>
    /// Получает текущее вертикальное вращение камеры
    /// </summary>
    public float GetVerticalRotation()
    {
        return verticalRotation;
    }

    /// <summary>
    /// Устанавливает вертикальное вращение камеры
    /// </summary>
    public void SetVerticalRotation(float rotation)
    {
        verticalRotation = Mathf.Clamp(rotation, -verticalLookLimit, verticalLookLimit);
#if CINEMACHINE_EXISTS
        if (povComponent != null)
        {
            povComponent.m_VerticalAxis.Value = verticalRotation;
        }
#endif
    }

#if CINEMACHINE_EXISTS
    /// <summary>
    /// Получает ссылку на Virtual Camera
    /// </summary>
    public CinemachineVirtualCamera GetVirtualCamera()
    {
        return virtualCamera;
    }
#endif
}

