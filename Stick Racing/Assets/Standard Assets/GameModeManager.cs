using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameModeManager : MonoBehaviour {

	#region

	private Text LapTimerText;
	private Text LapText;
	private Text CountdownText;
	private Text FinishingMessage;
	private Text FinalTime;
	private Image BackgroundImage;

	#endregion

	public enum GameMode
	{
		TimeTrial,
		CheckpointSprint,
		Race,
		Elimination
	}

	public GameMode ActiveGameMode;

	public bool GameModeBegun;
	private bool PreMatchCountdown = false;
	private float RaceTimer;
	private float CountdownTimer = 3;
	private string FinalTimeText;
	public int MaxNumofLaps;
	private int CurrentLap = 0;
	public int CurrentCheckpoint = 0;
	public int MaxNumofCheckpoints;
	private GameObject[] Checkpoints;

	//CheckpointVariables
	public float StartingTimer;
	public float AddOnTime;
	public int[] KeyCheckpoints;
	public GameObject[] CoreCheckpoints;
	private int CheckpointId = 0;


	// Use this for initialization
	void Start () {

		CountdownText = GameObject.Find ("CountdownText").GetComponent<Text> ();

		LapTimerText = GameObject.Find ("TimeText").GetComponent<Text> ();
		LapText = GameObject.Find ("LapText").GetComponent<Text> ();
		LapText.text = "Current Lap: " + CurrentLap.ToString () + "/" + MaxNumofLaps.ToString ();


		FinishingMessage = GameObject.Find ("FinishingMessage").GetComponent<Text> ();
		FinalTime = GameObject.Find ("FinalTime").GetComponent<Text> ();
		BackgroundImage = GameObject.Find ("BackgroundImage").GetComponent<Image> ();

		Checkpoints = GameObject.FindGameObjectsWithTag ("Checkpoint");
		MaxNumofCheckpoints = Checkpoints.Length;

		CoreCheckpoints [CheckpointId].renderer.enabled = true;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (GameModeBegun && PreMatchCountdown)
		{
			if(ActiveGameMode != GameMode.CheckpointSprint)
			{
				LapTimer();
			}
			else
			{
				TimerCountdown();
			}
		}

		if(!PreMatchCountdown)
		{
			InitialCountdown();
		}
	}

	void TimerCountdown()
	{
		StartingTimer -= Time.deltaTime;

		if(StartingTimer <= 0)
		{
			GameModeBegun = false;
			BackgroundImage.enabled = true;
			FinalTime.text = "You failed to Finish!";
			FinalTime.enabled = true;
		}
		
		float in_Time = (int)StartingTimer;
		float minutes = (int)in_Time / 60;
		float seconds = (int)in_Time % 60;
		float fraction = StartingTimer * 1000;
		fraction = fraction % 1000;
		
		LapTimerText.text = "Current Time: " + string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, fraction);
		
		FinalTimeText = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, fraction);
	}

	void InitialCountdown()
	{
		CountdownTimer -= Time.deltaTime;

		if(CountdownTimer <= 2 && CountdownTimer > 1)
		{
			CountdownText.text = "2";
		}

		if(CountdownTimer <= 1 && CountdownTimer > 0)
		{
			CountdownText.text = "1";
		}

		if(CountdownTimer < 0 )
		{
			CountdownText.text = "Go!";
			StartCoroutine(RemoveCountdownText());
			PreMatchCountdown = true;
			GameModeBegun = true;
		}
	}

	IEnumerator RemoveCountdownText()
	{
		yield return new WaitForSeconds (0.5F);
		CountdownText.enabled = false;
	}

	public void CheckpointChecker(int HitCheckpoint)
	{
		if(ActiveGameMode == GameMode.CheckpointSprint)
		{
			if(HitCheckpoint == KeyCheckpoints[CheckpointId])
			{
				StartingTimer += AddOnTime;
				CoreCheckpoints[CheckpointId].renderer.enabled = false;
				CheckpointId++;
				if(CheckpointId >= CoreCheckpoints.Length)
				{
					CheckpointId = 0;
				}

				CoreCheckpoints[CheckpointId].renderer.enabled = true;
			}
		}

		if(HitCheckpoint == 0 && CurrentCheckpoint == 0)
		{
			if(CurrentLap < MaxNumofLaps)
			{
				CurrentCheckpoint += 1;
				CurrentLap += 1;
				LapText.text = "Current Lap: " + CurrentLap.ToString () + "/" + MaxNumofLaps.ToString ();
			}
			else
			{
				Debug.Log("Finished");
				GameModeBegun = false;
				BackgroundImage.enabled = true;
				FinishingMessage.enabled = true;
				if(ActiveGameMode != GameMode.CheckpointSprint)
				{
					FinalTime.text = "You finished in " + FinalTimeText;
				}
				else
				{
					FinalTime.text = "You finished with " + FinalTimeText + " to spare";
				}

				FinalTime.enabled = true;
			}
		}
		else if (HitCheckpoint == CurrentCheckpoint )
		{
			CurrentCheckpoint += 1;
			if(CurrentCheckpoint == MaxNumofCheckpoints)
			{
				CurrentCheckpoint = 0;
			}

		}



	}

	void LapTimer()
	{
		RaceTimer += Time.deltaTime;

		float in_Time = (int)RaceTimer;
		float minutes = (int)in_Time / 60;
		float seconds = (int)in_Time % 60;
		float fraction = RaceTimer * 1000;
		fraction = fraction % 1000;
		
		LapTimerText.text = "Current Time: " + string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, fraction);

		FinalTimeText = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, fraction);
	}

}
