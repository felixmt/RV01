using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Linq;

public class MainControl : MonoBehaviour {
	private List<GameObject> markers;
	private List<GameObject> mockupObjects;
	private GameObject bigElement; // store current bigScaleElement
	private GameObject Target; // current mockupElement
	private int avancement;
	private Vector3 initialTargetPosition;
	// used for didacticiel
	private bool alreadyReleased = false; // is true when the user never released an element
	private bool alreadyRotated = false; // is true when a user already used gestures to rotate the camera
	
	public float markersScale = 0.1f;
	public float mockupElementsScale = 0.5f;
	public float elementsScale = 1;

	// Use this for initialization
	void Start () {
		avancement = 1;
		mockupObjects = new List<GameObject> ();

		InstantiateMarkers ();
		
		StartCoroutine (NewObject (bigElement));
		Audio ();
	
	}
	
	// Update is called once per frame
	void Update () {
		// show comapss
		GetComponent<Compass> ().CompassChange (GameObject.Find ("Camera0").transform.eulerAngles.y);

		if (avancement == 10) {
			StartCoroutine (NewObject (bigElement));
			GameObject.Find ("Terrain").GetComponent<NotificationCenter> ().setIdIndic (0);
		} else if (avancement == 9) { 
			GameObject.Find ("Terrain").GetComponent<NotificationCenter> ().setIdIndic (6);
		} else if (avancement == 8) {
			GameObject.Find ("Terrain").GetComponent<NotificationCenter> ().setIdIndic (5);
			StartCoroutine (NewObject (bigElement));
		} else if (avancement == 2) {
			if (!alreadyRotated)
				GameObject.Find ("Terrain").GetComponent<NotificationCenter> ().setIdIndic (4);
			else
				// mask rotation instructions if user already used it
				if (GameObject.Find("Terrain").GetComponent<NotificationCenter> ().getIdIndic() == 4)
					GameObject.Find("Terrain").GetComponent<NotificationCenter> ().setIdIndic (0);
		}

	}

	// attach an object to the hand
	public void Pinch (GameObject obj) {
		if ((avancement == 1 && obj.name == "Canon des forces alliées") || (avancement == 2 && obj.name == "Wagon du maréchal Foch") || (avancement == 3 && obj.name == "Canon des forces allemandes") || (avancement == 4 && obj.name == "Wagon des forces allemandes") || (avancement == 5 && obj.name == "Délégation alliée") || (avancement == 6 && obj.name == "Délégation allemande"))
		{
			setTarget (obj);
			setInitialTargetPosition (obj.transform.position);
			if (avancement == 1 && !alreadyReleased)
				GameObject.Find ("Terrain").GetComponent<NotificationCenter> ().setIdIndic (3);
		}
	}

