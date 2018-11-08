using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalSky.Tracker;

[System.Serializable]
public class TrackDataAnimation
{
    [SerializeField]
    public AnimationCurve curve = null;

    [SerializeField]
    public string bindName = null;

    private ARFaceTrackerNet _tracker;

    public TrackDataAnimation(string bindName, ARFaceTrackerNet tracker)
    {
        this.bindName = bindName;
        this.curve = new AnimationCurve();
        this.curve.AddKey(0, 0);

        _tracker = tracker;
    }

    public void AddKey(float time)
    {
        if (_tracker == null)
            return;

        TrackData trackData = _tracker.trackDatas.Find(obj => obj.bindName == bindName);
        if (trackData == null)
            return;

        curve.AddKey(time, trackData.blendshapeValue);
    }
}

public class ARFaceTrackDataRecorder : MonoBehaviour
{
    // 保存trackData值的动画曲线集合
    public List<TrackDataAnimation> animationCurves = new List<TrackDataAnimation>();

    private List<TrackData> _trackDatas = new List<TrackData>();
    public List<TrackData> trackDatas
    {
        get { return _trackDatas; }
    }

    protected ARFaceTrackerNet _arfaceTracker;
    public ARFaceTrackerNet arfaceTracker
    {
        get { return _arfaceTracker; }
    }

    protected bool _init = false;
    public bool init
    {
        get { return _init; }
    }

    protected bool _isRecording = false;
    protected float _frameTime = 0;

    // Use this for initialization
    void Start()
    {

    }

    public virtual void InitRecord(ARFaceTrackerNet arfaceTracker)
    {
        _arfaceTracker = arfaceTracker;
        if (_arfaceTracker == null || _arfaceTracker.trackerActive == false || _arfaceTracker.trackDatas.Count == 0)
        {
            _init = false;
            return;
        }

        // 通过trackBinding数据生成动画曲线信息
        animationCurves = new List<TrackDataAnimation>();
        for (int i = 0; i < arfaceTracker.trackDatas.Count; i++)
        {
            animationCurves.Add(new TrackDataAnimation(arfaceTracker.trackDatas[i].bindName, _arfaceTracker));
        }

        _init = true;
    }

    public virtual void InitAnimation()
    {
        _trackDatas.Clear();

        for (int i = 0; i < animationCurves.Count; i++)
        {
            TrackData data = new TrackData();
            data.bindName = animationCurves[i].bindName;
            data.used = true;
            data.blendshapeValue = 0f;
            _trackDatas.Add(data);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_init || !_isRecording)
            return;

        //_frameTime += Time.deltaTime;
        //Record(_frameTime);
    }

    public virtual void Record(float time)
    {
        if (!_init || !_isRecording)
            return;

        _frameTime = time;
        // 根据tracker数据在TrackData动画曲线上添加关键帧
        for (int i = 0; i < animationCurves.Count; i++)
        {
            animationCurves[i].AddKey(time);
        }
    }

    public void StartRecord()
    {
        if (!_init)
            return;

        _isRecording = true;
        _frameTime = 0f;
    }

    public void StopRecord()
    {
        _isRecording = false;
    }

    public virtual void OnAnimation(float time)
    {
        _frameTime = time;

        for(int i = 0; i < _trackDatas.Count; i++)
        {
            _trackDatas[i].blendshapeValue = Evaluate(_trackDatas[i].bindName);
        }
    }

    public virtual float Evaluate(string bindName)
    {
        TrackDataAnimation trackDataAnim = animationCurves.Find(obj => obj.bindName == bindName);
        if (trackDataAnim == null)
            return 0f;

        float evaValue = trackDataAnim.curve.Evaluate(_frameTime);
        return evaValue;
    }
}
