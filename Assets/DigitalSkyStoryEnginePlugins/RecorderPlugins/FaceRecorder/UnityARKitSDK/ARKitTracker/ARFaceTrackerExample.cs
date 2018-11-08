using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DigitalSky.Tracker;
using UnityEngine.XR.iOS;

public class ARFaceTrackerExample : MonoBehaviour
{
    public TrackRetargeter[] arRetargeters;
    public UnityARVideo arVideo;
    public ARFaceMeshObject arfaceMeshObject;
    public ARFaceTracker arTracker;
    public ARCameraTracker arCameraTracker;

    //UI界面逻辑
    private GameObject _playPanel = null;
    private Texture2D _cameraTexture = null;
    private Text _timerText = null;
    private Image _previewImg;

    private Button _recordBtn = null;
    private bool _isRecording = false;

    private Button _playBtn = null;
    private bool _isPlaying = false;

    private Button _saveBtn = null;
    private bool _isSaving = false;

    private Button _loadBtn = null;
    private bool _isLoading = false;

    private GameObject _loadPanel = null;
    private GameObject _loadItem = null;

    private GameObject _progressPanel = null;
    private Image _loadingProgress = null;
    private Image _loadingBg = null;


    // 用于计算Update的时间变量
    private float _renderTime = 0f;
    private float _frameTime = 0f;
    private int _frameIndex = 0;
    private int _frameRate = 30;
    private int _frameCount = 0;
    private List<ARFaceMeshData> _frameFaceDatas = new List<ARFaceMeshData>();

    private string _saveFolderPrefix = "FaceCapture_";
    private string _saveFolderName = "";

    void Awake()
    {
        
    }

    // Use this for initialization
    void Start ()
    {
        Application.targetFrameRate = 30;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        InitUI();

        arTracker = ARFaceTracker.instance;
        arTracker.Init();

        if (arRetargeters != null)
        {
            for (int i = 0; i < arRetargeters.Length; i++)
            {
                arTracker.AddListener(arRetargeters[i]);
            }
        }

        if (!arTracker.Open())
        {
            return;
        }
        arTracker.EnableTracking(true);

        if (arfaceMeshObject != null)
            arfaceMeshObject.InitAREvent();
    }

    void InitUI()
    {
        Resolution resolution = Screen.currentResolution;

        if (arVideo != null)
        {
            // 将截取的屏幕图像缩放一半
            RenderTexture rt = new RenderTexture(resolution.width / 2, resolution.height / 2, 16, RenderTextureFormat.ARGB32);
            rt.Create();

            Camera cam = arVideo.GetComponent<Camera>();
            cam.targetTexture = rt;
        }

        _playPanel = GameObject.Find("PlayPanel");
        
        GameObject previewImgObj = GameObject.Find("PreviewImage");
        if(previewImgObj != null)
        {
            _previewImg = previewImgObj.GetComponent<Image>();
        }

        GameObject timerObj = GameObject.Find("Timer");
        if(timerObj != null)
        {
            _timerText = timerObj.GetComponent<Text>();
            //_timerText.gameObject.SetActive(false);
        }

        GameObject recordBtnObj = GameObject.Find("RecordBtn");
        if(recordBtnObj != null)
        {
            _recordBtn = recordBtnObj.GetComponent<Button>();
            _recordBtn.onClick.AddListener(OnUIRecordBtnClicked);
        }

        GameObject playBtnObj = GameObject.Find("PlayBtn");
        if (playBtnObj != null)
        {
            _playBtn = playBtnObj.GetComponent<Button>();
            _playBtn.onClick.AddListener(OnUIPlayBtnClicked);
        }

        GameObject saveBtnObj = GameObject.Find("SaveBtn");
        if (saveBtnObj != null)
        {
            _saveBtn = saveBtnObj.GetComponent<Button>();
            _saveBtn.onClick.AddListener(OnUISaveBtnClicked);
        }

        GameObject loadBtnObj = GameObject.Find("LoadBtn");
        if(loadBtnObj != null)
        {
            _loadBtn = loadBtnObj.GetComponent<Button>();
            _loadBtn.onClick.AddListener(OnUILoadBtnClicked);
        }
        
        _loadItem = GameObject.Find("loadItem");
        _loadItem.SetActive(false);

        GameObject loadBackBtn = GameObject.Find("LoadBackBtn");
        if (loadBackBtn != null)
        {
            loadBackBtn.GetComponent<Button>().onClick.AddListener(() => {
                _loadPanel.SetActive(false);
            });
        }

        _loadPanel = GameObject.Find("LoadPanel");
        _loadPanel.SetActive(false);

        GameObject loadingProgressObj = GameObject.Find("loadingProgress");
        if (loadingProgressObj != null)
        {
            _loadingProgress = loadingProgressObj.GetComponent<Image>();
            _loadingBg = _loadingProgress.transform.GetChild(0).GetComponent<Image>();
        }

        _progressPanel = GameObject.Find("ProgressPanel");
        _progressPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isSaving)
            return;

