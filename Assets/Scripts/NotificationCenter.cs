using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NotificationCenter : MonoBehaviour {
	private string indication;
	private int num_info;
	private List<Vector2> drawPoints;

	// Use this for initialization
	void Start () {
		num_info = 1;
		drawPoints = new List<Vector2> ();
	
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

	public void setDidacticielNotif (int avancement) {
		num_info = avancement;
	}

	public int getNumInfo () {
		return num_info;
	}

	public void setDrawPoint (Vector2 dp) {
		drawPoints.Add (dp);
	}

	public List<Vector2> getDrawPoints () {
		return drawPoints;
	}
}
