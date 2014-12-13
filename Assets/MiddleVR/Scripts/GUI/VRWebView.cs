using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class VRWebView : MonoBehaviour {

    // Public attributes
    public int m_Width  = 1024;
    public int m_Height = 768;
    public string m_URL = "http://www.middlevr.com/";
    public float m_Zoom = 1.0f;

    // View management
    private vrWebView m_WebView = null;
    private vrImage m_Image = null;
    private Texture2D m_Texture = null;

    // Virtual mouse management
    private List<Camera> m_Cameras = null;
    private Vector2 m_VirtualMousePosition;
    private bool m_MouseButtonState = false;

    public vrImage image
    {
        get { return m_Image; }
    }

    public vrWebView webView
    {
        get { return m_WebView; }
    }

    public void SetVirtualMouseButtonPressed()
    {
        if (m_WebView != null)
        {
            m_WebView.SendMouseButtonPressed((int)m_VirtualMousePosition.x, (int)m_VirtualMousePosition.y);
        }
    }

    public void SetVirtualMouseButtonReleased()
    {
        if (m_WebView != null)
        {
            m_WebView.SendMouseButtonReleased((int)m_VirtualMousePosition.x, (int)m_VirtualMousePosition.y);
        }
    }

    // pos: texture coordinate of raycast hit
    public void SetVirtualMousePosition(Vector2 pos)
    {
        m_VirtualMousePosition = new Vector2(pos.x * m_Texture.width, (float)m_WebView.GetHeight() - (pos.y * m_Texture.height));
        if (m_WebView != null)
        {
            m_WebView.SendMouseMove((int)m_VirtualMousePosition.x, (int)m_VirtualMousePosition.y);
        }
    }

    void Start ()
    {
        // Check if we are running MiddleVR
        if(MiddleVR.VRKernel == null)
        {
            Debug.Log("VRManager is missing from the scene !");
            enabled = false;
            return;
        }

        m_VirtualMousePosition = new Vector2(0, 0);

        if (Application.isEditor)
        {
            // Get the vrCameras corresponding Cameras
            m_Cameras = new List<Camera>();

            uint camerasNb = MiddleVR.VRDisplayMgr.GetCamerasNb();
            for (uint i = 0; i < camerasNb; ++i)
            {
                vrCamera vrcamera = MiddleVR.VRDisplayMgr.GetCameraByIndex(i);
                GameObject cameraObj = GameObject.Find(vrcamera.GetName());
                Camera camera = cameraObj.GetComponent<Camera>();
                if (camera != null)
                {
                    m_Cameras.Add(camera);
                }
            }
        }

        // Create vrImage and Texture2D
        // OpenGL and Direct3D 9: Memory order for texture upload is BGRA (little-endian representation of ARGB32)
        // Direct3D 11: Unity seems to ignore TextureFormat.ARGB32 and always creates an RGBA texture.
        // We let vrImage do the pixel format conversion because this operation is done in another thread.
        if (SystemInfo.graphicsDeviceVersion.Contains("Direct3D 11"))
        {
            m_Image = new vrImage("", (uint)m_Width, (uint)m_Height, VRImagePixelFormat.VRImagePixelFormat_RGBA);
        }
        else
        {
            m_Image = new vrImage("", (uint)m_Width, (uint)m_Height, VRImagePixelFormat.VRImagePixelFormat_BGRA);
        }

        m_Texture = new Texture2D(m_Width, m_Height, TextureFormat.ARGB32, false);
        m_Texture.wrapMode = TextureWrapMode.Clamp;

        // Fill texture
        Color32[] colors = new Color32[(m_Width * m_Height)];
        
        for (int i = 0; i < (m_Width * m_Height); i++)
        {
            colors[i].r = 0;
            colors[i].g = 0;
            colors[i].b = 0;
            colors[i].a = 0;
        }
        
        m_Texture.SetPixels32(colors);
        m_Texture.Apply();

        // Attach texture
        if (gameObject != null && gameObject.GetComponent<GUITexture>() == null && gameObject.GetComponent<Renderer>() != null)
        {
            // Assign the material/shader to the object attached
            gameObject.GetComponent<Renderer>().material = Resources.Load("VRWebViewMaterial", typeof(Material)) as Material;
            gameObject.GetComponent<Renderer>().material.mainTexture = this.m_Texture;
        }
        else if (gameObject != null && gameObject.GetComponent<GUITexture>() != null )
        {
            gameObject.GetComponent<GUITexture>().texture = this.m_Texture;
        }
        else
        {
            MiddleVR.VRLog(2, "VRWebView must be attached to a GameObject with a renderer or a GUITexture !");
            enabled = false;
            return;
        }
        
        // Handle Cluster
        if ( MiddleVR.VRClusterMgr.IsServer() && ! MiddleVR.VRKernel.GetEditorMode() )
        {
            MiddleVR.VRClusterMgr.AddSynchronizedObject( m_Image );
        }
        
        if( ! MiddleVR.VRClusterMgr.IsClient() )
        {
            m_WebView = new vrWebView("", GetAbsoluteURL( m_URL ) , (uint)m_Width, (uint)m_Height, m_Image );
            m_WebView.SetZoom( m_Zoom );
        }
    }

    [DllImport("MiddleVR_UnityRendering")]
    private static extern void MiddleVR_CopyBufferToUnityNativeTexture(IntPtr iBuffer, IntPtr iNativeTexturePtr, uint iWidth, uint iHeight);

    void Update ()
    {
        // Handle mouse input
        if( ! MiddleVR.VRClusterMgr.IsClient() )
        {
            Vector2 mouseHit = new Vector2(0, 0);
            bool hasMouseHit = false;

            if (gameObject.GetComponent<GUITexture>() != null)
            {
                // GUITexture mouse input
                Rect r = gameObject.GetComponent<GUITexture>().GetScreenRect();
                
                if( Input.mousePosition.x >= r.x && Input.mousePosition.x < (r.x + r.width) &&
                    Input.mousePosition.y >= r.y && Input.mousePosition.y < (r.y + r.height) )
                {

                    float x = (Input.mousePosition.x - r.x) / r.width;
                    float y = (Input.mousePosition.y - r.y) / r.height;
                    
                    mouseHit = new Vector2(x, y);
                    hasMouseHit = true;
                }
            }
            else if( gameObject.GetComponent<Renderer>() != null && Application.isEditor )
            {
                // 3D object mouse input
                mouseHit = GetClosestMouseHit();

                if (mouseHit.x != -1 && mouseHit.y != -1)
                {
                    hasMouseHit = true;
                }
            }

            if (hasMouseHit)
            {
                bool newMouseButtonState = Input.GetMouseButton(0);

                if (m_MouseButtonState == false && newMouseButtonState == true)
                {
                    SetVirtualMousePosition(mouseHit);
                    SetVirtualMouseButtonPressed();
                }
                else if (m_MouseButtonState == true && newMouseButtonState == false)
                {
                    SetVirtualMouseButtonReleased();
                    SetVirtualMousePosition(mouseHit);
                }
                else
                {
                    SetVirtualMousePosition(mouseHit);
                }

                m_MouseButtonState = newMouseButtonState;
            }
        }

        // Handle texture update
        if ( m_Image.HasChanged() )
        {
            using (vrImageFormat format = m_Image.GetImageFormat())
            {
                if ((uint)m_Texture.width != format.GetWidth() || (uint)m_Texture.height != format.GetHeight())
                {
                    m_Texture.Resize((int)format.GetWidth(), (int)format.GetHeight());
                    m_Texture.Apply();
                }

                if (format.GetWidth() > 0 && format.GetHeight() > 0)
                {
                    MiddleVR_CopyBufferToUnityNativeTexture(m_Image.GetReadBuffer(), m_Texture.GetNativeTexturePtr(), format.GetWidth(), format.GetHeight());
                }
            }
        }
    }

    private Vector2 GetClosestMouseHit()
    {
        foreach( Camera camera in m_Cameras )
        {
            RaycastHit[] hits = Physics.RaycastAll( camera.ScreenPointToRay(Input.mousePosition));

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject == gameObject)
                {
                    return hit.textureCoord;
                }
            }
        }

        return new Vector2(-1,-1);
    }

    private string GetAbsoluteURL( string iUrl )
    {
        string url = iUrl;
        
        // If url does not start with http/https we assume it's a file
        if( !url.StartsWith( "http://" ) && !url.StartsWith( "https://" ) )
        {
            if( url.StartsWith( "file://" ) )
            {
                url = url.Substring(7, url.Length-7 );
                
                if( Application.platform == RuntimePlatform.WindowsPlayer && url.StartsWith( "/" ) )
                {
                    url = url.Substring(1, url.Length-1);
                }
            }
            
            if( ! System.IO.Path.IsPathRooted( url ) )
            {
                url = Application.dataPath + System.IO.Path.DirectorySeparatorChar + url;
            }
            
            if( Application.platform == RuntimePlatform.WindowsPlayer )
            {
                url = "/" + url;
            }
            
            url = "file://" + url;
        }
        
        return url;
    }
}
