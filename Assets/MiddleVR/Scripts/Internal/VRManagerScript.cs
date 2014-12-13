/* VRManagerScript
 * MiddleVR
 * (c) i'm in VR
 */

using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;
using System;

public class VRManagerScript : MonoBehaviour
{
    public enum ENavigationMode{
        None,
        Joystick,
        Elastic,
        GrabWorld/*,
        PointAndGo*/
    }

    public enum EVirtualHand{
        Direct,
        Gogo
    }

    public enum EManipulationMode{
        None,
        Ray,
        Homer
    }

    // Public readable parameters
    [HideInInspector]
    public float WandAxisHorizontal = 0.0f;

    [HideInInspector]
    public float WandAxisVertical = 0.0f;

    [HideInInspector]
    public bool WandButton0 = false;

    [HideInInspector]
    public bool WandButton1 = false;

    [HideInInspector]
    public bool WandButton2 = false;

    [HideInInspector]
    public bool WandButton3 = false;

    [HideInInspector]
    public bool WandButton4 = false;

    [HideInInspector]
    public bool WandButton5 = false;

    [HideInInspector]
    public double DeltaTime = 0.0f;

    // Exposed parameters:
    public string ConfigFile = "c:\\config.vrx";
    public GameObject VRSystemCenterNode = null;
    public GameObject TemplateCamera     = null;

    public bool ShowWand = true;

    [SerializeField]
    private bool m_UseDefaultMenu = false;
    public bool UseDefaultMenu
    {
        get
        {
            return m_UseDefaultMenu;
        }
        set
        {
            m_UseDefaultMenu = value;
            _RefreshUseDefaultMenu();
        }
    }

    [SerializeField]
    private ENavigationMode m_NavigationMethod = ENavigationMode.Joystick;
    public ENavigationMode NavigationMethod
    {
        get
        {
            return m_NavigationMethod;
        }
        set
        {
            m_NavigationMethod = value;
            _RefreshNavigationMode();
        }
    }

    [SerializeField]
    private EManipulationMode m_ManipulationMethod = EManipulationMode.Ray;
    public EManipulationMode ManipulationMethod
    {
        get
        {
            return m_ManipulationMethod;
        }
        set
        {
            m_ManipulationMethod = value;
            _RefreshManipulationMode();
        }
    }

    [SerializeField]
    private EVirtualHand m_VirtualHandMapping = EVirtualHand.Direct;
    public EVirtualHand VirtualHandMapping
    {
        get
        {
            return m_VirtualHandMapping;
        }
        set
        {
            m_VirtualHandMapping = value;
            _RefreshVirtualHand();
        }
    }

    [SerializeField]
    private bool m_ShowScreenProximityWarnings = false;
    public bool ShowScreenProximityWarnings
    {
        get
        {
            return m_ShowScreenProximityWarnings;
        }
        set
        {
            m_ShowScreenProximityWarnings = value;
            _RefreshScreenProximityWarning();
        }
    }

    [SerializeField]
    private bool m_Fly = false;
    public bool Fly
    {
        get
        {
            return m_Fly;
        }
        set
        {
            m_Fly = value;
            _RefreshFlyMode();
        }
    }

    [SerializeField]
    private bool m_NavigationCollisions = false;
    public bool NavigationCollisions
    {
        get
        {
            return m_NavigationCollisions;
        }
        set
        {
            m_NavigationCollisions = value;
            _RefreshCollisionMode();
        }
    }

    [SerializeField]
    private bool m_ManipulationReturnObjects = false;
    public bool ManipulationReturnObjects
    {
        get
        {
            return m_ManipulationReturnObjects;
        }
        set
        {
            m_ManipulationReturnObjects = value;
            _RefreshManipulationReturnObjects();
        }
    }

    [SerializeField]
    private bool m_ShowFPS = true;
    public bool ShowFPS
    {
        get
        {
            return m_ShowFPS;
        }
        set
        {
            m_ShowFPS = value;
            _RefreshShowFPS();
        }
    }

    public bool              DisableExistingCameras      = true;
    public bool              GrabExistingNodes           = false;
    public bool              DebugNodes                  = false;
    public bool              DebugScreens                = false;
    public bool              QuitOnEsc                   = true;
    public bool              DontChangeWindowGeometry    = false;
    public bool              SimpleCluster               = true;
    public bool              ForceQuality                = false;
    public int               ForceQualityIndex           = 3;

    // Private members
    private vrKernel         kernel     = null;
    private vrDisplayManager displayMgr = null;