	// release an object
	public void Release () {
		alreadyReleased = true;
		if (GameObject.Find ("Terrain").GetComponent<NotificationCenter> ().getIdIndic () == 3)
			GameObject.Find ("Terrain").GetComponent<NotificationCenter> ().setIdIndic (0);

		if (Target != null) {
			bool markerIsFound = false;
			foreach (GameObject marker in markers) {
				if (Target.transform.position.x >= marker.transform.position.x - 0.1 && Target.transform.position.x <= marker.transform.position.x + 0.1 && Target.transform.position.z >= marker.transform.position.z - 0.1 && Target.transform.position.z <= marker.transform.position.z + 0.1) {
					// get bigElement and move in function of mockup element
					bigElement = getBigElementByName(Target.name);

					// move mockup element
					Target.transform.position = new Vector3 (marker.transform.position.x, Target.transform.position.y, marker.transform.position.z);
					//move real element
					bigElement.GetComponent<Move>().setDestPos (new Vector3 (marker.transform.position.x * 38.46f, 0.85f, marker.transform.position.z * 38.46f));
					bigElement.GetComponent<Move>().setDestRot (bigElement.GetComponent<ObjectInfos>().getFinalRotation());
					//bigElement.GetComponent<Move>().setDestPos (new Vector3 (Target.transform.localPosition.x, 0.09f, Target.transform.localPosition.z));

					//Move camera in function of moved element position
					VerticalRotation (false);
					StartCoroutine (WaitBeforeHorizontalRotation(marker));
					
					// TEST IF element bien positionné
					if (Target.GetComponent<MockupObjectInfos>().associatedMarker == marker) {
						if (GameObject.Find ("Terrain").GetComponent<NotificationCenter> ().getIdIndic () == 3) {
							GameObject.Find ("Terrain").GetComponent<NotificationCenter> ().setIdIndic (0);
						}
						marker.renderer.material.SetColor("_Color", new Color (9f,248f,9f,255f));
						//marker.renderer.material.mainTexture = Resources.Load ("Textures/green") as Texture;

						avancement++;
						// Instantiate next object if the last one has been well placed
						StartCoroutine (NewObject (bigElement));
					} else {
						marker.renderer.material.SetColor("_Color", new Color (255f,0f,0f,1f));
						//marker.renderer.material.mainTexture = Resources.Load ("Textures/red") as Texture;
						
						
						GameObject.Find ("Terrain").GetComponent<NotificationCenter> ().setIdIndic (7);
						
						// audio to say that the element is at the wrong placed
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
				Target.transform.position = initialTargetPosition;
				Target = null;
			}
		}
	}

	// when an element is well placed by the user, instantiate the new one after a while
	IEnumerator NewObject (GameObject bigElement) {
		if (avancement == 10) {
			Audio ();
			avancement++;
		} else if (avancement == 8) {
			yield return new WaitForSeconds (4.0f); // A CONFIRMER ? ---------------------------------
			foreach (GameObject mc in mockupObjects) {
				if (mc.name == "Délégation alliée" || mc.name == "Délégation allemande") {
					bigElement = getBigElementByName (mc.name);
					bigElement.renderer.enabled = false;
					mc.renderer.enabled = false;
				}
			}
		}
		else {
			setTargetAsCurrent (avancement, 1);
			if (avancement == 1) {
				InstantiateMockupObject (avancement);
				yield return new WaitForSeconds (13.0f);
				GameObject.Find ("Terrain").GetComponent<NotificationCenter> ().setIdIndic (1);
			}
			else {
				yield return new WaitForSeconds (5.5f);
				InstantiateMockupObject (avancement);
			}
			if (avancement == 7) {
				Vector3 initVector = GameObject.Find ("Camera0").transform.forward; // A REVOIR AVEC OCULUS
				Vector3 destVector = new Vector3 (0, 0, 1) - new Vector3 (0, 0, 0);//GameObject.Find ("Camera0").transform.position;
				Rotation (initVector, destVector);
				avancement++;
			} else {
				bigElement = getBigElementByName (mockupObjects [avancement-1].name);
				// rotate to the new object instanciated
				Vector3 initVector = GameObject.Find ("Camera0").transform.forward; // A REVOIR AVEC OCULUS
				Vector3 destVector = bigElement.transform.position - new Vector3 (0, 0, 0);//GameObject.Find ("Camera0").transform.position;
				Rotation (initVector, destVector);
			}
			if (avancement != 1)
				Audio ();
		}
	}

	// wait before horizontal rotation
	IEnumerator WaitBeforeHorizontalRotation (GameObject bigElement) {
		yield return new WaitForSeconds (2.0f);
		Vector3 initVector = GameObject.Find ("Camera0").transform.forward; // A REVOIR AVEC OCULUS
		Vector3 destVector = new Vector3 (bigElement.transform.position.x * 38.46f, 0.45f, bigElement.transform.position.z * 38.46f) - new Vector3 (0, 0, 0);//GameObject.Find ("Camera0").transform.position;
		Rotation (initVector, destVector);
	}

	// play the sound corresponding to the game state
	public void Audio (bool isWrong = false) {
		string son = "Audio/" + avancement;
		if (isWrong)
			son += "w";
		audio.clip = (AudioClip) Resources.Load(son);
		audio.Play();
	}

	// play again audio info
	public void PlayBack () {
		if (GameObject.Find ("Terrain").GetComponent<NotificationCenter> ().getIdIndic () == 3) {
			GameObject.Find ("Terrain").GetComponent<NotificationCenter> ().setIdIndic (0);
		}
		Audio();
	}

	/* horizontal rotation */
	public void Rotation (Vector3 initVector, Vector3 destVector) {
		float angle = 0.0f;
		float sign = 0.0f;
		/* horizontal swipe from right to left --> clockwise rotation*/
		if (initVector == new Vector3 (0, 0, 0) && destVector == new Vector3 (0, 0, 0)) {
			angle = 60.0f;
			sign = 1;
			alreadyRotated = true;
			/* horizontal swipe from left to right --> anti-clockwise rotation */
		} else if (initVector == new Vector3 (-1, -1, -1) && destVector == new Vector3 (-1, -1, -1)) {
			angle = 60.0f;
			sign = -1;
			alreadyRotated = true;
		// automatic rotation
		} else {
			angle = Vector3.Angle (initVector, destVector);
			sign = Mathf.Sign (Vector3.Dot (new Vector3 (0, 1, 0), Vector3.Cross (initVector, destVector)));
		}
		GameObject.Find ("CenterNode").GetComponent<Rotation> ().setRotationAngle (angle);
		GameObject.Find ("CenterNode").GetComponent<Rotation> ().setRotationAxis (true);
		//gameObject.GetComponentInParent<Rotation>().setRotationAngle (angle);
		if ((angle * sign) < 0)
			GameObject.Find ("CenterNode").GetComponent<Rotation> ().setRotationDirection (false);
		else
			GameObject.Find ("CenterNode").GetComponent<Rotation> ().setRotationDirection (true);
		GameObject.Find ("CenterNode").GetComponent<Rotation> ().setBigElementIsMoved (true);
	}

	// vertical rotation (false = from down to up
	public void VerticalRotation (bool sens = true) {
		if (avancement == 1 && GameObject.Find ("Terrain").GetComponent<NotificationCenter> ().getIdIndic () == 1)
		GameObject.Find ("Terrain").GetComponent<NotificationCenter> ().setIdIndic (2);
		GameObject.Find ("CenterNode").GetComponent<Rotation> ().setRotationAngle (90.0f);
		GameObject.Find ("CenterNode").GetComponent<Rotation> ().setRotationAxis (false);
		GameObject.Find ("CenterNode").GetComponent<Rotation> ().setRotationDirection (sens);
		GameObject.Find ("CenterNode").GetComponent<Rotation> ().setBigElementIsMoved (true);
	}
	
	// if user can perform a rotation
	public bool Unrotable () {
		return GameObject.Find ("CenterNode").GetComponent<Rotation> ().getBigElementIsMoved();
	}

	// OBJECTS INSTANTIATIONS -------------------------------------------------------------------------------------------------------
	// Instantiate markers in fct of db
	void InstantiateMarkers () {
		IDataReader data = gameObject.GetComponent<DatabaseSync> ().getTableByName ("Marker");		
		// Instantiate Markers
		markers = new List<GameObject> ();
		int i = 0;
		while (data.Read()) {
			Vector3 tmp = new Vector3 ((float) data["posX"], (float) data["posY"], (float) data["posZ"]);
			markers.Add (Instantiate (Resources.Load ("Prefabs/Markers"), tmp, new Quaternion (0, 0, 0, 0)) as GameObject);
			markers[i].name = (string) data["name"];
			markers[i].transform.parent = GameObject.Find ("MapNode").transform;
			//markers[i].transform.localScale = new Vector3 (markersScale, 0, markersScale);
			i++;	
		}
		data.Close();
		data = null;
	}

	// instantiate a mockup object in function of his order of apparition, get infos of the element in db
	void InstantiateMockupObject (int order) {
		IDataReader data = gameObject.GetComponent<DatabaseSync> ().getObjectByOrder (order);
		string rscType = "Prefabs/Mockup";
		string rscBigType = "Prefabs/";
		while (data.Read()) {
			Vector3 tmp = new Vector3 ((float) data["posX"], (float) data["posY"], (float) data["posZ"]);
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
			mockupObjects.Last().transform.parent = gameObject.transform;
			//mockupObjects.Last().transform.localScale = new Vector3 (mockupElementsScale, mockupElementsScale, mockupElementsScale);
			// set associated marker to object
			foreach (GameObject marker in markers) {
				if (marker.name == mockupObjects.Last().name)
					mockupObjects.Last().GetComponent<MockupObjectInfos>().setAssociatedMarker(marker);
			}
			InstantiateObject (mockupObjects.Count - 1, rscBigType, (int) data["initRot"], (int) data["finalRot"]);
		}
		data.Close();
		data = null;
	}

	// instantiate an object in fct of his mockup equivalent
	void InstantiateObject (int index, string rsc, int rotY, int fRotY) {
		GameObject obj;
		Quaternion rot = new Quaternion (0, 0, 0, 0);
		obj = Instantiate (Resources.Load(rsc), new Vector3 (0, 0, 0), rot) as GameObject;
		obj.name = mockupObjects[index].name;
		obj.transform.parent = GameObject.Find ("Terrain").transform;
		//obj.transform.localPosition = mockupObjects [index].transform.localPosition;
		obj.transform.localPosition = new Vector3 (mockupObjects [index].transform.localPosition.x, 0.166f, mockupObjects [index].transform.localPosition.z);
		if (rsc != "Prefabs/Person"){
			obj.transform.localScale = new Vector3 (20, 14, 20);
			obj.transform.rotation = Quaternion.Euler (-90, rotY, 0);
			obj.GetComponent<ObjectInfos> ().setFinalRotation (new Vector3 (-90, fRotY, 0));
		} else {
			obj.GetComponent<ObjectInfos> ().setFinalRotation (new Vector3 (0, fRotY, 0));
		}
	}

	// ACCESSOR MUTATOR --------------------------------------------
	public GameObject getBigElementByName (string name) {
		return GameObject.Find("/Terrain/" + name);
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

	// get the current element
	public GameObject getTarget () {
		return Target;
	}

	// set the selected element as Target
	public void setTarget (GameObject targ) {
		Target = targ;
	}

	// save the initial position of an element when it is being selected
	public void setInitialTargetPosition (Vector3 vec) {
		initialTargetPosition = vec;
	}

	// set selected object as current object in db / unset released object
	public void setTargetAsCurrent (int objectOrder, int value) {
		gameObject.GetComponent<DatabaseSync>().setIsCurrent (objectOrder, value);
	}

	void setCurrentMarker (string objectName, string markerName) {
		gameObject.GetComponent<DatabaseSync> ().setCurrentMarker (objectName, markerName);
	}
}
