/* VRGUICustomSample

 * MiddleVR
 * (c) i'm in VR
 */

/*
 * 
 * Use with data/GUI/CustomSample/index.html in MiddleVR install directory
 * 
 */

using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;

public class VRGUICustomSample : MonoBehaviour {
		

	// Disable warning CS0414 "The private field 'XXX' is assigned but its value is never used"
	// We need to hold these commands in a field to prevent the Garbage Collector from
	// destroying them
	#pragma warning disable 0414

	private vrCommandUnity m_ButtonCommand;
	private vrCommandUnity m_RadioCommand;
	private vrCommandUnity m_SliderCommand;

	#pragma warning restore 0414

	private int m_Progress = 0;

	private vrValue ButtonHandler(vrValue iValue)
	{
		m_Progress += 1;
		
        GetComponent<VRWebView>().webView.ExecuteJavascript("$('#progressbar').progressbar('value'," + m_Progress.ToString() + ");");

		return null;
	}

	private vrValue RadioHandler(vrValue iValue)
	{
		if(iValue.IsString())
		{
			Debug.Log("Radio value = " + iValue.GetString() );
		}
		return null;
	}

	private vrValue SliderHandler(vrValue iValue) {
		if(iValue.IsNumber())
		{
			Debug.Log("Slider value as Number = " + iValue.GetNumber() );
		}
		return null;
	}
	
	void Start ()
	{		
		m_ButtonCommand = new vrCommandUnity("ButtonCommand", ButtonHandler);
		m_RadioCommand = new vrCommandUnity("RadioCommand", RadioHandler);
		m_SliderCommand = new vrCommandUnity("SliderCommand", SliderHandler);
	}
	
	void Update () {
	}
}
