using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalSky.Network;
using UnityEngine.XR.iOS.Utils;
using UnityEngine.XR.iOS;

public class ARFaceTrackerNetEventManager : NetEventManager
{
    private Dictionary<string, ARFaceTrackerNet> _remoteTrackers = new Dictionary<string, ARFaceTrackerNet>();
    public Dictionary<string, ARFaceTrackerNet> remoteTrackers
    {
        get { return _remoteTrackers; }
    }

    public ARFaceTrackerNetEventManager() : base()
    {
        _remoteTrackers = new Dictionary<string, ARFaceTrackerNet>();
    }

    // Use this for initialization
    public override void Init()
    {
        RegisterEvent(NetMessageID.JoinMsgId, new NetEvent<string, string>(OnJoinEvent));
        RegisterEvent(NetMessageID.ARFaceBlendShapeMsgId, new NetEvent<SerializableARFaceBlendShapes, string>(OnARFaceBlendShapeEvent));
        RegisterEvent(NetMessageID.ARCameraMsgId, new NetEvent<serializableUnityARCamera, string>(OnARCameraEvent));
        RegisterEvent(NetMessageID.ARCameraTextureMsgId, new NetEvent<SerializableTextureData, string>(OnARCameraTextureEvent));
        RegisterEvent(NetMessageID.ScreenCaptureYMsgId, new NetEvent<SerializableByteData, string>(OnScreenCaptureYEvent));
        RegisterEvent(NetMessageID.ScreenCaptureUVMsgId, new NetEvent<SerializableByteData, string>(OnScreenCaptureUVEvent));

        NetServer.instance.onConnect += OnConnect;
        NetServer.instance.onDisconnect += OnDisconnect;
    }

    public override void OnDestroy()
    {
        UnRegisterEvent(NetMessageID.JoinMsgId);
        UnRegisterEvent(NetMessageID.ARFaceBlendShapeMsgId);
        UnRegisterEvent(NetMessageID.ARCameraMsgId);
        UnRegisterEvent(NetMessageID.ScreenCaptureYMsgId);
        UnRegisterEvent(NetMessageID.ScreenCaptureUVMsgId);

        NetServer.instance.onConnect -= OnConnect;
        NetServer.instance.onDisconnect -= OnDisconnect;
    }

    private void OnConnect(string clientId)
    {
        if (!_remoteTrackers.ContainsKey(clientId))
        {
            _remoteTrackers.Add(clientId, new ARFaceTrackerNet(clientId));
            _remoteTrackers[clientId].Open();
        }
    }

    private void OnDisconnect(string clientId)
    {
        if (_remoteTrackers.ContainsKey(clientId))
        {
            _remoteTrackers[clientId].Close();
            _remoteTrackers[clientId].OnDestroy();
            _remoteTrackers.Remove(clientId);
        }
    }

    private void OnJoinEvent(string data, string sender)
    {
        Debug.Log("[ARFaceTrackerNetEventManager.OnJoinEvent] -> sender: " + sender + " data: " + data);
    }

    private void OnARFaceBlendShapeEvent(SerializableARFaceBlendShapes data, string sender)
    {       
        Dictionary<string, float> blendShapes = data;
        if(blendShapes != null)
        {
            if (_remoteTrackers.ContainsKey(sender))
                _remoteTrackers[sender].UpdateBlendshapes(blendShapes);
        }
    }

    private void OnARCameraEvent(serializableUnityARCamera data, string sender)
    {
        UnityARCamera scamera = new UnityARCamera();
        scamera = data;

        if (_remoteTrackers.ContainsKey(sender))
            _remoteTrackers[sender].UpdateARCamera(scamera);
    }

    private void OnARCameraTextureEvent(SerializableTextureData data, string sender)
    {
        if (_remoteTrackers.ContainsKey(sender))
            _remoteTrackers[sender].UpdateARCameraTexture(data);
    }

    private void OnScreenCaptureYEvent(SerializableByteData data, string sender)
    {
        //Debug.Log("[ARFaceTrackerNetEventManager.OnScreenCaptureYEvent] -> sender: " + sender);

        if (_remoteTrackers.ContainsKey(sender))
            _remoteTrackers[sender].UpdateScreenYTex(data.data);
    }

    private void OnScreenCaptureUVEvent(SerializableByteData data, string sender)
    {
        //Debug.Log("[ARFaceTrackerNetEventManager.OnScreenCaptureUVEvent] -> sender: " + sender);

        if (_remoteTrackers.ContainsKey(sender))
            _remoteTrackers[sender].UpdateScreenUVTex(data.data);
    }
}