    private bool m_isInit        = false;
    private bool m_isGeometrySet = false;
    private bool m_displayLog    = false;

    private int  m_AntiAliasingLevel = 0;

    private bool m_NeedDelayedRenderingReset = false;
    private int  m_RenderingResetDelay       = 1;

    private GUIText    m_GUI         = null;
    private GameObject m_Wand        = null;
    private GameObject m_DefaultMenu = null;

    private bool[] mouseButtons = new bool[3];

    private bool m_AllowRenderTargetAA = false;

    private uint m_FirstFrameAfterReset = 0;

    // Public methods

    public void Log(string text)
    {
        MiddleVRTools.Log(text);
    }

    public bool IsKeyPressed(uint iKey)
    {
        return MiddleVR.VRDeviceMgr != null && MiddleVR.VRDeviceMgr.IsKeyPressed(iKey);
    }

    public bool IsMouseButtonPressed(uint iButtonIndex)
    {
        return MiddleVR.VRDeviceMgr != null && MiddleVR.VRDeviceMgr.IsMouseButtonPressed(iButtonIndex);
    }

    public float GetMouseAxisValue(uint iAxisIndex)
    {
        if (MiddleVR.VRDeviceMgr != null)
        {
            return MiddleVR.VRDeviceMgr.GetMouseAxisValue(iAxisIndex);
        }

        return 0.0f;
    }


    // Private methods

    void InitializeVR()
    {
        mouseButtons[0] = mouseButtons[1] = mouseButtons[2] = false;

        if (m_displayLog)
        {
            GameObject gui = new GameObject();
            m_GUI = gui.AddComponent<GUIText>() as GUIText;
            gui.transform.localPosition = new UnityEngine.Vector3(0.5f, 0.0f, 0.0f);
            m_GUI.pixelOffset = new UnityEngine.Vector2(15, 0);
            m_GUI.anchor = TextAnchor.LowerCenter;
        }

        MiddleVRTools.IsEditor = Application.isEditor;

        if( MiddleVR.VRKernel != null )
        {
            MiddleVRTools.Log(3, "[ ] VRKernel already alive, reset Unity Manager.");
            MiddleVRTools.VRReset();
            m_isInit = true;
            // Not needed because this is the first execution of this script instance
            // m_isGeometrySet = false;
            m_FirstFrameAfterReset = MiddleVR.VRKernel.GetFrame();
        }
        else
        {
            m_isInit = MiddleVRTools.VRInitialize(ConfigFile);
        }

        // Get AA from vrx configuration file
        m_AntiAliasingLevel = (int)MiddleVR.VRDisplayMgr.GetAntiAliasing();

        DumpOptions();

        kernel = MiddleVR.VRKernel;
        displayMgr = MiddleVR.VRDisplayMgr;

        if (!m_isInit)
        {
            GameObject gui = new GameObject();
            m_GUI = gui.AddComponent<GUIText>() as GUIText;
            gui.transform.localPosition = new UnityEngine.Vector3(0.2f, 0.0f, 0.0f);
            m_GUI.pixelOffset = new UnityEngine.Vector2(0, 0);
            m_GUI.anchor = TextAnchor.LowerLeft;

            string txt = kernel.GetLogString(true);
            print(txt);
            m_GUI.text = txt;

            return;
        }

        if (SimpleCluster)
        {
            SetupSimpleCluster();
        }

        if (DisableExistingCameras)
        {
            Camera[] cameras = GameObject.FindObjectsOfType(typeof(Camera)) as Camera[];

            foreach (Camera cam in cameras)
            {
                if (cam.targetTexture == null)
                {
                    cam.enabled = false;
                }
            }
        }

        MiddleVRNodesManager.CreateNodes(VRSystemCenterNode, DebugNodes, DebugScreens, GrabExistingNodes, TemplateCamera);
        MiddleVRTools.CreateViewportsAndCameras(DontChangeWindowGeometry, m_AllowRenderTargetAA);

        //AttachCameraCB();

        MiddleVRTools.Log(4, "[<] End of VR initialization script");
    }

    void AttachCameraCB()
    {
        Camera[] cameras = GameObject.FindObjectsOfType(typeof(Camera)) as Camera[];

        foreach (Camera cam in cameras)
        {
            if (cam.targetTexture != null)
            {
                cam.gameObject.AddComponent<VRCameraCB>();
            }
        }
    }

