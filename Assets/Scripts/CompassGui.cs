using UnityEngine;
using System.Collections;

public class CompassGui : MonoBehaviour {
	public Texture arrow;

	// Use this for initialization
	void Start () {	
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnGUI () {
		Compass c = gameObject.GetComponent<Compass> ();
		GUIUtility.RotateAroundPivot (c.getAngle (), new Vector2 (Screen.width - 50, 55));
		GUI.Label (new Rect (Screen.width - 80, 5, 100, 100), arrow);
	}
}
