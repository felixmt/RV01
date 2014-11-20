using UnityEngine;
using System.Collections;

public class NotificationCenter : MonoBehaviour {
	private string indication;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void objectChange (string indic) {
		indication = indic;
	}

	public string getIndication () {
		return indication;
	}
}
