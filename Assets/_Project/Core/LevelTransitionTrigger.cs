using UnityEngine;
public class LevelTransitionTrigger : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] private bool loadNextLevel = true;
    [SerializeField] private string specificLevelName = "";
    [Header("Visual Feedback")]
    [SerializeField] private bool showDebugMessage = true;
    [SerializeField] private string debugMessage = "Переход на следующий уровень...";
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<PlayerController>() != null)
        {
            if (showDebugMessage)
            {
                Debug.Log(debugMessage);
            }
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveGame();
            }
            if (loadNextLevel)
            {
                if (LevelManager.Instance != null)
                {
                    LevelManager.Instance.LoadNextLevel();
                }
                else
                {
                    Debug.LogError("LevelManager not found! Cannot load next level.");
                }
            }
            else if (!string.IsNullOrEmpty(specificLevelName))
            {
                if (LevelManager.Instance != null)
                {
                    LevelManager.Instance.LoadLevel(specificLevelName);
                }
                else if (SceneLoader.Instance != null)
                {
                    SceneLoader.Instance.LoadScene(specificLevelName);
                }
                else
                {
                    Debug.LogError("LevelManager and SceneLoader not found! Cannot load level.");
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col != null && col.isTrigger)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            if (col is BoxCollider)
            {
                BoxCollider box = col as BoxCollider;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(box.center, box.size);
            }
            else if (col is SphereCollider)
            {
                SphereCollider sphere = col as SphereCollider;
                Gizmos.DrawSphere(transform.position + sphere.center, sphere.radius);
            }
            else if (col is CapsuleCollider)
            {
                CapsuleCollider capsule = col as CapsuleCollider;
                Gizmos.DrawWireSphere(transform.position + capsule.center, capsule.radius);
            }
        }
    }
}
