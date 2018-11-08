using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class SoundRecorderWindow : EditorWindow
{


    private int m_CurrentDeviceIndex = -1;

    private void OnEnable()
    {
        string[] deviceList = Microphone.devices;

        m_CurrentDeviceIndex = Array.IndexOf<string>(deviceList, SoundRecorderManager.Instance.Device);
        if (m_CurrentDeviceIndex == -1 && deviceList.Length > 0)
        {
            m_CurrentDeviceIndex = 0;
        }

        if (m_CurrentDeviceIndex >= 0)
        {
            SoundRecorderManager.Instance.Device = deviceList[m_CurrentDeviceIndex];
        }
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("声音输出路径"))
        {
            string path = EditorUtility.OpenFolderPanel("请选择声音输出路径", Application.dataPath + SoundRecorderManager.Instance.GetRecordSavePath(), Application.dataPath);
            if (!string.IsNullOrEmpty(path))
            {
                if (path.StartsWith(Application.dataPath))
                {
                    path = path.Substring(Application.dataPath.Length);
                }
                else
                {
                    path = string.Empty;
                }
            }

            if (!string.IsNullOrEmpty(path) && path != SoundRecorderManager.Instance.GetRecordSavePath())
            {
                SoundRecorderManager.Instance.SetRecordSavePath(path);
            }
        }
        GUILayout.Label(SoundRecorderManager.Instance.GetRecordSavePath());
        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();

        int index = EditorGUILayout.Popup(m_CurrentDeviceIndex, Microphone.devices);
        if (m_CurrentDeviceIndex != index)
        {
            m_CurrentDeviceIndex = index;

            if (m_CurrentDeviceIndex >= 0)
            {
                SoundRecorderManager.Instance.Device = Microphone.devices[m_CurrentDeviceIndex];
            }
        }
    }
}

