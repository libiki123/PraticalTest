using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TMPro;

[RequireComponent(typeof(ARSessionOrigin))]
public class AR_ScaleController : MonoBehaviour
{
	[SerializeField]
	[Tooltip("The slider used to control the scale factor.")]
	Slider m_Slider;

	public Slider slider
	{
		get { return m_Slider; }
		set { m_Slider = value; }
	}

	[SerializeField]
	[Tooltip("The text used to display the current scale factor on the screen.")]
	TextMeshProUGUI m_Text;

	public TextMeshProUGUI text
	{
		get { return m_Text; }
		set { m_Text = value; }
	}

	[SerializeField]
	[Tooltip("Minimum scale factor.")]
	public float m_Min = .1f;

	public float min
	{
		get { return m_Min; }
		set { m_Min = value; }
	}

	[SerializeField]
	[Tooltip("Maximum scale factor.")]
	public float m_Max = 10f;

	public float max
	{
		get { return m_Max; }
		set { m_Max = value; }
	}

	public void OnSliderValueChanged()
	{
		if (slider != null)
			scale = slider.value * (max - min) + min;
	}

	float scale
	{
		get
		{
			return m_SessionOrigin.transform.localScale.x;
		}
		set
		{
			m_SessionOrigin.transform.localScale = Vector3.one * value;
			UpdateText();
		}
	}

	void Awake()
	{
		m_SessionOrigin = GetComponent<ARSessionOrigin>();
	}

	void OnEnable()
	{
		if (slider != null)
			slider.value = (scale - min) / (max - min);
		UpdateText();
	}

	void UpdateText()
	{
		if (text != null)
			text.text = "Scale: " + scale;
	}

	ARSessionOrigin m_SessionOrigin;
}