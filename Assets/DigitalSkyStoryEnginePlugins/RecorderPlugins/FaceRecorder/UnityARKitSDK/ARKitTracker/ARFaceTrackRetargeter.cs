using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalSky.Tracker;

public class ARFaceTrackRetargeter : TrackRetargeter
{
    /// <summary>
    /// 重定向配置信息
    /// </summary>
    public TextAsset bindingConfiguration;

    /// <summary>
    /// 是否初始化成功
    /// </summary>
    private bool _isInit = false;
    public override bool isInit
    {
        get { return _isInit; }
    }

    private List<ITrackBinding> _trackBindings;
    /// <summary>
    /// 重定向列表
    /// </summary>
    public override List<ITrackBinding> trackBindings
    {
        get { return _trackBindings; }
    }

    /// <summary>
    /// 是否绑定成功
    /// </summary>
    public override bool isBinding
    {
        get { return _trackBindings != null && _trackBindings.Count > 0; }
    }

    private List<TrackData> _trackDatas;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override bool Init()
    {
        if (bindingConfiguration == null)
        {
            //Debug.LogError("[ARFaceRetargeter.Init] -> init failed, bindingConfiguration is null");
            return false;
        }

        if (isInit)
            return true;

        if (target == null)
            target = gameObject;

        SkinnedMeshRenderer[] rigSkinnedMeshRenderers = target.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        // 如果未找到SkinnedMeshRender组件，则初始化失败
        if (rigSkinnedMeshRenderers == null || rigSkinnedMeshRenderers.Length == 0)
            return false;

        _trackBindings = new List<ITrackBinding>();
        _bindingData = bindingConfiguration;
        _trackDatas = new List<TrackData>();

        _isInit = true;
        return true;
    }

    /// <summary>
    /// 更新Track数据到重定向对象
    /// </summary>
    public override void OnUpdateTrackDatas(ITracker tracker)
    {
        ARFaceTracker arfaceTracker = (ARFaceTracker)tracker;
        _trackDatas = arfaceTracker.trackDatas;

        ApplyTrackDatas();
    }

    public void OnUpdateTrackDatas(List<TrackData> trackDatas)
    {
        _trackDatas = trackDatas;
        ApplyTrackDatas();
    }

    public void ApplyTrackDatas()
    {
        float jawOpenCof = 0f;
        float mouseCloseCof = 0f;

        Dictionary<int, TrackData> jawOpens = new Dictionary<int, TrackData>();
        Dictionary<int, TrackData> mouseCloses = new Dictionary<int, TrackData>();

        if (_trackDatas == null || _trackDatas.Count == 0)
            return;

        for(int i = 0; i < _trackBindings.Count; i++)
        {
            TrackData trackData = _trackDatas.Find(obj => obj.bindName == _trackBindings[i].bindName);
            if (trackData == null)
                continue;

            if (trackData.bindName == "jawOpen")
            {
                jawOpenCof = trackData.blendshapeValue;
                jawOpens.Add(i, trackData);
                //Debug.Log("jawOpen data: " + jawOpen.blendshapeValue);
                continue;
            }

            if (trackData.bindName == "mouthClose")
            {
                mouseCloseCof = trackData.blendshapeValue;
                mouseCloses.Add(i, trackData);
                //Debug.Log("mouthClose data: " + mouseClose.blendshapeValue);
                continue;
            }

            _trackBindings[i].OnUpdateTrackData(trackData);
        }

        // jawOpen = 1.0  mouseClose = 1.0的情况下， 需要闭合嘴巴
        if(mouseCloseCof > 0.15f && jawOpenCof > 0.15f && Mathf.Abs(jawOpenCof - mouseCloseCof) <= 0.15f)
        {
            foreach(var item in jawOpens)
            {
                item.Value.blendshapeValue = 0f;
            }
        }
        else
        {
            foreach (var item in mouseCloses)
            {
                item.Value.blendshapeValue = 0f;
            }
        }

        foreach (var item in jawOpens)
        {
            _trackBindings[item.Key].OnUpdateTrackData(item.Value);
        }

        foreach (var item in mouseCloses)
        {
            _trackBindings[item.Key].OnUpdateTrackData(item.Value);
        }
    }

    /// <summary>
    /// 根据重定向数据生成重定向关系表
    /// </summary>
    /// <returns></returns>
    public override bool CreateTrackBinding()
    {
        // Reset all targets to initial value.
        if (trackBindings != null && trackBindings.Count > 0)
        {
            foreach (var item in trackBindings)
            {
                if (item.target != null)
                    item.target.Reset();
            }

            _trackBindings.Clear();
            _trackDatas.Clear();
        }


        TextAsset data = (TextAsset)bindingData;
        _trackBindings = ParseBinding(data);

        if (_trackBindings == null || _trackBindings.Count <= 0)
        {
            Debug.LogError("[ARFaceRetargeter.CreateTrackBinding] -> 创建绑定关系失败, target: " + target.name);
            return false;
        }

        return true;
    }
}
