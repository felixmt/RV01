using UnityEngine;
using System.Collections;

public class LeapControl : MonoBehaviour {

	private HandModel hand;
	private GameObject Target;
	GameObject obj;
	Camera mycam;


	// Use this for initialization
	void Start () {
		hand = GetComponent<HandModel>();
		obj = GameObject.Find ("TopCamera");
		mycam = obj.camera;
	}
	
	// Update is called once per frame
	void Update () {
		//print(hand.GetPalmPosition());
		RaycastHit rayHit;
		Ray ray = mycam.ScreenPointToRay (hand.GetPalmPosition ());
		//print (hand.GetHandOffset ());

		if (Physics.Raycast (ray.origin, ray.direction, out rayHit, 1000.0F)) {
			if (rayHit.collider.name == "Plane") {
				if (Target != null) {
					Target.transform.position = new Vector3 (rayHit.point.x, 2, rayHit.point.z);
				}
			} else {
				print ("blabla");
				if (Input.GetKey ("q")) {
					Target = rayHit.collider.gameObject;
					print ("allo");
					print (Target.name);
				}
			}
		}
	}
}
