using DigitalSky.Recorder;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SoundRecorderManager : RecorderManager
{
    public new static SoundRecorderManager Instance
    {       
        get
        {
            var instance = Slate.SlateExtensions.Instance.RecordUtility.GetRecorderManager(typeof(SoundRecorderManager));
            if (instance != null)
            {
                return (SoundRecorderManager)instance;
            }
            else 
            {
                return Singleton<SoundRecorderManager>.Instance;
            }
        }
    }

    private string m_Device = string.Empty;

    public override string Device
    {
        get
        {
            return m_Device;
        }
        set
        {
            m_Device = value;
            SaveConfig();
        }
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
        return "RecordAudio/";
    }

    public override void Init()
    {
        LoadConfig();

        string[] deviceList = Microphone.devices;
        if (deviceList != null && deviceList.Length > 0)
        {
            _currentSoundDeviceIndex = Array.IndexOf<string>(deviceList, SoundRecorderManager.Instance.Device);

            if (_currentSoundDeviceIndex == -1 && deviceList.Length > 0)
            {
                _currentSoundDeviceIndex = 0;
            }

            if (_currentSoundDeviceIndex >= 0)
            {
                SoundRecorderManager.Instance.Device = deviceList[_currentSoundDeviceIndex];
            }
        }

    }

    public override void LoadConfig()
    {
        m_Device = PlayerPrefs.GetString("MicrophoneDevice");
        m_recordSavePath = PlayerPrefs.GetString("RecordAudioDir", "RecordAudio/");
    }

    // 当前音频输入设备
    private int _currentSoundDeviceIndex = -1;

    public override void RenderGeneraConifg(Color color, Color _buttonColor)
    {
        GUILayout.Space(5);

        if (true || (Microphone.devices != null && Microphone.devices.Length > 0))
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("音频输入: ", GUILayout.Width(85.0f));

            int index = EditorGUILayout.Popup(_currentSoundDeviceIndex, Microphone.devices);
            if (_currentSoundDeviceIndex != index)
            {
                _currentSoundDeviceIndex = index;

                if (_currentSoundDeviceIndex >= 0)
                {
                    SoundRecorderManager.Instance.Device = Microphone.devices[_currentSoundDeviceIndex];
                }
            }
            GUILayout.Space(2);
            GUILayout.EndHorizontal();
        }
    }

    //public override void RenderConfigObjectItem(ConfigComponent config, GameObject selectobj, Rect rc)
    //{ 
    //    if (true ||(Microphone.devices != null && Microphone.devices.Length > 0))
    //    {
    //        GUILayout.BeginHorizontal();
    //        GUILayout.Label("音频输入: ", GUILayout.Width(85.0f));

    //        int index = EditorGUILayout.Popup(_currentSoundDeviceIndex, Microphone.devices);
    //        if (_currentSoundDeviceIndex != index)
    //        {
    //            _currentSoundDeviceIndex = index;

    //            if (_currentSoundDeviceIndex >= 0)
    //            {
    //                SoundRecorderManager.Instance.Device = Microphone.devices[_currentSoundDeviceIndex];
    //            }
    //        }
            
    //        GUILayout.EndHorizontal();
    //        GUILayout.Space(20.0f);
    //        GUILayout.Label("___________________________________");
    //    }
    //}

    public override void SaveConfig()
    {
        PlayerPrefs.SetString("MicrophoneDevice", m_Device);
        PlayerPrefs.SetString("RecordAudioDir", m_recordSavePath);
    }

    
}
