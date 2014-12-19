using UnityEngine;
using System.Collections;

public class ObjectInfos : MonoBehaviour {
	private Vector3 finalRotation;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setIndication (string indic) {
		SendMessageUpwards ("objectChange", indic);
		
	}

	public void setFinalRotation (Vector3 rot) {
		finalRotation = rot;
	}

	public Vector3 getFinalRotation () {
		return finalRotation;
	}
}
