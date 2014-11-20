using UnityEngine;
using System.Collections;

public class ElementsInfo : MonoBehaviour {
	private string indications;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setIndication (string indic) {
		indications = indic;
		SendMessageUpwards ("objectChange", indic);
		
	}
}
