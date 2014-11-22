using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class MouseControl : MonoBehaviour {
	private GameObject Target;
	private GameObject bigElement;
	private GameObject[] markers;
	private Vector3 myPosition; // backup initial position of an element;
	public int elementsScale = 5;
	private bool endOfGame;
	private GameObject[] mockupWagons;

	// Use this for initialization
	void Start () {
		endOfGame = false;

		List<GameObject> wagons = new List<GameObject> ();
		mockupWagons = GameObject.FindGameObjectsWithTag ("mockupWagons");
		int i;
		for (i = 0; i < mockupWagons.Length; i++) {
			float x = getBigElementCoord (mockupWagons[i].transform.position.x);
			float z = getBigElementCoord (mockupWagons[i].transform.position.z);
			//Debug.Log ("x val: " + x + " z val :" + z);
			Vector3 tmp = new Vector3 (x, mockupWagons[i].transform.position.y, z);
			Quaternion rot = new Quaternion (0, 0, 0, 0);
			wagons.Add(Instantiate (Resources.Load("Wagon"), tmp, /*mockupWagons[i].transform.rotation*/rot) as GameObject);
			wagons[i].name = mockupWagons[i].name;
			wagons[i].transform.parent = GameObject.Find ("Terrain").transform;
			wagons[i].transform.localScale = new Vector3 (elementsScale, elementsScale, elementsScale);
		}
		markers = GameObject.FindGameObjectsWithTag("markers");	
	}
	
	// Update is called once per frame
	void Update () {
		MoveElements ();

		endOfGame = true;
		for (int i = 0; i < mockupWagons.Length; i++) {
			    if (mockupWagons[i].GetComponent<ElementsMarker>().currentMarker != mockupWagons[i].GetComponent<ElementsMarker>().associatedMarker) {
						endOfGame = false;
				}
		}
		if (endOfGame == true) {
			Debug.Log ("Jeu Fini!!");
		}
	}

	GameObject getBigElement (string name) {
		return GameObject.Find("/Terrain/" + name);
	}

	void MoveElements() {
		RaycastHit rayHit;
		Ray ray = camera.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast(ray.origin, ray.direction, out rayHit, 1000.0F)) {
			if (rayHit.collider.name == "Plane") {
				if	(Target != null) {
					Target.transform.position = new Vector3 (rayHit.point.x, 2, rayHit.point.z);
				}
			} else {
				if (Input.GetMouseButtonDown(0)) {
					Target = rayHit.collider.gameObject;
					// backup initial object position
					myPosition = Target.transform.position;
					
					// get bigElement and move in function of mockup element
					bigElement = getBigElement (Target.name);
					// send notification piece of information
					ElementsInfo ei = bigElement.GetComponent<ElementsInfo>();
					string infos;
					if(bigElement.name == "Wagon1") {
						infos = "infos wagon 1";
					} else {
						infos = "infos wagon 2";
					}
					ei.setIndication (infos);
				}
				if (Input.GetMouseButtonDown (1) && Target != null) {
					int i;
					bool markerIsFound = false;
					for(i = 0; i < markers.Length; i++) {
						if (Target.transform.position.x >= markers[i].transform.position.x - 1 && Target.transform.position.x <= markers[i].transform.position.x + 1 && Target.transform.position.z >= markers[i].transform.position.z - 1 && Target.transform.position.x <= markers[i].transform.position.x + 1) {
							// get bigElement and move in function of mockup element
							bigElement = getBigElement(Target.name);
							Vector3 bigElementInitPos = bigElement.transform.position;
							float x = getBigElementCoord (markers[i].transform.position.x);
							float z = getBigElementCoord (markers[i].transform.position.z);
							Vector3 bigElementDestPos = new Vector3 (x, bigElement.transform.position.y, z);
							//move real element
							bigElement.GetComponent<Move>().setDestPos (bigElementDestPos);
							// move mockup element
							Target.transform.position = new Vector3 (markers[i].transform.position.x, Target.transform.position.y, markers[i].transform.position.z);

							//Move camera in function of moved element position
							Vector3 initVector = bigElementInitPos - camera.transform.position;
							Vector3 destVector = bigElementDestPos - camera.transform.position;
							float angle = Vector3.Angle (initVector, destVector);
							float sign = Mathf.Sign (Vector3.Dot (new Vector3 (0, 1, 0), Vector3.Cross (initVector, destVector)));
							print (angle * sign);
							GetComponent<CameraPosition>().setDestPos (bigElementDestPos);
							// Specify rotation orientation
							if ((angle * sign) < 0)
								GetComponent<CameraPosition>().setRotationDirection (false);
							else
								GetComponent<CameraPosition>().setRotationDirection (true);
							GetComponent<CameraPosition>().setBigElementIsMoved (true);
							
							// TEST IF element bien positionné
							if (Target.GetComponent<ElementsMarker>().associatedMarker == markers[i]) {
								markers[i].renderer.material.SetColor("_Color", new Color (9f,248f,9f,255f));
							} else {
								markers[i].renderer.material.SetColor("_Color", new Color (255f,0f,0f,1f));
							}
							// Réinitialiser la couleur du marqueur à "non occupé" si l'élément était sur un marqueur avant ce déplacement
							if (Target.GetComponent<ElementsMarker>().currentMarker != null) {
								Target.GetComponent<ElementsMarker>().currentMarker.renderer.material.SetColor("_Color", new Color (0f,0f,0f,0f));
							}
							// Associe marqueur à l'élément
							Target.GetComponent<ElementsMarker>().currentMarker = markers[i];
							bigElement = null;
							Target = null;
							i = markers.Length;
							markerIsFound = true;
						}
					}
					if (! markerIsFound) {
						Target.transform.position = myPosition;
						Target = null;
					}
				}
			}
		}
	}

	float getBigElementCoord (float coord) {
		return ((30 - (265 - coord)) * 500) / 30;
	}
}
