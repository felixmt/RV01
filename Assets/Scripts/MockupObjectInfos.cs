using UnityEngine;
using System.Collections;

public class MockupObjectInfos : MonoBehaviour {

	public GameObject associatedMarker;
	public GameObject currentMarker;
	public string description;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setAssociatedMarker (GameObject am) {
		associatedMarker = am;
	}
}
