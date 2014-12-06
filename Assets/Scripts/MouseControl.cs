using UnityEngine;
using System.Collections;

public class mouseControl : MonoBehaviour {
	private Control controller;

	// Use this for initialization
	void Start () {
		controller = camera.GetComponent<Control> ();
	}
	
	// Update is called once per frame
	void Update () {
		MoveElements ();
	}

	void MoveElements() {
		
		RaycastHit rayHit;
		Ray ray = camera.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast(ray.origin, ray.direction, out rayHit, 1000.0F)) {
			if (rayHit.collider.name == "Plane") {
				if	(controller.getTarget() != null) {
					controller.getTarget().transform.position = new Vector3 (rayHit.point.x, 2, rayHit.point.z);
				}
			} else {
				if (Input.GetMouseButtonDown(0)) {
					controller.setTarget (rayHit.collider.gameObject);
					
					controller.setTargetAsCurrent (controller.getTarget().name, 1);
					// backup initial object position
					controller.setMyPosition ( controller.getTarget().transform.position);
				}
				if (Input.GetMouseButtonDown (1)) {
					controller.Release ();
				}
			}
		}
	}
}
