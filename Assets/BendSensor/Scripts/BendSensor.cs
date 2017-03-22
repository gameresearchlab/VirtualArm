using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BendSensor : MonoBehaviour {
	
	[Range(0,1)]
	public float[] fingers = new float[5];

	[Range(0,1)]
	public float roll = 0;
	public AnimationEnum gesture = AnimationEnum.Unknown;
	public bool connected = false;
	public float current_value = 0;

	void Start () {
		
	}

	void Update () {

		// thumb
		if (fingers[0] > 0) {
			current_value = fingers [0];
			gesture = AnimationEnum.Thumb;
		}
		// index finger
		if (fingers[1] > 0) {
			current_value = fingers [1];
			gesture = AnimationEnum.Index;
		}

		// middle finger
		if (fingers[2] > 0) {
			current_value = fingers [2];
			gesture = AnimationEnum.Middle;
		}

		// ring finger
		if (fingers[3] > 0) {
			current_value = fingers [3];
			gesture = AnimationEnum.Ring;
		}

		// pinky finger
		if (fingers[4] > 0) {
			current_value = fingers [4];
			gesture = AnimationEnum.Pinky;
		}

		// fist
		if (fingers[0] > 0 && 
			fingers[1] > 0 &&
			fingers[2] > 0 && 
			fingers[3] > 0 &&
			fingers[4] > 0) {
			current_value = fingers [0];
			gesture = AnimationEnum.Fist;
		}
			
	}
}