    void Start () {
        MiddleVRTools.Log(4, "[>] VR Manager Start.");

#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
        m_AllowRenderTargetAA = true;
#endif

        InitializeVR();

        // Reset Manager's position so text display is correct.
        transform.position = new UnityEngine.Vector3(0, 0, 0);
        transform.rotation = new Quaternion();
        transform.localScale = new UnityEngine.Vector3(1, 1, 1);

        m_Wand = GameObject.Find("VRWand");

        m_DefaultMenu = GameObject.Find("VRDefaultMenu");

        if(ShowWand) {
            ShowWandGeometry(true);
        } else {
            ShowWandGeometry(false);
        }

        // Initialize navigation interaction technique
        _RefreshNavigationMode();

        // Initialize navigation interaction technique
        _RefreshManipulationMode();

        // Initialize virtual hand
        _RefreshVirtualHand();

        _RefreshScreenProximityWarning();

        _RefreshShowFPS();

        _RefreshCollisionMode();

        _RefreshManipulationReturnObjects();

        _RefreshUseDefaultMenu();

        if (ForceQuality)
        {
            QualitySettings.SetQualityLevel(ForceQualityIndex);
        }

        // Manage VSync after the quality settings
        MiddleVRTools.ManageVSync();

        // Set AA from vrx configuration file
        QualitySettings.antiAliasing = m_AntiAliasingLevel;

        // Check if MiddleVR Reset is needed
        if (!Application.isEditor && (ForceQuality || QualitySettings.antiAliasing > 1))
        {
            bool useOpenGLQuadbuffer = MiddleVR.VRDisplayMgr.GetActiveViewport(0).GetStereo() && (MiddleVR.VRDisplayMgr.GetActiveViewport(0).GetStereoMode() == 0); //VRStereoMode_QuadBuffer = 0
            if (useOpenGLQuadbuffer || MiddleVR.VRClusterMgr.GetForceOpenGLConversion())
            {
                m_NeedDelayedRenderingReset = true;
                m_RenderingResetDelay = 1;
            }
        }

        MiddleVRTools.Log(4, "[<] End of VR Manager Start.");
    }

    private void _RefreshNavigationMode()
    {
        VRInteractionNavigationWandJoystick navigationWandJoystick = m_Wand.GetComponent<VRInteractionNavigationWandJoystick>();
        VRInteractionNavigationElastic      navigationElastic      = m_Wand.GetComponent<VRInteractionNavigationElastic>();
        VRInteractionNavigationGrabWorld    navigationGrabWorld    = m_Wand.GetComponent<VRInteractionNavigationGrabWorld>();
        //VRInteractionNavigationPointAndGo   navigationPointAndGo   = m_Wand.GetComponent<VRInteractionNavigationPointAndGo>();
        if (navigationWandJoystick == null || navigationElastic == null || navigationGrabWorld == null /*|| navigationPointAndGo == null*/)
        {
            MiddleVRTools.Log(2, "[~] Some navigation scripts are missing on the Wand.");
            return;
        }

        switch (m_NavigationMethod)
        {
            case ENavigationMode.None:
                navigationWandJoystick.enabled = false;
                navigationElastic.enabled      = false;
                navigationGrabWorld.enabled    = false;
                //navigationPointAndGo.enabled   = false;
                break;

            case ENavigationMode.Joystick:
                navigationWandJoystick.enabled = true;
                navigationElastic.enabled      = false;
                navigationGrabWorld.enabled    = false;
                //navigationPointAndGo.enabled   = false;
                break;

            case ENavigationMode.Elastic:
                navigationWandJoystick.enabled = false;
                navigationElastic.enabled      = true;
                navigationGrabWorld.enabled    = false;
                //navigationPointAndGo.enabled   = false;
                break;

            case ENavigationMode.GrabWorld:
                navigationWandJoystick.enabled = false;
                navigationElastic.enabled      = false;
                navigationGrabWorld.enabled    = true;
                //navigationPointAndGo.enabled   = false;
                break;

            /*case ENavigationMode.PointAndGo:
                navigationWandJoystick.enabled = false;
                navigationElastic.enabled      = false;
                navigationGrabWorld.enabled    = false;
                navigationPointAndGo.enabled   = true;
                break;*/

            default:
                break;
        }
    }

