using UnityEngine;
using System.Collections;

public class NotificationCenterGui : MonoBehaviour {
	private Rect rect;
	public Texture pinch;
	public Texture release;
	public Texture repeat;
	public Texture point;
	public Texture rotate;
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

		// didacticiel infos
		switch (gameObject.GetComponent<NotificationCenter> ().getIdIndic ()) {
		case 1:
		{
			GUI.Label (new Rect (10, 20, 410, 30), "Pour déplacer un élément, placez votre main au dessus puis fermez la.");
			GUI.Label (new Rect (10 + 420, 0, pinch.width, pinch.height), pinch);
			break;
		}
		case 2:
		{
			//GUI.Label (new Rect (Screen.width * 0.3f + 30, Screen.height - 30, 520, 30), "Pour lacher un élément, il suffit d'ouvrir votre main au dessus d'une cible (cercles noirs)");
			//GUI.Label (new Rect (Screen.width * 0.3f + 550, Screen.height - 60, release.width, release.height), release);

			GUI.Label (new Rect (10, 20, 520, 30), "Pour lacher un élément, il suffit d'ouvrir votre main au dessus d'une cible (cercles noirs)");
			GUI.Label (new Rect (10 + 520, 0, release.width, release.height), release);
			break;
		}
		case 3:
		{
			//GUI.Label (new Rect (Screen.width * 0.3f + 30, Screen.height - 30, 540, 30), "Pour réécouter l'information concernant cet élément, effectuez une rotation avec votre main");
			GUI.Label (new Rect (10, 20, 550, 30), "Pour réécouter l'information concernant cet élément, effectuez un geste rapide de bas en haut");
			GUI.Label (new Rect (10 + 545, 0, repeat.width, repeat.height), repeat);
			break;
		}
		case 4:
		{
			GUI.Label (new Rect (10, 20, 410, 40), "Pour déclencher la saisie de la signature, effectuez un geste de bas en haut. Vous disposerez ensuite de 4 secondes.");
			GUI.Label (new Rect (10 + 420, 0, repeat.width, repeat.height), repeat);
			break;
		}
		case 5:
		{
			GUI.Label (new Rect (10, 20, 410, 40), "Pour faire tourner lenvironnement autour de vous, effectuez un geste horizontal rapide.");
			GUI.Label (new Rect (10 + 420, 0, rotate.width, rotate.height), rotate);
			break;
		}
		}
		//signature
		foreach (Vector2 dp in nc.getDrawPoints()) {
			GUI.Label (new Rect (dp.x * Screen.width * 0.3f,  Screen.height - (dp.y * Screen.height * 0.5f), 10, 10), point);
		}
	}
}
