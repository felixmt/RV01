using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;

public class VRGUIMenuSample : MonoBehaviour {

	// Disable warning CS0414 "The private field 'XXX' is assigned but its value is never used"
	#pragma warning disable 0414

	private vrGUIRendererWeb m_GUIRendererWeb;
	private vrWidgetMenu m_Menu;
	private vrWidgetButton m_Button1;
	private vrWidgetToggleButton m_Checkbox;
	private vrWidgetMenu m_Submenu;
	private vrWidgetRadioButton m_Radio1;
	private vrWidgetRadioButton m_Radio2;
	private vrWidgetRadioButton m_Radio3;
	private vrWidgetColorPicker m_Picker;
	private vrWidgetSlider m_Slider;
	private vrWidgetList m_List;

	#pragma warning restore 0414

	private vrCommandUnity m_ButtonCommand;
	private vrCommandUnity m_CheckboxCommand;
	private vrCommandUnity m_RadioCommand;
	private vrCommandUnity m_ColorPickerCommand;
	private vrCommandUnity m_SliderCommand;
	private vrCommandUnity m_ListCommand;

	private vrValue ButtonHandler(vrValue iValue)
	{
		m_Checkbox.SetChecked( ! m_Checkbox.IsChecked() );
		Debug.Log( "ButtonHandler() called" );
		return null;
	}

	private vrValue CheckboxHandler(vrValue iValue)
	{
		Debug.Log( "Checkbox value : " + iValue.GetBool().ToString() );
		return null;
	}
	
	private vrValue RadioHandler(vrValue iValue)
	{
		Debug.Log( "Radio value : " + iValue.GetString() );
		return null;
	}
	
	private vrValue ColorPickerHandler(vrValue iValue)
	{
		vrVec4 color = iValue.GetVec4();
		Debug.Log( "Selected color : " + color.x().ToString() + " " + color.y().ToString() + " " + color.z().ToString() );
		return null;
	}
	
	private vrValue SliderHandler(vrValue iValue)
	{
		Debug.Log( "Slider value : " + iValue.GetFloat().ToString() );
		return null;
	}
	
	private vrValue ListHandler(vrValue iValue)
	{
		Debug.Log( "List Selected Index : " + iValue.GetInt() );
		return null;
	}

	// Use this for initialization
	void Start () {
		// Create commands
		
		m_ButtonCommand = new vrCommandUnity("ButtonCommand", ButtonHandler);
		m_CheckboxCommand = new vrCommandUnity("CheckboxCommand", CheckboxHandler);
		m_RadioCommand = new vrCommandUnity("RadioCommand", RadioHandler);
		m_ColorPickerCommand = new vrCommandUnity("ColorPickerCommand", ColorPickerHandler);
		m_SliderCommand = new vrCommandUnity("SliderCommand", SliderHandler);
		m_ListCommand = new vrCommandUnity("ListCommand", ListHandler);

		// Create GUI

		m_GUIRendererWeb = null;

		VRWebView webViewScript = GetComponent<VRWebView>();

        if (webViewScript == null || webViewScript.webView == null )
        {
			return;
		}

        m_GUIRendererWeb = new vrGUIRendererWeb("", webViewScript.webView);

		m_Menu = new vrWidgetMenu("", m_GUIRendererWeb);
		
		m_Button1 = new vrWidgetButton("", m_Menu, "Button", m_ButtonCommand);

        new vrWidgetSeparator("", m_Menu);

		m_Checkbox = new vrWidgetToggleButton("", m_Menu, "Toggle Button", m_CheckboxCommand, true);
		
		m_Submenu = new vrWidgetMenu("", m_Menu, "Sub Menu");
		m_Submenu.SetVisible(true);

		m_Radio1 = new vrWidgetRadioButton("", m_Submenu, "Huey", m_RadioCommand, "Huey" );
		m_Radio2 = new vrWidgetRadioButton("", m_Submenu, "Dewey", m_RadioCommand, "Dewey" );
		m_Radio3 = new vrWidgetRadioButton("", m_Submenu, "Louie", m_RadioCommand, "Louie" );
		
		m_Picker = new vrWidgetColorPicker("", m_Menu, "Color Picker", m_ColorPickerCommand, new vrVec4(0,0,0,0));

		m_Slider = new vrWidgetSlider("", m_Menu, "Slider", m_SliderCommand, 50.0f, 0.0f, 100.0f, 1.0f);
		
		vrValue listContents = vrValue.CreateList();
		listContents.AddListItem( "Item 1" );
		listContents.AddListItem( "Item 2" );
		
		m_List = new vrWidgetList("", m_Menu, "List", m_ListCommand, listContents, 0 );
	}
	
	// Update is called once per frame
	void Update () {
	}
}
