using UnityEngine;
#if CINEMACHINE_EXISTS
using Cinemachine;
#endif

/// <summary>
/// Автоматически настраивает Cinemachine камеру при запуске уровня.
/// Создает необходимые объекты и настраивает связи между компонентами.
/// </summary>
public class CinemachineAutoSetup : MonoBehaviour
{
    [Header("Auto Setup Settings")]
    [Tooltip("Автоматически настроить камеру при старте")]
    [SerializeField] private bool autoSetupOnStart = true;
    
    [Tooltip("Создать CameraPivot, если не существует")]
    [SerializeField] private bool createCameraPivot = true;
    
    [Tooltip("Создать Virtual Camera, если не существует")]
    [SerializeField] private bool createVirtualCamera = true;
    
    [Tooltip("Создать CinemachineBrain на Main Camera, если не существует")]
    [SerializeField] private bool setupCinemachineBrain = true;
    
    [Header("Camera Settings")]
    [Tooltip("Позиция CameraPivot относительно Player (высота глаз)")]
    [SerializeField] private Vector3 cameraPivotOffset = new Vector3(0, 1.6f, 0);
    
    [Tooltip("Приоритет Virtual Camera")]
    [SerializeField] private int virtualCameraPriority = 10;
    
    [Tooltip("Ограничение вертикального обзора")]
    [SerializeField] private float verticalLookLimit = 80f;
    
    [Header("References (Auto-filled)")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private GameObject virtualCameraObject;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CinemachineCameraController cinemachineController;
    
#if CINEMACHINE_EXISTS
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBrain cinemachineBrain;
#endif

    private void Start()
    {
        if (autoSetupOnStart)
        {
            // Запускаем настройку с небольшой задержкой, чтобы все объекты успели инициализироваться
            StartCoroutine(SetupAfterDelay());
        }
    }

    private void OnEnable()
    {
        // Подписываемся на событие загрузки уровня
        EventBus.OnLevelLoaded += OnLevelLoaded;
    }

    private void OnDisable()
    {
        // Отписываемся от события
        EventBus.OnLevelLoaded -= OnLevelLoaded;
    }

    private void OnLevelLoaded(string levelName)
    {
        if (autoSetupOnStart)
        {
            // Запускаем настройку при загрузке уровня
            StartCoroutine(SetupAfterDelay());
        }
    }

    private System.Collections.IEnumerator SetupAfterDelay()
    {
        // Ждем несколько кадров, чтобы все объекты успели инициализироваться
        yield return null;
        yield return null;
        yield return null;
        
        SetupCinemachineCamera();
    }

    /// <summary>
    /// Автоматически настраивает всю систему Cinemachine
    /// </summary>
    [ContextMenu("Setup Cinemachine Camera")]
    public void SetupCinemachineCamera()
    {
#if !CINEMACHINE_EXISTS
        Debug.LogWarning("CinemachineAutoSetup: Cinemachine package is not installed! Please install Cinemachine via Package Manager.");
        return;
#endif

        // Находим или создаем Player
        GameObject player = FindPlayer();
        if (player == null)
        {
            Debug.LogError("CinemachineAutoSetup: Player not found! Cannot setup camera.");
            return;
        }

        // Находим или создаем PlayerController
        playerController = player.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("CinemachineAutoSetup: PlayerController not found on Player! Cannot setup camera.");
            return;
        }

        // Находим или создаем CinemachineCameraController
        cinemachineController = player.GetComponent<CinemachineCameraController>();
        if (cinemachineController == null)
        {
            cinemachineController = player.AddComponent<CinemachineCameraController>();
            Debug.Log("CinemachineAutoSetup: Added CinemachineCameraController to Player.");
        }

        // Находим или создаем CameraPivot
        if (createCameraPivot)
        {
            cameraPivot = FindOrCreateCameraPivot(player.transform);
        }
        else
        {
            cameraPivot = FindCameraPivot(player.transform);
        }

        // Находим или создаем Virtual Camera
        if (createVirtualCamera)
        {
            virtualCameraObject = FindOrCreateVirtualCamera();
        }
        else
        {
            virtualCameraObject = FindVirtualCamera();
        }

#if CINEMACHINE_EXISTS
        // Настраиваем Virtual Camera
        if (virtualCameraObject != null)
        {
            SetupVirtualCamera();
        }

        // Настраиваем CinemachineBrain
        if (setupCinemachineBrain)
        {
            SetupCinemachineBrain();
        }
#endif

        // Настраиваем PlayerController
        SetupPlayerController();

        Debug.Log("CinemachineAutoSetup: Camera setup completed successfully!");
    }

