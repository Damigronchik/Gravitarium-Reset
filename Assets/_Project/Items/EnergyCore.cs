using UnityEngine;
public class EnergyCore : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private string coreId = "core_001";
	[SerializeField] private bool isCollected = false;
	[Tooltip("ID головоломки, которая использует это ядро. Если указано и головоломка решена, ядро не будет появляться.")]
	[SerializeField] private string puzzleId = "";
	[Header("Animation")]
	[SerializeField] private float rotationSpeed = 90f;
	[SerializeField] private float floatAmplitude = 0.5f;
	[SerializeField] private float floatSpeed = 2f;
	private Vector3 startPosition;
	private float floatOffset;
	private void Start()
	{
		if (InventoryManager.Instance != null && InventoryManager.Instance.HasEnergyCore(coreId))
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
		if (!string.IsNullOrEmpty(puzzleId))
		{
			StartCoroutine(CheckPuzzleAfterDelay());
		}
		else
		{
			startPosition = transform.position;
			floatOffset = Random.Range(0f, Mathf.PI * 2f);
		}
	}
	private System.Collections.IEnumerator CheckPuzzleAfterDelay()
	{
		yield return null;
		yield return null;
		yield return null;
		if (!string.IsNullOrEmpty(puzzleId))
		{
			var puzzleManager = FindObjectOfType<PuzzleManager>();
			if (puzzleManager != null)
			{
				var puzzle = puzzleManager.GetPuzzle(puzzleId);
				if (puzzle != null && puzzle.IsSolved)
				{
					gameObject.SetActive(false);
					isCollected = true;
                    Collider col = GetComponent<Collider>();
                    if (col != null)
                    {
                        col.enabled = false;
                    }
                    yield break;
				}
			}
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
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PlaySFX(AudioManager.Instance.GetItemCollectSound(), transform.position);
		}
		if (InventoryManager.Instance != null)
		{
			InventoryManager.Instance.AddEnergyCore(coreId);
		}
		gameObject.SetActive(false);
		EventBus.InvokeEnergyCoreCollected(gameObject);
		EventBus.InvokeItemCollected(gameObject);
		Collider col = GetComponent<Collider>();
		if (col != null)
		{
			col.enabled = false;
		}
		Destroy(gameObject, 2f);
	}
	public string CoreId => coreId;
	public bool IsCollected => isCollected;
}