    private void _RefreshManipulationMode()
    {
        VRInteractionManipulationRay   manipulationRay   = m_Wand.GetComponent<VRInteractionManipulationRay>();
        VRInteractionManipulationHomer manipulationHomer = m_Wand.GetComponent<VRInteractionManipulationHomer>();
        if (manipulationRay == null || manipulationHomer == null)
        {
            MiddleVRTools.Log(2, "[~] Some manipulation scripts are missing on the Wand.");
            return;
        }

        switch (m_ManipulationMethod)
        {
            case EManipulationMode.None:
                manipulationRay.enabled   = false;
                manipulationHomer.enabled = false;
                break;

            case EManipulationMode.Ray:
                manipulationRay.enabled   = true;
                manipulationHomer.enabled = false;
                break;

            case EManipulationMode.Homer:
                manipulationRay.enabled   = false;
                manipulationHomer.enabled = true;
                break;

            default:
                break;
        }
    }

    private void _RefreshVirtualHand()
    {
        VRInteractionVirtualHandGogo virtualHandGogo = m_Wand.GetComponent<VRInteractionVirtualHandGogo>();
        if (virtualHandGogo == null)
        {
            MiddleVRTools.Log(2, "[~] The virtual hand  Gogo script is missing on the Wand.");
            return;
        }

        switch (m_VirtualHandMapping)
        {
            case EVirtualHand.Direct:
                virtualHandGogo.enabled = false;
                break;

            case EVirtualHand.Gogo:
                virtualHandGogo.enabled = true;
                break;

            default:
                break;
        }
    }

    private void _RefreshScreenProximityWarning()
    {
        VRInteractionScreenProximityWarning proximityWarning = m_Wand.GetComponent<VRInteractionScreenProximityWarning>();

        if (proximityWarning != null)
        {
            proximityWarning.enabled = m_ShowScreenProximityWarnings;
        }
    }

    private void _RefreshShowFPS()
    {
		this.GetComponent<GUIText>().enabled = m_ShowFPS;
    }

    private void _RefreshUseDefaultMenu()
    {
        if (m_DefaultMenu != null)
        {
            m_DefaultMenu.SetActive( m_UseDefaultMenu );
        }
    }

    private void _RefreshFlyMode()
    {
        vrInteractionManager interMan = vrInteractionManager.GetInstance();
        uint interactionNb = interMan.GetInteractionsNb();
        for (uint i = 0; i < interactionNb; ++i)
        {
            vrProperty flyProp = interMan.GetInteractionByIndex(i).GetProperty("Fly");
            if (flyProp != null)
            {
                flyProp.SetBool(m_Fly);
            }
        }
    }

    private void _RefreshCollisionMode()
    {
        VRNavigationCollision navigationCollision = m_Wand.GetComponent<VRNavigationCollision>();

        if (navigationCollision != null)
        {
            navigationCollision.enabled = m_NavigationCollisions;
        }
    }

    private void _RefreshManipulationReturnObjects()
    {
        VRInteractionManipulationReturnObjects returnObjects = m_Wand.GetComponent<VRInteractionManipulationReturnObjects>();

        if (returnObjects != null)
        {
            returnObjects.enabled = m_ManipulationReturnObjects;
        }
    }

    public void ShowWandGeometry(bool iState)
    {
        if( m_Wand != null )
        {
            foreach (Transform child in m_Wand.transform)
            {
                if (child.GetComponent<Renderer>() != null)
                {
                    child.GetComponent<Renderer>().enabled = iState;
                }
            }

            if (VRInteractionSelectionManager.Instance != null)
            {
                if (iState)
                {
                    VRInteractionSelectionManager.Instance.ShowRay();
                }
                else
                {
                    VRInteractionSelectionManager.Instance.HideRay();
                }
            }
        }
    }

    void UpdateInput()
    {
        vrDeviceManager dmgr = MiddleVR.VRDeviceMgr;

        if (dmgr != null)
        {
            vrButtons wandButtons = dmgr.GetWandButtons();

            if (wandButtons != null)
            {
                uint buttonNb = wandButtons.GetButtonsNb();
                if( buttonNb > 0 )
                {
                    WandButton0 = wandButtons.IsPressed(dmgr.GetWandButton0());
                }
                if( buttonNb > 1 )
                {
                    WandButton1 = wandButtons.IsPressed(dmgr.GetWandButton1());
                }
                if( buttonNb > 2 )
                {
                    WandButton2 = wandButtons.IsPressed(dmgr.GetWandButton2());
                }
                if( buttonNb > 3 )
                {
                    WandButton3 = wandButtons.IsPressed(dmgr.GetWandButton3());
                }
                if( buttonNb > 4 )
                {
                    WandButton4 = wandButtons.IsPressed(dmgr.GetWandButton4());
                }
                if( buttonNb > 5 )
                {
                    WandButton5 = wandButtons.IsPressed(dmgr.GetWandButton5());
                }
            }

            WandAxisHorizontal = dmgr.GetWandHorizontalAxisValue();
            WandAxisVertical   = dmgr.GetWandVerticalAxisValue();
        }
    }

