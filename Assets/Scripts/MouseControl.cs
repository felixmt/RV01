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
			if (rayHit.collider.name == "Plateau_jeu") {
				if	(controller.getTarget() != null) {
					controller.getTarget().transform.position = new Vector3 (rayHit.point.x, 122, rayHit.point.z);
				}
			} else {
				if (Input.GetMouseButtonDown(0)) {
					controller.Pinch (rayHit.collider.gameObject);
				}
				if (Input.GetMouseButtonDown (1)) {
					controller.Release ();
				}
			}
		}
	}
}
