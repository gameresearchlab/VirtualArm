/******************************************************************************\
* KP: INHERITED FROM LEAP's HANDCONTROLLER.CS. MODIFIED THE CLASS TO WORK WITH *
* BEND SENSOR IN ABSENCE OF LEAP. Hand moves based on band-sensor input   						   *
* input. Hand gestures estures are played as pre-recorded leap animations               *
* 
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections.Generic;
using Leap;

/**
* The Controller object that instantiates hands and tools to represent the hands and tools tracked
* by the Leap Motion device.
*
* HandController is a Unity MonoBehavior instance that serves as the interface between your Unity application
* and the Leap Motion service.
*
* The HandController script is attached to the HandController prefab. Drop a HandController prefab 
* into a scene to add 3D, motion-controlled hands. The hands are placed above the prefab at their 
* real-world relationship to the physical Leap device. You can change the transform of the prefab to 
* adjust the orientation and the size of the hands in the scene. You can change the 
* HandController.handMovementScale property to change the range
* of motion of the hands without changing the apparent model size.
*
* When the HandController is active in a scene, it adds the specified 3D models for the hands to the
* scene whenever physical hands are tracked by the Leap Motion hardware. By default, these objects are
* destroyed when the physical hands are lost and recreated when tracking resumes. The asset package
* provides a variety of hands that you can use in conjunction with the hand controller. 
*/
public class BendSensorHandController : HandController {

  public HandModel bendSensorHandModel;
  private bool flag_initialized_1 = false;
  private long prev_graphics_id_1 = 0;
  private long prev_physics_id_1  = 0;
  public BendSensor bendSensor;
  public float previousValue = 0;
  public AnimationEnum _lastGesture = AnimationEnum.Fist;
  public TextAsset[] recordingAssets;
  private Dictionary<AnimationEnum,TextAsset> gestureRecordings;

  /** Creates a new Leap Controller object. */
  void Awake() {
	leap_controller_ = new Controller();
	// KP: If there is no Leap, use leap recording
	enableRecordPlayback = true;
    // KP: mapping Myo poses to Leap motion recording files
	gestureRecordings = new Dictionary<AnimationEnum, TextAsset> ();
	gestureRecordings.Add (AnimationEnum.Rest, recordingAssets [0]);			
	gestureRecordings.Add (AnimationEnum.Fist, recordingAssets [1]);
	gestureRecordings.Add (AnimationEnum.Thumb, recordingAssets [2]);			
	gestureRecordings.Add (AnimationEnum.Index, recordingAssets [3]);
	gestureRecordings.Add (AnimationEnum.Middle, recordingAssets [4]);			
	gestureRecordings.Add (AnimationEnum.Ring, recordingAssets [5]);
	gestureRecordings.Add (AnimationEnum.Pinky, recordingAssets [6]);
	gestureRecordings.Add (AnimationEnum.Unknown, recordingAssets [7]);			
}

  /** Initalizes the hand and tool lists and recording, if enabled.*/
  void Start() {
    // Initialize hand lookup tables.
    hand_graphics_ = new Dictionary<int, HandModel>();
    hand_physics_ = new Dictionary<int, HandModel>();
	playerLoop = true;
    tools_ = new Dictionary<int, ToolModel>();
  }


  /** KP: Unlike in the superclass, don't creates a new HandModel 
      instance. Instead, use the one already attached to MyoHand **/
  protected override HandModel CreateHand(HandModel model) {
 	HandModel hand_model = bendSensorHandModel;
	hand_model.gameObject.SetActive(true);
    Leap.Utils.IgnoreCollisions(hand_model.gameObject, gameObject);
    if (handParent != null) {
      hand_model.transform.SetParent(handParent.transform);
    }
    return hand_model;
  }

  /**
  * KP: Load new hand gesture recording, if gesture changed.
  */ 
  protected bool UpdateHandGesture() {
	if (bendSensor != null && bendSensor.gesture != _lastGesture && enableRecordPlayback) {
		_lastGesture = bendSensor.gesture ;
		if (_lastGesture == AnimationEnum.Unknown || _lastGesture == AnimationEnum.Rest) {
			playerLoop = true;
		} else { 
			playerLoop = false;
		}
		bool success = gestureRecordings.TryGetValue(_lastGesture, out recordingAsset);
		
		if (success)
			animationPlayer_.Load (recordingAsset);
		else
			GameObject.Find ("GUI").GetComponent<BendSensorGUI> ().textToDisplay = "Error loading animation "+ _lastGesture.ToString();
		return true;
	 }
	 return false;
  }	


  /** Updates the graphics objects. */
  void Update() {

	//update wrist roll (primitive, needs improvements)
	transform.rotation = new Quaternion (0,0,1,Mathf.Cos(Mathf.PI+bendSensor.roll*Mathf.PI));

	// choose animation template and goal frame, based on bendSensor input
	UpdateAnimationFrame ();
	
	// get next animation frame, based on updated settings
	Frame frame = GetFrame();
    
	// apply animation frame to hand model
	if (frame.Id != prev_graphics_id_1)
    {
      UpdateHandModels(hand_graphics_, frame.Hands, leftGraphicsModel, rightGraphicsModel);
      prev_graphics_id_1 = frame.Id;
    }
  }

  void UpdateAnimationFrame() {
		UpdateHandGesture ();	
		UpdateAnimator (bendSensor.current_value,previousValue);
		previousValue = bendSensor.current_value;
  }


  /** Updates the physics objects */
  void FixedUpdate() {

	// get next animation frame, based on updated settings
    Frame frame = GetFrame();

    if (frame.Id != prev_physics_id_1)
    {
      UpdateHandModels(hand_physics_, frame.Hands, leftPhysicsModel, rightPhysicsModel);
      UpdateToolModels(tools_, frame.Tools, toolModel);
      prev_physics_id_1 = frame.Id;
    }
  }

  /** Returns a copy of the hand model list. */
  public HandModel[] GetAllGraphicsHands() {
    if (hand_graphics_ == null)
      return new HandModel[0];

    HandModel[] models = new HandModel[hand_graphics_.Count];
    hand_graphics_.Values.CopyTo(models, 0);
    return models;
  }
		  
}
