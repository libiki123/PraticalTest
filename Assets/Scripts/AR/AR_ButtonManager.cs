using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using TMPro;

[RequireComponent(typeof(ARPlaneManager))]
public class AR_ButtonManager : MonoBehaviour
{
	ARPlaneManager arPlaneManager;
	
	[SerializeField] GameObject returnButton;
	[SerializeField] TextMeshProUGUI togglePlaneDetectionText;
	[SerializeField] ARSession arSession;
	[SerializeField] TextMeshProUGUI toggleCameraText;
	[SerializeField] TextMeshProUGUI moveModeText;

	[HideInInspector] public bool moveModeEnabled = true;

	void Awake()
	{
		arPlaneManager = GetComponent<ARPlaneManager>();
	}

	public void ReturnButtonPressed()
	{
		GameManager.Instance.ResetGame();
		SceneManager.LoadScene("Main", LoadSceneMode.Single);
	}

	public void TogglePlaneDetection()
	{
		arPlaneManager.enabled = !arPlaneManager.enabled;

		string planeDetectionMessage = "";
		if (arPlaneManager.enabled)
		{
			planeDetectionMessage = "Disable Plane";
			SetAllPlanesActive(true);
		}
		else
		{
			planeDetectionMessage = "Enable Plane";
			SetAllPlanesActive(false);
		}

		if (togglePlaneDetectionText != null)
			togglePlaneDetectionText.text = planeDetectionMessage;
	}

	void SetAllPlanesActive(bool value)
	{
		foreach (var plane in arPlaneManager.trackables)
			plane.gameObject.SetActive(value);
	}

	public void SwitchCamera()
	{

		if (arSession.isActiveAndEnabled)
		{
			toggleCameraText.text = "Disable Camera";
			arSession.enabled = false;
		}
		else
		{
			toggleCameraText.text = "Enable Camera";
			arSession.enabled = true;
		}
	}

	public void ToggleMoveMode()
	{
		if (moveModeEnabled)
		{
			moveModeText.text = "Move Mode: OFF";
			moveModeEnabled = false;
		}
		else
		{
			moveModeText.text = "Move Mode: ON";
			moveModeEnabled = true;
		}
	}
}
