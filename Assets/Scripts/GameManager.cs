using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public enum MatchState { START, PAUSE }
public enum MatchResult { WON, LOST, DRAW }

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	public MatchState currentState;

	[Header("Game Stats")] 
	[SerializeField] private int numberOfMatches = 5;
    [SerializeField] private int timeLimitPerMatch = 140;

	[Header("Players")] 
    [SerializeField] private GameObject playerGO;
    [SerializeField] private GameObject enemyGO;

	[Header("Game UI")] 
	[SerializeField] private GameObject gameplayFrame;
	[SerializeField] private GameObject resultBoard;
    [SerializeField] private TextMeshProUGUI timerTextUI;
    [SerializeField] private TextMeshProUGUI macthCountTextUI;
    [SerializeField] private TextMeshProUGUI resultTextUI;
    [SerializeField] private TextMeshProUGUI playerScoreTextUI;
    [SerializeField] private TextMeshProUGUI enemyScoreTextUI;
	[SerializeField] private Button arSceneButton;
	[SerializeField] private Button nextMacthButton;
	[SerializeField] private Button playAgainButton;
	[SerializeField] private Button exitButton;

	private Controller playerController;
    private Controller enemyController;

    private static int currentMatch = 1;
	private float startTime;

	private static int playerWinsCount = 0;
	private static int enemyWinsCount = 0;

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
		SetupMatch();
    }

	private void Update()
	{
		if(currentState == MatchState.START)		// Start timer when the match begin
		{
			timerTextUI.text = ((int)(Time.time - startTime)).ToString() + " s";
			if(Time.time - startTime >= timeLimitPerMatch)
			{
				MatchEnded(MatchResult.DRAW, null);
			}
		}
	}

	private void SetupMatch()
	{
		currentState = MatchState.START;
		startTime = Time.time;
		macthCountTextUI.text = "Match " + currentMatch;

		if (currentMatch % 2 == 0)       // if current macth is even player = defender, enemy = attacker
		{
			playerController.isAttacking = false;
			enemyController.isAttacking = true;
		}
		else 
		{
			playerController.isAttacking = true;
			enemyController.isAttacking = false;
		}

		playerController.OnMatchStarted();      // Reset player/enemy when starting a new match
		enemyController.OnMatchStarted();
	}

	public void MatchEnded(MatchResult result, Controller attacker)
	{
		currentState = MatchState.PAUSE;
		Time.timeScale = 0f;
		gameplayFrame.SetActive(false);
		resultBoard.SetActive(true);
		bool playerWon = false;

		if (result != MatchResult.DRAW)
		{
			if (result == MatchResult.WON && attacker.isPlayer)
			{
				playerWon = true;
				playerWinsCount++;
			}
			else if (result == MatchResult.WON && !attacker.isPlayer)
			{
				playerWon = false;
				enemyWinsCount++;
			}

			if (result == MatchResult.LOST && attacker.isPlayer)
			{
				playerWon = false;
				enemyWinsCount++;
			}
			else if (result == MatchResult.LOST && !attacker.isPlayer)
			{
				playerWon = true;
				playerWinsCount++;
			}
		}

		playerScoreTextUI.text = playerWinsCount.ToString();
		enemyScoreTextUI.text = enemyWinsCount.ToString();

		if (currentMatch < numberOfMatches)     // If the current match = number of matchse the game end
		{
			nextMacthButton.gameObject.SetActive(true);
			playAgainButton.gameObject.SetActive(false);
			exitButton.gameObject.SetActive(false);

			if(result == MatchResult.DRAW)
			{
				resultTextUI.text = "DRAW";
			}
			else if (playerWon)
			{
				resultTextUI.text = "You Won";
			}
			else
			{
				resultTextUI.text = "You Lost";
			}

			currentMatch++;
		}
		else
		{
			nextMacthButton.gameObject.SetActive(false);
			playAgainButton.gameObject.SetActive(true);
			exitButton.gameObject.SetActive(true);

			if (playerWinsCount == enemyWinsCount)
			{
				resultTextUI.text = "DRAW";
				//////////////////////////// PENALTY //////////////////////////////////
			}
			else if(playerWinsCount > enemyWinsCount)
			{
				resultTextUI.text = "Player WON";
			}
			else
			{
				resultTextUI.text = "Enemy WON";
			}
		}
	}

	public void ARButtonPressed()
	{
		if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
		{
			Permission.RequestUserPermission(Permission.Camera);
		}

		ResetGame();
		SceneManager.LoadScene("AR", LoadSceneMode.Single);
	}

	public void NextMatchPressed()
	{
		Time.timeScale = 1f;
		ResetScene();
	}

	public void PlayAgainPressed()
	{
		ResetGame();
		ResetScene();
	}

	void ResetScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void ExitPressed()
	{
		Application.Quit();
	}

	public void ResetGame()
	{
		currentMatch = 1;
		playerWinsCount = 0;
		enemyWinsCount = 0;
		
		if(Time.timeScale == 0) Time.timeScale = 1f;
	}
}