        if (!_isPlaying)
        {
            arTracker.OnUpdate();
            arfaceMeshObject.OnUpdate();

            if (arVideo != null)
                ToTexture2D(arVideo.GetComponent<Camera>().targetTexture);
        }

        if (_timerText != null)
            _timerText.text = GetTimeSpendString(_renderTime);

        Texture2D previewTex = _cameraTexture;

        if (_isRecording)
        {
            // 更新时间, _frameTime不是第0帧, update未超过一帧时间
            _renderTime += Time.deltaTime;
            if ((_renderTime < _frameTime + (1f / _frameRate)) && _frameTime != 0)
            {
                return;
            }
            _frameTime = _renderTime == 0 ? 0 : _frameTime + (1f / _frameRate);

            ARFaceMeshData arfaceData = new ARFaceMeshData();
            arfaceData.localPosition = arfaceMeshObject.transform.localPosition;
            arfaceData.localRotation = arfaceMeshObject.transform.localEulerAngles;

            arfaceData.vertices = new List<Vector3>(arfaceMeshObject.faceMesh.vertices);
            arfaceData.triangles = new List<int>(arfaceMeshObject.faceMesh.triangles);

            arfaceData.index = _frameFaceDatas.Count;

            arfaceData.texture = TextureScale.BilinearNew(previewTex, previewTex.width / 2, previewTex.height / 2);
            previewTex = arfaceData.texture;

            _frameFaceDatas.Add(arfaceData);
            _frameIndex++;
        }
        else if(_isPlaying)
        {
            if (_frameIndex >= _frameFaceDatas.Count)
            {
                StopPlay();
                return;
            }

            // 更新时间, _frameTime不是第0帧, update未超过一帧时间
            _renderTime += Time.deltaTime;
            if ((_renderTime < _frameTime + (1f / _frameRate)) && _frameTime != 0)
            {
                return;
            }
            _frameTime = _renderTime == 0 ? 0 : _frameTime + (1f / _frameRate);

            ARFaceMeshData arfaceData = _frameFaceDatas[_frameIndex];
            //arfaceMeshObject.transform.localPosition = arfaceData.localPosition;
            //arfaceMeshObject.transform.localEulerAngles = arfaceData.localRotation;

            arfaceMeshObject.faceMesh.vertices = arfaceData.vertices.ToArray();
            arfaceMeshObject.faceMesh.triangles = arfaceData.triangles.ToArray();

            arfaceMeshObject.faceMesh.RecalculateBounds();
            arfaceMeshObject.faceMesh.RecalculateNormals();

            previewTex = _frameFaceDatas[_frameIndex].texture;

            _frameIndex++;
        }

