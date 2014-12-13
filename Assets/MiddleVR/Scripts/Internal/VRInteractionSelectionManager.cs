/* VRInteractionSelectionManager
 * MiddleVR
 * (c) i'm in VR
 */

using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;

public class VRInteractionSelectionManager : MonoBehaviour {

    public enum EAction
    {
        eNoAction,
        eManipulation,
        eVRAction,
        eGUI
    }

    public float RayLength = 2;
    public GameObject ClassicRay;
    public GameObject ActionRay;
    public Color NeutralColor   = Color.white;
    public Color HighlightColor = new Color(23.0f * 255.0f, 1.0f, 0.0f);
    public bool RepeatAction = false;

    private static VRInteractionSelectionManager m_Instance;
    private VRManagerScript m_VRManager = null;
    private vrButtons m_Buttons = null;
    private static byte ALPHA_LIMIT = 50;
    private GameObject m_SelectedObject = null;
    private float m_SelectedDistance = 0.0f;
    private Vector2 m_SelectedTexCoord = new Vector2(0, 0);
    private EAction m_PossibleAction = EAction.eNoAction;
    private bool m_Active = true;
    private bool m_ShowRay = true;


    public static VRInteractionSelectionManager Instance
    {
        get
        {
            return m_Instance;
        }
    }

    private void Awake()
    {
        m_Instance = this;
    }

    private void Start ()
    {
        // Retrieve the VRManager
        VRManagerScript[] foundVRManager = FindObjectsOfType(typeof(VRManagerScript)) as VRManagerScript[];
        if (foundVRManager.Length != 0)
        {
            m_VRManager = foundVRManager[0];

            if (m_VRManager.ShowWand)
            {
                ShowRay();
            }
            else
            {
                HideRay();
            }
        }
        else
        {
            MiddleVRTools.Log("[X] VRInteractionSelectionManager: impossible to retrieve the VRManager.");
            return;
        }
    }

    private void Update ()
    {
        if (!m_Active)
        {
            return;
        }

        if (m_Buttons == null)
        {
            m_Buttons = MiddleVR.VRDeviceMgr.GetWandButtons();
        }

        _RaySelection();
        _RefreshyRayFeedback();

        // Send action if needed
        if (m_Buttons != null && m_SelectedObject != null )
        {
            uint MainButton = MiddleVR.VRDeviceMgr.GetWandButton0();
            if (m_PossibleAction == EAction.eVRAction || m_PossibleAction == EAction.eManipulation)
            {
                if (((!RepeatAction && m_Buttons.IsToggled(MainButton)) || (RepeatAction && m_Buttons.IsPressed(MainButton))))
                {
                    m_SelectedObject.SendMessage("VRAction", SendMessageOptions.DontRequireReceiver);
                }
            }
            else if( m_PossibleAction == EAction.eGUI)
            {
                VRWebView webViewUnity = m_SelectedObject.GetComponent<VRWebView>();
                if( m_Buttons.IsToggled(MainButton, true) )
                {
                    webViewUnity.SetVirtualMousePosition(m_SelectedTexCoord);
                    webViewUnity.SetVirtualMouseButtonPressed();
                }
                else if(  m_Buttons.IsToggled(MainButton, false) )
                {
                    webViewUnity.SetVirtualMouseButtonReleased();
                    webViewUnity.SetVirtualMousePosition(m_SelectedTexCoord);
                }
                else
                {
                    webViewUnity.SetVirtualMousePosition(m_SelectedTexCoord);
                }
            }
        }
    }