    // Update is called once per frame
    void Update ()
    {
        //MiddleVRTools.Log("VRManagerUpdate");

        MiddleVRNodesManager.UpdateNodesUnityToMiddleVR();

        if (m_isInit)
        {
            MiddleVRTools.Log(4, "[>] Unity Update - Start");

            if (kernel.GetFrame() >= m_FirstFrameAfterReset+1 && !m_isGeometrySet && !Application.isEditor)
            {
                if (!DontChangeWindowGeometry)
                {
                    displayMgr.SetUnityWindowGeometry();
                }
                m_isGeometrySet = true;
            }

            if (kernel.GetFrame() == 0)
            {
                // Set the random seed in kernel for dispatching only during start-up.
                // With clustering, a client will be set by a call to kernel.Update().
                if (!MiddleVR.VRClusterMgr.IsCluster() ||
                    (MiddleVR.VRClusterMgr.IsCluster() && MiddleVR.VRClusterMgr.IsServer()))
                {
                    // The cast is safe because the seed is always positive.
                    uint seed = (uint) UnityEngine.Random.seed;
                    kernel._SetRandomSeed(seed);
                }
            }

            kernel.Update();

            if (kernel.GetFrame() == 0)
            {
                // Set the random seed in a client only during start-up.
                if (MiddleVR.VRClusterMgr.IsCluster() && MiddleVR.VRClusterMgr.IsClient())
                {
                    // The cast is safe because the seed comes from
                    // a previous value of Unity.
                    int seed = (int) kernel.GetRandomSeed();
                    UnityEngine.Random.seed = seed;
                }
            }

            UpdateInput();

            if (ShowFPS)
            {
				this.GetComponent<GUIText>().text = kernel.GetFPS().ToString("f2");
            }

            MiddleVRNodesManager.UpdateNodes();

            if (m_displayLog)
            {
                string txt = kernel.GetLogString(true);
                print(txt);
                m_GUI.text = txt;
            }

            vrKeyboard keyb = MiddleVR.VRDeviceMgr.GetKeyboard();

            if (keyb != null && keyb.IsKeyToggled(MiddleVR.VRK_D) && (keyb.IsKeyPressed(MiddleVR.VRK_LSHIFT) || keyb.IsKeyPressed(MiddleVR.VRK_RSHIFT)))
            {
                ShowFPS = !ShowFPS;
            }

            if (keyb != null && (keyb.IsKeyToggled(MiddleVR.VRK_W) || keyb.IsKeyToggled(MiddleVR.VRK_Z)) && (keyb.IsKeyPressed(MiddleVR.VRK_LSHIFT) || keyb.IsKeyPressed(MiddleVR.VRK_RSHIFT)))
            {
                ShowWand = !ShowWand;
                ShowWandGeometry(ShowWand);
            }

            // Toggle Fly mode on interactions
            if (keyb != null && keyb.IsKeyToggled(MiddleVR.VRK_F) && (keyb.IsKeyPressed(MiddleVR.VRK_LSHIFT) || keyb.IsKeyPressed(MiddleVR.VRK_RSHIFT)))
            {
                Fly = !Fly;
            }

            // Navigation mode switch
            if(keyb != null && keyb.IsKeyToggled(MiddleVR.VRK_N) && (keyb.IsKeyPressed(MiddleVR.VRK_LSHIFT) || keyb.IsKeyPressed(MiddleVR.VRK_RSHIFT)))
            {
                NavigationMethod = _GetNextNavigationMode(NavigationMethod);
            }

            DeltaTime = kernel.GetDeltaTime();

            MiddleVRTools.Log(4, "[<] Unity Update - End");
        }
        else
        {
            //Debug.LogWarning("[ ] If you have an error mentionning 'DLLNotFoundException: MiddleVR_CSharp', please restart Unity. If this does not fix the problem, please make sure MiddleVR is in the PATH environment variable.");
        }

        // If QualityLevel changed, we have to reset the Unity Manager
        if( m_NeedDelayedRenderingReset )
        {
            if( m_RenderingResetDelay == 0 )
            {
                MiddleVRTools.Log(3,"[ ] Graphic quality forced, reset Unity Manager.");
                MiddleVRTools.VRReset();
                MiddleVRTools.CreateViewportsAndCameras(DontChangeWindowGeometry, m_AllowRenderTargetAA);
                m_isGeometrySet = false;
                m_NeedDelayedRenderingReset = false;
            }
            else
            {
                --m_RenderingResetDelay;
            }
        }
    }

