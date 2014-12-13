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
	public float mockupElementsScale = 1.5f;
	public int markersScale = 3;
	public GameObject leap;
	private List<GameObject> markers;
	/*private List<GameObject> mockupWagons;
	private List<GameObject> mockupGuns;
	private List<GameObject> mockupPersons;*/
	private List<GameObject> mockupObjects;
	private int avancement;
	private bool alreadyReleased = false; // is true when the user never released an element

	// Use this for initialization
	void Start () {
		avancement = 1;
/*		mockupWagons = new List<GameObject> ();
		mockupGuns = new List<GameObject> ();
		mockupPersons = new List<GameObject> ();*/

		mockupObjects = new List<GameObject> ();
		InstantiateMarkers ();

		StartCoroutine (NewObject (bigElement));
		Audio ();


		//BroadcastMessage ("setNotification", avancement);
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Compass>().CompassChange (camera.transform.eulerAngles.y);
		if (avancement == 9) {
			StartCoroutine (NewObject (bigElement));
		} else if (avancement == 8) {
			StartCoroutine (NewObject (bigElement));
		}

	}

	public List<GameObject> getMockupObjects () {
		return mockupObjects;
	}

	public int getAvancement () {
		return avancement;
	}

	public void setAvancement (int av) {
		avancement = av;
	}

	/*
	 * 	public List<GameObject> getMockupWagons () {
		return mockupWagons;
	}

	public List<GameObject> getMockupGuns () {
		return mockupGuns;
	}

	public List<GameObject> getMockupPersons () {
		return mockupPersons;
	}
	 * 
	 * // unused
	public List<GameObject> getMarkers () {
		return markers;
	}

	// unused
	public void setBigElement (GameObject obj) {
		bigElement = obj;
	}
	// unused
	public GameObject getBigElement () {
		return bigElement;
	}*/
	
	// save the initial position of an element when it is being selected
	public void setMyPosition (Vector3 vec) {
		myPosition = vec;
	}

	// get a real environment Element by name
	public GameObject getBigElement (string name) {
		return GameObject.Find("/Terrain/" + name);
	}
	
	// set the selected element as Target
	public void setTarget (GameObject targ) {
		Target = targ;
	}
	
	// get the current element
	public GameObject getTarget () {
		return Target;
	}
	
	// get x & z coord of an object from database
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

	public void Pinch (GameObject obj) {
		if ((avancement == 1 && obj.name == "Canon des forces alliées") || (avancement == 2 && obj.name == "Wagon du maréchal Foch") || (avancement == 3 && obj.name == "Canon des forces allemandes") || (avancement == 4 && obj.name == "Wagon des forces allemandes") || (avancement == 5 && obj.name == "Délégation alliée") || (avancement == 6 && obj.name == "Délégation allemande"))
		{
			setTarget (obj);
			setTargetAsCurrent (getTarget().name, 1);
			setMyPosition (obj.transform.position);
			if (avancement == 1 && !alreadyReleased)
				GameObject.Find ("Terrain").GetComponent<NotificationCenter> ().setDidacticielNotif (2);
		}
	}

	// release an object
	public void Release () {
		alreadyReleased = true;
		if (GameObject.Find ("Terrain").GetComponent<NotificationCenter> ().getNumInfo () == 2)
			GameObject.Find ("Terrain").GetComponent<NotificationCenter> ().setDidacticielNotif (0);

		float angle = 0;
		float sign = 0;
		if (Target != null) {
			setTargetAsCurrent (Target.name, 0);
			bool markerIsFound = false;
			foreach (GameObject marker in markers) {
				if (Target.transform.position.x >= marker.transform.position.x - 2 && Target.transform.position.x <= marker.transform.position.x + 2 && Target.transform.position.z >= marker.transform.position.z - 2 && Target.transform.position.z <= marker.transform.position.z + 2) {
					// get bigElement and move in function of mockup element
					bigElement = getBigElement(Target.name);
					float x = getBigElementCoord (marker.transform.position.x);
					float z = getBigElementCoord (marker.transform.position.z);
					Vector3 bigElementDestPos = new Vector3 (x, bigElement.transform.position.y, z);
					//move real element
					bigElement.GetComponent<Move>().setDestPos (bigElementDestPos);
					bigElement.GetComponent<Move>().setDestRot (bigElement.GetComponent<ElementsInfo>().getFinalRotation());
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
					
					// TEST IF element bien positionné
					if (Target.GetComponent<MockupObjectInfos>().associatedMarker == marker) {
						if (GameObject.Find ("Terrain").GetComponent<NotificationCenter> ().getNumInfo () == 3) {
							GameObject.Find ("Terrain").GetComponent<NotificationCenter> ().setDidacticielNotif (0);
						}
						marker.renderer.material.SetColor("_Color", new Color (9f,248f,9f,255f));
						//marker.renderer.material.mainTexture = Resources.Load ("Textures/green") as Texture;
						avancement++;

						// Instantiate next object if the last one has been well placed
						StartCoroutine (NewObject (bigElement));
					} else {
						marker.renderer.material.SetColor("_Color", new Color (255f,0f,0f,1f));
						//marker.renderer.material.mainTexture = Resources.Load ("Textures/red") as Texture;
						// ajouter tests pr notif spécialisée en fonction de l'objet mal placé
						ElementsInfo ei = bigElement.GetComponent<ElementsInfo>();
						ei.setIndication ("Vous avez mal positionné l'élément, veuillez le placer au bon endroit.");

						GameObject.Find ("Terrain").GetComponent<NotificationCenter> ().setDidacticielNotif (3);

						// audio to say that the element is at the wrong place
						Audio (true);
					}
					// Réinitialiser la couleur du marqueur à "non occupé" si l'élément était sur un marqueur avant ce déplacement
					if (Target.GetComponent<MockupObjectInfos>().currentMarker != null) {
						Target.GetComponent<MockupObjectInfos>().currentMarker.renderer.material.SetColor("_Color", new Color (0f,0f,0f,0f));
						//marker.renderer.material.mainTexture = Resources.Load ("Textures/black") as Texture;
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

	// play again audio info
	public void PlayBack () {
		if (GameObject.Find ("Terrain").GetComponent<NotificationCenter> ().getNumInfo () == 3) {
			GameObject.Find ("Terrain").GetComponent<NotificationCenter> ().setDidacticielNotif (0);
		}
		Audio();
	}

	// play the sound corresponding to the game state
	public void Audio (bool isWrong = false) {
		string son = "Audio/" + avancement;
		if (isWrong)
			son += "w";
		audio.clip = (AudioClip) Resources.Load(son);
		audio.Play();
	}

	// when an element is well placed by the user, instantiate the new one after a while
	IEnumerator NewObject (GameObject bigElement) {
		if (avancement == 9) {
			//yield return new WaitForSeconds (2.0f);
		 	Audio ();
			avancement++;
			//Application.Quit();

		} else if (avancement == 8) {
			//print ("avancement 8");
			yield return new WaitForSeconds (/*14*/2.0f);
			foreach (GameObject mc in mockupObjects){
				if (mc.name == "Délégation alliée" || mc.name == "Délégation allemande") {;
					bigElement = getBigElement (mc.name);
					bigElement.renderer.enabled = false;
					mc.renderer.enabled = false;
				}
			}
			// Get infos of this object
			ElementsInfo ei = bigElement.GetComponent<ElementsInfo>();
			ei.setIndication ("Les deux délégations se rendent dans le wagon du maréchal Foch pour la ratification du traité, ils attendent désormais votre signature.");

		} else {
			if (avancement == 1)
				yield return new WaitForSeconds (/*14*/2.0f);
			else
				yield return new WaitForSeconds (2.0f);
			InstantiateMockupObject (avancement);
			if (avancement == 1)
					bigElement = getBigElement (mockupObjects[0].name);

			// Get infos of this object
			ElementsInfo ei = bigElement.GetComponent<ElementsInfo>();
			string infos = getObjectInfos (avancement);
			ei.setIndication (infos);

			Vector3 initVector = camera.transform.up;
			if (avancement == 7){
				// go back to initial position
				Vector3 destVector = new Vector3 (250, 120.51f, 350) - camera.transform.position;
				Rotation (initVector, destVector);
				avancement = 8;
			} else {
				// rotate to the new object instanciated
				float[] coord = new float[2];
				coord = getObjectCoord (avancement);
				Vector3 bigElementDestPos = new Vector3 (getBigElementCoord (coord[0]), 120.51f, getBigElementCoord(coord[1]));
				Vector3 destVector = bigElementDestPos - camera.transform.position;
				Rotation (initVector, destVector);
			}
			if (avancement != 1)
				Audio ();
		}
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
			markers[i].transform.localScale = new Vector3 (markersScale, 0, markersScale);
			i++;	
		}
		//markers.Add (GameObject.FindGameObjectWithTag("markerCentral"));
		data.Close();
		data = null;
	}

	// instantiate a mockup object in function of his order of apparition, get infos of the element in db
	void InstantiateMockupObject (int order) {
		IDataReader data = camera.GetComponent<DatabaseSync> ().getObject (order);
		string rscType = "Prefabs/Mockup";
		string rscBigType = "Prefabs/";
		while (data.Read()) {
			Vector3 tmp = new Vector3 ((float) data["posX"], (float) 120.51f, (float) data["posZ"]);
			if ((int)data["object_type_id"] == 1) {
				rscType += "Wagon";
				rscBigType += "Wagon";
			} else if ((int)data["object_type_id"] == 2) {
				rscType += "Gun";
				rscBigType += "Canon";
			} else if ((int)data["object_type_id"] == 3) {
				rscType += "Person";
				rscBigType += "Person";
			}
			mockupObjects.Add (Instantiate (Resources.Load (rscType), tmp, new Quaternion (0, 0, 0, 0)) as GameObject);
			mockupObjects.Last().name = (string) data["name"];
			mockupObjects.Last().transform.localScale = new Vector3 (mockupElementsScale, mockupElementsScale, mockupElementsScale);
			// set associated marker to object
			foreach (GameObject marker in markers) {
				if (marker.name == mockupObjects.Last().name)
					mockupObjects.Last().GetComponent<MockupObjectInfos>().setAssociatedMarker(marker);
			}
			InstantiateObject (mockupObjects.Count - 1, rscBigType, (int) data["initRot"], (int) data["finalRot"]);

			// assuprimer ------------------------------------------------------------------------------------------------------------------
			/*if ((int)data["object_type_id"] == 1) {
				mockupWagons.Add (Instantiate (Resources.Load ("Prefabs/MockupWagon"), tmp, new Quaternion (0, 0, 0, 0)) as GameObject);
				mockupWagons.Last().name = (string) data["name"];
				mockupWagons.Last().transform.localScale = new Vector3 (mockupElementsScale, mockupElementsScale, mockupElementsScale);
				foreach (GameObject marker in markers) {
					if (marker.name == mockupWagons.Last().name)
						mockupWagons.Last().GetComponent<MockupObjectInfos>().setAssociatedMarker(marker);
				}
				InstantiateObject (mockupWagons.Count - 1, 1, (int) data["initRot"], (int) data["finalRot"]);
			} else if ((int)data["object_type_id"] == 2) {
				mockupGuns.Add (Instantiate (Resources.Load ("Prefabs/MockupGun"), tmp, new Quaternion (0, 0, 0, 0)) as GameObject);
				mockupGuns.Last().name = (string) data["name"];
				mockupGuns.Last().transform.localScale = new Vector3 (mockupElementsScale, mockupElementsScale, mockupElementsScale);
				foreach (GameObject marker in markers) {
					if (marker.name == mockupGuns.Last().name)
						mockupGuns.Last().GetComponent<MockupObjectInfos>().setAssociatedMarker(marker);
				}
				InstantiateObject (mockupGuns.Count - 1, 2, (int) data["initRot"], (int) data["finalRot"]);
			} else if ((int)data["object_type_id"] == 3) {
				mockupPersons.Add (Instantiate (Resources.Load ("Prefabs/MockupPerson"), tmp, new Quaternion (0, 0, 0, 0)) as GameObject);
				mockupPersons.Last().name = (string) data["name"];
				mockupPersons.Last().transform.localScale = new Vector3 (mockupElementsScale, mockupElementsScale, mockupElementsScale);
				foreach (GameObject marker in markers) {
					if (marker.name == mockupPersons.Last().name)
						mockupPersons.Last().GetComponent<MockupObjectInfos>().setAssociatedMarker(marker);
				}
				InstantiateObject (mockupPersons.Count - 1, 3, (int) data["initRot"], (int) data["finalRot"]);
			}*/
		}
		data.Close();
		data = null;
	}

	// instantiate an object in fct of his mockup equivalent
	void InstantiateObject (int index, string rsc, int rotY, int fRotY) {
		GameObject obj;
		Quaternion rot = new Quaternion (0, 0, 0, 0);
		float x = getBigElementCoord (mockupObjects[index].transform.position.x);
		float z = getBigElementCoord (mockupObjects[index].transform.position.z);
		float y = 8;
		if (rsc == "Prefabs/Person")
			y = 5;
		Vector3 tmp = new Vector3 (x, y, z);
		obj = Instantiate (Resources.Load(rsc), tmp, rot) as GameObject;
		obj.name = mockupObjects[index].name;
		if (rsc != "Prefabs/Person"){
			obj.transform.rotation = Quaternion.Euler (-90, rotY, 0);
			obj.GetComponent<ElementsInfo> ().setFinalRotation (new Vector3 (-90, fRotY, 0));
		} else {
			obj.GetComponent<ElementsInfo> ().setFinalRotation (new Vector3 (0, fRotY, 0));
		}
		/*if (type == 1) {
			float x = getBigElementCoord (mockupWagons[index].transform.position.x);
			float z = getBigElementCoord (mockupWagons[index].transform.position.z);
			Vector3 tmp = new Vector3 (x, 8, z);
			obj = Instantiate (Resources.Load("Prefabs/Wagon"), tmp, rot) as GameObject;
			obj.name = mockupWagons[index].name;
			obj.transform.rotation = Quaternion.Euler (-90, rotY, 0);
		} else if (type == 2) {
			float x = getBigElementCoord (mockupGuns[index].transform.position.x);
			float z = getBigElementCoord (mockupGuns[index].transform.position.z);
			Vector3 tmp = new Vector3 (x, 8, z);
			obj = Instantiate (Resources.Load("Prefabs/Canon"), tmp, rot) as GameObject;
			obj.name = mockupGuns[index].name;
			obj.transform.rotation = Quaternion.Euler (-90, rotY, 0);
		} else {
			float x = getBigElementCoord (mockupPersons[index].transform.position.x);
			float z = getBigElementCoord (mockupPersons[index].transform.position.z);
			Vector3 tmp = new Vector3 (x, 2.3f, z);
			obj = Instantiate (Resources.Load("Prefabs/Person"), tmp, rot) as GameObject;
			obj.name = mockupPersons[index].name;
		}*/
		obj.transform.parent = GameObject.Find ("Terrain").transform;
		if (avancement == 1) {
			ElementsInfo ei = obj.GetComponent<ElementsInfo>();
			string infos = getObjectInfos (avancement);
			ei.setIndication (infos);
		}
	}

	public void Rotation (Vector3 initVector, Vector3 destVector) {
		float angle;
		float sign;
		if (initVector != new Vector3 (0, 0, 0) && destVector != new Vector3 (0, 0, 0)) {
			angle = Vector3.Angle (initVector, destVector);
			sign = Mathf.Sign (Vector3.Dot (new Vector3 (0, 1, 0), Vector3.Cross (initVector, destVector)));
		} else {
			angle = 60.0f;
			sign = 1;
		}
		GetComponent<CameraPosition>().setRotationAngle (angle);
		if ((angle * sign) < 0)
			GetComponent<CameraPosition>().setRotationDirection (false);
		else
			GetComponent<CameraPosition>().setRotationDirection (true);
		GetComponent<CameraPosition>().setBigElementIsMoved (true);
	}
	// unused
	/*void InstantiateObjects () {
		int i = 0;
		List<GameObject> wagons = new List<GameObject> ();
		foreach (GameObject mockupWagon in mockupWagons) {
			float x = getBigElementCoord (mockupWagon.transform.position.x);
			float z = getBigElementCoord (mockupWagon.transform.position.z);
			//Debug.Log ("x val: " + x + " z val :" + z);
			Vector3 tmp = new Vector3 (x, mockupWagon.transform.position.y, z);
			Quaternion rot = new Quaternion (0, 0, 0, 0);
			wagons.Add(Instantiate (Resources.Load("Prefabs/Wagon"), tmp, rot) as GameObject);
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
			guns.Add(Instantiate (Resources.Load("Prefabs/Canon"), tmp, rot) as GameObject);
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
			persons.Add(Instantiate (Resources.Load("Prefabs/Person"), tmp, rot) as GameObject);
			persons[i].name = mockupPerson.name;
			persons[i].transform.parent = GameObject.Find ("Terrain").transform;
			persons[i].transform.localScale = new Vector3 (elementsScale, elementsScale, elementsScale);
			i++;
		}
	}

	// unused
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
	}*/
}
