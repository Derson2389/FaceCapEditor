using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalSky.Tracker;
using dxyz;

public class DynamixyzExample : MonoBehaviour 
{
	[Header("Buttons")]
	public Texture2D imageSwitchCam;
	public Texture2D imageStartTracking;
	public Texture2D imageStopTracking;
	private bool trackButton;
	private bool stopTrackButton;
	private bool normalButton;
	private bool switchCamButton;
	GUIContent contentSwitchCam = new GUIContent();
	GUIContent contentStartTracking = new GUIContent();
	GUIContent contentStopTracking = new GUIContent();

	private GUIStyle startTrackingStyle = null;
	private GUIStyle stopTrackingStyle = null;
	private GUIStyle switchCamButtonStyle = null;

	private ITracker _tracker;

	private bool _isRecording = false;
	public TrackRetargeter[] _retargeters;

	void Awake()
	{
		
	}

	/** This method is called before any of the Update methods are called the first time.
     * 
     * It initializes helper variables and the tracker.
     */
	void Start()
	{
		contentSwitchCam.image = (Texture2D)imageSwitchCam;
		contentStartTracking.image = (Texture2D)imageStartTracking;
		contentStopTracking.image = (Texture2D)imageStopTracking;

		startTrackingStyle = new GUIStyle();
		startTrackingStyle.normal.background = (Texture2D)contentStartTracking.image;
		startTrackingStyle.active.background = (Texture2D)contentStartTracking.image;
		startTrackingStyle.hover.background = (Texture2D)contentStartTracking.image;

		stopTrackingStyle = new GUIStyle();
		stopTrackingStyle.normal.background = (Texture2D)contentStopTracking.image;
		stopTrackingStyle.active.background = (Texture2D)contentStopTracking.image;
		stopTrackingStyle.hover.background = (Texture2D)contentStopTracking.image;

		switchCamButtonStyle = new GUIStyle();
		switchCamButtonStyle.normal.background = (Texture2D)contentSwitchCam.image;
		switchCamButtonStyle.active.background = (Texture2D)contentSwitchCam.image;
		switchCamButtonStyle.hover.background = (Texture2D)contentSwitchCam.image;

		_tracker = PrevizTracker.instance;
		if(_retargeters != null)
		{
			for (int i = 0; i < _retargeters.Length; i++)
			{
				_tracker.AddListener(_retargeters[i]);
			}
		}

		_tracker.Init();
	}

	void OnGUI()
	{
		if (!_tracker.isTracking)
		{
			trackButton = GUI.Button(new Rect(0, Screen.height - Screen.width / 7, Screen.width / 8, Screen.width / 8), " ", startTrackingStyle);

			if (trackButton)
				_tracker.EnableTracking(true);
		}

		if (_tracker.isTracking)
		{
			stopTrackButton = GUI.Button(new Rect(0, Screen.height - Screen.width / 7, Screen.width / 8, Screen.width / 8), " ", stopTrackingStyle);


			if (stopTrackButton)
				_tracker.EnableTracking(false);
		}

		switchCamButton = GUI.Button(new Rect(Screen.width - Screen.width / 6, Screen.height - Screen.width / 6, Screen.width / 6, Screen.width / 6), " ", switchCamButtonStyle);


		if (_isRecording)
		{
			if (GUI.Button(new Rect(Screen.width - Screen.height / 30 - Screen.height / 3, Screen.width / 5, Screen.height / 3, Screen.width / 12), "Stop"))
			{
				if(_retargeters != null)
				{
					for(int i = 0; i < _retargeters.Length; i++)
					{
						TrackRecorder recorder = _retargeters[i].GetComponent<TrackRecorder>();
						if(recorder != null)
							recorder.StopRecord();
					}
				}

				_isRecording = false;
			}
		}
		else
		{
			if (GUI.Button(new Rect(Screen.width - Screen.height / 30 - Screen.height / 3, Screen.width / 5, Screen.height / 3, Screen.width / 12), "Start"))
			{
				if (_retargeters != null)
				{
					for (int i = 0; i < _retargeters.Length; i++)
					{
						TrackRecorder recorder = _retargeters[i].GetComponent<TrackRecorder>();
						if (recorder != null)
						{
                            recorder.Init(_retargeters[i]);
							recorder.StartRecord();
						}                           
					}

					_isRecording = true;
				}
			}
		}

	}

	/** This method is called every frame.
     * 
     * It fetches the tracking data from the tracker and transforms controlled objects accordingly. 
     * It also fetches vertex, triangle and texture coordinate data to generate 3D face model from the tracker.
     * And lastly it refreshes the video frame texture with the new frame data.
     * 
     */
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}

		_tracker.OnUpdate();
	}

	void OnDestroy()
	{
		_tracker.OnDestroy();
	}

}
