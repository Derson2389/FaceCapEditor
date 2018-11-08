using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalSky.Tracker;
using DigitalSky.Network;
using UnityEngine.XR.iOS;

public class ARFaceTrackerNet : ARFaceTracker
{
    public override bool isInit
    {
        get
        {
            return _playerId != "";
        }
    }

    public override string trackerName
    {
        get { return "iphoneX-" + _playerId; }
    }

    /** Flag to check if tracker is active or not.*/
    public override bool trackerActive
    {
        get { return true; }
    }

    private string _playerId = "";
    public string PlayerId
    {
        get { return _playerId; }
    }

    Texture2D _remoteScreenYTex = null;
    public Texture2D remoteScreenYTex
    {
        get { return _remoteScreenYTex; }
    }

    Texture2D _remoteScreenUVTex = null;
    public Texture2D remoteScreenUVTex
    {
        get { return _remoteScreenUVTex; }
    }

    bool _bTexturesInitialized = false;

    Texture2D _cameraTexture = null;
    public Texture2D cameraTexture
    {
        get { return _cameraTexture; }
    }

    public ARFaceTrackerNet(string playerId) : base()
    {
        _playerId = playerId;
    }

    /// <summary>
    /// 实现ITracker的Open接口
    /// </summary>
    /// <returns></returns>
    public override bool Open()
    {   
        return true;
    }

    /// <summary>
    /// 实现ITracker的Close接口
    /// </summary>
    public override void Close()
    {
        
    }

    /// <summary>
    /// 实现ITracker的OnUpdate接口
    /// </summary>
    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    /// <summary>
    /// 实现ITracker的OnDestroy接口
    /// </summary>
    public override void OnDestroy()
    {
        _retargeters.Clear();
        _isTracking = false;
        _playerId = "";
    }

    public void UpdateBlendshapes(Dictionary<string, float> newBlendShapes)
    {
        _currentBlendShapes = newBlendShapes;
    }

    public void UpdateARCameraTexture(SerializableTextureData textureData)
    {
        if (_cameraTexture == null)
            _cameraTexture = new Texture2D(textureData.width, textureData.height, TextureFormat.RGBA32, false);

        _cameraTexture.LoadImage(textureData.data);
    }

    public void UpdateARCamera(UnityARCamera arCamera)
    {
        int yWidth = arCamera.videoParams.yWidth;
        int yHeight = arCamera.videoParams.yHeight;
        int uvWidth = yWidth / 2;
        int uvHeight = yHeight / 2;

        if (_remoteScreenYTex == null || _remoteScreenYTex.width != yWidth || _remoteScreenYTex.height != yHeight)
        {
            if (_remoteScreenYTex)
            {
                GameObject.Destroy(_remoteScreenYTex);
            }
            _remoteScreenYTex = new Texture2D(yWidth, yHeight, TextureFormat.R8, false, true);
        }

        if (_remoteScreenUVTex == null || _remoteScreenUVTex.width != uvWidth || _remoteScreenUVTex.height != uvHeight)
        {
            if (_remoteScreenUVTex)
            {
                GameObject.Destroy(_remoteScreenUVTex);
            }
            _remoteScreenUVTex = new Texture2D(uvWidth, uvHeight, TextureFormat.RG16, false, true);
        }

        _bTexturesInitialized = true;
    }

    public void UpdateScreenYTex(byte[] data)
    {
        if (!_bTexturesInitialized)
            return;

        _remoteScreenYTex.LoadRawTextureData(data);
        _remoteScreenYTex.Apply();
    }

    public void UpdateScreenUVTex(byte[] data)
    {
        if (!_bTexturesInitialized)
            return;

        _remoteScreenUVTex.LoadRawTextureData(data);
        _remoteScreenUVTex.Apply();
    }
}
