using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum SoldierState { ACTIVATE, INACTIVATE}

public class Soldier : MonoBehaviour
{
	public SoldierState currentState;
	public Material soldierColor;
	public Material soldierInActivateColor;

	[SerializeField] protected Transform soldierBody;
	[SerializeField] protected float spawnTime = 0.5f;
	[SerializeField] protected float reactivateTime = 2.5f;
	[SerializeField] protected float normalSpeed = 1.5f;
	[SerializeField] protected ParticleSystem highlightRing;


	protected NavMeshAgent navMeshAgent;

	private void Awake()
	{
        navMeshAgent = GetComponent<NavMeshAgent>();
		soldierBody = transform.Find("SoldierBody");
		highlightRing = gameObject.GetComponentInChildren<ParticleSystem>();
	}

	protected virtual void Start()
	{
		soldierBody.GetComponent<MeshRenderer>().material = soldierColor;
		StartCoroutine(OnSpawned());
	}

	IEnumerator OnSpawned()
	{
		yield return new WaitForSeconds(spawnTime);
		UpdateState(SoldierState.ACTIVATE);
	}

	protected void UpdateState(SoldierState state)
	{
		currentState = state;

		switch (currentState)
		{
			case SoldierState.ACTIVATE:
				Activated();
				break;
			case SoldierState.INACTIVATE:
				StartCoroutine(InActivated());
				break;
		}
	}

	protected virtual void Activated()
	{

	}

	protected virtual IEnumerator InActivated()
	{
		yield return new WaitForSeconds(reactivateTime);
		soldierBody.GetComponent<MeshRenderer>().material = soldierColor;
		UpdateState(SoldierState.ACTIVATE);
	}

	protected virtual void OnCollisionEnter(Collision collision)
	{

	}

	protected void GetToDestinationWithSpeedAndIgnoreCollision(Vector3 destination, float speed, bool ignoreCollision = false)
	{
		if (destination != Vector3.zero)
		{
			if (navMeshAgent.isStopped)
			{
				navMeshAgent.isStopped = false;
			}
			navMeshAgent.destination = destination;
		}
		else
		{
			navMeshAgent.isStopped = true;
		}

		navMeshAgent.speed = speed != 0 ? speed : 0;

		Physics.IgnoreLayerCollision(gameObject.layer, gameObject.layer, ignoreCollision);
	}
}
