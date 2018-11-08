using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalSky.Tracker;
using UnityEngine.XR.iOS;

public class ARFaceTracker : ITracker
{
    private static ARFaceTracker _instance = null;
    public static ARFaceTracker instance
    {
        get
        {
            if (_instance == null)
                _instance = new ARFaceTracker();

            return _instance;
        }
    }

    protected bool _isInit = false;
    public virtual bool isInit
    {
        get { return _isInit; }
    }

    public virtual string trackerName
    {
        get { return "iphoneX"; }
    }

    /** Flag to check if tracker is enabled. */
    protected bool _isTracking = false;
    public bool isTracking
    {
        get { return _isTracking; }
    }

    /** Flag to check if tracker is active or not.*/
    public virtual bool trackerActive
    {
        get { return _session != null; }
    }

    protected UnityARSessionNativeInterface _session;
    public UnityARSessionNativeInterface session
    {
        get { return _session; }
    }

    protected List<TrackData> _trackDatas;
    public List<TrackData> trackDatas
    {
        get { return _trackDatas; }
    }

    protected List<TrackRetargeter> _retargeters;
    protected Dictionary<string, float> _currentBlendShapes;

    protected ARFaceTracker()
    {
        _trackDatas = new List<TrackData>();
        _retargeters = new List<TrackRetargeter>();
        _currentBlendShapes = new Dictionary<string, float>();
    }

    public static void Destroy()
    {
        if (_instance != null)
            _instance.OnDestroy();

        _instance = null;
    }

    /// <summary>
    /// 实现ITracker的Init接口
    /// </summary>
    /// <returns></returns>
    public virtual bool Init()
    {
        //get the config and runoption from editor and use them to initialize arkit on device
        //Application.targetFrameRate = 60;
                  
        _isInit = true;
        _isTracking = false;
        return true;
    }

    /// <summary>
    /// 实现ITracker的Open接口
    /// </summary>
    /// <returns></returns>
    public virtual bool Open()
    {
        _session = UnityARSessionNativeInterface.GetARSessionNativeInterface();

        ARKitFaceTrackingConfiguration config = new ARKitFaceTrackingConfiguration();
        config.alignment = UnityARAlignment.UnityARAlignmentGravity;
        config.enableLightEstimation = true;

        if (config.IsSupported)
        {
            _session.RunWithConfig(config);

            UnityARSessionNativeInterface.ARFaceAnchorAddedEvent += FaceAdded;
            UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent += FaceUpdated;

            return true;
        }else
        {
            _session = null;
        }

        return false;
    }

    /// <summary>
    /// 实现ITracker的Close接口
    /// </summary>
    public virtual void Close()
    {
        UnityARSessionNativeInterface.ARFaceAnchorAddedEvent -= FaceAdded;
        UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent -= FaceUpdated;

        _session.Pause();
        _session = null;
    }

    /// <summary>
    /// 实现ITracker的OnUpdate接口
    /// </summary>
    public virtual void OnUpdate()
    {
        if (!trackerActive || !isTracking)
            return;

        UpdateTrackDatas();

        // 为所有retargeter更新重定向track数据
        for (int i = 0; i < _retargeters.Count; i++)
        {
            if (_retargeters[i].isBinding)
                _retargeters[i].OnUpdateTrackDatas(this);
        }
    }

    /// <summary>
    /// 实现ITracker的OnDestroy接口
    /// </summary>
    public virtual void OnDestroy()
    {
        _retargeters.Clear();
        _isInit = false;
        _isTracking = false;
        _session = null;
    }

    /// <summary>
    /// 实现ITracker的EnableTracking接口
    /// </summary>
    /// <param name="enabled">是否激活tracker</param>
    public void EnableTracking(bool enabled)
    {
        _isTracking = enabled;
    }

    /// <summary>
    /// 实现ITracker的AddListener接口
    /// </summary>
    /// <param name="listener"></param>
    public bool AddListener(TrackRetargeter listener)
    {
        if (listener == null)
            return false;

        if (_retargeters.Contains(listener))
        {
            Debug.LogWarning("[ARFaceTracker.AddListener] -> listener already exists.");
            return false;
        }

        if (!listener.Init() || !listener.CreateTrackBinding())
            return false;

        _retargeters.Add(listener);
        return true;
    }

    /// <summary>
    /// 实现ITracker的RemoveListener接口
    /// </summary>
    /// <param name="listener"></param>
    public void RemoveListener(TrackRetargeter listener)
    {
        _retargeters.Remove(listener);
    }

    /// <summary>
    /// 实现ITracker的HasListener接口
    /// </summary>
    /// <param name="listener"></param>
    /// <returns></returns>
    public bool HasListener(TrackRetargeter listener)
    {
        if (_retargeters.Contains(listener))
        {
            return true;
        }

        return false;
    }

    void FaceAdded(ARFaceAnchor anchorData)
    {
        _currentBlendShapes = anchorData.blendShapes;
    }

    void FaceUpdated(ARFaceAnchor anchorData)
    {
        _currentBlendShapes = anchorData.blendShapes;
    }

    void UpdateTrackDatas()
    {
        if (_currentBlendShapes != null)
        {
            foreach (KeyValuePair<string, float> kvp in _currentBlendShapes)
            {
                TrackData data = _trackDatas.Find(delegate (TrackData td)
                {
                    return td.bindName == kvp.Key;
                });

                if(data == null)
                {
                    data = new TrackData();
                    data.bindName = kvp.Key;
                    data.used = true;
                    _trackDatas.Add(data);
                }

                data.blendshapeValue = kvp.Value;
            }
        }
    }
}
