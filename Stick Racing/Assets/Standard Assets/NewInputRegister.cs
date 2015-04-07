using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NewInputRegister : MonoBehaviour {

	public float in_Horizontal;
	public bool bl_Braking;
	public float fl_TapCooler;
	public int in_TapCount;

	public float InputSensitivity = 0.04F;




	// Use this for initialization
	void Start () {

		bl_Braking = false;


	
	}
	
	// Update is called once per frame
	void Update () {

		SteerCar ();

		if(Input.touchCount <= 0)
		{
			in_Horizontal = 0;
		}

	}
	
	void SteerCar()
	{

		if(Input.touchCount > 0)
		{			
			Ray _ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
							
			RaycastHit hit;
			
			if(Physics.Raycast(_ray, out hit))
			{	
				if(hit.collider.name == "Left" && in_Horizontal > -1)
				{
					in_Horizontal -= InputSensitivity;
				}

				if(hit.collider.name == "Right" && in_Horizontal < 1)
				{
					in_Horizontal += InputSensitivity;
				}
			}
		}
	}
	
}
