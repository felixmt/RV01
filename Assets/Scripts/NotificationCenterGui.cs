﻿using UnityEngine;
using System.Collections;

public class NotificationCenterGui : MonoBehaviour {
	private Rect rect;
	// Use this for initialization
	void Start () {
		Rect rect = new Rect (5,5,300,200);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		NotificationCenter nc = gameObject.GetComponent<NotificationCenter> ();
		//GUI.Label (new Rect (5,5,300,200), "Informations pour placer l'élément :" + nc.getIndication());
	}
}
