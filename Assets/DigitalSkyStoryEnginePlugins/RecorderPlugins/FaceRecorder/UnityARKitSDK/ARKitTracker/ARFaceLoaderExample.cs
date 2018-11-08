using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARFaceLoaderExample : MonoBehaviour
{
    public string folderPath;
    public ARFaceMeshObject arfaceMeshObject;

    //UI界面逻辑
    private Texture2D _cameraTexture = null;
    private Image _previewImg;

    private Button _playBtn = null;
    private bool _isPlaying = false;

    private Button _loadBtn = null;
    private bool _isLoading = false;

    private GameObject _playPanel = null;
    private GameObject _savePanel = null;

    private Image _loadingProgress = null;
    private Image _loadingBg = null;

    // 用于计算Update的时间变量
    private float _renderTime = 0f;
    private float _frameTime = 0f;
    private int _frameIndex = 0;
    private int _frameCount = 0;
    private int _frameRate = 0;
    private List<ARFaceMeshData> _frameFaceDatas = new List<ARFaceMeshData>();

    // Use this for initialization
    void Start ()
    {
        Application.targetFrameRate = 30;

        InitUI();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (_isLoading)
            return;

        if (_isPlaying)
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
            arfaceMeshObject.transform.localPosition = arfaceData.localPosition;
            arfaceMeshObject.transform.localEulerAngles = arfaceData.localRotation;

            arfaceMeshObject.faceMesh.vertices = arfaceData.vertices.ToArray();
            arfaceMeshObject.faceMesh.triangles = arfaceData.triangles.ToArray();

            arfaceMeshObject.faceMesh.RecalculateBounds();
            arfaceMeshObject.faceMesh.RecalculateNormals();

            Texture2D previewTex = _frameFaceDatas[_frameIndex].texture;
            if (_previewImg != null)
                _previewImg.sprite = Sprite.Create(previewTex, new Rect(0, 0, previewTex.width, previewTex.height), Vector2.zero);

            _frameIndex++;
        }
    }

    void InitUI()
    {
        _playPanel = GameObject.Find("PlayPanel");

        GameObject previewImgObj = GameObject.Find("PreviewImage");
        if (previewImgObj != null)
        {
            _previewImg = previewImgObj.GetComponent<Image>();
        }

        GameObject playBtnObj = GameObject.Find("PlayBtn");
        if (playBtnObj != null)
        {
            _playBtn = playBtnObj.GetComponent<Button>();
            _playBtn.onClick.AddListener(OnUIPlayBtnClicked);
        }

        GameObject loadBtnObj = GameObject.Find("LoadBtn");
        if (loadBtnObj != null)
        {
            _loadBtn = loadBtnObj.GetComponent<Button>();
            _loadBtn.onClick.AddListener(OnUILoadBtnClicked);
        }

        GameObject loadingProgressObj = GameObject.Find("loadingProgress");
        if (loadingProgressObj != null)
        {
            _loadingProgress = loadingProgressObj.GetComponent<Image>();
            _loadingBg = _loadingProgress.transform.GetChild(0).GetComponent<Image>();
        }

        _savePanel = GameObject.Find("SavePanel");
        _savePanel.SetActive(false);
    }

    void OnUIPlayBtnClicked()
    {
        if (!_isPlaying)
        {
            StartPlay();
        }
        else
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

        _loadBtn.enabled = false;

        Text text = _playBtn.gameObject.GetComponentInChildren<Text>();
        if (text == null)
            return;

        text.text = "Stop";
    }

    void StopPlay()
    {
        _isPlaying = false;

        _loadBtn.enabled = true;
        //arTracker.session.Run();

        if(_frameFaceDatas.Count > 0)
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

    void OnUILoadBtnClicked()
    {
        if (folderPath == "")
            return;

        StartCoroutine(StartLoading());
    }

    void SetUIProgressBar(float ratio)
    {
        if (_loadingProgress == null || _loadingBg == null)
            return;

        float width = _loadingProgress.rectTransform.sizeDelta.x;
        float height = _loadingProgress.rectTransform.sizeDelta.y;
        _loadingBg.rectTransform.sizeDelta = new Vector2(width * ratio, height);
    }

    private IEnumerator StartLoading()
    {
        _isLoading = true;
        _savePanel.SetActive(true);

        SetUIProgressBar(0);
        LoadFrameData(folderPath);

        if (_frameCount == 0 || _frameRate == 0)
        {
            _isLoading = false;
            _savePanel.SetActive(false);
            yield break;
        }

        _frameFaceDatas = new List<ARFaceMeshData>();
        for (int i = 0; i < _frameCount; i++)
        {
            SetUIProgressBar((float)i / _frameCount);
            ARFaceMeshData arfaceData = LoadARFaceData(folderPath, i);
            _frameFaceDatas.Add(arfaceData);
            yield return null;
        }

        StopPlay();

        _isLoading = false;
        _savePanel.SetActive(false);
    }

    void LoadFrameData(string path)
    {
        // 录制帧信息
        string frameDataPath = GetFullPath(path) + "/" + "frameData.txt";

        if (!File.Exists(frameDataPath))
        {
            Debug.LogError("[ARFaceLoaderExample.LoadFrameData] -> " + frameDataPath + " is not exist.");
            return;
        }

        string[] lines = System.IO.File.ReadAllLines(frameDataPath);
        if (lines == null || lines.Length < 2)
        {
            Debug.LogError("[ARFaceLoaderExample.LoadFrameData] -> load frameData.txt error.");
            return;
        }

        _frameCount = int.Parse(lines[0]);
        _frameRate = int.Parse(lines[1]);
    }

    ARFaceMeshData LoadARFaceData(string path, int index)
    {
        ARFaceMeshData arfaceData = null;

        //二进制文件流信息
        string meshDataPath = GetFullPath(path) + "/" + "meshData_" + index + ".bin";
        if (!File.Exists(meshDataPath))
        {
            Debug.LogError("[ARFaceLoaderExample.LoadARFaceData] -> " + meshDataPath + " is not exist.");
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

            Debug.Log("[ARFaceLoaderExample.LoadARFaceData] -> vertices count: " + verticesCount + " triangles count: " + trianglesCount);
            br.Close();
        }
        catch (IOException e)
        {
            Debug.LogError(e.Message);
            return null;
        }

        string texturePath = GetFullPath(path) + "/" + "texture_" + index + ".jpg";
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

    string GetFullPath(string path)
    {
        string projectFolder = System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, ".."));
        string fullPath = projectFolder + "/" + path;

        return fullPath;
    }
}
