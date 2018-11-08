using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using DigitalSky.Tracker;
using UnityEngine.XR.iOS;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine.XR.iOS.Utils;

public class ARRemoteFaceTrackerServer : ScriptableObject
{
    private static ARRemoteFaceTrackerServer _instance = null;
    public static ARRemoteFaceTrackerServer instance
    {
        get
        {
            if (_instance == null)
                _instance = ScriptableObject.CreateInstance<ARRemoteFaceTrackerServer>();

            return _instance;
        }
    }

    public UnityARAlignment startAlignment = UnityARAlignment.UnityARAlignmentGravity;
    public UnityARPlaneDetection planeDetection = UnityARPlaneDetection.Horizontal;
    public bool getPointCloud = true;
    public bool enableLightEstimation = true;
    public bool enableAutoFocus = true;

    public bool resetTracking = true;
    public bool removeExistingAnchors = true;

    private bool _isInit = false;
    public bool isInit
    {
        get { return _isInit; }
    }

    private Dictionary<string, float> _currentBlendShapes;

    private List<ARRemoteFaceTracker> _remoteTrackers = new List<ARRemoteFaceTracker>();
    public List<ARRemoteFaceTracker> remoteTrackers
    {
        get { return _remoteTrackers; }
    }

    private EditorConnection _editorConnection;
    public EditorConnection editorConnection
    {
        get { return _editorConnection; }
    }

    private int _currentPlayerID = -1;

    public ARRemoteFaceTrackerServer()
    {
        _isInit = false;
        _currentPlayerID = -1;
        _currentBlendShapes = new Dictionary<string, float>();
    }

    public void RegisterEditorConnect()
    {
        if (_editorConnection == null)
        {
            _editorConnection = EditorConnection.instance;
        }

        _editorConnection.Initialize();
        _editorConnection.RegisterConnection(PlayerConnected);
        _editorConnection.RegisterDisconnection(PlayerDisconnected);
        _editorConnection.Register(ConnectionMessageIds.updateCameraFrameMsgId, UpdateCameraFrame);
    }

    public static void Destroy()
    {
        _instance = null;
    }

    /// <summary>
    /// 实现ITracker的Init接口
    /// </summary>
    /// <returns></returns>
    public bool Init()
    {
        // 初始化
        RegisterEditorConnect();

        _isInit = true;
        return true;
    }

    void PlayerConnected(int playerID)
    {
        Debug.Log("[ARRemoteFaceTracker.PlayerConnected] -> received connect: " + playerID);
        _currentPlayerID = playerID;

        ARRemoteFaceTracker tracker = new ARRemoteFaceTracker(playerID);
        tracker.Init();
        _remoteTrackers.Add(tracker);

        SendInitToPlayer();
    }

    void PlayerDisconnected(int playerID)
    {
        if (_currentPlayerID == playerID)
        {
            Debug.Log("[ARRemoteFaceTracker.PlayerDisconnected] -> miss connect: " + playerID);
            _currentPlayerID = -1;

            for(int i = 0; i < remoteTrackers.Count; i++)
            {
                if(remoteTrackers[i].playerId == playerID)
                {
                    remoteTrackers[i].OnDestroy();
                    remoteTrackers.RemoveAt(i);
                    break;
                }
            }
        }
    }

    void UpdateCameraFrame(MessageEventArgs mea)
    {
        serializableUnityARFaceBlendShapes serARFaceBlendShape = mea.data.Deserialize<serializableUnityARFaceBlendShapes>();
        _currentBlendShapes = serARFaceBlendShape;

        if (remoteTrackers.Count > 0)
            remoteTrackers[0].UpdateBlendshapes(_currentBlendShapes);
    }

    void SendInitToPlayer()
    {
        serializableFromEditorMessage sfem = new serializableFromEditorMessage();
        sfem.subMessageId = SubMessageIds.editorInitARKit;
        serializableARSessionConfiguration ssc = new serializableARSessionConfiguration(startAlignment, planeDetection, getPointCloud, enableLightEstimation, enableAutoFocus);
        UnityARSessionRunOption roTracking = resetTracking ? UnityARSessionRunOption.ARSessionRunOptionResetTracking : 0;
        UnityARSessionRunOption roAnchors = removeExistingAnchors ? UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors : 0;
        sfem.arkitConfigMsg = new serializableARKitInit(ssc, roTracking | roAnchors);
        SendToPlayer(ConnectionMessageIds.fromEditorARKitSessionMsgId, sfem);
    }

    void SendToPlayer(System.Guid msgId, byte[] data)
    {
        if (_editorConnection != null)
            _editorConnection.Send(msgId, data);
    }

    public void SendToPlayer(System.Guid msgId, object serializableObject)
    {
        byte[] arrayToSend = serializableObject.SerializeToByteArray();
        SendToPlayer(msgId, arrayToSend);
    }

    /// <summary>
    /// 实现ITracker的OnUpdate接口
    /// </summary>
    public void OnUpdate()
    {
        if (_editorConnection == null || _editorConnection.ConnectedPlayers.Count <= 0)
            return;
    }

    public void OnDestroy()
    {
        _currentPlayerID = -1;
        //_editorConnection.Unregister(ConnectionMessageIds.updateCameraFrameMsgId, UpdateCameraFrame);

        _editorConnection = null;
        _isInit = false;

        for (int i = 0; i < remoteTrackers.Count; i++)
        {
            remoteTrackers[i].OnDestroy();
        }

        remoteTrackers.Clear();
    }
}
