using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier_Defend : Soldier
{
	[SerializeField] private float returnSpeed = 2f;
	[SerializeField] private float detectionRadius = 8.4f;
	[SerializeField] private LayerMask soldierLayerMask;

	private Vector3 spawnedPos;
	private Collider[] hitColliders;
	private Transform target;

	protected override void Start()
	{
		base.Start();

		spawnedPos = transform.position;
		highlightRing.Play();
	}

	private void FixedUpdate()
	{
		if (currentState == SoldierState.ACTIVATE)		// only check when activate 
		{
			hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, soldierLayerMask);	// only check soldier layermask
			if (hitColliders.Length != 0)
			{
				foreach (var hit in hitColliders)        // loop through all soldier hit 
				{
					if (hit.gameObject.tag == "Attacker")
					{
						Soldier_Attack attacker = hit.gameObject.GetComponent<Soldier_Attack>();
						if (attacker != null)
						{
							if (attacker.isHoldingBall && attacker.currentState == SoldierState.ACTIVATE)
							{
								target = hit.gameObject.transform;
								Activated();
							}
						}
					}
				}
			}
		}
	}

	protected override void Activated()
	{
		base.Activated();

		if (!highlightRing.isPlaying)
		{
			highlightRing.Play();
		}

		if(target != null)
		{
			GetToDestinationWithSpeedAndIgnoreCollision(target.position, normalSpeed);
		}
		
	}

	protected override IEnumerator InActivated()
	{
		highlightRing.Clear();
		highlightRing.Stop();

		target = null;
		soldierBody.GetComponent<MeshRenderer>().material = soldierInActivateColor;
		GetToDestinationWithSpeedAndIgnoreCollision(spawnedPos, returnSpeed, true);

		return base.InActivated();
	}

	protected override void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "Attacker")
		{
			Soldier_Attack attacker = collision.gameObject.GetComponent<Soldier_Attack>();
			if (attacker != null)
			{
				if(attacker.isHoldingBall && attacker.currentState == SoldierState.ACTIVATE)
				{
					collision.gameObject.GetComponent<Soldier_Attack>().Caught();
					UpdateState(SoldierState.INACTIVATE);
				}
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, detectionRadius);
	}
}
