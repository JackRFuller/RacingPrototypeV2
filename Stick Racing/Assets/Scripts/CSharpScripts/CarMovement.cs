using UnityEngine;
using System.Collections;

public class CarMovement : MonoBehaviour {

	public WheelCollider[] WheelColliders;

	private float NormalPower;

	private float accerlerationTimer;

	private float wheelRadius = 0.4F;
	private float suspensionRange = 0.1F;
	private float suspensionDamper = 50;
	private float suspensionSprintFront = 185000;
	private float suspensionSprintRear = 9000;

	private Vector3 dragMultiplier;

	public float Throttle = 0;
	private float Steer = 0;

	private Transform CenterofMass;

	public Transform[] FrontWheels;
	public Transform[] RearWheels;

	private Wheel[] Wheels;

	private WheelFrictionCurve WFC;

	public float TopSpeed = 160;
	public int NumberofGears = 5;

	public int maximumTurn = 15;
	public int minimumTurn = 10;

	public float ResetTime = 5.0F;
	private float ResetTimer = 0.0F;

	private float[] EngineForceValues;
	private float[] GearSpeeds;

	private int CurrentGear;
	private float CurrentEnginePower = 0.0F;

	private float inititalDragMultiplierX = 10.0F;

	private bool canSteer;
	private bool canDrive;

	public class Wheel
	{
		public WheelCollider WheelCol;
		public Transform WheelGraphic;
		public Transform TyreGraphic;
		public bool DriveWheel = false;
		public bool SteerWheel = false;
		public Vector3 WheelVelo = Vector3.zero;
		public Vector3 GroundSpeed = Vector3.zero;

	}

	// Use this for initialization
	void Start () {

		accerlerationTimer = Time.time;

//		SetupWheelColliders ();

		SetupFrictionCurve ();

		SetupCentreOfMass ();

		SetupGears ();

		inititalDragMultiplierX = dragMultiplier.x;
	
	}

	// Update is called once per frame
	void Update () {


		Vector3 relativeVelocity = transform.InverseTransformDirection (rigidbody.velocity);

		GetInput();

		CheckIfCarHasFlipped ();

		UpdateGear (relativeVelocity);


		
	}

	void FixedUpdate()
	{
		Vector3 relativeVelocity = transform.InverseTransformDirection (rigidbody.velocity);

		CalculateState ();

		UpdateFriction (relativeVelocity);

		UpdateDrag (relativeVelocity);

		CalculateEnginePower (relativeVelocity);

		ApplyThrottle (canDrive, relativeVelocity);

		ApplySteering (canSteer, relativeVelocity);

	}

	void ApplySteering(bool canSteer, Vector3 relativeVelocity)
	{
		if(canSteer)
		{
			float turnRadius = 3.0F / Mathf.Sin((90 - (Steer * 30)) * Mathf.Deg2Rad);
			float minMaxTurn = EvalulateSpeedToTurn(rigidbody.velocity.magnitude);
			float turnSpeed = Mathf.Clamp(relativeVelocity.z / turnRadius, -minMaxTurn / 10, minMaxTurn /10);

			transform.RotateAround(transform.position + transform.right * turnRadius * Steer,
			                       transform.up,
			                       turnSpeed * Mathf.Rad2Deg * Time.deltaTime * Steer);

			Vector3 debugStartPoint = transform.position + transform.right * turnRadius * Steer;
			Vector3 debugEndPoint = debugStartPoint + Vector3.up * 5;

			Debug.DrawLine(debugStartPoint, debugEndPoint, Color.red);
		}
	}

	void ApplyThrottle(bool canDrive, Vector3 relativeVelocity)
	{
		if(canDrive)
		{
			float throttleForce = 0;
			float brakeForce = 0;

			if(HaveTheSameSign(relativeVelocity.z, Throttle))
			{
				throttleForce = Mathf.Sign(Throttle) * CurrentEnginePower * rigidbody.mass;
				Debug.Log("Hit");
			}
			else
			{
				brakeForce = Mathf.Sign(Throttle) * EngineForceValues[0] * rigidbody.mass;
			}

			rigidbody.AddForce(transform.forward * Time.deltaTime * (throttleForce + brakeForce));
		}
	}

