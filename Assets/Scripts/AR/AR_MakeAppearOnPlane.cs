using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


[RequireComponent(typeof(ARSessionOrigin))]
[RequireComponent(typeof(ARRaycastManager))]
public class AR_MakeAppearOnPlane : MonoBehaviour
{
	[SerializeField] AR_ButtonManager buttonManager;
	static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

	ARSessionOrigin m_SessionOrigin;
	ARRaycastManager m_RaycastManager;

	[SerializeField]
	[Tooltip("A transform which should be made to appear to be at the touch point.")]
	Transform m_Content;

	public Transform content
	{
		get { return m_Content; }
		set { m_Content = value; }
	}

	[SerializeField]
	[Tooltip("The rotation the content should appear to have.")]
	Quaternion m_Rotation;

	public Quaternion rotation
	{
		get { return m_Rotation; }
		set
		{
			m_Rotation = value;
			if (m_SessionOrigin != null)
				m_SessionOrigin.MakeContentAppearAt(content, content.transform.position, m_Rotation);
		}
	}

	void Awake()
	{
		m_SessionOrigin = GetComponent<ARSessionOrigin>();
		m_RaycastManager = GetComponent<ARRaycastManager>();
		buttonManager = GetComponent<AR_ButtonManager>();
	}

	void Update()
	{
		if (Input.touchCount == 0 || m_Content == null || !buttonManager.moveModeEnabled)
			return;

		var touch = Input.GetTouch(0);
		bool isOverUI = touch.position.IsPointOverUIObject();

		if (!isOverUI && m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
		{
			var hitPose = s_Hits[0].pose;
			m_SessionOrigin.MakeContentAppearAt(content, hitPose.position, m_Rotation);
		}
	}
}