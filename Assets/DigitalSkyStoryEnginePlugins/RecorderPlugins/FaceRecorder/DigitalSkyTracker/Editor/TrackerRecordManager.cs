using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using dxyz;
using DigitalSky.Recorder;
using DigitalSky.Network;
using Slate;
using System;
using System.IO;

namespace DigitalSky.Tracker
{
    public class TrackerRecordObject
    {        
        private TrackRetargeter _emotionRetargeter;
        public TrackRetargeter emotionRetargeter
        {
            get { return _emotionRetargeter; }
        }

        private ITracker _emotionTracker;
        public ITracker emotionTracker
        {
            get { return _emotionTracker; }
            set {
                _emotionTracker = value;
            }
        }

        private TrackRetargeter _viconRetargeter;
        public TrackRetargeter viconRetargeter
        {
            get { return _viconRetargeter; }
        }

        private ITracker _viconTracker;
        public ITracker viconTracker
        {
            get { return _viconTracker; }
            set { _viconTracker = value; }
        }

        private TrackRecorder _recorder;
        public TrackRecorder recorder
        {
            get { return _recorder; }
        }

        private GameObject _target;
        public GameObject target
        {
            get { return _target; }
        }

        private GameObject _targetSource;
        public GameObject targetSource
        {
            get { return _targetSource; }
        }

        public TextAsset data = null;
        public bool isEditing = false;
        public bool isNeedRecord = true;

        public TrackerRecordObject(GameObject obj, bool _isNeedRecord)
        {
            _targetSource = obj;
            _emotionRetargeter = null;
            _emotionTracker = null;
            _recorder = null;
            isNeedRecord = _isNeedRecord;
        }

        public void StartRecord()
        {
            if (_emotionRetargeter == null)
                return;


            //_recorder = _emotionRetargeter.gameObject.GetComponent<BlendTrackRecorder>();
            //if(_recorder == null)
            //    _recorder = _emotionRetargeter.gameObject.AddComponent<BlendTrackRecorder>();

            _recorder = _emotionRetargeter.gameObject.GetAddComponent<BoneTrackRecorder>();
            if(_recorder == null)
                _recorder = _emotionRetargeter.gameObject.AddComponent<BoneTrackRecorder>();

            _recorder.Init(_emotionRetargeter);
            _recorder.StartRecord();
        }

        public void StopRecord()
        {
            if (recorder == null)
                return;

            recorder.StopRecord();
        }

        public void EnableTracking()
        {
            //_target = GameObject.Instantiate(_targetSource);
            _target = SlateRecorderCache.Instance.CreateClone(_targetSource);
            if (_target == null)
                return;

            //_target.transform.position = _targetSource.transform.position;
            //_target.transform.rotation = _targetSource.transform.rotation;
            //_target.name = _targetSource.name;

            ///EnablePreviewRecursive(_targetSource, false);
            EnableRendererRecursive(_targetSource, false);

            // 如果tracker不为空并且tracker已初始化
            if (_emotionTracker != null && _emotionTracker.isInit)
            {
                _emotionRetargeter = _target.gameObject.GetComponent<TrackRetargeter>();
                if (_emotionRetargeter != null)
                {
                    _emotionTracker.AddListener(_emotionRetargeter);
                    _emotionTracker.EnableTracking(true);
                    return;
                }

                if (_emotionTracker is PrevizTracker)
                {
                    if (data)
                    {
                        _emotionRetargeter = _target.gameObject.AddComponent<PrevizTrackRetargeter>();
                        var previzRt = _emotionRetargeter as PrevizTrackRetargeter;
                        if(previzRt!= null)
                            previzRt.controllerConfiguration = data;

                    }
                }
                else if (_emotionTracker is ARRemoteFaceTracker)
                {
                    _emotionRetargeter = _target.gameObject.AddComponent<ARFaceTrackRetargeter>();
                    if (data)
                    {
                        ARFaceTrackRetargeter arRetargeter = (ARFaceTrackRetargeter)_emotionRetargeter;
                        arRetargeter.bindingConfiguration = data;
                    }
                }
                else if (_emotionTracker is ARFaceTrackerNet)
                {
                    _emotionRetargeter = _target.gameObject.AddComponent<ARFaceTrackRetargeter>();
                    if (data)
                    {
                        ARFaceTrackRetargeter arRetargeter = (ARFaceTrackRetargeter)_emotionRetargeter;
                        arRetargeter.bindingConfiguration = data;
                    }
                }

                // 将被录制对象添加到tracker监听列表
                _emotionTracker.AddListener(_emotionRetargeter);
                _emotionTracker.EnableTracking(true);
            }            
        }

