using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class LeapControl : MonoBehaviour {

	private Control controller;
	private GameObject topCamera;

	public const float TRIGGER_DISTANCE_RATIO = 0.7f;
	
	public float forceSpringConstant = 100.0f;
	public float magnetDistance = 10.0f;
	
	protected bool pinching_;

	Leap.Controller leap_controller;

	// Use this for initialization
	void Start () {
		controller = GameObject.Find ("TopCamera").GetComponent<Control> ();
		topCamera = GameObject.Find ("TopCamera");
		pinching_ = false;
		leap_controller = new Leap.Controller ();
		leap_controller.EnableGesture (Gesture.GestureType.TYPE_SWIPE);
		//leap_controller.Config.SetFloat ("Gesture.swipe.MinVelocity", 500f);
	}
	
	// Update is called once per frame
	void Update () {
		bool trigger_pinch = false;
		HandModel hand_model = GetComponent<HandModel>();
		Hand leap_hand = hand_model.GetLeapHand();
		//if (leap_controller.IsGestureEnabled(Gesture.GestureType.TYPE_SWIPE))				
		//	print ("yes");
		Frame frame = leap_controller.Frame();
		foreach (Gesture gesture in frame.Gestures())
		{
			switch(gesture.Type)
			{
				case (Gesture.GestureType.TYPESWIPE):
				{
					print ("Swipe gesture recognized");
					controller.Rotation ();
					break;
				}
			}
		}
		
		if (leap_hand == null)
			return;
		
		// Scale trigger distance by thumb proximal bone length.
		Vector leap_thumb_tip = leap_hand.Fingers[0].TipPosition;
		float proximal_length = leap_hand.Fingers[0].Bone(Bone.BoneType.TYPE_PROXIMAL).Length;
		float trigger_distance = proximal_length * TRIGGER_DISTANCE_RATIO;
		
		// Check thumb tip distance to joints on all other fingers.
		// If it's close enough, start pinching.
		for (int i = 1; i < HandModel.NUM_FINGERS && !trigger_pinch; ++i) {
			Finger finger = leap_hand.Fingers[i];
			
			for (int j = 0; j < FingerModel.NUM_BONES && !trigger_pinch; ++j) {
				Vector leap_joint_position = finger.Bone((Bone.BoneType)j).NextJoint;
				if (leap_joint_position.DistanceTo(leap_thumb_tip) < trigger_distance)
					trigger_pinch = true;
			}
		}
		
		Vector3 pinch_position = hand_model.fingers[0].GetTipPosition();
		
		// Only change state if it's different.
		if (trigger_pinch && !pinching_)
			OnPinch(pinch_position);
		else if (!trigger_pinch && pinching_)
			OnRelease();

		if	(controller.getTarget() != null) {
			controller.getTarget().transform.position = new Vector3 (hand_model.fingers[0].GetTipPosition().x, 3.0f, hand_model.fingers[0].GetTipPosition().z);
		}
	}

	void OnPinch(Vector3 pinch_position) {
		pinching_ = true;
		List<GameObject> guns = controller.getMockupGuns ();
		foreach (GameObject gun in guns) {
			if (pinch_position.x > gun.transform.position.x -2 && pinch_position.x < gun.transform.position.x + 2  && pinch_position.z > gun.transform.position.z -2 && pinch_position.z < gun.transform.position.z + 2) {
				controller.setTarget (gun);
				controller.setTargetAsCurrent (controller.getTarget().name, 1);
				controller.setMyPosition (gun.transform.position);
				break;
			}
		}
		List<GameObject> wagons = controller.getMockupWagons ();
		foreach (GameObject wagon in wagons) {
			if (pinch_position.x > wagon.transform.position.x -2 && pinch_position.x < wagon.transform.position.x + 2  && pinch_position.z > wagon.transform.position.z -2 && pinch_position.z < wagon.transform.position.z + 2) {
				controller.setTarget (wagon);
				controller.setTargetAsCurrent (controller.getTarget().name, 1);
				controller.setMyPosition (wagon.transform.position);
				break;
			}
		}

		List<GameObject> persons = controller.getMockupPersons ();
		foreach (GameObject person in persons) {
			if (pinch_position.x > person.transform.position.x -2 && pinch_position.x < person.transform.position.x + 2  && pinch_position.z > person.transform.position.z -2 && pinch_position.z < person.transform.position.z + 2) {
				controller.setTarget (person);
				controller.setTargetAsCurrent (controller.getTarget().name, 1);
				controller.setMyPosition (person.transform.position);
				break;
			}
		}
	}
	
	void OnRelease() {
		bool angle = controller.Release ();
		pinching_ = false;
	}
}