    private GameObject FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            player = FindObjectOfType<PlayerController>()?.gameObject;
        }
        return player;
    }

    private Transform FindOrCreateCameraPivot(Transform playerTransform)
    {
        // Ищем существующий CameraPivot
        Transform pivot = FindCameraPivot(playerTransform);
        if (pivot != null)
        {
            return pivot;
        }

        // Создаем новый CameraPivot
        GameObject pivotObj = new GameObject("CameraPivot");
        pivotObj.transform.SetParent(playerTransform);
        pivotObj.transform.localPosition = cameraPivotOffset;
        pivotObj.transform.localRotation = Quaternion.identity;
        pivotObj.transform.localScale = Vector3.one;

        Debug.Log($"CinemachineAutoSetup: Created CameraPivot at {cameraPivotOffset} relative to Player.");
        return pivotObj.transform;
    }

    private Transform FindCameraPivot(Transform playerTransform)
    {
        // Ищем в дочерних объектах
        for (int i = 0; i < playerTransform.childCount; i++)
        {
            Transform child = playerTransform.GetChild(i);
            if (child.name == "CameraPivot" || child.name.Contains("CameraPivot"))
            {
                return child;
            }
        }
        return null;
    }

    private GameObject FindOrCreateVirtualCamera()
    {
        // Ищем существующий Virtual Camera
        GameObject vcam = FindVirtualCamera();
        if (vcam != null)
        {
            return vcam;
        }

#if CINEMACHINE_EXISTS
        // Создаем новый Virtual Camera
        GameObject vcamObj = new GameObject("CM vcam Player");
        virtualCamera = vcamObj.AddComponent<CinemachineVirtualCamera>();
        
        // Настраиваем базовые параметры
        virtualCamera.Priority = virtualCameraPriority;
        
        // Устанавливаем Follow и Look At
        if (cameraPivot != null)
        {
            virtualCamera.Follow = cameraPivot;
            virtualCamera.LookAt = cameraPivot;
        }

        Debug.Log("CinemachineAutoSetup: Created Cinemachine Virtual Camera.");
        return vcamObj;
#else
        Debug.LogWarning("CinemachineAutoSetup: Cinemachine not installed! Cannot create Virtual Camera.");
        return null;
#endif
    }

    private GameObject FindVirtualCamera()
    {
#if CINEMACHINE_EXISTS
        CinemachineVirtualCamera[] vcams = FindObjectsOfType<CinemachineVirtualCamera>();
        foreach (var vcam in vcams)
        {
            if (vcam.name.Contains("Player") || vcam.name.Contains("CM vcam"))
            {
                return vcam.gameObject;
            }
        }
        
        // Если не нашли по имени, берем первый с высоким приоритетом
        if (vcams.Length > 0)
        {
            return vcams[0].gameObject;
        }
#endif
        return null;
    }

