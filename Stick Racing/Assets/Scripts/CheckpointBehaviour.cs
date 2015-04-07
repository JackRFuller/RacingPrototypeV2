using UnityEngine;
using System.Collections;

public class CheckpointBehaviour : MonoBehaviour {

	public int CheckpointID;
	private GameModeManager GMM_Script;

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
	}
}
