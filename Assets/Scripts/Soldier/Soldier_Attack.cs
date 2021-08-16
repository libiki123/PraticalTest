using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier_Attack : Soldier
{
	

	public GameObject opponentGoal;
	public Controller owner;

	[SerializeField] private GameObject ballGO;
	[SerializeField] private float carryingSpeed = 0.75f;

	private BallBehavior ball;
	public bool isHoldingBall = false;

	protected override void Start()
	{
		base.Start();

		ballGO = GameObject.FindGameObjectWithTag("Ball");
		ball = ballGO.GetComponent<BallBehavior>();
		highlightRing.Stop();
	}

	protected override void Activated()		// soldier behaviour during Activate
	{
		base.Activated();
		
		if (!isHoldingBall && ball.isAvailable)
		{
			// get the ball
			GetToDestinationWithSpeedAndIgnoreCollision(ball.transform.position, normalSpeed);
		}
		else if(!isHoldingBall && !ball.isAvailable) // && !isReceivingBall)
		{
			// walk toward the goal/fence
			Vector3 destination = new Vector3(transform.position.x, transform.position.y, opponentGoal.transform.position.z); 
			GetToDestinationWithSpeedAndIgnoreCollision(destination, normalSpeed, true);
		}
		else if (isHoldingBall)
		{
			// get to the opponent goal
			GetToDestinationWithSpeedAndIgnoreCollision(opponentGoal.transform.position, carryingSpeed);
		}

	}

	protected override IEnumerator InActivated()        // soldier behaviour during InActivate
	{
		soldierBody.GetComponent<MeshRenderer>().material = soldierInActivateColor;
		GetToDestinationWithSpeedAndIgnoreCollision(Vector3.zero,0, true);

		return base.InActivated();
	}

	protected override void OnCollisionEnter(Collision collision)
	{
		base.OnCollisionEnter(collision);

		if(collision.gameObject.tag == "Fence")
		{
			Destroy(gameObject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Goal")
		{
			if (isHoldingBall)
			{
				GameManager.Instance.MatchEnded(MatchResult.WON, owner);
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}

	public void GotBall()
	{
		isHoldingBall = true;
		highlightRing.Play();
	}

	public void LostBall()
	{
		isHoldingBall = false;
		highlightRing.Clear();
		highlightRing.Stop();
	}

	public void Caught()
	{
		PassingBall();
		UpdateState(SoldierState.INACTIVATE);
	}

	private void PassingBall()
	{
		GameObject[] soldiers = GameObject.FindGameObjectsWithTag("Attacker");
		if (soldiers.Length != 0)
		{
			GameObject closestSoldier = null;
			float closestSoldierDistance = Mathf.Infinity;
			foreach (GameObject soldier in soldiers)
			{
				if (soldier != gameObject)
				{
					if(soldier.GetComponent<Soldier_Attack>().currentState == SoldierState.ACTIVATE)
					{
						if (Vector3.Distance(transform.position, soldier.transform.position) < closestSoldierDistance)
						{
							closestSoldier = soldier;
						}
					}
				}
			}

			if (closestSoldier != null)
			{
				LostBall();
				ball.PassToNewOwner(closestSoldier);
			}
			else
			{
				GameManager.Instance.MatchEnded(MatchResult.LOST, owner);
			}
		}
	}

	public void ReActivate()
	{
		Activated();
	}

	public bool GetIsHoldingBall()
	{
		return isHoldingBall;
	}
}
