using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;
using System;

public class VRInteractionVirtualHandGogo : MonoBehaviour {

    public string Name               = "InteractionVirtualHandGogo";
    public string HandNode           = "HandNode";
    public string HeadNode           = "HeadNode";
    public float  GogoStartDistance  = 0.5f;
    public float  RealDistanceMax    = 0.7f;
    public float  VirtualDistanceMax = 10.0f;

    vrInteractionVirtualHandGogo m_it = null;


    // Use this for initialization
    void Start () {

        m_it = new vrInteractionVirtualHandGogo(Name);

        MiddleVR.VRInteractionMgr.AddInteraction(m_it);
        MiddleVR.VRInteractionMgr.Activate(m_it);

        vrNode3D handNode = MiddleVR.VRDisplayMgr.GetNode( HandNode );
        vrNode3D headNode = MiddleVR.VRDisplayMgr.GetNode( HeadNode );

        if ( handNode!= null && headNode != null )
        {
            m_it.SetHandNode(handNode);
            m_it.SetHeadNode(headNode);
            m_it.SetGogoStartDistance(GogoStartDistance);
            m_it.SetRealDistanceMax(RealDistanceMax);
            m_it.SetVirtualDistanceMax(VirtualDistanceMax);
        }
        else
        {
            MiddleVR.VRLog( 2, "[X] VRInteractionVirtualHandGogo: One or several nodes are missing." );
        }
    }

    void Update () {
        // Nothing to do for this interaction, everything is done in the kernel
    }

    void OnEnable()
    {
        MiddleVR.VRLog( 3, "[ ] VRInteractionVirtualHandGogo: enabled" );
        if( m_it != null )
        {
            MiddleVR.VRInteractionMgr.Activate( m_it );
        }
    }

    void OnDisable()
    {
        MiddleVR.VRLog( 3, "[ ] VRInteractionVirtualHandGogo: disabled" );

        if( m_it != null && MiddleVR.VRInteractionMgr != null )
        {
            MiddleVR.VRInteractionMgr.Deactivate( m_it );
        }
    }
}
