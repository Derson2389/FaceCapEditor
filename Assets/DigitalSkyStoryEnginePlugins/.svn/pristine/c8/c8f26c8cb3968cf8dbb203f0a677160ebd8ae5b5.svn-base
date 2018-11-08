using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalSky.Tracker;

public class ARRemoteFaceTracker : ARFaceTracker
{
    public override bool isInit
    {
        get
        {
            return playerId != -1;
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

    private int _playerId = -1;
    public int playerId
    {
        get { return _playerId; }
    }

    public ARRemoteFaceTracker(int playerId) : base()
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
    /// 实现ITracker的OnDestroy接口
    /// </summary>
    public override void OnDestroy()
    {
        _retargeters.Clear();
        _isTracking = false;
        _playerId = -1;
    }

    public void UpdateBlendshapes(Dictionary<string, float> newBlendShapes)
    {
        _currentBlendShapes = newBlendShapes;
    }
}
