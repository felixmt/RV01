using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Data;
using System.Linq;

public class Control : MonoBehaviour {
	private GameObject Target;
	private GameObject bigElement;
	private Vector3 myPosition; // backup initial position of an element;
	public int elementsScale = 5;
	public GameObject leap;
	private List<GameObject> markers;
	private List<GameObject> mockupWagons;
	private List<GameObject> mockupGuns;
	private List<GameObject> mockupPersons;
	private int avancement;

	// Use this for initialization
	void Start () {
		avancement = 1;
		mockupWagons = new List<GameObject> ();
		mockupGuns = new List<GameObject> ();
		mockupPersons = new List<GameObject> ();
		InstantiateMarkers ();
		InstantiateMockupObject (avancement);
		//InstantiateMockupObjects ();
		//InstantiateObjects ();
		
		audio.clip = (AudioClip) Resources.Load("Audio/1");
		audio.Play();
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Compass>().CompassChange (camera.transform.eulerAngles.y);
	}

	public List<GameObject> getMockupWagons () {
		return mockupWagons;
	}

	public List<GameObject> getMockupGuns () {
		return mockupGuns;
	}

	public List<GameObject> getMockupPersons () {
		return mockupPersons;
	}

	public List<GameObject> getMarkers () {
		return markers;
	}

	public void setBigElement (GameObject obj) {
		bigElement = obj;
	}

	public GameObject getBigElement () {
		return bigElement;
	}

	public void setMyPosition (Vector3 vec) {
		myPosition = vec;
	}

	// when an element is well placed by the user, make appears the new one
	IEnumerator NewObject (int avancement, GameObject bigElement) {
		if (avancement != 7) {
			yield return new WaitForSeconds (2.0f);
			InstantiateMockupObject (avancement);
			// Get infos of this object
			ElementsInfo ei = bigElement.GetComponent<ElementsInfo>();
			string infos = getObjectInfos (avancement);
			ei.setIndication (infos);
			/*if (avancement == 5) {
				avancement++;
				InstantiateMockupObject (avancement);
			}*/
			// rotate to the new object instanciated
			float[] coord = new float[2];
			coord = getObjectCoord (avancement);
			Vector3 bigElementDestPos = new Vector3 (getBigElementCoord (coord[0]), (float)2.3, getBigElementCoord(coord[1]));
			Vector3 initVector = camera.transform.up;
			Vector3 destVector = bigElementDestPos - camera.transform.position;
			float angle = Vector3.Angle (initVector, destVector);
			float sign = Mathf.Sign (Vector3.Dot (new Vector3 (0, 1, 0), Vector3.Cross (initVector, destVector)));
			// Specify rotation orientation
			GetComponent<CameraPosition>().setRotationAngle (angle);
			if ((angle * sign) < 0)
				GetComponent<CameraPosition>().setRotationDirection (false);
			else
				GetComponent<CameraPosition>().setRotationDirection (true);
			GetComponent<CameraPosition>().setBigElementIsMoved (true);
			GameObject.Find("HandController").transform.Rotate (0, angle * sign, 0);
			
			audio.clip = (AudioClip) Resources.Load("Audio/" + avancement);
			audio.Play();

			if (avancement == 5) {
				avancement++;
				NewObject (avancement, bigElement);
			}
		} else {
			audio.clip = (AudioClip) Resources.Load("Audio/7");
			audio.Play();
			//new WaitForSeconds (audio.clip.length);
			//audio.clip = (AudioClip) Resources.Load("Audio/Clairon");
			//audio.Play ();
			//Application.Quit();
		}
	}

	public GameObject getBigElement (string name) {
		return GameObject.Find("/Terrain/" + name);
	}

	public void setTarget (GameObject targ) {
		Target = targ;
	}

	public GameObject getTarget () {
		return Target;
	}

	float[] getObjectCoord (int order) {
		return camera.GetComponent<DatabaseSync> ().getObjectCoord (order);
	}

	// get big objects coord in function of associated mockup object coord
	public float getBigElementCoord (float coord) {
		return ((30 - (265 - coord)) * 500) / 30;
	}