	void CalculateEnginePower(Vector3 relativeVelcoity)
	{
		if(Throttle == 0)
		{
			CurrentEnginePower -= Time.deltaTime * 200;
		}
		else if(HaveTheSameSign(relativeVelcoity.z, Throttle))
		{
			NormalPower = EvaluateNormPower(NormalPower);
			NormalPower = (CurrentEnginePower / EngineForceValues[EngineForceValues.Length - 1]) * 2;
			CurrentEnginePower += Time.deltaTime * 200 * EvaluateNormPower(NormalPower);
		}
		else
		{
			CurrentEnginePower -= Time.deltaTime * 300;
		}

		if(CurrentGear == 0)
		{
			CurrentEnginePower = Mathf.Clamp(CurrentEnginePower, 0, EngineForceValues[0]);
		}
		else
		{
			CurrentEnginePower = Mathf.Clamp(CurrentEnginePower, EngineForceValues[CurrentGear - 1], EngineForceValues[CurrentGear]);
		}
		
	}

	void UpdateDrag(Vector3 relativeVelocity)
	{
		Vector3 relativeDrag = new Vector3 (-relativeVelocity.x * Mathf.Abs (relativeVelocity.x),
		                                   -relativeVelocity.y * Mathf.Abs (relativeVelocity.y),
		                                   -relativeVelocity.z * Mathf.Abs (relativeVelocity.z));

		Vector3 Drag = Vector3.Scale (dragMultiplier, relativeDrag);

		Drag.x *= TopSpeed / relativeVelocity.magnitude;

		if(Mathf.Abs(relativeVelocity.x) < 5)
		{
			Drag.x = -relativeVelocity.x * dragMultiplier.x;
		}

		rigidbody.AddForce (transform.TransformDirection (Drag) * rigidbody.mass * Time.deltaTime);

	}

	void UpdateFriction(Vector3 relativeVelocity)
	{
		float sqrValue = relativeVelocity.x * relativeVelocity.x;

		//Add extra sideways friction based on the car's turning velocity to avoid slipping
		WFC.extremumValue = Mathf.Clamp (300 - sqrValue, 0, 300);
		WFC.asymptoteValue = Mathf.Clamp (150 - (sqrValue / 2), 0, 150);

		foreach(WheelCollider w in WheelColliders)
		{
			w.sidewaysFriction = WFC;
			w.sidewaysFriction = WFC;
		}


//		foreach(Wheel w in Wheels)
//		{
//			w.WheelCol.sidewaysFriction = WFC;
//			w.WheelCol.sidewaysFriction = WFC;
//		}
	}

	void CalculateState()
	{
		canDrive = true;
		canSteer = true;
//		foreach(Wheel w in Wheels)
//		{
//			if(w.WheelCol.isGrounded)
//			{
//				if(w.SteerWheel)
//				{
//					canSteer = true;
//				}
//				if(w.DriveWheel)
//				{
//					canDrive = true;
//				}
//			}
//		}
	}

	void GetInput()
	{
		Throttle = Input.GetAxis ("Vertical");
		Steer = Input.GetAxis ("Horizontal");
	}

	void CheckIfCarHasFlipped()
	{
		if(transform.localEulerAngles.z > 80 && transform.localEulerAngles.z < 280)
		{
			ResetTimer += Time.deltaTime;
		}
		else
		{
			ResetTimer = 0;
		}

		if(ResetTimer > ResetTime)
		{
			FlipCar();
		}
	}

	void FlipCar()
	{
		transform.rotation = Quaternion.LookRotation (transform.forward);
		transform.position += Vector3.up * 0.5F;
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		ResetTimer = 0;
		CurrentEnginePower = 0;
	}

	void UpdateGear(Vector3 relativeVelocity)
	{
		CurrentGear = 0;
		for(int i = 0; i < NumberofGears - 1; i++)
		{
			if(relativeVelocity.z > GearSpeeds[i])
			{
				CurrentGear = i + 1;
			}
		}
	}



	/// <summary>
	/// Functions Called From Start
	/// </summary>

//	void SetupWheelColliders()
//	{
//		SetupFrictionCurve ();
//
//		int wheelCount = 0;
//
//		foreach(Transform t in FrontWheels)
//		{
//			Wheels[wheelCount] = SetupWheel(t, true);
//			wheelCount++;
//		}
//
//		foreach(Transform t in RearWheels)
//		{
//			Wheels[wheelCount] = SetupWheel(t, false);
//			wheelCount++;
//		}
//	}

