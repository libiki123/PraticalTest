using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BallBehavior : MonoBehaviour
{
    public bool isAvailable = true;
    
	[SerializeField] private float ballSpeed = 1.5f;
	[SerializeField] public float ballSpinSpeed = 10f;

	private bool isPassing = false;

	private Transform myOwner;
	private Rigidbody ballRB;
	private Vector3 closestTargetPos;

	private void Awake()
	{
		ballRB = gameObject.GetComponent<Rigidbody>();
	}

	void Update()
    {
        if(myOwner != null && !isPassing)
		{
            // keep the ball infront of the owner
            transform.position = new Vector3(0, transform.position.y, 0) + new Vector3(myOwner.position.x, 0, myOwner.position.z) + myOwner.forward * 2;

            // add ball rotation when owner moving
            float ownerVelocity = myOwner.GetComponent<NavMeshAgent>().velocity.sqrMagnitude;
            if (ownerVelocity > 0.4)
			{
                transform.RotateAround(transform.position, myOwner.right, ballSpinSpeed * Time.deltaTime * ownerVelocity);
            }
        }
    }

	private void FixedUpdate()
	{
		if (isPassing)
		{
			ballRB.velocity = Vector3.Normalize(closestTargetPos - transform.position) * ballSpeed;
		}
	}

	public void PassToNewOwner(GameObject closestSoldier)
	{
		Physics.IgnoreLayerCollision(gameObject.layer, myOwner.gameObject.layer, false);
		transform.parent = null;
		myOwner = null;
		closestTargetPos = closestSoldier.transform.position;
		isAvailable = true;
		isPassing = true;
		NotifyActiveAttackers();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "Attacker")      // wont collide if it already owned or not passing
		{
            myOwner = collision.gameObject.transform;
            transform.parent = myOwner;
			collision.gameObject.GetComponent<Soldier_Attack>().GotBall();
            Physics.IgnoreLayerCollision(gameObject.layer, myOwner.gameObject.layer);
			isAvailable = false;
			isPassing = false;

			NotifyActiveAttackers();
		}
	}

	void NotifyActiveAttackers()
	{
		GameObject[] soldiers = GameObject.FindGameObjectsWithTag("Attacker");
		if (soldiers.Length != 0)
		{
			foreach (GameObject soldier in soldiers)
			{
				if (soldier.GetComponent<Soldier_Attack>().currentState == SoldierState.ACTIVATE)
				{
					soldier.GetComponent<Soldier_Attack>().ReActivate();        // tell all attackers to reroute their destination
				}
			}
		}
	}

}
