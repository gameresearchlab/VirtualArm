using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;

public class SerialPortReader : MonoBehaviour {

	SerialPort stream = new SerialPort("COM4", 9600); //Set the port (com4) and the baud rate (9600, is standard on most devices)
	public BendSensor bendSensor;

	void Start () {
		stream.Open(); //Open the Serial Stream.
		if (stream.IsOpen) 
			bendSensor.connected = true; 
	}
		
	void Update () {
		string value = stream.ReadLine(); //Read serial

		try {
			string[] vec = value.Split(',');
			bendSensor.fingers[0] = float.Parse(vec[0]);  
			bendSensor.fingers[1] = float.Parse(vec[1]);
			bendSensor.fingers[2] = float.Parse(vec[2]);
			bendSensor.fingers[3] = float.Parse(vec[3]);
			bendSensor.fingers[4] = float.Parse(vec[4]);
			bendSensor.roll = float.Parse(vec[5]);
		} catch (Exception e) {
			// nothing for now
		}

		stream.BaseStream.Flush(); //Clear the serial information so we are sure we get new information.

	}
}
