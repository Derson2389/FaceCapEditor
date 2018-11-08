using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using UnityEngine.XR.iOS.Utils;
using UnityEngine.XR.iOS;
using DigitalSky.Tracker;

public class ARRemoteFaceTrackerPlayer : MonoBehaviour
{
    public TrackRetargeter[] _retargeters;

    public UnityARVideo unityARVideo;
    public RenderTexture rt = null;

    private ITracker _tracker;
    private Rect _videoRect;

    PlayerConnection playerConnection;
    int editorID;
    GUIStyle centeredStyle;

    //private UnityARSessionNativeInterface _session;
    //Texture2D frameBufferTex;

    // Use this for initialization
    void Start()
    {
        Debug.Log("STARTING ConnectToEditor");
        editorID = -1;
        playerConnection = PlayerConnection.instance;
        playerConnection.RegisterConnection(EditorConnected);
        playerConnection.RegisterDisconnection(EditorDisconnected);
        playerConnection.Register(ConnectionMessageIds.fromEditorARKitSessionMsgId, HandleEditorMessage);

        _tracker = ARFaceTracker.instance;
        _tracker.Init();

        _tracker.Open();

        if (_retargeters != null)
        {
            for (int i = 0; i < _retargeters.Length; i++)
            {
                _tracker.AddListener(_retargeters[i]);
            }
        }

        _tracker.EnableTracking(true);

        if (rt == null)
        {
            rt = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
            rt.Create();
        }

        if (unityARVideo != null)
        {
            Camera cam = unityARVideo.GetComponent<Camera>();
            cam.targetTexture = rt;
        }
    }

    // Update is called once per frame
    void Update()
    {
        _tracker.OnUpdate();
    }

    void OnDestroy()
    {
        editorID = -1;
        DisconnectFromEditor();

        _tracker.OnDestroy();
        rt.Release();

        UnityARSessionNativeInterface.ARFaceAnchorAddedEvent -= FaceAdded;
        UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent -= FaceUpdated;
    }

    void OnGUI()
    {
        if(centeredStyle == null)
        {
            centeredStyle = GUI.skin.GetStyle("Box");
            centeredStyle.alignment = TextAnchor.MiddleCenter;
            centeredStyle.fontSize = 36;
        }

        Color color = GUI.color;
        GUI.color = Color.red;
        switch (Application.internetReachability)
        {
            case NetworkReachability.NotReachable:
                //网络断开
                {
                    GUI.Box(new Rect((Screen.width / 2) - 250, (Screen.height) - 420, 500, 200), "网络不可用", centeredStyle);
                }
                break;
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                //WIFI
                {
                    if (playerConnection != null && !playerConnection.isConnected)
                    {
                        GUI.Box(new Rect((Screen.width / 2) - 250, (Screen.height) - 420, 500, 200), "等待连接Unity编辑器...", centeredStyle);
                    }
                }
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                //4G/3G
                break;
        }
        GUI.color = color;

        Resolution currentResolution = Screen.currentResolution;
        _videoRect = new Rect(0, 0, (int)(currentResolution.width * 0.35), (int)(currentResolution.height * 0.35));

        if (rt != null)
            GUI.DrawTexture(_videoRect, rt);
    }

    void HandleEditorMessage(MessageEventArgs mea)
    {
        serializableFromEditorMessage sfem = mea.data.Deserialize<serializableFromEditorMessage>();
        if (sfem != null && sfem.subMessageId == SubMessageIds.editorInitARKit)
        {
            InitializeARKit(sfem.arkitConfigMsg);
        }
    }

    void InitializeARKit(serializableARKitInit sai)
    {
#if !UNITY_EDITOR
        //get the config and runoption from editor and use them to initialize arkit on device
        //ARKitWorldTrackingSessionConfiguration config = sai.config;
        //UnityARSessionRunOption runOptions = sai.runOption;
        //_session.RunWithConfigAndOptions(config, runOptions);

        //UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdated;
        //UnityARSessionNativeInterface.ARAnchorAddedEvent += ARAnchorAdded;
        //UnityARSessionNativeInterface.ARAnchorUpdatedEvent += ARAnchorUpdated;
        //UnityARSessionNativeInterface.ARAnchorRemovedEvent += ARAnchorRemoved;
        //UnityARSessionNativeInterface.ARFaceAnchorAddedEvent += FaceAdded;
        //UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent += FaceUpdated;

        UnityARSessionNativeInterface.ARFaceAnchorAddedEvent += FaceAdded;
        UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent += FaceUpdated;
#endif
    }

    void FaceAdded(ARFaceAnchor anchorData)
    {
        
    }

    void FaceUpdated(ARFaceAnchor anchorData)
    {
        serializableUnityARFaceBlendShapes serARFaceBlendShape = anchorData.blendShapes;
        SendToEditor(ConnectionMessageIds.updateCameraFrameMsgId, serARFaceBlendShape);
    }

    /*public void ARFrameUpdated(UnityARCamera camera)
    {
        serializableUnityARCamera serARCamera = camera;
        SendToEditor(ConnectionMessageIds.updateCameraFrameMsgId, serARCamera);
    }

    public void ARAnchorAdded(ARPlaneAnchor planeAnchor)
    {
        serializableUnityARPlaneAnchor serPlaneAnchor = planeAnchor;
        SendToEditor(ConnectionMessageIds.addPlaneAnchorMsgeId, serPlaneAnchor);
    }

    public void ARAnchorUpdated(ARPlaneAnchor planeAnchor)
    {
        serializableUnityARPlaneAnchor serPlaneAnchor = planeAnchor;
        SendToEditor(ConnectionMessageIds.updatePlaneAnchorMsgeId, serPlaneAnchor);
    }

    public void ARAnchorRemoved(ARPlaneAnchor planeAnchor)
    {
        serializableUnityARPlaneAnchor serPlaneAnchor = planeAnchor;
        SendToEditor(ConnectionMessageIds.removePlaneAnchorMsgeId, serPlaneAnchor);
    }*/

    void EditorConnected(int playerID)
    {
        Debug.Log("connected success from unity editor");
        editorID = playerID;
    }

    void EditorDisconnected(int playerID)
    {
        Debug.Log("disconnected error from unity editor");

        if (editorID == playerID)
        {
            editorID = -1;
        }

        DisconnectFromEditor();

        UnityARSessionNativeInterface.ARFaceAnchorAddedEvent -= FaceAdded;
        UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent -= FaceUpdated;
    }

    public void SendToEditor(System.Guid msgId, object serializableObject)
    {
        byte[] arrayToSend = serializableObject.SerializeToByteArray();
        SendToEditor(msgId, arrayToSend);
    }

    public void SendToEditor(System.Guid msgId, byte[] data)
    {
        if (playerConnection.isConnected)
        {
            playerConnection.Send(msgId, data);
        }
    }

    public void DisconnectFromEditor()
    {
#if UNITY_2017_1_OR_NEWER
		playerConnection.DisconnectAll();
#endif
    }
}
