using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ResetScene()
	{
		Application.LoadLevel (Application.loadedLevelName);
	}

	public void GoHome()
	{
		Application.LoadLevel ("MainMenu");
	}
}
