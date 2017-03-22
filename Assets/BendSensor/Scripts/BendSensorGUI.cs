using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Draw sensor input.

public class BendSensorGUI : MonoBehaviour
{
    // BendSensor game object to connect with, linked in Unity Editor
    public BendSensor bendSensor;
	public string textToDisplay = "Nothing";

    void OnGUI ()
    {
        GUI.skin.label.fontSize = 40;

		if (!bendSensor.connected) {
			GUI.Label (new Rect (12, 30, Screen.width, Screen.height),
				"No bend sensor currently connected."
			);
		}

        GUI.Label (new Rect (12, 8, Screen.width, Screen.height),
			bendSensor.fingers[0].ToString() + "," +
			bendSensor.fingers[1].ToString() + "," +
			bendSensor.fingers[2].ToString() + "," +
			bendSensor.fingers[3].ToString() + "," +
			bendSensor.fingers[4].ToString() + "," +
			bendSensor.roll.ToString()
        );

    }

    void Update ()
    {
    }
}
