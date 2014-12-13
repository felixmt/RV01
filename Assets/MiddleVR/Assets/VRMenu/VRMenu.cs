/* VRMenu
 * MiddleVR
 * (c) i'm in VR
 */

using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;

public class VRMenu : MonoBehaviour {

    private VRManagerScript m_VRManager;

    // Disable warning CS0414 "The private field 'XXX' is assigned but its value is never used"
    #pragma warning disable 0414

    private vrGUIRendererWeb m_GUIRendererWeb;
    private vrWidgetMenu     m_Menu;

    private vrEventListenerUnity m_Listener;

    // General
    private vrWidgetMenu         m_ResetButtonMenu;
    private vrWidgetButton       m_ResetCurrentButton;
    private vrWidgetButton       m_ResetZeroButton;
    private vrWidgetMenu         m_ExitButtonMenu;
    private vrWidgetButton       m_ExitButton;
    private vrWidgetMenu         m_GeneralOptions;
    private vrWidgetToggleButton m_FramerateCheckbox;
    private vrWidgetToggleButton m_ProxiWarningCheckbox;
    private vrWidgetSeparator    m_GeneralSeparator;

    // Navigation
    private vrWidgetMenu         m_NavigationOptions;
    private vrWidgetRadioButton  m_NoNavigationRadioButton;
    private vrWidgetRadioButton  m_JoystickNavigationRadioButton;
    private vrWidgetRadioButton  m_ElasticNavigationRadioButton;
    private vrWidgetRadioButton  m_GrabWorldNavigationRadioButton;
    //private vrWidgetRadioButton  m_PointAndGoNavigationRadioButton;
    private vrWidgetSeparator    m_NavigationSeparator;
    private vrWidgetToggleButton m_FlyCheckbox;
    private vrWidgetToggleButton m_CollisionsCheckbox;

    // Manipulation
    private vrWidgetMenu         m_ManipulationOptions;
    private vrWidgetRadioButton  m_NoManipulationRadioButton;
    private vrWidgetRadioButton  m_RayManipulationRadioButton;
    private vrWidgetRadioButton  m_HomerManipulationRadioButton;
    private vrWidgetSeparator    m_ManipulationSeparator;
    private vrWidgetToggleButton m_ReturnObjectsCheckbox;

    // Virtual Hand
    private vrWidgetMenu        m_VirtualHandOptions;
    private vrWidgetRadioButton m_DirectHandRadioButton;
    private vrWidgetRadioButton m_GogoHandRadioButton;


    #pragma warning restore 0414

    // General
    private vrCommandUnity m_ResetCurrentButtonCommand;
    private vrCommandUnity m_ResetZeroButtonCommand;
    private vrCommandUnity m_ExitButtonCommand;
    private vrCommandUnity m_FramerateCheckboxCommand;
    private vrCommandUnity m_ProxiWarningCheckboxCommand;

    // Navigation
    private vrCommandUnity m_NavigationModeRadioCommand;
    private vrCommandUnity m_FlyCheckboxCommand;
    private vrCommandUnity m_CollisionsCheckboxCommand;

    // Manipulation
    private vrCommandUnity m_ManipulationModeRadioCommand;
    private vrCommandUnity m_ReturnObjectsCheckboxCommand;

    // Virtual Hand
    private vrCommandUnity m_VirtualHandModeRadioCommand;

