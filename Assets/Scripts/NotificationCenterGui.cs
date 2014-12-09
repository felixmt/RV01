using UnityEngine;
using System.Collections;

public class NotificationCenterGui : MonoBehaviour {
	private Rect rect;
	public Texture test;
	// Use this for initialization
	void Start () {
		rect = new Rect (5,5,300,200);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		NotificationCenter nc = gameObject.GetComponent<NotificationCenter> ();
		GUI.Label (rect, nc.getIndication());
	}
}