	// set selected object as current object in db / unset released object
	public void setTargetAsCurrent (string objectName, int value) {
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
	string getObjectInfos (int order) {
		return camera.GetComponent<DatabaseSync> ().getObjectDescription (order);
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

	void InstantiateMockupObject (int order) {
		IDataReader data = camera.GetComponent<DatabaseSync> ().getObject (order);
		while (data.Read()) {
			Vector3 tmp = new Vector3 ((float) data["posX"], (float) data["posY"], (float) data["posZ"]);
			if ((int)data["object_type_id"] == 1) {
				mockupWagons.Add (Instantiate (Resources.Load ("Prefabs/MockupWagon"), tmp, new Quaternion (0, 0, 0, 0)) as GameObject);
				mockupWagons.Last().name = (string) data["name"];
				foreach (GameObject marker in markers) {
					if (marker.name == mockupWagons.Last().name)
						mockupWagons.Last().GetComponent<MockupObjectInfos>().setAssociatedMarker(marker);
				}
				InstantiateObject (mockupWagons.Count - 1, 1);
			} else if ((int)data["object_type_id"] == 2) {
				mockupGuns.Add (Instantiate (Resources.Load ("Prefabs/MockupGun"), tmp, new Quaternion (0, 0, 0, 0)) as GameObject);
				mockupGuns.Last().name = (string) data["name"];
				foreach (GameObject marker in markers) {
					if (marker.name == mockupGuns.Last().name)
						mockupGuns.Last().GetComponent<MockupObjectInfos>().setAssociatedMarker(marker);
				}
				InstantiateObject (mockupGuns.Count - 1, 2);
			} else if ((int)data["object_type_id"] == 3) {
				mockupPersons.Add (Instantiate (Resources.Load ("Prefabs/MockupPerson"), tmp, new Quaternion (0, 0, 0, 0)) as GameObject);
				mockupPersons.Last().name = (string) data["name"];
				foreach (GameObject marker in markers) {
					if (marker.name == mockupPersons.Last().name)
						mockupPersons.Last().GetComponent<MockupObjectInfos>().setAssociatedMarker(marker);
				}
				InstantiateObject (mockupPersons.Count - 1, 3);
			}
		}
		data.Close();
		data = null;
	}

	void InstantiateObject (int index, int type) {
		GameObject obj;
		if (type == 1) {
			float x = getBigElementCoord (mockupWagons[index].transform.position.x);
			float z = getBigElementCoord (mockupWagons[index].transform.position.z);
			Vector3 tmp = new Vector3 (x, mockupWagons[index].transform.position.y, z);
			Quaternion rot = new Quaternion (0, 0, 0, 0);
			obj = Instantiate (Resources.Load("Prefabs/Wagon"), tmp, /*mockupWagons[i].transform.rotation*/rot) as GameObject;
			obj.name = mockupWagons[index].name;
		} else if (type == 2) {
			float x = getBigElementCoord (mockupGuns[index].transform.position.x);
			float z = getBigElementCoord (mockupGuns[index].transform.position.z);
			Vector3 tmp = new Vector3 (x, mockupGuns[index].transform.position.y, z);
			Quaternion rot = new Quaternion (0, 0, 0, 0);
			obj = Instantiate (Resources.Load("Prefabs/Canon"), tmp, rot) as GameObject;
			obj.name = mockupGuns[index].name;
		} else {
			float x = getBigElementCoord (mockupPersons[index].transform.position.x);
			float z = getBigElementCoord (mockupPersons[index].transform.position.z);
			Vector3 tmp = new Vector3 (x, mockupPersons[index].transform.position.y, z);
			Quaternion rot = new Quaternion (0, 0, 0, 0);
			obj = Instantiate (Resources.Load("Prefabs/Person"), tmp, rot) as GameObject;
			obj.name = mockupPersons[index].name;
		}
		obj.transform.parent = GameObject.Find ("Terrain").transform;
		obj.transform.localScale = new Vector3 (elementsScale, elementsScale, elementsScale);
		if (avancement == 1) {
			ElementsInfo ei = obj.GetComponent<ElementsInfo>();
			string infos = getObjectInfos (avancement);
			ei.setIndication (infos);
		}
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

	public bool Release () {
		float angle = 0;
		float sign = 0;
		if (Target != null) {
			setTargetAsCurrent (Target.name, 0);
			bool markerIsFound = false;
			foreach (GameObject marker in markers) {
				if (Target.transform.position.x >= marker.transform.position.x - 2 && Target.transform.position.x <= marker.transform.position.x + 2 && Target.transform.position.z >= marker.transform.position.z - 2 && Target.transform.position.z <= marker.transform.position.z + 2) {
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
					Vector3 initVector = camera.transform.up;
					Vector3 destVector = bigElementDestPos - camera.transform.position;
					
					angle = Vector3.Angle (initVector, destVector);
					sign = Mathf.Sign (Vector3.Dot (new Vector3 (0, 1, 0), Vector3.Cross (initVector, destVector)));
					// Specify rotation orientation
					//print (angle * sign);
					GetComponent<CameraPosition>().setRotationAngle (angle);
					if ((angle * sign) < 0)
						GetComponent<CameraPosition>().setRotationDirection (false);
					else
						GetComponent<CameraPosition>().setRotationDirection (true);
					GetComponent<CameraPosition>().setBigElementIsMoved (true);
					GameObject.Find("HandController").transform.Rotate (0, angle * sign, 0);
					
					// TEST IF element bien positionné
					if (Target.GetComponent<MockupObjectInfos>().associatedMarker == marker) {
						marker.renderer.material.SetColor("_Color", new Color (9f,248f,9f,255f));
						avancement++;
						// Instantiate next object if the last one has been well placed
						StartCoroutine (NewObject (avancement, bigElement));
					} else {
						marker.renderer.material.SetColor("_Color", new Color (255f,0f,0f,1f));
						// ajouter tests pr notif spécialisée en fonction de l'objet mal placé
						ElementsInfo ei = bigElement.GetComponent<ElementsInfo>();
						ei.setIndication ("Vous avez mal positionné le canon allemand, veuillez le placer au bon endroit.");
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
				return false;
			} else
				return true;
		}
		return false;
	}
}