    void AddClusterScripts(GameObject iObject)
    {
        MiddleVRTools.Log(2, "[ ] Adding cluster sharing scripts to " + iObject.name);
        if (iObject.GetComponent<VRShareTransform>() == null)
        {
            VRShareTransform script = iObject.AddComponent<VRShareTransform>();
            script.Start();
        }

        if (iObject.GetComponent<VRApplySharedTransform>() == null)
        {
            VRApplySharedTransform script = iObject.AddComponent<VRApplySharedTransform>();
            script.Start();
        }
    }

    void SetupSimpleCluster()
    {
        if (MiddleVR.VRClusterMgr.IsCluster())
        {
            // Rigid bodies
            Rigidbody[] bodies = FindObjectsOfType(typeof(Rigidbody)) as Rigidbody[];
            foreach (Rigidbody body in bodies)
            {
                if (!body.isKinematic)
                {
                    GameObject iObject = body.gameObject;
                    AddClusterScripts(iObject);
                }
            }

            // Character controller
            CharacterController[] ctrls = FindObjectsOfType(typeof(CharacterController)) as CharacterController[];
            foreach (CharacterController ctrl in ctrls)
            {
                GameObject iObject = ctrl.gameObject;
                AddClusterScripts(iObject);
            }
        }
    }

    void DumpOptions()
    {
        MiddleVRTools.Log(3, "[ ] Dumping VRManager's options:");
        MiddleVRTools.Log(3, "[ ] - Config File : " + ConfigFile);
        MiddleVRTools.Log(3, "[ ] - System Center Node : " + VRSystemCenterNode);
        MiddleVRTools.Log(3, "[ ] - Template Camera : " + TemplateCamera);
        MiddleVRTools.Log(3, "[ ] - Show Wand : " + ShowWand);
        MiddleVRTools.Log(3, "[ ] - Show FPS  : " + ShowFPS);
        MiddleVRTools.Log(3, "[ ] - Disable Existing Cameras : " + DisableExistingCameras);
        MiddleVRTools.Log(3, "[ ] - Grab Existing Nodes : " + GrabExistingNodes);
        MiddleVRTools.Log(3, "[ ] - Debug Nodes : " + DebugNodes);
        MiddleVRTools.Log(3, "[ ] - Debug Screens : " + DebugScreens);
        MiddleVRTools.Log(3, "[ ] - Quit On Esc : " + QuitOnEsc);
        MiddleVRTools.Log(3, "[ ] - Don't Change Window Geometry : " + DontChangeWindowGeometry);
        MiddleVRTools.Log(3, "[ ] - Simple Cluster : " + SimpleCluster);
        MiddleVRTools.Log(3, "[ ] - Force Quality : " + ForceQuality );
        MiddleVRTools.Log(3, "[ ] - Force QualityIndex : " + ForceQualityIndex );
        MiddleVRTools.Log(3, "[ ] - Anti-Aliasing Level : " + m_AntiAliasingLevel );
    }

    private ENavigationMode _GetNextNavigationMode(ENavigationMode iNavModeEnum)
    {
        switch (iNavModeEnum)
        {
            case ENavigationMode.None:
                return ENavigationMode.Joystick;

            case ENavigationMode.Joystick:
                return ENavigationMode.Elastic;

            case ENavigationMode.Elastic:
                return ENavigationMode.GrabWorld;

            /*case ENavigationMode.GrabWorld:
                return ENavigationMode.PointAndGo;

            case ENavigationMode.PointAndGo:
                return ENavigationMode.None;*/
        }

        return ENavigationMode.None;
    }

    public void QuitApplication()
    {
        if (Application.isEditor)
        {
            MiddleVRTools.Log("[ ] If we were in player mode, MiddleVR would exit.");
        }
        else
        {
            // If we're not in cluster, we quit when ESCAPE is pressed
            // If we're in cluster, only the master should quit
            //if (!cmgr.IsCluster() || (cmgr.IsCluster() && cmgr.IsServer()))
            {
                MiddleVRTools.Log("[ ] Unity says we're quitting.");
                MiddleVR.VRKernel.SetQuitting();
                Application.Quit();
            }
        }
    }

    void OnApplicationQuit()
    {
        MiddleVRTools.VRDestroy(Application.isEditor);
    }
}
