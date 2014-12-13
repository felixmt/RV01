/* VRActor
 * MiddleVR
 * (c) i'm in VR
 */

using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;

public class VRActor : MonoBehaviour {

    public string MiddleVRNodeName = "";
    public MiddleVRNodesManager.ENodesSyncDirection SyncDirection = MiddleVRNodesManager.ENodesSyncDirection.BothDirections;

    public bool Grabable = true;

    private bool m_InitializedNode = false;
    private vrNode3D m_MiddleVRNode = null;


    protected void Start ()
    {
        InitializeNode();

        if(gameObject.GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }

        if (gameObject.GetComponent<Collider>() == null)
        {
            MiddleVRTools.Log("[X] Actor object " + gameObject.name + " has no collider !! Put one or it won't act. " + gameObject.name );
        }
    }

    public vrNode3D InitializeNode()
    {
        return InitializeNode(MiddleVRNodeName, SyncDirection);
    }

    public vrNode3D InitializeNode(string iMiddleVRNodeName, MiddleVRNodesManager.ENodesSyncDirection iSyncDirection)
    {
        if (m_InitializedNode)
        {
            return m_MiddleVRNode;
        }

        if (iSyncDirection == MiddleVRNodesManager.ENodesSyncDirection.NoSynchronization)
        {
            m_MiddleVRNode = null;
            m_InitializedNode = true;
            return null;
        }

        // Set MiddleVR - Unity nodes binding
        m_MiddleVRNode = MiddleVRNodesManager.SynchronizeUnityNode(this.gameObject, iMiddleVRNodeName, iSyncDirection);

        if (m_MiddleVRNode != null)
        {
            m_InitializedNode = true;
        }

        return m_MiddleVRNode;
    }

    public vrNode3D GetMiddleVRNode()
    {
        if (m_InitializedNode)
        {
            return m_MiddleVRNode;
        }
        else
        {
            return InitializeNode();
        }
    }

    protected void OnDestroy()
    {
        // Stop node synchronization
        if (MiddleVR.VRKernel != null && m_MiddleVRNode != null)
        {
            MiddleVRNodesManager.StopNodeSynchronization(m_MiddleVRNode.GetName());
        }
    }
}
