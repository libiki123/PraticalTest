using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using System;

public class Controller : MonoBehaviour
{
    public Camera cam;

	[Space]

	public bool isPlayer = false;
	public GameObject opponentGoal;

	[HideInInspector] public int maxEnergyPoints = 6;
	[HideInInspector] public bool isAttacking = false;
	 public List<GameObject> soldierList = new List<GameObject>();

	[Space]
	[SerializeField] private Material userColor;
	[SerializeField] private GameObject soldierAttackPrefab;
	[SerializeField] private GameObject soldierDefendPrefab;
	[SerializeField] private Slider energySliderUI;
	[SerializeField] private TextMeshProUGUI profileTextUI;

	[Header("Clickable Area")]
	[SerializeField] private float minX = 0f;
	[SerializeField] private float maxX = 0f;
	[SerializeField] private float minZ = 0f;
	[SerializeField] private float maxZ = 0f;
	[SerializeField] private LayerMask clickableMask;

	[Space]
	[SerializeField] private float energyRegen = 0.5f;
	[SerializeField] private int attackCost = 2;
	[SerializeField] private int defendCost = 3;

	private float currentEnergy = 0f;
	private float energyRegenPerSec = 0f;
	private int energyCost = 0;

	private void Awake()
	{

	}
	void Start()
    {
		List<GameObject> soldierList = new List<GameObject>();
	}

    void Update()
    {
		CheckUserInput();
		RefillEnergy();
    }

	void CheckUserInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);        // cast a ray from camera to mouse
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 100f, clickableMask))
			{
/*				if (WithinRange(hit.point.x, minX, maxX) && WithinRange(hit.point.z, minZ, maxZ))       // Check if player click on their own field
				{*/
					if (currentEnergy > energyCost)
					{
						SpawnSoldier(hit.point);    // spawn soldier at hit location
						currentEnergy -= energyCost;    // minus the energy cost
					}

				//}
			}
		}



		if (GameManager.Instance.ARMode)
		{
			
		}
	}

	bool TryGetTouchPosition(out Vector2 touchPosition)
	{
		if (Input.touchCount > 0)
		{
			touchPosition = Input.GetTouch(0).position;
			return true;
		}

		touchPosition = default;
		return false;
	}

	private void RefillEnergy()
	{
		if(currentEnergy < maxEnergyPoints)		// if energy not full refill it
		{
			currentEnergy = currentEnergy + energyRegenPerSec * Time.deltaTime;
			energySliderUI.value = currentEnergy / maxEnergyPoints;
		}
	}

	void SpawnSoldier(Vector3 spawnLocation)
	{
		if (isAttacking)
		{
			GameObject soldier = (GameObject)Instantiate(soldierAttackPrefab, spawnLocation, transform.rotation);
			soldierList.Add(soldier);
			soldier.GetComponent<Soldier_Attack>().soldierColor = userColor;
			soldier.GetComponent<Soldier_Attack>().opponentGoal = opponentGoal;
			soldier.GetComponent<Soldier_Attack>().owner = this;
		}
		else
		{
			GameObject soldier = (GameObject)Instantiate(soldierDefendPrefab, spawnLocation, transform.rotation);
			soldierList.Add(soldier);
			soldier.GetComponent<Soldier_Defend>().soldierColor = userColor;
		}
	}

    bool WithinRange(float numberToCheck, float min, float max)
	{
        return numberToCheck >= min && numberToCheck <= max;
	}

	public void OnMatchStarted()
	{
		currentEnergy = 0;			// reset energy
		soldierList.Clear();

		// Update player stats base on isAttacking
		profileTextUI.text = (isPlayer ? "Player" : "Enemy") + (isAttacking ? " - (Attacker)" : " - (Defender)");
		energyRegenPerSec = energyRegen;
		energyCost = isAttacking ? attackCost : defendCost;
	}

	public void MatchEnded()
	{

	}
}
