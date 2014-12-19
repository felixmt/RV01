using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NotificationCenter : MonoBehaviour {
	private int idIndic;
	private List<Vector2> drawPoints;

	// Use this for initialization
	void Start () {
		idIndic = 1;
		drawPoints = new List<Vector2> ();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setIdIndic (int avancement) {
		idIndic = avancement;
	}

	public int getIdIndic () {
		return idIndic;
	}

	// signature drawing
	public void setDrawPoint (Vector2 dp) {
		drawPoints.Add (dp);
	}

	public List<Vector2> getDrawPoints () {
		return drawPoints;
	}
}
