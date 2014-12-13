using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class LeapControl : MonoBehaviour {

	private Control controller;
	Leap.Controller leap_controller;

	public const float TRIGGER_DISTANCE_RATIO = 0.7f;
	
	public float forceSpringConstant = 100.0f;
	public float magnetDistance = 10.0f;
	public int sign = 0;
	
	protected bool pinching_;

	// signature
	List<Vector2> signaturePoints;
	GameObject signature;

	public void setSign (int s) {
		sign = s;
	}
	// Use this for initialization
	void Start () {
		signaturePoints = new List<Vector2> ();
		//controller = transform.parent.GetComponent<Control> ();
		controller = GameObject.Find ("TopCamera").GetComponent<Control> ();
		pinching_ = false;
		leap_controller = new Leap.Controller ();
		leap_controller.EnableGesture (Gesture.GestureType.TYPE_SWIPE);
		leap_controller.EnableGesture (Gesture.GestureType.TYPE_CIRCLE);
		leap_controller.EnableGesture (Gesture.GestureType.TYPE_KEY_TAP);
		//leap_controller.Config.SetFloat ("Gesture.swipe.MinVelocity", 500f);
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
					//print ("Swipe gesture recognized");
					controller.Rotation (new Vector3 (0, 0, 0), new Vector3 (0, 0, 0));
					break;
				}
				case (Gesture.GestureType.TYPE_CIRCLE):
				{
					//print ("circle!");
					controller.PlayBack();
					break;
				}
				case (Gesture.GestureType.TYPE_KEY_TAP):
				{
					//print ("keytap");
					//controller.Audio();
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
				//controller.getTarget().transform.position = new Vector3 (hand_model.fingers[0].GetTipPosition().x, 3.0f, hand_model.fingers[0].GetTipPosition().z);
				controller.getTarget().transform.position = new Vector3 (hand_model.GetPalmPosition().x, 122.0f, hand_model.GetPalmPosition().z);
			}
			break;
		}
		case 1:
		{
			Frame frame = leap_controller.Frame();
			foreach (Gesture gesture in frame.Gestures())
			{
				switch(gesture.Type)
				{
				case (Gesture.GestureType.TYPE_CIRCLE):
				{
					print ("start register");
					sign = 2;
					break;
				}
				}
			}
			break;
		}
		case 2:
		{
			HandModel hand_model = GetComponent<HandModel>();
			signaturePoints.Add (new Vector2 (GameObject.Find ("TopCamera").camera.WorldToViewportPoint (hand_model.fingers[1].GetTipPosition()).x, GameObject.Find ("TopCamera").camera.WorldToViewportPoint (hand_model.fingers[1].GetTipPosition()).y));
			GameObject.Find ("Terrain").GetComponent<NotificationCenter> ().setDrawPoint (new Vector2 (GameObject.Find ("TopCamera").camera.WorldToViewportPoint (hand_model.fingers[1].GetTipPosition()).x, GameObject.Find ("TopCamera").camera.WorldToViewportPoint (hand_model.fingers[1].GetTipPosition()).y));
			StartCoroutine (SignatureTimer (4.0f, 3));
			break;
		}
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
	
	IEnumerator SignatureTimer (float nbsec, int valsign) {
		yield return new WaitForSeconds (nbsec);
		sign = valsign;
	}

	void OnPinch(Vector3 pinch_position) {
		pinching_ = true;
		List<GameObject> objects = controller.getMockupObjects ();
		foreach (GameObject obj in objects) {
			if (pinch_position.x > obj.transform.position.x -2 && pinch_position.x < obj.transform.position.x + 2  && pinch_position.z > obj.transform.position.z -2 && pinch_position.z < obj.transform.position.z + 2) {
				controller.Pinch(obj);
				break;
			}
		}
		/*List<GameObject> guns = controller.getMockupGuns ();
		foreach (GameObject gun in guns) {
			if (pinch_position.x > gun.transform.position.x -2 && pinch_position.x < gun.transform.position.x + 2  && pinch_position.z > gun.transform.position.z -2 && pinch_position.z < gun.transform.position.z + 2) {
				controller.Pinch(gun);
				break;
			}
		}
		List<GameObject> wagons = controller.getMockupWagons ();
		foreach (GameObject wagon in wagons) {
			if (pinch_position.x > wagon.transform.position.x -2 && pinch_position.x < wagon.transform.position.x + 2  && pinch_position.z > wagon.transform.position.z -2 && pinch_position.z < wagon.transform.position.z + 2) {
				controller.Pinch (wagon);
				break;
			}
		}

		List<GameObject> persons = controller.getMockupPersons ();
		foreach (GameObject person in persons) {
			if (pinch_position.x > person.transform.position.x -2 && pinch_position.x < person.transform.position.x + 2  && pinch_position.z > person.transform.position.z -2 && pinch_position.z < person.transform.position.z + 2) {
				controller.Pinch (person);
				break;
			}
		}*/
	}
	
	void OnRelease() {
		controller.Release ();
		pinching_ = false;
	}
}
