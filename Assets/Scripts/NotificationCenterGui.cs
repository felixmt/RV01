using UnityEngine;
using System.Collections;

public class NotificationCenterGui : MonoBehaviour {
	public Texture pinch;
	public Texture release;
	public Texture repeat;
	public Texture point;
	public Texture rotate;
	public Texture circle;
	public GUIStyle style;
	// Use this for initialization
	void Start () {
		//
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		// elements infos
		NotificationCenter nc = gameObject.GetComponent<NotificationCenter> ();

		// didacticiel infos
		switch (gameObject.GetComponent<NotificationCenter> ().getIdIndic ()) {
		case 1:
		{
			GUI.Label (new Rect (10, 20, 370, 17), "  Pour afficher la maquette, effectuez un geste de haut en bas.", style);
			GUI.Label (new Rect (15 + 370, 0, repeat.width, repeat.height), repeat);
			break;
		}
		case 2:
		{
			//GUI.Label (new Rect (Screen.width * 0.3f + 30, Screen.height - 30, 520, 30), "Pour lacher un élément, il suffit d'ouvrir votre main au dessus d'une cible (cercles noirs)");
			//GUI.Label (new Rect (Screen.width * 0.3f + 550, Screen.height - 60, release.width, release.height), release);

			GUI.Label (new Rect (10, 20, 420, 17), " Pour déplacer un élément, placez votre main au dessus puis fermez la.", style);
			GUI.Label (new Rect (15 + 420, 0, pinch.width, pinch.height), pinch);
			break;
		}
		case 3:
		{
			GUI.Label (new Rect (10, 20, 520, 17), "Pour lacher un élément, il suffit d'ouvrir votre main au dessus d'une cible (cercles noirs)", style);
			GUI.Label (new Rect (15 + 520, 0, release.width, release.height), release);
			break;
		}
		case 4:
		{
			GUI.Label (new Rect (10, 20, 390, 17), " Pour faire tourner l'environnement, effectuez un geste horizontal.", style);
			GUI.Label (new Rect (15 + 390, 0, rotate.width, rotate.height), rotate);
			break;
		}
		case 5:
		{
			GUI.Label (new Rect (10, 20, 340, 17), " Pour afficher le traité, effectuez un geste de haut en bas", style);
			GUI.Label (new Rect (15 + 340, 0, repeat.width, repeat.height), repeat);
			break;
		}
		case 6:
		{
			GUI.Label (new Rect (10, 20, 450, 17), " Signez le traité avec votre index, puis effectuez un geste de bas en haut.", style);
			GUI.Label (new Rect (15 + 450, 0, repeat.width, repeat.height), repeat);
			break;
		}
		case 7:
		{
			GUI.Label (new Rect (10, 20, 595, 17), " Pour réécouter l'information concernant cet élément, effectuez une rotation circulaire avec votre index", style);
			GUI.Label (new Rect (15 + 595, 0, circle.width, circle.height), circle);
			break;
		}
		}
		//signature
		foreach (Vector2 dp in nc.getDrawPoints()) {
			GUI.Label (new Rect (dp.x * Screen.width,  Screen.height - dp.y * Screen.height, 10, 10), point);
		}
	}
}
