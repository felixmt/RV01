using UnityEngine;
using System.Collections;

public class NotificationCenterGui : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		NotificationCenter nc = gameObject.GetComponent<NotificationCenter> ();
		GUI.Label (new Rect(5,5,300,200), "Informations pour placer l'élément :" + nc.getIndication());
	}
}
