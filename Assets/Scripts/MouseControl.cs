using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Data;

public class MouseControl : MonoBehaviour {
	private GameObject Target;
	private GameObject bigElement;
	private Vector3 myPosition; // backup initial position of an element;
	public int elementsScale = 5;
	public GameObject leap;
	private bool endOfGame;
	private List<GameObject> markers;
	private List<GameObject> mockupWagons;
	private List<GameObject> mockupGuns;
	private List<GameObject> mockupPersons;

	// Use this for initialization
	void Start () {
		endOfGame = false;

		InstantiateMarkers ();
		InstantiateMockupObjects ();
		InstantiateObjects ();
	}
	
	// Update is called once per frame
	void Update () {
		MoveElements ();

		endOfGame = true;
		foreach (GameObject mockupWagon in mockupWagons) {
			if (mockupWagon.GetComponent<MockupObjectInfos>().currentMarker != mockupWagon.GetComponent<MockupObjectInfos>().associatedMarker) {
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
		GetComponent<Compass>().CompassChange (camera.transform.eulerAngles.y);

		RaycastHit rayHit;
		Ray ray = camera.ScreenPointToRay (Input.mousePosition);
		//print (Input.mousePosition);

		/*GameObject obj = GameObject.Find ("HandController");
		HandController handController = obj.GetComponent<HandController> ();
		HandModel[] hands = handController.GetAllGraphicsHands();
		foreach(HandModel hand in hands) {
			print (hand.GetPalmPosition());
			Ray ray2 = camera.ScreenPointToRay (hand.GetPalmPosition());
			Debug.DrawRay( ray2.origin , ray2.direction * 1000 , Color.red ) ; 
		}*/

		if (Physics.Raycast(ray.origin, ray.direction, out rayHit, 1000.0F)) {
			if (rayHit.collider.name == "Plane") {
				if	(Target != null) {
					Target.transform.position = new Vector3 (rayHit.point.x, 2, rayHit.point.z);
				}
			} else {
				if (Input.GetMouseButtonDown(0)) {
					Target = rayHit.collider.gameObject;

					string targetName = Target.name;
					audio.clip = (AudioClip) Resources.Load("Audio/" + targetName);
					AudioClip sound =  (AudioClip) Resources.Load("Audio/" + targetName);
					audio.PlayOneShot(sound);

					setTargetAsCurrent (Target.name, 1);
					// backup initial object position
					myPosition = Target.transform.position;
					
					// get bigElement and move in function of mockup element
					bigElement = getBigElement (Target.name);
					// send notification piece of information
					ElementsInfo ei = bigElement.GetComponent<ElementsInfo>();
					string infos = getObjectInfos (bigElement.name);
					ei.setIndication (Target.name + " : " + infos);
				}
				if (Input.GetMouseButtonDown (1) && Target != null) {
					setTargetAsCurrent (Target.name, 0);
					bool markerIsFound = false;
					foreach (GameObject marker in markers) {
						if (Target.transform.position.x >= marker.transform.position.x - 1 && Target.transform.position.x <= marker.transform.position.x + 1 && Target.transform.position.z >= marker.transform.position.z - 1 && Target.transform.position.x <= marker.transform.position.x + 1) {
							// get bigElement and move in function of mockup element
							bigElement = getBigElement(Target.name);
							Vector3 bigElementInitPos = bigElement.transform.position;
							float x = getBigElementCoord (marker.transform.position.x);
							float z = getBigElementCoord (marker.transform.position.z);
							Vector3 bigElementDestPos = new Vector3 (x, bigElement.transform.position.y, z);
							//move real element
							bigElement.GetComponent<Move>().setDestPos (bigElementDestPos);
							// move mockup element
							Target.transform.position = new Vector3 (marker.transform.position.x, Target.transform.position.y, marker.transform.position.z);

							//Move camera in function of moved element position
							Vector3 initVector = camera.transform.up;//bigElementInitPos - camera.transform.position;
							Vector3 destVector = bigElementDestPos - camera.transform.position;

							float angle = Vector3.Angle (initVector, destVector);
							float sign = Mathf.Sign (Vector3.Dot (new Vector3 (0, 1, 0), Vector3.Cross (initVector, destVector)));
							GetComponent<CameraPosition>().setDestPos (bigElementDestPos);
							// Specify rotation orientation
							//print (angle * sign);
							GetComponent<CameraPosition>().setRotationAngle (angle);
							if ((angle * sign) < 0)
								GetComponent<CameraPosition>().setRotationDirection (false);
							else
								GetComponent<CameraPosition>().setRotationDirection (true);
							GetComponent<CameraPosition>().setBigElementIsMoved (true);
							
							// TEST IF element bien positionné
							if (Target.GetComponent<MockupObjectInfos>().associatedMarker == marker) {
								marker.renderer.material.SetColor("_Color", new Color (9f,248f,9f,255f));
							} else {
								marker.renderer.material.SetColor("_Color", new Color (255f,0f,0f,1f));
							}
							// Réinitialiser la couleur du marqueur à "non occupé" si l'élément était sur un marqueur avant ce déplacement
							if (Target.GetComponent<MockupObjectInfos>().currentMarker != null) {
								Target.GetComponent<MockupObjectInfos>().currentMarker.renderer.material.SetColor("_Color", new Color (0f,0f,0f,0f));
							}
							// Associe marqueur à l'élément
							Target.GetComponent<MockupObjectInfos>().currentMarker = marker;
							setCurrentMarker(Target.name, marker.name);
							bigElement = null;
							Target = null;
							markerIsFound = true;
							break;
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

	// get big objects coord in function of associated mockup object coord
	float getBigElementCoord (float coord) {
		return ((30 - (265 - coord)) * 500) / 30;
	}

	// set selected object as current object in db / unset released object
	void setTargetAsCurrent (string objectName, int value) {
		camera.GetComponent<DatabaseSync>().setCurrentObject (objectName, value);
	}

	void setCurrentMarker (string objectName, string markerName) {
		camera.GetComponent<DatabaseSync> ().setCurrentMarker (objectName, markerName);
	}

	// get elements of a table in db
	IDataReader getFromDb (string table_name) {
		return camera.GetComponent<DatabaseSync> ().getTable (table_name);
	}

	// return an object's description from db
	string getObjectInfos (string object_name) {
		return camera.GetComponent<DatabaseSync> ().getObjectDescription (object_name);
	}

	// Instantiate markers in fct of db
	void InstantiateMarkers () {
		IDataReader data = getFromDb ("Marker");
		
		// Instantiate Markers
		markers = new List<GameObject> ();
		int i = 0;
		while (data.Read()) {
			Vector3 tmp = new Vector3 ((float) data["posX"], (float) data["posY"], (float) data["posZ"]);
			markers.Add (Instantiate (Resources.Load ("Prefabs/Markers"), tmp, new Quaternion (0, 0, 0, 0)) as GameObject);
			markers[i].name = (string) data["name"];
			i++;	
		}
		markers.Add (GameObject.FindGameObjectWithTag("markerCentral"));
		data.Close();
		data = null;

	}

	void InstantiateMockupObjects () {
		int w = 0;
		int g = 0;
		int p = 0;
		IDataReader data = getFromDb ("Object");
		mockupWagons = new List<GameObject> ();
		mockupGuns = new List<GameObject> ();
		mockupPersons = new List<GameObject> ();
		while (data.Read()) {
			Vector3 tmp = new Vector3 ((float) data["posX"], (float) data["posY"], (float) data["posZ"]);
			if ((int)data["object_type_id"] == 1) {
				mockupWagons.Add (Instantiate (Resources.Load ("Prefabs/MockupWagon"), tmp, new Quaternion (0, 0, 0, 0)) as GameObject);
				mockupWagons[w].name = (string) data["name"];
				foreach (GameObject marker in markers) {
					if (marker.name == mockupWagons[w].name)
						mockupWagons[w].GetComponent<MockupObjectInfos>().setAssociatedMarker(marker);
				}
				w++;
			} else if ((int)data["object_type_id"] == 2) {
				mockupGuns.Add (Instantiate (Resources.Load ("Prefabs/MockupGun"), tmp, new Quaternion (0, 0, 0, 0)) as GameObject);
				mockupGuns[g].name = (string) data["name"];
				foreach (GameObject marker in markers) {
					if (marker.name == mockupGuns[g].name)
						mockupGuns[g].GetComponent<MockupObjectInfos>().setAssociatedMarker(marker);
				}
				g++;
			} else if ((int)data["object_type_id"] == 3) {
				mockupPersons.Add (Instantiate (Resources.Load ("Prefabs/MockupPerson"), tmp, new Quaternion (0, 0, 0, 0)) as GameObject);
				mockupPersons[p].name = (string) data["name"];
				foreach (GameObject marker in markers) {
					if (marker.name == mockupPersons[p].name)
						mockupPersons[p].GetComponent<MockupObjectInfos>().setAssociatedMarker(marker);
				}
				p++;
			}
		}
		data.Close();
		data = null;
	}

	void InstantiateObjects () {
		int i = 0;
		List<GameObject> wagons = new List<GameObject> ();
		foreach (GameObject mockupWagon in mockupWagons) {
			float x = getBigElementCoord (mockupWagon.transform.position.x);
			float z = getBigElementCoord (mockupWagon.transform.position.z);
			//Debug.Log ("x val: " + x + " z val :" + z);
			Vector3 tmp = new Vector3 (x, mockupWagon.transform.position.y, z);
			Quaternion rot = new Quaternion (0, 0, 0, 0);
			wagons.Add(Instantiate (Resources.Load("Prefabs/Wagon"), tmp, /*mockupWagons[i].transform.rotation*/rot) as GameObject);
			wagons[i].name = mockupWagon.name;
			wagons[i].transform.parent = GameObject.Find ("Terrain").transform;
			wagons[i].transform.localScale = new Vector3 (elementsScale, elementsScale, elementsScale);
			i++;
		}

		i = 0;
		List<GameObject> guns = new List<GameObject> ();
		foreach (GameObject mockupGun in mockupGuns) {
			float x = getBigElementCoord (mockupGun.transform.position.x);
			float z = getBigElementCoord (mockupGun.transform.position.z);
			Vector3 tmp = new Vector3 (x, mockupGun.transform.position.y, z);
			Quaternion rot = new Quaternion (0, 0, 0, 0);
			guns.Add(Instantiate (Resources.Load("Prefabs/Canon"), tmp, /*mockupWagons[i].transform.rotation*/rot) as GameObject);
			guns[i].name = mockupGun.name;
			guns[i].transform.parent = GameObject.Find ("Terrain").transform;
			guns[i].transform.localScale = new Vector3 (elementsScale, elementsScale, elementsScale);
			i++;
		}

		i = 0;
		List<GameObject> persons = new List<GameObject> ();
		foreach (GameObject mockupPerson in mockupPersons) {
			float x = getBigElementCoord (mockupPerson.transform.position.x);
			float z = getBigElementCoord (mockupPerson.transform.position.z);
			Vector3 tmp = new Vector3 (x, mockupPerson.transform.position.y, z);
			Quaternion rot = new Quaternion (0, 0, 0, 0);
			persons.Add(Instantiate (Resources.Load("Prefabs/Person"), tmp, /*mockupWagons[i].transform.rotation*/rot) as GameObject);
			persons[i].name = mockupPerson.name;
			persons[i].transform.parent = GameObject.Find ("Terrain").transform;
			persons[i].transform.localScale = new Vector3 (elementsScale, elementsScale, elementsScale);
			i++;
		}
	}
}
