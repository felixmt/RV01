/* VRClusterCommand
 * MiddleVR
 * (c) i'm in VR
 */

using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;

// Cluster commands are callbacks distributed across the cluster.
// - Cluster commands are not executed instantly but the next time the cluster synchronizes,
// in the same order they are called.
// - Return values are ignored.

public class VRClusterCommand : MonoBehaviour {

    // If m_Name is an empty string, one will be automatically generated
    public string m_Name = "";

    private vrCommandUnity m_Command = null;
    
    public void Start()
    {
        m_Command = new vrCommandUnity(m_Name, _CommandHandler, (uint)VRCommandFlags.VRCommandFlag_Cluster);        
    }

    // When called on the server (master) node, an asynchronous call
    // to _CommandHandler is triggered on all nodes at next
    // synchronization (VRManagerScript or VRManagerPostFrame)
    public void Call(vrValue val)
    {
        // Do not execute any cluster command on client (slave) nodes
        if (m_Command != null && MiddleVR.VRClusterMgr != null && !MiddleVR.VRClusterMgr.IsClient())
        {
            m_Command.Do(val);
        }
    }

    // This method is called on all nodes immediately after
    // the same synchronization point
    // Return value is ignored
    private vrValue _CommandHandler(vrValue iValue)
    {
        // Do stuff here
        return null; // Return value is ignored
    }

}
