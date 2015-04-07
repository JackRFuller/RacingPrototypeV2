#pragma strict

public var Horizontal : float;
public var InputSensitivity : float;

function Start () {

	InputSensitivity = 0.16;

}

function Update () {

SteerCar();

if(Input.touchCount <= 0)
{
	Horizontal = 0;
}


}

function SteerCar()
{
	if(Input.touchCount > 0)
		{			
			var _ray : Ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
							
			var hit : RaycastHit;
			
			if(Physics.Raycast(_ray, hit, 100))
			{	
				if(hit.collider.name == "Left" && Horizontal > -1)
				{
					Horizontal -= InputSensitivity;
					
				}

				if(hit.collider.name == "Right" && Horizontal < 1)
				{
					Horizontal += InputSensitivity;
					
				}
			}
		}
}