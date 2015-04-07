using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Transform Target;
	public float distance;
	public float XOffset;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		float TempZ = Target.position.y + distance;
		transform.position = new Vector3(Target.position.x,TempZ,Target.position.z);


	
	}
}
