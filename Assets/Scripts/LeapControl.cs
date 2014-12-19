using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class LeapControl : MonoBehaviour {

	private MainControl controller;
	Leap.Controller leap_controller;

	public const float TRIGGER_DISTANCE_RATIO = 0.7f;
	
	public float forceSpringConstant = 100.0f;
	public float magnetDistance = 10.0f;
	private int sign = 0; /* value of sign change when all elements are at the good position, to enable the signature */
	
	protected bool pinching_;

	// signature
	List<Vector2> signaturePoints;
	GameObject signature;
	
	// Use this for initialization
	void Start () {
		signaturePoints = new List<Vector2> ();
		//controller = transform.parent.GetComponent<MainControl> ();
		controller = GameObject.Find ("Map").GetComponent<MainControl> ();
		pinching_ = false;
		/* enable the swipe gesture */
		leap_controller = new Leap.Controller ();
		leap_controller.EnableGesture (Gesture.GestureType.TYPE_SWIPE);
		leap_controller.Config.Save ();
	}
	
	// Update is called once per frame
	void Update () {
		if (controller.getAvancement () == 8 && sign == 0)
			sign = 1;
		else if (controller.getAvancement () == 9 && sign == 0)
			sign = 4;
		if (controller.getAvancement() == 7) {
			StartCoroutine (SignatureTimer(4.0f, 1));
		}
		switch (sign) {
		// all the mockup objects are still not at the right place, so they are movable
		case 0:
		{
			bool trigger_pinch = false;
			HandModel hand_model = GetComponent<HandModel>();
			Hand leap_hand = hand_model.GetLeapHand();
			Frame frame = leap_controller.Frame();
			foreach (Gesture gesture in frame.Gestures())
			{
				switch(gesture.Type)
				{
				case (Gesture.GestureType.TYPE_SWIPE):
				{
					SwipeGesture swipeGesture = new SwipeGesture(gesture);
					Vector swipeVector  = swipeGesture.Direction;
					var isHorizontal = Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y);
					//Classify as honrizontal of vertical movement
					if(isHorizontal) {
						if (swipeVector.x > 0) {
							// if swipe is from righ to left, anti-clockwise rotation
							controller.Rotation (new Vector3 (-1, -1, -1), new Vector3 (-1, -1, -1));
						} else {
							// if swipe is from left to right, clockwise rotation
							controller.Rotation (new Vector3 (0, 0, 0), new Vector3 (0, 0, 0));
						}
					} else {
						// if swipe is from bottom to top : playback the audio instructions
						if (swipeVector.y > 0) {
							controller.PlayBack();
						}
					}
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
			
			Vector3 pinch_position = hand_model.fingers[2].GetTipPosition();
			
			// Only change state if it's different.
			if (trigger_pinch && !pinching_)
				OnPinch(pinch_position);
			else if (!trigger_pinch && pinching_)
				OnRelease();
			
			if	(controller.getTarget() != null) {
				//controller.getTarget().transform.position = new Vector3 (hand_model.fingers[0].GetTipPosition().x, 3.0f, hand_model.fingers[0].GetTipPosition().z);
				controller.getTarget().transform.position = new Vector3 (hand_model.GetPalmPosition().x, 0.7f, hand_model.GetPalmPosition().z);
			}
			break;
		}
		// the mockup objects are all well placed, the computer is waiting for the user's swipe to enable the signature
		case 1:
		{
			Frame frame = leap_controller.Frame();
			foreach (Gesture gesture in frame.Gestures())
			{
				switch(gesture.Type)
				{
				case (Gesture.GestureType.TYPE_SWIPE):
				{
					SwipeGesture swipeGesture = new SwipeGesture(gesture);
					Vector swipeVector  = swipeGesture.Direction;
					var isHorizontal = Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y);
					//if vertical movement
					if(!isHorizontal) {
						// if swipe is from bottom to top : start signature
						if (swipeVector.y > 0) {
							print ("start register");
							sign = 2;
						}
					}
					break;
				}
				}
			}
			break;
		}
		// the user has now 4 seconds to write his signature
		case 2:
		{
			HandModel hand_model = GetComponent<HandModel>();
			signaturePoints.Add (new Vector2 (GameObject.Find ("TopCamera").camera.WorldToViewportPoint (hand_model.fingers[1].GetTipPosition()).x, GameObject.Find ("TopCamera").camera.WorldToViewportPoint (hand_model.fingers[1].GetTipPosition()).y));
			GameObject.Find ("Terrain").GetComponent<NotificationCenter> ().setDrawPoint (new Vector2 (GameObject.Find ("TopCamera").camera.WorldToViewportPoint (hand_model.fingers[1].GetTipPosition()).x, GameObject.Find ("TopCamera").camera.WorldToViewportPoint (hand_model.fingers[1].GetTipPosition()).y));
			StartCoroutine (SignatureTimer (4.0f, 3));
			break;
		}
		// the game is finished and the signature is set on the armistice treaty
		case 3:
		{
			GameObject.Find ("Paper").renderer.enabled = true;
			sign = 4;
			controller.setAvancement (9);
			//GameObject paper = Instantiate (Resources.Load ("Prefabs/Paper"), new Vector3 (255, 11, 270), new Quaternion (0, 0, 0, 0)) as GameObject;
			//paper.transform.rotation = Quaternion.Euler (-90, 0, 0);

			if (! GameObject.Find ("signature")) {
				signature = new GameObject("signature");
				print ("llo");
			}
			signature.AddComponent("LineRenderer");  
			//signature.GetComponent<LineRenderer>().material = new Material (Shader.Find("Materials/black"));
			signature.GetComponent<LineRenderer>().SetVertexCount(0);
			if (signaturePoints.Count > 0){
				int i = 0;
				signature.GetComponent<LineRenderer>().SetVertexCount((signaturePoints.Count));
				foreach (Vector2 sp in signaturePoints) {
					if (sp.x != 0 && sp.y != 0)
						signature.GetComponent<LineRenderer>().SetPosition (i, new Vector3 (255 + sp.x * 7, 2 + sp.y * 3, 252.5f));
					i++;
				}
			}
			break;
		}
		}
	}

	/* called when the user make a pinch gesture, and if his hand is over an element, the element is movable */
	void OnPinch(Vector3 pinch_position) {
		pinching_ = true;
		List<GameObject> objects = controller.getMockupObjects ();
		// check if the hand is over an element
		foreach (GameObject obj in objects) {
			if (pinch_position.x > obj.transform.position.x - 0.1 && pinch_position.x < obj.transform.position.x + 0.1  && pinch_position.z > obj.transform.position.z - 0.1 && pinch_position.z < obj.transform.position.z + 0.1) {
				controller.Pinch(obj);
				break;
			}
		}
	}
	
	/* if the user is pinching an object, this function is called when he open his hand in order to release the element */
	void OnRelease() {
		controller.Release ();
		pinching_ = false;
	}

	/* when the user make the gesture to start his signature (swipe from bottom to top), he has 4 secondes to write */
	IEnumerator SignatureTimer (float nbsec, int valsign) {
		yield return new WaitForSeconds (nbsec);
		sign = valsign;
	}

	// ACCESSORS / MUTATORS --------------------------
	public void setSign (int s) {
		sign = s;
	}
}