    private void _RaySelection()
    {
        // Ray picking
        RaycastHit[] hits;
        Vector3 dir = transform.localToWorldMatrix * Vector3.forward;

        hits = Physics.RaycastAll(transform.position, dir, RayLength);

        int i = 0;
        GameObject selectedObject = null;
        float distance = Mathf.Infinity;
        Vector2 texCoord = new Vector2(0, 0);

        while (i < hits.Length)
        {
            RaycastHit hit = hits[i];

            if (hit.distance < distance && hit.collider.name != "VRWand")
            {
                // Pass through empty/transparent GUI pixels
                VRWebView webView = hit.collider.GetComponent<VRWebView>();
                if (webView != null)
                {
                    if (!webView.GetComponent<Renderer>().enabled || _IsPixelEmpty(webView, hit.textureCoord))
                    {
                        i++;
                        continue;
                    }
                }

                distance = hit.distance;
                selectedObject = hit.collider.gameObject;
                texCoord = hit.textureCoord;
            }

            i++;
        }

        // If something found, select
        m_SelectedObject = selectedObject;
        if (m_SelectedObject == null)
        {
            m_PossibleAction = EAction.eNoAction;
            return;
        }

        m_SelectedDistance = distance;
        m_SelectedTexCoord = texCoord;

        VRActor actor = m_SelectedObject.GetComponent<VRActor>();

        if (m_SelectedObject.GetComponent<VRWebView>() != null)
        {
            // GUI
            m_PossibleAction = EAction.eGUI;
        }
        else if (actor != null)
        {
            if (actor.Grabable == true)
            {
                // Grab
                m_PossibleAction = EAction.eManipulation;
            }
            else
            {
                // VR action
                m_PossibleAction = EAction.eVRAction;
            }
        }
        else
        {
            // No action
            m_PossibleAction = EAction.eNoAction;
        }
    }


    private void _RefreshyRayFeedback()
    {
        if (ClassicRay == null || ActionRay == null )
        {
            return;
        }

        if (!m_ShowRay)
        {
            // Hide rays
            ClassicRay.SetActive(false);
            ActionRay.SetActive(false);
            return;
        }

        switch (m_PossibleAction)
        {
            case EAction.eNoAction:
                {
                    // Disable GUI ray
                    ActionRay.SetActive(false);

                    // Display classic ray
                    ClassicRay.SetActive(true);

                    // Change ray color
                    ClassicRay.transform.GetChild(0).GetComponent<Renderer>().material.color = NeutralColor;

                    // Change ray length
                    ClassicRay.transform.localScale = new Vector3(1.0f, 1.0f, RayLength);

                    break;
                }
            case EAction.eManipulation:
                {
                    // Hide classic ray
                    ClassicRay.SetActive(false);

                    // Disable GUI ray
                    ActionRay.SetActive(true);

                    // Change ray length
                    ActionRay.transform.localScale = new Vector3(1.0f, 1.0f, m_SelectedDistance);

                    // Change ray color
                    ActionRay.transform.GetChild(0).GetComponent<Renderer>().material.color = HighlightColor;

                    break;
                }
            case EAction.eVRAction:
                {
                    // Hide classic ray
                    ClassicRay.SetActive(false);

                    // Disable GUI ray
                    ActionRay.SetActive(true);

                    // Change ray length
                    ActionRay.transform.localScale = new Vector3(1.0f, 1.0f, m_SelectedDistance);

                    // Change ray color
                    ActionRay.transform.GetChild(0).GetComponent<Renderer>().material.color = HighlightColor;

                    break;
                }
            case EAction.eGUI:
                {
                    // Hide classic ray
                    ClassicRay.SetActive(false);

                    // Disable GUI ray
                    ActionRay.SetActive(true);

                    // Change ray length
                    ActionRay.transform.localScale = new Vector3(1.0f, 1.0f, m_SelectedDistance);

                    // Change ray color
                    ActionRay.transform.GetChild(0).GetComponent<Renderer>().material.color = HighlightColor;

                    break;
                }
        }
    }

    private bool _IsPixelEmpty(VRWebView iWebView, Vector2 iTextureCoord)
    {
        byte alpha = iWebView.image.GetAlphaAtPoint((int)(iTextureCoord.x * iWebView.m_Width), (int)(iTextureCoord.y * iWebView.m_Height));
        return alpha < ALPHA_LIMIT;
    }


    public GameObject GetCurrentSelectedObject()
    {
        return m_SelectedObject;
    }

    public EAction GetCurrentPossibleAction()
    {
        return m_PossibleAction;
    }

    public void ShowRay()
    {
        m_ShowRay = true;
        _RefreshyRayFeedback();
    }

    public void HideRay()
    {
        m_ShowRay = false;
        _RefreshyRayFeedback();
    }

    public void PauseSelection()
    {
        m_Active = false;
        m_SelectedObject = null;
        m_PossibleAction = EAction.eNoAction;
        _RefreshyRayFeedback();
    }

    public void UnpauseSelection()
    {
        m_Active = true;
    }
}