    private bool EventListener(vrEvent iEvent)
    {
        vrGenericPluginEvent evt = vrGenericPluginEvent.Cast(iEvent);

        if (evt != null)
        {
            if (evt.GetName() == "OSCEvent")
            {
                uint nb = evt.value.GetListItemsNb();
                print( "OSC Event, nb args: " + nb );

                string name = evt.value.GetListItem(0).GetString();
                print(" - name: " + name );

                /*
                for (uint32 i = 1; i < nb; ++i)
                {
                    VRLOGI << " - arg " << i << ": ";
                    vrValue val = evt->value.GetListItem(i);

                    if (val.IsBool())
                    {
                        VRLOGI << val.GetBool() << endl;
                    }

                    if (val.IsNumber())
                    {
                        VRLOGI << val.GetFloat() << endl;
                    }
                }*/

                if (name == "/main/fps")
                {
                    m_FramerateCheckbox.Toggle();
                }
                else if (name == "/main/fly")
                {
                    m_FlyCheckbox.Toggle();
                }
                else if (name == "/main/exit")
                {
                    m_ExitButton.Trigger();
                }
                else if (name == "/main/navig_joystick")
                {
                    m_JoystickNavigationRadioButton.SetChecked();
                }
                else if (name == "/main/navig_elastic")
                {
                    m_ElasticNavigationRadioButton.SetChecked();
                }
            }
        }

        return true;
    }

    // General
    private vrValue ResetCurrentButtonHandler(vrValue iValue)
    {
        MiddleVRTools.Log("[ ] Reload current level.");
        Application.LoadLevel(Application.loadedLevel);
        return null;
    }

    private vrValue ResetZeroButtonHandler(vrValue iValue)
    {
        MiddleVRTools.Log("[ ] Reload level zero.");
        Application.LoadLevel(0);
        return null;
    }

    private vrValue ExitButtonHandler(vrValue iValue)
    {
        MiddleVRTools.Log("[ ] Exit simulation.");
        m_VRManager.QuitApplication();
        return null;
    }

    private vrValue FramerateCheckboxHandler(vrValue iValue)
    {
        m_VRManager.ShowFPS = iValue.GetBool();
        MiddleVRTools.Log("[ ] Show Frame Rate: " + iValue.GetBool().ToString());
        return null;
    }

    private vrValue ProxiWarningCheckboxHandler(vrValue iValue)
    {
        m_VRManager.ShowScreenProximityWarnings = iValue.GetBool();
        MiddleVRTools.Log("[ ] Show proximity warnings: " + iValue.GetBool().ToString());
        return null;
    }

    private vrValue NavigationModeRadioHandler(vrValue iValue)
    {
        VRManagerScript.ENavigationMode newNavigationMode = (VRManagerScript.ENavigationMode)iValue.GetInt();
        m_VRManager.NavigationMethod = newNavigationMode;
        _RefreshNavigationMode();

        MiddleVRTools.Log("[ ] Switch to navigation mode: " + iValue.GetString());
        return null;
    }

    private vrValue FlyCheckboxHandler(vrValue iValue)
    {
        m_VRManager.Fly = iValue.GetBool();
        MiddleVRTools.Log("[ ] Fly mode: " + iValue.GetBool().ToString());
        return null;
    }

    private vrValue CollisionsCheckboxHandler(vrValue iValue)
    {
        m_VRManager.NavigationCollisions = iValue.GetBool();
        MiddleVRTools.Log("[ ] Navigation Collisions: " + iValue.GetBool().ToString());
        return null;
    }

    private vrValue ManipulationModeRadioHandler(vrValue iValue)
    {
        VRManagerScript.EManipulationMode newManipulationMode = (VRManagerScript.EManipulationMode)iValue.GetInt();
        m_VRManager.ManipulationMethod = newManipulationMode;
        _RefreshManipulationMode();

        MiddleVRTools.Log("[ ] Switch to manipulation mode: " + newManipulationMode);
        return null;
    }

    private vrValue ReturnObjectsCheckboxHandler(vrValue iValue)
    {
        m_VRManager.ManipulationReturnObjects = iValue.GetBool();
        MiddleVRTools.Log("[ ] Manipulation return objects: " + iValue.GetBool().ToString());
        return null;
    }

    private vrValue VirtualHandRadioHandler(vrValue iValue)
    {
        VRManagerScript.EVirtualHand newVirtualHand = (VRManagerScript.EVirtualHand)iValue.GetInt();
        m_VRManager.VirtualHandMapping = newVirtualHand;
        _RefreshVirtualHand();

        MiddleVRTools.Log("[ ] Switch to virtual hand: " + newVirtualHand);
        return null;
    }


