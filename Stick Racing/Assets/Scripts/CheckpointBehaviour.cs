using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CheckpointBehaviour : MonoBehaviour {

	public int CheckpointID;
	private GameModeManager GMM_Script;

	List<GameObject> Cars = new List<GameObject>();

	// Use this for initialization
	void Start () {

		GMM_Script = GameObject.Find ("GameManager").GetComponent<GameModeManager> ();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider cc_Hit)
	{
		if(cc_Hit.gameObject.tag == "Car")
		{
			GMM_Script.CheckpointChecker(CheckpointID);
		}

		if(GMM_Script.ActiveGameMode == GameModeManager.GameMode.Race)
		{
			if(cc_Hit.gameObject.tag == "Car" || cc_Hit.gameObject.tag == "AI")
			{
				Cars.Add(cc_Hit.gameObject);
				Debug.Log(cc_Hit.gameObject.name);

				if(cc_Hit.gameObject.transform.parent.name == "Car")
				{

					Text CarPosition = GameObject.Find("PositionText").GetComponent<Text>();
					GMM_Script.CurrentPosition = Cars.Count;
					CarPosition.text = "Position: " + Cars.Count.ToString() + " / " + GMM_Script.NumberofCars.ToString();
				}

				if(Cars.Count == GMM_Script.NumberofCars)
				{
					Cars.Clear();
				}
			}
		}


	}
}
