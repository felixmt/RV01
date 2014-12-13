/* VRInteractionManipulationRay
 * MiddleVR
 * (c) i'm in VR
 *
 * Note: Made to be attached to the Wand
 */

using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;
using System;

public class VRInteractionManipulationRay : MonoBehaviour {

    public string Name               = "InteractionManipulationRay";
    public string HandNode           = "HandNode";
    public uint   WandGrabButton     = 0;

    private vrInteractionManipulationRay m_it     = null;

    private vrNode3D          m_HandNode          = null;

    private GameObject m_CurrentSelectedObject    = null;
    private GameObject m_CurrentManipulatedObject = null;

    private Vector3    m_ObjectInitialLocalPosition;
    private Quaternion m_ObjectInitialLocalRotation;


    void Start () {

        m_it = new vrInteractionManipulationRay(Name);

        MiddleVR.VRInteractionMgr.AddInteraction(m_it);
        MiddleVR.VRInteractionMgr.Activate(m_it);

        m_HandNode = MiddleVR.VRDisplayMgr.GetNode( HandNode );

        if ( m_HandNode != null )
        {
            m_it.SetGrabWandButton( WandGrabButton );
            m_it.SetManipulatorNode(m_HandNode);
        }
        else
        {
            MiddleVR.VRLog( 2, "[X] VRInteractionManipulationRay: One or several nodes are missing." );
        }
    }

    void Update ()
    {
        // Retrieve selection result
        m_CurrentSelectedObject = VRInteractionSelectionManager.Instance.GetCurrentSelectedObject();

        // Manipulation
        if( m_it.HasManipulationStarted() )
        {
            // Try to grab
            if (m_CurrentSelectedObject != null && VRInteractionSelectionManager.Instance.GetCurrentPossibleAction() == VRInteractionSelectionManager.EAction.eManipulation)
            {
                Grab(m_CurrentSelectedObject);
            }
        }
        else if( m_it.IsManipulationRunning() && m_CurrentManipulatedObject != null )
        {
            // Nothing to do here
        }
        else if( m_it.IsManipulationStopped() )
        {
            Ungrab();
        }
    }

    void OnEnable()
    {
        MiddleVR.VRLog( 3, "[ ] VRInteractionManipulationRay: enabled" );
        if( m_it != null )
        {
            MiddleVR.VRInteractionMgr.Activate( m_it );
        }
    }

    void OnDisable()
    {
        MiddleVR.VRLog( 3, "[ ] VRInteractionManipulationRay: disabled" );

        if( m_it != null && MiddleVR.VRInteractionMgr != null )
        {
            MiddleVR.VRInteractionMgr.Deactivate( m_it );
        }
    }

    private void Grab( GameObject iGrabbedObject )
    {
        if( iGrabbedObject == null )
        {
            return;
        }

        //MiddleVRTools.Log("[ ] VRInteractionManipulationRay: Grab" );

        // Initialize manipulated node
        m_CurrentManipulatedObject = iGrabbedObject;
        VRActor vrActorScript = m_CurrentManipulatedObject.GetComponent<VRActor>();
        vrNode3D middleVRNode = vrActorScript.GetMiddleVRNode();
        m_it.SetManipulatedNode(middleVRNode);

        // Save initial position
        m_ObjectInitialLocalPosition = m_CurrentManipulatedObject.transform.localPosition;
        m_ObjectInitialLocalRotation = m_CurrentManipulatedObject.transform.localRotation;

        // Hide Wand
        //m_VRMgr.ShowWandGeometry(false);
    }

    private void Ungrab()
    {
        if( m_CurrentManipulatedObject == null )
        {
            return;
        }

        // Give to return objects script
        VRInteractionManipulationReturnObjects returningObjectScript = this.GetComponent<VRInteractionManipulationReturnObjects>();
        if( returningObjectScript != null )
        {
            if (returningObjectScript.enabled)
            {
                returningObjectScript.AddReturningObject(m_CurrentManipulatedObject, m_ObjectInitialLocalPosition,
                                                         m_ObjectInitialLocalRotation, false);
            }
        }

        //MiddleVRTools.Log("[ ] VRInteractionManipulationRay: Ungrab" );

        // Reset
        m_it.SetManipulatedNode(null);
        m_CurrentManipulatedObject = null;

        // Show Wand
        //m_VRMgr.ShowWandGeometry(true);
    }
}
