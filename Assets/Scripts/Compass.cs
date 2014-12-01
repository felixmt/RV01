using UnityEngine;
using System.Collections;

public class Compass : MonoBehaviour {
	private float angle;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void CompassChange (float a) {
		angle = a;
		//print (angle);
	}
	
	public float getAngle () {
		return angle;
	}
}