    void Start ()
    {
        // Retrieve the VRManager
        VRManagerScript[] foundVRManager = FindObjectsOfType(typeof(VRManagerScript)) as VRManagerScript[];
        if (foundVRManager.Length != 0)
        {
            m_VRManager = foundVRManager[0];
        }
        else
        {
            MiddleVRTools.Log("[X] VRMenu: impossible to retrieve the VRManager.");
            return;
        }

        m_Listener = new vrEventListenerUnity(EventListener);
        MiddleVR.VRKernel.AddEventListener(m_Listener);

        // Create commands

        // General
        m_ResetCurrentButtonCommand   = new vrCommandUnity("ResetCurrentButtonCommand", ResetCurrentButtonHandler, (uint)VRCommandFlags.VRCommandFlag_Cluster);
        m_ResetZeroButtonCommand      = new vrCommandUnity("ResetZeroButtonCommand", ResetZeroButtonHandler, (uint)VRCommandFlags.VRCommandFlag_Cluster);
        m_ExitButtonCommand           = new vrCommandUnity("ExitButtonCommand", ExitButtonHandler, (uint)VRCommandFlags.VRCommandFlag_Cluster);
        m_FramerateCheckboxCommand    = new vrCommandUnity("FramerateCheckboxCommand", FramerateCheckboxHandler, (uint)VRCommandFlags.VRCommandFlag_Cluster);
        m_ProxiWarningCheckboxCommand = new vrCommandUnity("ProxiWarningCheckboxCommand", ProxiWarningCheckboxHandler, (uint)VRCommandFlags.VRCommandFlag_Cluster);

        // Navigation
        m_NavigationModeRadioCommand = new vrCommandUnity("NavigationModeRadioCommand", NavigationModeRadioHandler, (uint)VRCommandFlags.VRCommandFlag_Cluster);
        m_FlyCheckboxCommand         = new vrCommandUnity("FlyCheckboxCommand", FlyCheckboxHandler, (uint)VRCommandFlags.VRCommandFlag_Cluster);
        m_CollisionsCheckboxCommand  = new vrCommandUnity("CollisionsCheckboxCommand", CollisionsCheckboxHandler, (uint)VRCommandFlags.VRCommandFlag_Cluster);

        // Manipulation
        m_ManipulationModeRadioCommand = new vrCommandUnity("ManipulationModeRadioCommand", ManipulationModeRadioHandler, (uint)VRCommandFlags.VRCommandFlag_Cluster);
        m_ReturnObjectsCheckboxCommand      = new vrCommandUnity("ReturnObjectsCheckboxCommand", ReturnObjectsCheckboxHandler, (uint)VRCommandFlags.VRCommandFlag_Cluster);

        // Virtual Hand
        m_VirtualHandModeRadioCommand = new vrCommandUnity("VirtualHandModeRadioCommand", VirtualHandRadioHandler, (uint)VRCommandFlags.VRCommandFlag_Cluster);

        // Create GUI
        m_GUIRendererWeb = null;

        VRWebView webViewScript = GetComponent<VRWebView>();

        if (webViewScript == null || webViewScript.webView == null)
        {
            return;
        }

        m_GUIRendererWeb = new vrGUIRendererWeb("", webViewScript.webView);

        m_Menu = new vrWidgetMenu("VRManagerMenu", m_GUIRendererWeb);

        // Navigation
        m_NavigationOptions = new vrWidgetMenu("NavigationOptions", m_Menu, "Navigation");

        m_JoystickNavigationRadioButton   = new vrWidgetRadioButton("JoystickNavigationRadioButton", m_NavigationOptions, "Joystick", m_NavigationModeRadioCommand, (int)VRManagerScript.ENavigationMode.Joystick, m_VRManager.NavigationMethod == VRManagerScript.ENavigationMode.Joystick);
        m_ElasticNavigationRadioButton    = new vrWidgetRadioButton("ElasticNavigationRadioButton", m_NavigationOptions, "Elastic", m_NavigationModeRadioCommand, (int)VRManagerScript.ENavigationMode.Elastic, m_VRManager.NavigationMethod == VRManagerScript.ENavigationMode.Elastic);
        m_GrabWorldNavigationRadioButton  = new vrWidgetRadioButton("GrabWorldNavigationRadioButton", m_NavigationOptions, "Grab The World", m_NavigationModeRadioCommand, (int)VRManagerScript.ENavigationMode.GrabWorld, m_VRManager.NavigationMethod == VRManagerScript.ENavigationMode.GrabWorld);
        //m_PointAndGoNavigationRadioButton = new vrWidgetRadioButton("PointAndGoNavigationRadioButton", m_NavigationOptions, "Point And Go", m_NavigationModeRadioCommand, (int)VRManagerScript.ENavigationMode.PointAndGo, m_VRManager.NavigationMethod == VRManagerScript.ENavigationMode.PointAndGo);
        m_NoNavigationRadioButton         = new vrWidgetRadioButton("NoNavigationRadioButton", m_NavigationOptions, "No Navigation", m_NavigationModeRadioCommand, (int)VRManagerScript.ENavigationMode.None, m_VRManager.NavigationMethod == VRManagerScript.ENavigationMode.None);
        _RefreshNavigationMode();

        m_NavigationSeparator = new vrWidgetSeparator("NavigationSeparator", m_NavigationOptions);
        m_FlyCheckbox         = new vrWidgetToggleButton("FlyCheckbox", m_NavigationOptions, "Fly", m_FlyCheckboxCommand, m_VRManager.Fly);
        m_CollisionsCheckbox  = new vrWidgetToggleButton("CollisionsCheckbox", m_NavigationOptions, "Navigation Collisions", m_CollisionsCheckboxCommand, m_VRManager.NavigationCollisions);

        // Manipulation
        m_ManipulationOptions = new vrWidgetMenu("ManipulationOptions", m_Menu, "Manipulation");

        m_RayManipulationRadioButton   = new vrWidgetRadioButton("RayManipulationRadioButton", m_ManipulationOptions, "Ray", m_ManipulationModeRadioCommand, (int)VRManagerScript.EManipulationMode.Ray, m_VRManager.ManipulationMethod == VRManagerScript.EManipulationMode.Ray);
        m_HomerManipulationRadioButton = new vrWidgetRadioButton("HomerManipulationRadioButton", m_ManipulationOptions, "Homer", m_ManipulationModeRadioCommand, (int)VRManagerScript.EManipulationMode.Homer, m_VRManager.ManipulationMethod == VRManagerScript.EManipulationMode.Homer);
        m_NoManipulationRadioButton    = new vrWidgetRadioButton("NoManipulationRadioButton", m_ManipulationOptions, "No Manipulation", m_ManipulationModeRadioCommand, (int)VRManagerScript.EManipulationMode.None, m_VRManager.ManipulationMethod == VRManagerScript.EManipulationMode.None);
        _RefreshManipulationMode();

        m_ManipulationSeparator = new vrWidgetSeparator("ManipulationSeparator", m_ManipulationOptions);
        m_ReturnObjectsCheckbox      = new vrWidgetToggleButton("ReturnObjectsCheckbox", m_ManipulationOptions, "Return Objects", m_ReturnObjectsCheckboxCommand, m_VRManager.ManipulationReturnObjects);

        // Virtual Hand
        m_VirtualHandOptions = new vrWidgetMenu("VirtualHandOptions", m_Menu, "Virtual Hand");

        m_DirectHandRadioButton = new vrWidgetRadioButton("DirectHandRadioButton", m_VirtualHandOptions, "Direct", m_VirtualHandModeRadioCommand, (int)VRManagerScript.EVirtualHand.Direct, m_VRManager.VirtualHandMapping == VRManagerScript.EVirtualHand.Direct);
        m_GogoHandRadioButton   = new vrWidgetRadioButton("GogoHandRadioButton", m_VirtualHandOptions, "Gogo", m_VirtualHandModeRadioCommand, (int)VRManagerScript.EVirtualHand.Gogo, m_VRManager.VirtualHandMapping == VRManagerScript.EVirtualHand.Gogo);
        _RefreshVirtualHand();

        // General
        m_GeneralSeparator = new vrWidgetSeparator("GeneralSeparator", m_Menu);
        m_GeneralOptions   = new vrWidgetMenu("GeneralOptions", m_Menu, "General Options");

        m_FramerateCheckbox    = new vrWidgetToggleButton("FramerateCheckbox", m_GeneralOptions, "Show Frame Rate", m_FramerateCheckboxCommand, m_VRManager.ShowFPS);
        m_ProxiWarningCheckbox = new vrWidgetToggleButton("ProxiWarningCheckbox", m_GeneralOptions, "Show Proximity Warning", m_ProxiWarningCheckboxCommand, m_VRManager.ShowScreenProximityWarnings);

        // Reset and Exit
        m_ResetButtonMenu    = new vrWidgetMenu("ResetButtonMenu", m_Menu, "Reset Simulation");
        m_ResetCurrentButton = new vrWidgetButton("ResetCurrentButton", m_ResetButtonMenu, "Reload current level", m_ResetCurrentButtonCommand);
        m_ResetZeroButton    = new vrWidgetButton("ResetZeroButton", m_ResetButtonMenu, "Reload level zero", m_ResetZeroButtonCommand);

        m_ExitButtonMenu = new vrWidgetMenu("ExitButtonMenu", m_Menu, "Exit Simulation");
        m_ExitButton     = new vrWidgetButton("ExitButton", m_ExitButtonMenu, "Yes, Exit Simulation", m_ExitButtonCommand);
    }