        public void DisableTracking()
        {
            //GameObject.DestroyImmediate(_target);
            // 将被录制对象从tracker监听列表移除
            if (_emotionTracker != null)
            {
                _emotionTracker.RemoveListener(_emotionRetargeter);
                _emotionTracker.EnableTracking(false);
            }

            SlateRecorderCache.Instance.DestoryFromClone(_target);
            _target = null;
            _emotionRetargeter = null;

            ///EnablePreviewRecursive(_targetSource, true);
            EnableRendererRecursive(_targetSource, true);
        }

        //private static void EnablePreviewRecursive(GameObject go, bool enable)
        //{
        //    if(enable == false)
        //        go.hideFlags = HideFlags.HideInHierarchy;
        //    else
        //        go.hideFlags = HideFlags.None;

        //    //go.layer = 0;

        //    foreach (Transform transform in go.transform)
        //    {
        //        EnablePreviewRecursive(transform.gameObject, enable);
        //    }
        //}

        private static void EnableRendererRecursive(GameObject go, bool enabled)
        {
            Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                Renderer renderer = componentsInChildren[i];
                renderer.enabled = enabled;
            }
        }
    }

    public class TrackerRecordManager : RecorderManager
    {

        // ARFaceTracker服务器事件管理对象
        private ARFaceTrackerNetEventManager _trackerNetEventManager = null;

        public TrackerRecordManager()
        {
            EditorApplication.update -= OnNetUpdate;
            EditorApplication.update += OnNetUpdate;
        }

        public new static TrackerRecordManager Instance
        {
            get
            {
                var instance = Slate.SlateExtensions.Instance.RecordUtility.GetRecorderManager(typeof(TrackerRecordManager));
                if (instance != null)
                {
                    return (TrackerRecordManager)instance; 
                }
                else
                {
                    return Singleton<TrackerRecordManager>.Instance;
                }        
            }
        }

        // 当前录制对象列表
        private List<TrackerRecordObject> _recordObjectList = new List<TrackerRecordObject>();
        public List<TrackerRecordObject> recordObjectList
        {
            get
            {
                return _recordObjectList;
            }
        }

        // 当前tracker设备
        private List<ITracker> _trackers = new List<ITracker>();
       
        public List<ITracker> trackers
        {
            get { return _trackers; }
        }

        public override void SetRecordSavePath(string value)
        {
            m_recordSavePath = value;
            SaveConfig();
        }

        public override string GetRecordSavePath()
        {
            return m_recordSavePath;
        }
        

        public override string InitSavePath()
        {
            return "RecordFaces/";
        }

        public override void Destroy()
        {
            if (TrackerRecordManager.Instance != null)
            {
                TrackerRecordManager.Instance.OnDestroy();                
            }

            PrevizTracker.Destroy();
            NetServer.Destroy();
        }

        public new void OnDestroy()
        {
            _recordObjectList.Clear();

            for (int i = 0; i < trackers.Count; i++)
                trackers[i].OnDestroy();
            trackers.Clear();
        }

        public override void LoadConfig()
        {
            SetRecordSavePath( PlayerPrefs.GetString("TrackerRecordManager_savePath", "RecordFaces/"));
        }

        public override void SaveConfig()
        {
            PlayerPrefs.SetString("TrackerRecordManager_savePath", GetRecordSavePath());
        }

        public override void Init()
        {
            _recordObjectList = new List<TrackerRecordObject>();
            _trackers = new List<ITracker>();
            LoadConfig();
            AddTracker(PrevizTracker.instance);

            _trackerNetEventManager = new ARFaceTrackerNetEventManager();
            NetServer.instance.Init();
            NetServer.instance.AddEventManager(_trackerNetEventManager);
        }

        public override void Clear()
        {
            _recordObjectList.Clear(); 
        }

        public override void AddObject(GameObject obj, bool needRecord, GameObject hipsObj = null)
        {
            TrackerRecordObject objInfo = new TrackerRecordObject(obj, needRecord);
            recordObjectList.Add(objInfo);
        }

        public override void RemoveObject(object obj)
        {
            recordObjectList.Remove((TrackerRecordObject)obj);
        }

        public TrackerRecordObject FindObject(GameObject obj)
        {
            for (int i = 0; i < recordObjectList.Count; i++)
            {
                if (recordObjectList[i].targetSource == obj)
                    return recordObjectList[i];
            }

            return null;
        }

        public override void SetObjRecordEnalbe(GameObject obj, bool value)
        {
            var recordTrackerInfo = TrackerRecordManager.Instance.FindObject(obj);
            recordTrackerInfo.isNeedRecord = value;

        }

        public override void OnUpdate()
        {
            for (int i = recordObjectList.Count - 1; i >= 0; i--)
            {
                if (recordObjectList[i].targetSource == null || !recordObjectList[i].targetSource.activeSelf)
                    recordObjectList.RemoveAt(i);
            }

            for (int i = _trackers.Count - 1; i >= 0; i--)
            {
                if (_trackers[i] == null || !_trackers[i].isInit)
                    _trackers.RemoveAt(i);
            }
        }

        public void AddTracker(ITracker tracker)
        {
            if (tracker == null || _trackers.Contains(tracker))
                return;

            if (!tracker.Init())
                return;

            _trackers.Add(tracker);
        }

        public int FindTracker(ITracker tracker)
        {
            if (tracker == null)
                return -1;

            for (int i = 0; i < _trackers.Count; i++)
            {
                if (tracker == _trackers[i])
                    return i;
            }

            return -1;
        }

        public static void SaveAnimationClip(AnimationClip clip, string path)
        {
            if (AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip)) != null)
                AssetDatabase.DeleteAsset(path);

            AssetDatabase.CreateAsset(clip, path);
            AssetDatabase.SaveAssets();
        }

        public override void RenderGeneraConifg(Color color, Color _buttonColor)
        {
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Previs连接地址: ", GUILayout.Width(90.0f));

            string previsDefaultAddress = PrevizTracker.instance.IPAddress + ":" + PrevizTracker.instance.Port;
            string previsAddress = GUILayout.TextField(previsDefaultAddress);
            if (previsAddress != previsDefaultAddress)
            {
                string[] addrs = previsAddress.Split(':');
                if (addrs.Length == 2)
                {
                    PrevizTracker.instance.IPAddress = addrs[0];
                    PrevizTracker.instance.Port = int.Parse(addrs[1]);
                }
            }

            GUI.backgroundColor = _buttonColor;
            if (PrevizTracker.instance.trackerActive)
            {
                if (GUILayout.Button("关闭", GUILayout.Width(60.0f)))
                    PrevizTracker.instance.Close();
            }
            else
            {
                if (GUILayout.Button("连接", GUILayout.Width(60.0f)))
                    PrevizTracker.instance.Open();
            }

            if (GUILayout.Button("重置", GUILayout.Width(50.0f)))
                PrevizTracker.instance.Init();

            GUI.backgroundColor = color;

            GUILayout.Space(2);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("iphoneX服务器:  ");

            if (!TrackerRecordManager.Instance.IsConnected())
            {
                GUILayout.Label("未启动");

                GUI.backgroundColor = _buttonColor;
                if (GUILayout.Button("启动", GUILayout.Width(50.0f)))
                    TrackerRecordManager.Instance.StartConnect();
                GUI.backgroundColor = color;
            }
            else
            {
                GUILayout.Label("已启动");

                GUI.backgroundColor = _buttonColor;
                if (GUILayout.Button("关闭", GUILayout.Width(50.0f)))
                {
                    TrackerRecordManager.Instance.trackers.Clear();
                    TrackerRecordManager.Instance.AddTracker(PrevizTracker.instance);
                    TrackerRecordManager.Instance.StopConnect();
                }
                GUI.backgroundColor = color;
            }

            GUILayout.EndHorizontal();

            if (_trackerNetEventManager != null && TrackerRecordManager.Instance.IsConnected())
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("已连接到服务器设备: ");
                GUILayout.EndHorizontal();

                foreach (var item in _trackerNetEventManager.remoteTrackers)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("设备ID: " + item.Value.trackerName, GUILayout.Width(250.0f));
                    GUILayout.EndHorizontal();

                    TrackerRecordManager.Instance.AddTracker(item.Value);
                }
            }

            GUILayout.Space(5);

            GUI.backgroundColor = color;
        }


        public override bool IsConnected()
        {
            return NetServer.instance.isListen;
        }

        public override void StartConnect()
        {
            NetServer.instance.StartListen();
        }

        public override void StopConnect()
        {
            NetServer.instance.StopListen();
        }

        public override void StartRecord(float startTime)
        {
            foreach (var obj in TrackerRecordManager.Instance.recordObjectList)
            {
                if (obj.emotionTracker == null)
                    continue;
                if (obj.isNeedRecord == false)
                    continue;

                obj.StartRecord();
            }
        }

        public override void Record(float currentTime, float totalTime)
        {
            //Debug.Log("Recording");
            foreach (var obj in TrackerRecordManager.Instance.recordObjectList)
            {
                if (obj.recorder == null)
                    continue;
                if(obj.isNeedRecord == false)
                    continue;

                obj.recorder.Record(currentTime);
            }
        }


        public override void StopRecord(Cutscene cutscene, float startTime, float endTime, int? recordIdx = null)
        {
            foreach (var obj in TrackerRecordManager.Instance.recordObjectList)
            {
                if (obj.recorder == null)
                    continue;
                if(obj.isNeedRecord == false)
                    continue;

                obj.recorder.StopRecord();

                Slate.CutsceneGroup group = null;
                group = cutscene.groups.Find(g => g.actor == obj.targetSource);
                if (group == null)
                {
                    group = cutscene.AddGroup<Slate.ActorGroup>(obj.targetSource);
                    group.name = obj.targetSource.name;
                }

                Slate.AnimationTrack animationTrack = null/*group.tracks.Find(t => t is Slate.AnimationTrack && t.name == "Mopher") as Slate.AnimationTrack*/;
                Animation animComponent = obj.targetSource.GetComponent<Animation>();
                if (animComponent == null)
                    animComponent = obj.targetSource.AddComponent<Animation>();

                if (animationTrack == null)
                {
                    animationTrack = group.AddTrack<Slate.AnimationTrack>();
                    animationTrack.name = "Mopher " + DateTime.Now.ToString("MMddHHmmssfff");
                    animationTrack.RecordIdx = recordIdx;
                }

                AnimationClip clip = obj.recorder.CreateAnimationClip();

                string fileName = DateTime.Now.ToString("MMddHHmmssfff");
                string dirPath = "Assets/" + TrackerRecordManager.Instance.GetRecordSavePath() + obj.targetSource.name;
                // create Directory if not exist
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                string path = dirPath + "/" + "face_" + fileName + ".anim";
                TrackerRecordManager.SaveAnimationClip(clip, path);

                Slate.ActionClips.PlayAnimationClip playAnimationClip = animationTrack.AddAction<Slate.ActionClips.PlayAnimationClip>(startTime);
                playAnimationClip.animationClip = clip;
                playAnimationClip.length = playAnimationClip.animationClip.length / playAnimationClip.playbackSpeed;
            }
        }


        public override void  RenderConfigObjectItem(ConfigComponent config, GameObject selectobj, Rect rc)
        {
            TrackerRecordObject obj = TrackerRecordManager.Instance.FindObject(selectobj);
            if (obj == null)
                return ;
            if (config == null)
                return ;
            var actorConfig = config.GetActorConfigByObj(selectobj);

            obj.data = actorConfig.EmotionData;
            //GUILayout.BeginArea(rc);
            //GUILayout.BeginVertical();

            GUILayout.Space(10.0f);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Emotion Setting", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("配置数据：", GUILayout.Width(60));
            actorConfig.EmotionData = (TextAsset)EditorGUILayout.ObjectField(actorConfig.EmotionData, typeof(TextAsset), true);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            List<string> trackerNames = new List<string>();
            trackerNames.Add("无设备");
            for (int i = 0; i < TrackerRecordManager.Instance.trackers.Count; i++)
                trackerNames.Add(TrackerRecordManager.Instance.trackers[i].trackerName);

            int selectedIndex = TrackerRecordManager.Instance.FindTracker(obj.emotionTracker);
            if (selectedIndex == -1)
            {
                selectedIndex = 0;
            }
            selectedIndex = actorConfig.EmotionSelectedIndex;

            GUILayout.Label("捕捉设备：", GUILayout.Width(60));
            int select = EditorGUILayout.Popup(selectedIndex, trackerNames.ToArray());
            if (selectedIndex != 0 && obj.emotionTracker == null && select == selectedIndex && selectedIndex <= TrackerRecordManager.Instance.trackers.Count)
            {

                var traker = TrackerRecordManager.Instance.trackers[selectedIndex - 1];
                var ARTraker = traker as ARFaceTrackerNet;
                if (ARTraker == null)
                {
                    obj.emotionTracker = traker;
                }
                else
                {
                    if (string.IsNullOrEmpty(ARTraker.PlayerId))
                        obj.emotionTracker = null;
                    else
                        obj.emotionTracker = ARTraker;
                }

                
            }
            
            if (select != selectedIndex)
            {
                actorConfig.EmotionSelectedIndex = select;
                config.RefreshActorConfig(selectobj, actorConfig);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(config.gameObject.scene);

                if (select > TrackerRecordManager.Instance.trackers.Count || select == 0)
                    obj.emotionTracker = null;
                else
                {
                    var traker = TrackerRecordManager.Instance.trackers[select - 1];
                    var ARTraker = traker as ARFaceTrackerNet;
                    if (ARTraker == null)
                    {
                        obj.emotionTracker = traker;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(ARTraker.PlayerId))
                            obj.emotionTracker = null;
                        else
                            obj.emotionTracker = ARTraker;
                    }
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(20.0f);
            GUILayout.Label("___________________________________");

            //GUILayout.EndVertical();
            //GUILayout.EndArea();
        }




        public override bool CanRecord()
        {
            return true;
        }

        public override void OnNetUpdate()
        {
            NetServer.instance.OnUpdate();
        }

    }
}