	void SetupFrictionCurve()
	{
		WFC = new WheelFrictionCurve ();
		WFC.extremumSlip = 1;
		WFC.extremumValue = 50;
		WFC.asymptoteSlip = 2;
		WFC.asymptoteValue = 25;
		WFC.stiffness = 1;
	}

//	Wheel SetupWheel(Transform wheelTransform, bool isFrontWheel)
//	{
//		GameObject go = new GameObject (wheelTransform + " Collider");
//		go.transform.position = wheelTransform.position;
//		go.transform.parent = transform;
//		go.transform.rotation = wheelTransform.rotation;
//
//		WheelCollider WC = go.AddComponent (typeof(WheelCollider)) as WheelCollider;
//		WC.suspensionDistance = suspensionRange;
//
//		JointSpring js = WC.suspensionSpring;
//
//		if(isFrontWheel)
//		{
//			js.spring = suspensionSprintFront;
//		}
//		else
//		{
//			js.spring = suspensionSprintRear;
//		}
//
//		js.damper = suspensionDamper;
//		WC.suspensionSpring = js;
//
//		Wheel wheel = new Wheel ();
//		wheel.WheelCol = WC;
//		WC.sidewaysFriction = WFC;
//		wheel.WheelGraphic = wheelTransform;
//		wheel.TyreGraphic = wheelTransform.GetComponentsInChildren<Transform>()[1];
//
//		wheelRadius = wheel.TyreGraphic.renderer.bounds.size.y / 2;
//		wheel.WheelCol.radius = wheelRadius;
//
//		if (isFrontWheel)
//		{
//			wheel.SteerWheel = true;
//
//			GameObject go1 = new GameObject(wheelTransform.name + "Steer Column");
//			go1.transform.position = wheelTransform.position;
//			go1.transform.rotation = wheelTransform.rotation;
//			go1.transform.parent = transform;
//			wheelTransform.parent = go1.transform;
//		}
//		else
//		{
//			wheel.DriveWheel = true;
//		}
//
//		return wheel;
//
//	}

	void SetupCentreOfMass()
	{
		if(CenterofMass != null)
		{
			rigidbody.centerOfMass = CenterofMass.localPosition;
		}
	}

	void SetupGears()
	{

		EngineForceValues = new float[NumberofGears];
		GearSpeeds = new float[NumberofGears];

		float tempTopSpeed = TopSpeed;

		for(int i = 0; i < NumberofGears; i++)
		{
			if(i > 0)
			{
				GearSpeeds[i] = tempTopSpeed / 4 + GearSpeeds[i-1];
			}
			else
			{
				GearSpeeds[i] = tempTopSpeed / 4;
			}

			tempTopSpeed -= tempTopSpeed / 4;
		}

		float engineFactor = TopSpeed / GearSpeeds [GearSpeeds.Length - 1];

		for(int i = 0; i < NumberofGears; i++)
		{
			float maxLinearDrag = GearSpeeds[i] * GearSpeeds[i];
			EngineForceValues[i] = maxLinearDrag * engineFactor;
		}
	
	}


	/////////Functions
	/// 

	float Convert_Miles_Per_Hour_To_Meters_Per_Second(float value)
	{
		return value * 0.44704F;
	}

	float Convert_Meters_Per_Second_To_Miles_Per_Hour(float value)
	{
		return value * 2.23693629F;	
	}

	bool HaveTheSameSign(float first, float second)
	{
		if (Mathf.Sign (first) == Mathf.Sign (second))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	float EvalulateSpeedToTurn(float speed)
	{
		if(speed > TopSpeed / 2)
		{
			return minimumTurn;
		}

		float speedIndex = 1 - (speed / (TopSpeed / 2));
		return minimumTurn + speedIndex * (maximumTurn - minimumTurn);
	}

	float EvaluateNormPower(float normPower)
	{
		if(normPower < 1)
		{
			return 10 - normPower * 9;
		}
		else
		{
			return 1.9F - normPower * 0.9F;
		}
	}

//	float GetGearState()
//	{
////		Vector3 relativeVelocity = transform.InverseTransformDirection (rigidbody.velocity);
////		float lowlimit = (CurrentGear == 0 ? 0 : GearSpeeds[CurrentGear - 1]);
////		return(relativeVelocity.z-lowlimit) / (GearSpeeds[CurrentGear - lowlimit]) * (1 - CurrentGear * 0.1F) + CurrentGear * 0.1F;
//	}

}
