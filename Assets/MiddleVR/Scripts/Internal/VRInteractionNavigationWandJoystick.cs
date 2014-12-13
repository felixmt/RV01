/* VRInteractionNavigationWandJoystick
 * MiddleVR
 * (c) i'm in VR
 */

using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;
using System;


public class VRInteractionNavigationWandJoystick : MonoBehaviour {
    public string Name = "InteractionNavigationWandJoystick";

    public string DirectionReferenceNode = "HandNode";
    public string TurnAroundNode = "HeadNode";

    vrNode3D m_DirectionReferenceNode = null;
    vrNode3D m_TurnAroundNode = null;
    
    public float TranslationSpeed = 1.0f;
    public float RotationSpeed = 45.0f;

    public bool Fly = false;

    vrInteractionNavigationWandJoystick m_it = null;

    void Start ()
    {
        m_it = new vrInteractionNavigationWandJoystick(Name);

        MiddleVR.VRInteractionMgr.AddInteraction(m_it);
        MiddleVR.VRInteractionMgr.Activate(m_it);

        m_DirectionReferenceNode = MiddleVR.VRDisplayMgr.GetNode(DirectionReferenceNode);
        m_TurnAroundNode = MiddleVR.VRDisplayMgr.GetNode(TurnAroundNode);

        if ( m_DirectionReferenceNode!= null && m_TurnAroundNode != null )
        {
            m_it.SetDirectionReferenceNode(m_DirectionReferenceNode);
            m_it.SetTurnAroundNode(m_TurnAroundNode);
            m_it.SetTranslationSpeed(TranslationSpeed);
            m_it.SetRotationSpeed(RotationSpeed);
            m_it.SetFly(Fly);
        }
        else
        {
            MiddleVR.VRLog( 2, "[X] VRInteractionNavigationWandJoystick: One or several nodes are missing." );
        }
    }

    void OnEnable()
    {
        MiddleVR.VRLog( 3, "[ ] VRInteractionNavigationWandJoystick: enabled" );
        if( m_it != null )
        {
            MiddleVR.VRInteractionMgr.Activate( m_it );
        }
    }

    void OnDisable()
    {
        MiddleVR.VRLog( 3, "[ ] VRInteractionNavigationWandJoystick: disabled" );
        if( m_it != null && MiddleVR.VRInteractionMgr != null )
        {
            MiddleVR.VRInteractionMgr.Deactivate( m_it );
        }
    }
}