    private void _RefreshNavigationMode()
    {
        switch (m_VRManager.NavigationMethod)
        {
            case VRManagerScript.ENavigationMode.None:
                {
                    m_NavigationOptions.SetLabel("Navigation (No Navigation)");
                    break;
                }
            case VRManagerScript.ENavigationMode.Joystick:
                {
                    m_NavigationOptions.SetLabel("Navigation (Joystick)");
                    break;
                }
            case VRManagerScript.ENavigationMode.Elastic:
                {
                    m_NavigationOptions.SetLabel("Navigation (Elastic)");
                    break;
                }
            case VRManagerScript.ENavigationMode.GrabWorld:
                {
                    m_NavigationOptions.SetLabel("Navigation (Grab The World)");
                    break;
                }
            /*case VRManagerScript.ENavigationMode.PointAndGo:
                {
                    m_NavigationOptions.SetLabel("Navigation (Point And Go)");
                    break;
                }*/
        }
    }

    private void _RefreshManipulationMode()
    {
        switch (m_VRManager.ManipulationMethod)
        {
            case VRManagerScript.EManipulationMode.None:
                {
                    m_ManipulationOptions.SetLabel("Manipulation (No Manipulation)");
                    break;
                }
            case VRManagerScript.EManipulationMode.Ray:
                {
                    m_ManipulationOptions.SetLabel("Manipulation (Ray)");
                    break;
                }
            case VRManagerScript.EManipulationMode.Homer:
                {
                    m_ManipulationOptions.SetLabel("Manipulation (Homer)");
                    break;
                }
        }
    }

    private void _RefreshVirtualHand()
    {
        switch (m_VRManager.VirtualHandMapping)
        {
            case VRManagerScript.EVirtualHand.Direct:
                {
                    m_VirtualHandOptions.SetLabel("Virtual Hand (Direct)");
                    break;
                }
            case VRManagerScript.EVirtualHand.Gogo:
                {
                    m_VirtualHandOptions.SetLabel("Virtual Hand (Gogo)");
                    break;
                }
        }
    }
}
