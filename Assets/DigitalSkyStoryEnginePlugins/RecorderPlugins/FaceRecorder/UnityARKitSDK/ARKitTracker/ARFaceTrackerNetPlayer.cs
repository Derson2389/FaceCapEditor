using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.XR.iOS.Utils;
using UnityEngine.XR.iOS;
using DigitalSky.Network;
using DigitalSky.Tracker;

public class ARFaceTrackerNetPlayer : MonoBehaviour
{
    public NetClient netClient;

    private ITracker _tracker;
    public TrackRetargeter[] _retargeters;
    public UnityARVideo unityARVideo;

    private RenderTexture _arCameraRT = null;
    private Texture2D _cameraTexture = null;
    public bool syncRT = false;

    private GUIStyle _centeredStyle;
    private string _editIp = "";
    private int _editPort = 0;
    private bool _enablePreview = false;

    void Awake()
    {
        if (netClient != null)
        {
            netClient.onConnect += OnConnect;
            netClient.onDisConnect += OnDisConnect;
        }
    }

    // Use this for initialization  
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        _tracker = ARFaceTracker.instance;
        _tracker.Init();

        if (!_tracker.Open())
        {
            return;
        }

        if (_retargeters != null)
        {
            for (int i = 0; i < _retargeters.Length; i++)
            {
                _tracker.AddListener(_retargeters[i]);
            }
        }

        _tracker.EnableTracking(true);

        Resolution resolution = Screen.currentResolution;
        // 将截取的屏幕图像缩放一半
        if (_arCameraRT == null)
        {
            _arCameraRT = new RenderTexture(resolution.width / 2, resolution.height / 2, 16, RenderTextureFormat.ARGB32);
            _arCameraRT.Create();
        }

        if (unityARVideo != null)
        {
            Camera cam = unityARVideo.GetComponent<Camera>();
            cam.targetTexture = _arCameraRT;
            _enablePreview = true;
        }

        if (netClient != null)
        {
            _editIp = netClient.ip;
            _editPort = netClient.port;
        }        
    }

    // Update is called once per frame
    void Update()
    {
        _tracker.OnUpdate();

        if(_arCameraRT != null)
            ToTexture2D(_arCameraRT);
    }

    void OnDestroy()
    {
        _tracker.OnDestroy();
        _arCameraRT.Release();

        UnityARSessionNativeInterface.ARFaceAnchorAddedEvent -= FaceAdded;
        UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent -= FaceUpdated;

        if (netClient != null)
        {
            netClient.onConnect -= OnConnect;
            netClient.onDisConnect -= OnDisConnect;
        }
    } 

    void OnGUI()
    {
        if (_centeredStyle == null)
        {
            _centeredStyle = GUI.skin.GetStyle("Box");
            _centeredStyle.alignment = TextAnchor.MiddleCenter;
        }

        Color color = GUI.color;
        GUI.color = Color.red;
        switch (Application.internetReachability)
        {
            case NetworkReachability.NotReachable:
                //网络断开
                {
                    _centeredStyle.fontSize = 36;
                    GUI.Box(new Rect((Screen.width / 2) - 250, (Screen.height) - 420, 500, 200), "网络不可用", _centeredStyle);
                }
                break;

            case NetworkReachability.ReachableViaLocalAreaNetwork:
                //WIFI
                {
                    if (netClient == null || (netClient != null && !netClient.isConnected))
                    {
                        _centeredStyle.fontSize = 36;
                        GUI.Box(new Rect((Screen.width / 2) - 250, (Screen.height) - 420, 500, 200), "等待连接Unity编辑器...", _centeredStyle);

                        GUI.color = Color.green;
                        _centeredStyle.fontSize = 20;
                        _editIp = GUI.TextField(new Rect(30, (Screen.height) - 120, 200, 80), _editIp, _centeredStyle);
                        _editPort = int.Parse(GUI.TextField(new Rect(240, (Screen.height) - 120, 100, 80), _editPort.ToString(), _centeredStyle));

                        if (GUI.Button(new Rect(370, (Screen.height) - 120, 80, 80), "切换", _centeredStyle))
                        {
                            if (netClient != null)
                            {
                                netClient.ip = _editIp;
                                netClient.port = _editPort;
                            }                         
                        }
                    }
                }
                break;

            case NetworkReachability.ReachableViaCarrierDataNetwork:
                //4G/3G
                break;
        }

        GUI.color = color;
        if (unityARVideo == null)
            return;

        GUI.color = Color.green;
        if (_enablePreview)
        {
            if (GUI.Button(new Rect(Screen.width - 120, (Screen.height) - 120, 80, 80), "关闭", _centeredStyle))
            {
                unityARVideo.enabled = false;
                _enablePreview = false;
            }
        }else
        {
            if (GUI.Button(new Rect(Screen.width - 120, (Screen.height) - 120, 80, 80), "打开", _centeredStyle))
            {
                unityARVideo.enabled = true;
                _enablePreview = true;
            }
        }
        GUI.color = color;

        if (_enablePreview == true && _cameraTexture != null)
        {
            GUI.DrawTexture(new Rect(0, 10, _cameraTexture.width, _cameraTexture.height), _cameraTexture);
        }
        
        /*if(_enablePreview == true && _arCameraRT != null)
        {
            GUI.DrawTexture(new Rect(Screen.width / 2, 10, _arCameraRT.width, _arCameraRT.height), _arCameraRT);
        }*/        
    }

    void OnConnect()
    {
        Debug.Log("[ARFaceTrackerNetPlayer.OnConnect] client connected to server");
        UnityARSessionNativeInterface.ARFaceAnchorAddedEvent += FaceAdded;
        UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent += FaceUpdated;
    }

    void OnDisConnect()
    {
        Debug.Log("[ARFaceTrackerNetPlayer.OnDisConnect] client disconnected from server");
        UnityARSessionNativeInterface.ARFaceAnchorAddedEvent -= FaceAdded;
        UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent -= FaceUpdated;
    }

    void FaceAdded(ARFaceAnchor anchorData)
    {

    }

    void FaceUpdated(ARFaceAnchor anchorData)
    {
        NetMessage blendMessage = new NetMessage(NetMessageID.ARFaceBlendShapeMsgId);
        SerializableARFaceBlendShapes serARFaceBlendShape = anchorData.blendShapes;
        blendMessage.SetData(serARFaceBlendShape.NetSerializeToByteArray());
        if (netClient != null && netClient.isConnected)
        {
            netClient.EnQueueMessage(blendMessage);
        }

        if (!syncRT || _cameraTexture == null)
            return;

        NetMessage cameraMessage = new NetMessage(NetMessageID.ARCameraTextureMsgId);
        SerializableTextureData textureData = new SerializableTextureData();
        textureData.data = _cameraTexture.EncodeToJPG();
        textureData.width = _cameraTexture.width;
        textureData.height = _cameraTexture.height;
        cameraMessage.SetData(textureData.NetSerializeToByteArray());
        if (netClient != null && netClient.isConnected)
        {
            netClient.EnQueueMessage(cameraMessage);
        }
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
}
