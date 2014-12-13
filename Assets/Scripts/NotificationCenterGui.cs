using UnityEngine;
using System.Collections;

public class NotificationCenterGui : MonoBehaviour {
	private Rect rect;
	public Texture pinch;
	public Texture release;
	public Texture point;
	// Use this for initialization
	void Start () {
		rect = new Rect (5,5,300,200);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		// elements infos
		NotificationCenter nc = gameObject.GetComponent<NotificationCenter> ();
		//GUI.Label (rect, nc.getIndication());

		// didacticiel infos
		switch (gameObject.GetComponent<NotificationCenter> ().getNumInfo ()) {
		case 1:
		{
			GUI.Label (new Rect (Screen.width * 0.3f + 30, Screen.height - 30, 410, 30), "Pour déplacer un élément, placez votre main au dessus puis fermez là");
			GUI.Label (new Rect (Screen.width * 0.3f + 450, Screen.height - 60, pinch.width, pinch.height), pinch);

			break;
		}
		case 2:
		{
			GUI.Label (new Rect (Screen.width * 0.3f + 30, Screen.height - 30, 520, 30), "Pour lacher un élément, il suffit d'ouvrir votre main au dessus d'une cible (cercles noirs)");
			GUI.Label (new Rect (Screen.width * 0.3f + 550, Screen.height - 60, release.width, release.height), release);

			break;
		}
		case 3:
		{
			GUI.Label (new Rect (Screen.width * 0.3f + 30, Screen.height - 30, 540, 30), "Pour réécouter l'information concernant cet élément, effectuez une rotation avec votre main");
			break;
		}
		}

		//signature
		foreach (Vector2 dp in nc.getDrawPoints()) {
			GUI.Label (new Rect (dp.x * Screen.width * 0.3f,  Screen.height - (dp.y * Screen.height * 0.5f), 10, 10), point);
		}
	}
}
