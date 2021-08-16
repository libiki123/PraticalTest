using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum MatchState { START, WON, LOST, DRAW}

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	public MatchState currentState;
	public bool ARMode = false;

    [SerializeField] private int numberOfMatches = 5;
    [SerializeField] private int timeLimitPerMatch = 140;

	[Space]
    [SerializeField] private GameObject playerGO;
    [SerializeField] private GameObject enemyGO;
    [SerializeField] private TextMeshProUGUI timerTextUI;
    [SerializeField] private TextMeshProUGUI macthCountTextUI;

    private Controller playerController;
    private Controller enemyController;

    private int currentMatch;
	private float startTime;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}

		playerGO = GameObject.FindGameObjectWithTag("Player");
        enemyGO = GameObject.FindGameObjectWithTag("Enemy");

        playerController = playerGO.GetComponent<Controller>();
        enemyController = enemyGO.GetComponent<Controller>();
	}

	void Start()
    {
        currentMatch = 1;
        SetupMatch();
    }

	private void Update()
	{
		if(currentState == MatchState.START)		// Start timer when the match begin
		{
			timerTextUI.text = ((int)(Time.time - startTime)).ToString() + " s";
			if(Time.time - startTime >= 140)
			{
				Debug.Log("DRAW"); //////////////////////////// TODO ///////////////////////////////
				MatchEnded();
			}
		}
	}

	private void SetupMatch()
	{
		currentState = MatchState.START;
		startTime = Time.time;
		macthCountTextUI.text = "Match " + currentMatch;

		if (currentMatch == 1)       // Have player as attacker first match, next match swicth them around
		{
			playerController.isAttacking = true;
			enemyController.isAttacking = false;
		}
		else 
		{
			playerController.isAttacking = !playerController.isAttacking;
			enemyController.isAttacking = !enemyController.isAttacking;
		}

		playerController.OnMatchStarted();      // Reset player/enemy when starting a new match
		enemyController.OnMatchStarted();
	}

	private void MatchEnded()
	{
		if (currentMatch < numberOfMatches)		// If the current match = number of matchse the game end
		{

			SetupMatch();
		}
		else
		{
			EndGame();
		}
	}

	private void EndGame()
	{

	}

}
