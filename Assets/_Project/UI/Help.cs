using UnityEngine;

public class Help : MonoBehaviour
{
	[SerializeField] private GameObject helpPanel;
	[SerializeField] private GameObject helpLabel;
	private bool needShow;

	private void Awake()
	{
		needShow = false;
		ToggleHelp();
	}

	public void ToggleHelp()
	{
		helpPanel.SetActive(needShow);
		helpLabel.SetActive(!needShow);
		needShow = !needShow;
	}
}
