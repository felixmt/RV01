/* VRShareTransform
 * MiddleVR
 * (c) i'm in VR
 */

using UnityEngine;
using System.Collections;

using MiddleVR_Unity3D;

// Share a GameObject tranformation using MiddleVR Cluster Command
public class VRShareTransform : MonoBehaviour {
    static private uint g_shareID = 1;

    private vrCommandUnity m_Command = null;

    // Create cluster command on script start
    // For more information, refer to the MiddleVR User Guide and the VRShareTransform script
    public void Start ()
    {
        uint shareID = g_shareID++;
        string shareName = "VRShareTransform_" + shareID.ToString();

        // Create the command with cluster flag
        m_Command = new vrCommandUnity(shareName, _CommandHandler, (uint)VRCommandFlags.VRCommandFlag_Cluster);        
    }

    // On the server, call the cluster command with a list of [position, rotation] every update
    // On all nodes, _CommandHandler will be called the next time there is a synchronization update :
    // either during VRManagerScript Update() or VRManagerPostFrame Update() (see script ordering)
    public void Update ()
    {
        if( MiddleVR.VRClusterMgr.IsServer() )
        {
            // put position and orientation in a vrValue as a list
            Vector3 p = transform.position;
            Quaternion q = transform.rotation;

            vrValue val = vrValue.CreateList();
            val.AddListItem( new vrVec3(p.x, p.y, p.z) );
            val.AddListItem( new vrQuat(q.x, q.y, q.z, q.w) );

            // Do the actual call
            // This returns immediately
            m_Command.Do( val );
        }
    }

    // On clients, handle the command call
    private vrValue _CommandHandler(vrValue iValue)
    {
        if( MiddleVR.VRClusterMgr.IsClient() )
        {
            // extract position and orientation from the vrValue
            vrVec3 pos = iValue[0].GetVec3();
            vrQuat orient = iValue[1].GetQuat();

            Vector3 p = new Vector3( pos.x(), pos.y(), pos.z() );
            Quaternion q = new Quaternion( (float)orient.x(), (float)orient.y(), (float)orient.z(), (float)orient.w() );

            transform.position = p;
            transform.rotation = q;
        }

        return null;
    }
}
