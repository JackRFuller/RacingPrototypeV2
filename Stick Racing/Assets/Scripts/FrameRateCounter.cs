using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FrameRateCounter : MonoBehaviour {

	private float updateInterval = 0.5F;
	private float accum = 0;
	private int frames = 0;
	private float timeleft;

	// Use this for initialization
	void Start () {

		timeleft = updateInterval;
	
	}
	
	// Update is called once per frame
	void Update () {

		timeleft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		++frames;

		if(timeleft < 0.0F)
		{
			float fps = accum/frames;
			string format = System.String.Format("{0:F2} FPS", fps);
			GetComponent<Text>().text = format;

			if(fps < 30)
			{
				GetComponent<Text>().color = Color.yellow;
			}
			else
			{
				if(fps < 10)
				{
					GetComponent<Text>().color = Color.red;
				}
				else
				{
					GetComponent<Text>().color = Color.green;
				}
			}

			timeleft = updateInterval;
			accum = 0.0F;
			frames = 0;
		}
	
	}
}
