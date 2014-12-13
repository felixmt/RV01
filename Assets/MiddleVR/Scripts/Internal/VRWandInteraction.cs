/* VRWandInteraction
 * MiddleVR
 * (c) i'm in VR
 */
/*
using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;

public class VRWandInteraction : MonoBehaviour {

    public float RayLength      = 2;
    public bool  Highlight      = true;
    public Color HighlightColor = new Color();
    public Color GrabColor      = new Color();
    public bool  RepeatAction   = false;

    private GameObject m_Ray             = null;
    private GameObject m_SelectedObject  = null;
    private vrButtons  m_Buttons         = null;
    private bool       m_SearchedButtons = false;


    void Start () {

        m_Ray       = GameObject.Find("WandRay");
        if (m_Ray != null)
        {
            m_Ray.transform.localScale = new Vector3( 0.01f, RayLength / 2.0f, 0.01f );
            m_Ray.transform.localPosition = new Vector3( 0,0, RayLength / 2.0f );
        }
    }

    private void RaySelection()
    {
        GameObject selectedObject = GetClosestHit();

        if( selectedObject != null )
        {
            //print("Closest : " + selectedObject.name);

            if( m_SelectedObject != selectedObject )
            {
                //MiddleVRTools.Log("Enter other : " + selectedObject.name);
                HighlightObject( m_SelectedObject, false );
                m_SelectedObject = selectedObject;
                HighlightObject(m_SelectedObject, true );
                //MiddleVRTools.Log("Current : " + m_SelectedObject.name);
            }
        }
        // Else
        else
        {
            //MiddleVRTools.Log("No touch ! ");

            if ( m_SelectedObject != null )
            {
                HighlightObject(m_SelectedObject, false, HighlightColor );
                m_SelectedObject = null;
            }
        }
    }

    private GameObject GetClosestHit()
    {
        // Detect objects
        RaycastHit[] hits;
        Vector3 dir = transform.localToWorldMatrix * Vector3.forward;

        hits = Physics.RaycastAll(transform.position, dir, RayLength);

        int i = 0;
        GameObject closest = null;
        float distance = Mathf.Infinity;

        while (i < hits.Length)
        {
            RaycastHit hit = hits[i];

            //print("HIT : " + i + " : " + hit.collider.name);

            if( hit.distance < distance && hit.collider.name != "VRWand" && hit.collider.GetComponent<VRActor>() != null )
            {
                distance = hit.distance;
                closest = hit.collider.gameObject;
            }

            i++;
        }

        return closest;
    }

    private void HighlightObject( GameObject obj, bool state )
    {
        HighlightObject(obj, state, HighlightColor);
    }

    private void HighlightObject( GameObject obj, bool state, Color hCol )
    {
        GameObject hobj = m_Ray;

        if (hobj != null && hobj.GetComponent<Renderer>() != null && Highlight)
        {
            if( state )
            {
                hobj.GetComponent<Renderer>().material.color = hCol;
            }
            else
            {
                //m_SelectedObject.GetComponent<Renderer>().material.color = Color.white;
                hobj.GetComponent<Renderer>().material.color = Color.white;
            }
        }
    }

    public GameObject GetCurrentSelectedObject()
    {
        return m_SelectedObject;
    }

    void Update () {
        if (m_Buttons == null)
        {
            m_Buttons = MiddleVR.VRDeviceMgr.GetWandButtons();
        }

        if( m_Buttons == null )
        {
            if (m_SearchedButtons == false)
            {
                //MiddleVRTools.Log("[~] VRWandInteraction: Wand buttons undefined. Please specify Wand Buttons in the configuration tool.");
                m_SearchedButtons = true;
            }
        }

        RaySelection();

        //MiddleVRTools.Log("Current : " + m_SelectedObject);

        if (m_Buttons != null && m_SelectedObject != null )
        {
            uint MainButton = MiddleVR.VRDeviceMgr.GetWandButton0();

            VRActor script = m_SelectedObject.GetComponent<VRActor>();

            //MiddleVRTools.Log("Trying to take :" + m_SelectedObject.name);
            if (script != null)
            {
                // Action
                if (((!RepeatAction && m_Buttons.IsToggled(MainButton)) || (RepeatAction&& m_Buttons.IsPressed(MainButton))))
                {
                    m_SelectedObject.SendMessage("VRAction", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
}
*/