#if CINEMACHINE_EXISTS
    private void SetupVirtualCamera()
    {
        if (virtualCameraObject == null) return;

        virtualCamera = virtualCameraObject.GetComponent<CinemachineVirtualCamera>();
        if (virtualCamera == null)
        {
            Debug.LogError("CinemachineAutoSetup: Virtual Camera component not found!");
            return;
        }

        // Настраиваем Follow и Look At
        if (cameraPivot != null)
        {
            virtualCamera.Follow = cameraPivot;
            virtualCamera.LookAt = cameraPivot;
        }

        // Настраиваем Body (Transposer для следования за CameraPivot)
        var transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        if (transposer == null)
        {
            // Удаляем другие Body компоненты, если есть
            var existingBody = virtualCamera.GetCinemachineComponent<CinemachineComponentBase>();
            if (existingBody != null)
            {
                Destroy(existingBody);
            }
            
            // Добавляем Transposer с нулевым offset (камера точно на позиции CameraPivot)
            transposer = virtualCamera.AddCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                transposer.m_FollowOffset = Vector3.zero;
            }
        }
        else
        {
            // Убеждаемся, что offset нулевой
            transposer.m_FollowOffset = Vector3.zero;
        }

        // Настраиваем Aim (POV)
        var pov = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        if (pov == null)
        {
            pov = virtualCamera.AddCinemachineComponent<CinemachinePOV>();
        }

        // Настраиваем POV параметры
        if (pov != null)
        {
            // Горизонтальная ось
            pov.m_HorizontalAxis.m_InputAxisName = "";
            pov.m_HorizontalAxis.m_InputAxisValue = 0;
            pov.m_HorizontalAxis.m_MaxSpeed = 0; // Управляется через PlayerController
            
            // Вертикальная ось
            pov.m_VerticalAxis.m_InputAxisName = "";
            pov.m_VerticalAxis.m_InputAxisValue = 0;
            pov.m_VerticalAxis.m_MaxSpeed = 0; // Управляется через PlayerController
            pov.m_VerticalAxis.m_MinValue = -verticalLookLimit;
            pov.m_VerticalAxis.m_MaxValue = verticalLookLimit;
        }

        // Устанавливаем приоритет
        virtualCamera.Priority = virtualCameraPriority;

        // Привязываем к CinemachineCameraController
        if (cinemachineController != null && virtualCamera != null)
        {
            cinemachineController.SetVirtualCamera(virtualCamera);
        }

        Debug.Log("CinemachineAutoSetup: Virtual Camera configured.");
    }

    private void SetupCinemachineBrain()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }

        if (mainCamera == null)
        {
            Debug.LogWarning("CinemachineAutoSetup: Main Camera not found! Creating one...");
            GameObject cameraObj = new GameObject("Main Camera");
            mainCamera = cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";
            cameraObj.AddComponent<AudioListener>();
        }

        cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
        if (cinemachineBrain == null)
        {
            cinemachineBrain = mainCamera.gameObject.AddComponent<CinemachineBrain>();
            cinemachineBrain.m_DefaultBlend.m_Time = 0f; // Мгновенный переход
            cinemachineBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
            Debug.Log("CinemachineAutoSetup: Added CinemachineBrain to Main Camera.");
        }
    }
#endif

    private void SetupPlayerController()
    {
        if (playerController == null) return;

        // Используем публичный метод для настройки
        playerController.SetUseCinemachine(true, cinemachineController);

        Debug.Log("CinemachineAutoSetup: PlayerController configured to use Cinemachine.");
    }

    /// <summary>
    /// Удаляет все созданные объекты (для тестирования)
    /// </summary>
    [ContextMenu("Cleanup Auto Setup")]
    public void CleanupAutoSetup()
    {
        if (cameraPivot != null && cameraPivot.name == "CameraPivot")
        {
            if (Application.isPlaying)
            {
                Destroy(cameraPivot.gameObject);
            }
            else
            {
                DestroyImmediate(cameraPivot.gameObject);
            }
        }

        if (virtualCameraObject != null && virtualCameraObject.name == "CM vcam Player")
        {
            if (Application.isPlaying)
            {
                Destroy(virtualCameraObject);
            }
            else
            {
                DestroyImmediate(virtualCameraObject);
            }
        }

        Debug.Log("CinemachineAutoSetup: Cleanup completed.");
    }
}