        if (_previewImg != null)
            _previewImg.sprite = Sprite.Create(previewTex, new Rect(0, 0, previewTex.width, previewTex.height), Vector2.zero);
    }

    void OnDestroy()
    {
        arTracker.OnDestroy();

        if (arVideo != null)
        {
            Camera cam = arVideo.GetComponent<Camera>();

            cam.targetTexture.Release();
            cam.targetTexture = null;
        }           
    }

    void OnGUI()
    {
#if !UNITY_EDITOR
        if (_cameraTexture != null)
        {
            //GUI.DrawTexture(new Rect(0, 10, _cameraTexture.width, _cameraTexture.height), _cameraTexture);
        }
#endif
    }

    void ToTexture2D(RenderTexture rTex)
    {
        // 只显示截取图像高度的0.7
        int texWidth = rTex.width;
        int texHeight = (int)(rTex.height * 0.5);

        if (_cameraTexture == null)
        {
            _cameraTexture = new Texture2D(texWidth, texHeight, TextureFormat.RGB24, false);
        }

        RenderTexture.active = rTex;
        _cameraTexture.ReadPixels(new Rect(0, rTex.height * 0.3f, texWidth, texHeight), 0, 0);
        _cameraTexture.Apply();
    }

    void OnUIRecordBtnClicked()
    {
        if(!_isRecording)
        {
            StartRecord();
        }
        else
        {
            StopRecord();
        }
    }

    void StartRecord()
    {
        _isRecording = true;

        _renderTime = 0;
        _frameTime = 0;
        _frameIndex = 0;

        for (int i = 0; i < _frameFaceDatas.Count; i++)
            GameObject.DestroyImmediate(_frameFaceDatas[i].texture);
        _frameFaceDatas.Clear();

        _playBtn.enabled = false;
        _saveBtn.enabled = false;

        Text text = _recordBtn.gameObject.GetComponentInChildren<Text>();
        if (text == null)
            return;

        text.text = "Stop";
    }

    void StopRecord()
    {
        _isRecording = false;

        _playBtn.enabled = true;
        _saveBtn.enabled = true;

        Text text = _recordBtn.gameObject.GetComponentInChildren<Text>();
        if (text == null)
            return;

        text.text = "Record";
    }

    void OnUIPlayBtnClicked()
    {
        if(!_isPlaying)
        {
            StartPlay();
        }else
        {
            StopPlay();
        }
    }

    void StartPlay()
    {
        _isPlaying = true;
        _renderTime = 0;
        _frameTime = 0;
        _frameIndex = 0;

        _recordBtn.enabled = false;
        _saveBtn.enabled = false;
        arCameraTracker.enabled = false;
        //arTracker.session.Pause();

        Text text = _playBtn.gameObject.GetComponentInChildren<Text>();
        if (text == null)
            return;

        text.text = "Stop";
    }

    void StopPlay()
    {
        _isPlaying = false;

        _recordBtn.enabled = true;
        _saveBtn.enabled = true;
        arCameraTracker.enabled = true;
        //arTracker.session.Run();

        if (_frameFaceDatas.Count > 0)
        {
            arfaceMeshObject.transform.localPosition = _frameFaceDatas[0].localPosition;
            arfaceMeshObject.transform.localEulerAngles = _frameFaceDatas[0].localRotation;

            arfaceMeshObject.faceMesh.vertices = _frameFaceDatas[0].vertices.ToArray();
            arfaceMeshObject.faceMesh.triangles = _frameFaceDatas[0].triangles.ToArray();

            arfaceMeshObject.faceMesh.RecalculateBounds();
            arfaceMeshObject.faceMesh.RecalculateNormals();

            Texture2D previewTex = _frameFaceDatas[0].texture;
            if (_previewImg != null)
                _previewImg.sprite = Sprite.Create(previewTex, new Rect(0, 0, previewTex.width, previewTex.height), Vector2.zero);
        }

        Text text = _playBtn.gameObject.GetComponentInChildren<Text>();
        if (text == null)
            return;

        text.text = "Play";
    }

    void OnUISaveBtnClicked()
    {
        if (_frameFaceDatas.Count == 0)
            return;

        StartCoroutine(StartSaveing());
        //StartCoroutine(StartLoading());
    }

    /*private IEnumerator StartLoading()
    {
        _isSaving = true;
        _progressPanel.SetActive(true);
        arCameraTracker.enabled = false;

        SetUIProgressBar(0);

        for(int i = 0; i < 100; i++)
        {
            SetUIProgressBar((float)i / 100);
            yield return new WaitForSeconds(0.1f);
        }

        _isSaving = false;
        _progressPanel.SetActive(false);
        arCameraTracker.enabled = true;
    }*/

    private IEnumerator StartSaveing()
    {
        _isSaving = true;
        _progressPanel.SetActive(true);
        arCameraTracker.enabled = false;

        SetUIProgressBar(0);

        _saveFolderName = _saveFolderPrefix + GetTimeStampedString();
        if (!Directory.Exists(Application.persistentDataPath + "/" + _saveFolderName))            //如果不存在就创建file文件夹　　             　　                
            Directory.CreateDirectory(Application.persistentDataPath + "/" + _saveFolderName);    //创建该文件夹

        yield return null;

        for (int i = 0; i < _frameFaceDatas.Count; i++)
        {
            SaveARFaceData(_frameFaceDatas[i]);
            SetUIProgressBar((float)i / _frameFaceDatas.Count);

            yield return null;
        }

        // 录制帧信息
        string frameDataPath = Application.persistentDataPath + "/" + _saveFolderName + "/" + "frameData.txt";
        string[] lines = new string[2] { _frameFaceDatas.Count.ToString(), _frameRate.ToString(),};
        File.WriteAllLines(frameDataPath, lines);

        _isSaving = false;
        _progressPanel.SetActive(false);
        arCameraTracker.enabled = true;
    }

    void SetUIProgressBar(float ratio)
    {
        if (_loadingProgress == null || _loadingBg == null)
            return;

        float width = _loadingProgress.rectTransform.sizeDelta.x;
        float height = _loadingProgress.rectTransform.sizeDelta.y;
        _loadingBg.rectTransform.sizeDelta = new Vector2(width * ratio, height);
    }

    void SaveARFaceData(ARFaceMeshData faceData)
    {
        // save frame texture to JPG
        string texturePath = Application.persistentDataPath + "/" + _saveFolderName + "/" + "texture_" + faceData.index + ".jpg";
        System.IO.File.WriteAllBytes(texturePath, faceData.texture.EncodeToJPG());

        //二进制文件流信息
        string meshDataPath = Application.persistentDataPath + "/" + _saveFolderName + "/" + "meshData_" + faceData.index + ".bin";
        BinaryWriter bw = null;
        try
        {
            bw = new BinaryWriter(new FileStream(meshDataPath, FileMode.OpenOrCreate));
        }
        catch (IOException e)
        {
            Debug.LogError(e.Message);
        }

        try
        {
            Debug.Log("[ARFaceTrackerExample.SaveARFaceData] -> vertices count: " + faceData.vertices.Count + " triangles count: " + faceData.triangles.Count);

            bw.Write(faceData.vertices.Count);
            for(int i = 0; i < faceData.vertices.Count; i++)
            {
                bw.Write(faceData.vertices[i].x);
                bw.Write(faceData.vertices[i].y);
                bw.Write(faceData.vertices[i].z);
            }

            bw.Write(faceData.triangles.Count);
            for (int i = 0; i < faceData.triangles.Count; i++)
            {
                bw.Write(faceData.triangles[i]);
            }

            bw.Flush();
            bw.Close();
        }
        catch (IOException e)
        {
            Debug.LogError(e.Message);
        }
    }

    void OnUILoadBtnClicked()
    {
        List<string> targetFiles = new List<string>();

        DirectoryInfo diFliles = new DirectoryInfo(Application.persistentDataPath);
        DirectoryInfo[] dirArr = diFliles.GetDirectories();

        foreach (DirectoryInfo dir in dirArr)
        {
            try
            {
                if (!Directory.Exists(dir.FullName) || !dir.Name.Contains(_saveFolderPrefix))
                    continue;
            }
            catch
            {
                continue;
            }

            targetFiles.Add(dir.Name);
        }

        if (targetFiles.Count == 0)
            return;

        Transform listPanel = _loadItem.transform.parent;
        List<Transform> deleteItems = new List<Transform>();
        for (int i = 0; i < listPanel.childCount; i++)
        {
            Transform item = listPanel.GetChild(i);
            if (item.gameObject.activeSelf == true)
                deleteItems.Add(item);
        }

        for(int i = 0; i < deleteItems.Count; i++)
            GameObject.DestroyImmediate(deleteItems[i].gameObject);

        for (int i = 0; i < targetFiles.Count; i++)
        {
            GameObject cloneItem = GameObject.Instantiate(_loadItem);
            cloneItem.SetActive(true);
            cloneItem.name = cloneItem.name + i;
            //cloneItem.transform.parent = _loadItem.transform.parent;
            cloneItem.transform.SetParent(_loadItem.transform.parent, false);

            string folderPath = targetFiles[i];
            cloneItem.transform.Find("FileNameText").GetComponent<Text>().text = targetFiles[i];
            cloneItem.transform.Find("OpenBtn").GetComponent<Button>().onClick.AddListener(() =>
            {
                _saveFolderName = folderPath;
                StartCoroutine(StartLoading());
            });
        }

        /* test
        for (int i = 0; i < 20; i++)
        {
            GameObject cloneItem = GameObject.Instantiate(_loadItem);
            cloneItem.SetActive(true);
            cloneItem.name = cloneItem.name + i;
            //cloneItem.transform.parent = _loadItem.transform.parent;
            cloneItem.transform.SetParent(_loadItem.transform.parent, false);
        }*/

        _loadPanel.SetActive(true);
    }

    private IEnumerator StartLoading()
    {
        _isLoading = true;
        _progressPanel.SetActive(true);

        SetUIProgressBar(0);
        LoadFrameData(_saveFolderName);

        if (_frameCount == 0 || _frameRate == 0)
        {
            _isLoading = false;
            _progressPanel.SetActive(false);
            yield break;
        }

        for (int i = 0; i < _frameFaceDatas.Count; i++)
            GameObject.DestroyImmediate(_frameFaceDatas[i].texture);
        _frameFaceDatas.Clear();

        for (int i = 0; i < _frameCount; i++)
        {
            SetUIProgressBar((float)i / _frameCount);
            ARFaceMeshData arfaceData = LoadARFaceData(_saveFolderName, i);
            _frameFaceDatas.Add(arfaceData);
            yield return null;
        }

        StopPlay();

        _isLoading = false;
        _progressPanel.SetActive(false);
        _loadPanel.SetActive(false);
    }

    void LoadFrameData(string path)
    {
        // 录制帧信息
        string frameDataPath = Application.persistentDataPath + "/" + path + "/" + "frameData.txt";

        if (!File.Exists(frameDataPath))
        {
            Debug.LogError("[ARFaceTrackerExample.LoadFrameData] -> " + frameDataPath + " is not exist.");
            return;
        }

        string[] lines = System.IO.File.ReadAllLines(frameDataPath);
        if (lines == null || lines.Length < 2)
        {
            Debug.LogError("[ARFaceTrackerExample.LoadFrameData] -> load frameData.txt error.");
            return;
        }

        _frameCount = int.Parse(lines[0]);
        _frameRate = int.Parse(lines[1]);
    }

    ARFaceMeshData LoadARFaceData(string path, int index)
    {
        ARFaceMeshData arfaceData = null;

        //二进制文件流信息
        string meshDataPath = Application.persistentDataPath + "/" + path + "/" + "meshData_" + index + ".bin";
        if (!File.Exists(meshDataPath))
        {
            Debug.LogError("[ARFaceTrackerExample.LoadARFaceData] -> " + meshDataPath + " is not exist.");
            return null;
        }

        BinaryReader br = null;
        try
        {
            br = new BinaryReader(new FileStream(meshDataPath, FileMode.Open));
        }
        catch (IOException e)
        {
            Debug.LogError(e.Message);
            return null;
        }

        arfaceData = new ARFaceMeshData();
        try
        {
            int verticesCount = br.ReadInt32();
            for (int i = 0; i < verticesCount; i++)
            {
                float x = br.ReadSingle();
                float y = br.ReadSingle();
                float z = br.ReadSingle();

                arfaceData.vertices.Add(new Vector3(x, y, z));
            }

            int trianglesCount = br.ReadInt32();
            for (int i = 0; i < trianglesCount; i++)
            {
                int triangle = br.ReadInt32();
                arfaceData.triangles.Add(triangle);
            }

            Debug.Log("[ARFaceTrackerExample.LoadARFaceData] -> vertices count: " + verticesCount + " triangles count: " + trianglesCount);
            br.Close();
        }
        catch (IOException e)
        {
            Debug.LogError(e.Message);
            return null;
        }

        string texturePath = Application.persistentDataPath + "/" + path + "/" + "texture_" + index + ".jpg";
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(texturePath))
        {
            fileData = File.ReadAllBytes(texturePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        arfaceData.texture = tex;
        arfaceData.index = index;

        return arfaceData;
    }

    string GetTimeStampedString()
    {
        //TimeSpan span = (DateTime.Now - DateTime.Now.Date);
        return string.Format("{0}-{1}-{2}-{3}.{4}.{5}", DateTime.Now.Year, DateTime.Now.Month.ToString("D2"), DateTime.Now.Day.ToString("D2"), DateTime.Now.Hour.ToString(), DateTime.Now.Minute.ToString(), DateTime.Now.Second.ToString());
    }

    string GetTimeSpendString(float timeSpend)
    {
        int hour = (int)timeSpend / 3600;
        int minute = ((int)timeSpend - hour * 3600) / 60;
        int second = (int)timeSpend - hour * 3600 - minute * 60;
        int millisecond = (int)((timeSpend - (int)timeSpend) * 1000);

        return string.Format("{0:D2}:{1:D2}.{2:D3}", minute, second, millisecond);
    }
}
