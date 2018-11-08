﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditor.Callbacks;

public class DirectorRoomWindow : EditorWindow
{
    [MenuItem("剧情工具/导播台", false, 55)]
    public static void ShowWindow()
    {
        var window = EditorWindow.GetWindow<DirectorRoomWindow>("导播台"); 
        window.Show();
    }

    private static readonly float kDefaultWidth = 1280.0f;
    private static readonly float kDefaultHeight = 720.0f;
    private static readonly float kDefaultFuncBarHeight = 80.0f;
    private static readonly float kDefaultRecordBarHeight = 80.0f;
    private static readonly float kDefaultCameraRatio = 16.0f / 9.0f;
    private static readonly float kSpace = 10.0f;
    private static readonly float kGridSpace = 10.0f;
    private static readonly int kGridCountX = 3;
    private static readonly float kControlSliderHeight = 300.0f;

    private float m_WidthScale = 1.0f;
    private float m_HeightScale = 1.0f;
    private bool m_IsFuncShown = false;
    private bool m_IsGridShown = false;
    private bool m_IsMainCameraMaxShown = false;
    private bool m_IsCameraEditMode = false;
    private bool m_IsEventTriggerMode = false;
    private bool m_IsRecordShown = true;
    private float m_ControlSpeed = 0.5f;
    private float m_RotationSpeed = 45.0f;

    private bool m_IsAddtiveFunc = false;
    private bool m_IsReOrder = false;
    public bool IsRecording
    {
        get
        {
            if (Slate.SlateExtensions.Instance == null)
            {
                return false;
            }
            return Slate.SlateExtensions.Instance.RecordUtility.IsRecording;
        }
    }

    public bool IsCameraEditMode
    {
        get
        {
            return m_IsCameraEditMode;
        }
        set
        {
            m_IsCameraEditMode = value;
            if (m_IsCameraEditMode)
            {
                m_ControlMode = ControlMode.None;
            }
        }
    }

    enum ControlMode
    {
        None,
        FOV,
        FocalLength,
        Speed,
        Rot,
        Count,
    }
    private ControlMode m_ControlMode = ControlMode.None;

    private CameraDirectorInstance m_WillChange = null;
    private CameraDirectorInstance Current
    {
        get
        {
            return DirectorRoomRecorderManager.Instance.SelectedInstance;
        }
        set
        {
            if (value != DirectorRoomRecorderManager.Instance.SelectedInstance)
            {
                DirectorRoomRecorderManager.Instance.SelectedInstance = value;

                CameraMarkerDirectorInstance marker = value as CameraMarkerDirectorInstance;
                if (marker != null && marker.Marker.TriggerSubCutscene != null)
                {
                    TriggerEventCutsene(marker.Marker.TriggerSubCutscene);
                }
            }
        }
    }

    private bool IsFuncBarShown
    {
        get
        {
            return !m_IsCameraEditMode;
        }
    }


    private void OnEnable()
    {
        UpdateEventCutsceneList();
        EditorApplication.update -= OnDriectorRoomUpdate;
        EditorApplication.update += OnDriectorRoomUpdate;
        DirectorRoomRecorderManager.Instance.Enable(); 
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnDriectorRoomUpdate;
        DirectorRoomRecorderManager.Instance.Disable();
    }

    private void OnDriectorRoomUpdate()
    {
         
    }




    [OnOpenAssetAttribute(1)]
    public static bool step1(int instanceID, int line)
    {
        string name = EditorUtility.InstanceIDToObject(instanceID).name;
        /// Debug.Log("Open Asset step: 1 (" + name + ")");
        return false; // we did not handle the open
    }

    // step2 has an attribute with index 2, so will be called after step1
    [OnOpenAssetAttribute(2)]
    public static bool step2(int instanceID, int line)
    {

        string path = AssetDatabase.GetAssetPath(EditorUtility.InstanceIDToObject(instanceID));
        string name = Application.dataPath + "/" + path.Replace("Assets/", "");

        if (name.EndsWith(".unity"))
        {

            var window = EditorWindow.GetWindow<DirectorRoomWindow>("导播台");
            window.Close();

        }

        // Debug.Log("Open Asset step: 2 (" + name + ")");
        return false; // we did not handle the open
    }

    void OnGUI()
    {
        m_WillChange = null;

        float width = Screen.width;
        float height = Screen.height;

        m_WidthScale = width / kDefaultWidth;
        m_HeightScale = height / kDefaultHeight;

        int labelSize = GUI.skin.label.fontSize;
        int buttonSize = GUI.skin.button.fontSize;
        bool wordWrap = GUI.skin.button.wordWrap;
        try
        {
            if (m_IsGridShown)
            {
                OnRenderCameraLayerGrid();
            }
            else
            {
                OnRenderCameraLayerDetail();
            }

            if (IsFuncBarShown)
            {
                float funcBarHeight = kDefaultFuncBarHeight * m_HeightScale;
                Rect funcBarRc = new Rect(kSpace * m_WidthScale, height - funcBarHeight - 2.0f * kSpace * m_HeightScale, width - kSpace * m_WidthScale * 2.0f, funcBarHeight);
                OnRenderFuncBar(funcBarRc);
            }

            if (!m_IsFuncShown && m_IsRecordShown && Slate.CutsceneEditor.current != null)
            {
                float recordBarHeight = kDefaultRecordBarHeight * m_HeightScale;
                Rect recordBarRc = new Rect(kSpace * m_WidthScale, height - recordBarHeight - 2.0f * kSpace * m_HeightScale, width - kSpace * m_WidthScale * 2.0f, recordBarHeight);
                OnRenderRecordBar(recordBarRc);
            }

            if (m_WillChange != null)
            {
                Current = m_WillChange;
                if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
                {
                    Event.current.Use();
                }
            }

            if (Slate.SlateExtensions.Instance != null && Slate.SlateExtensions.Instance.RecordUtility != null
                && Slate.SlateExtensions.Instance.RecordUtility.IsCountDownRecordState)
            {
                OnRenderCountDown(Slate.SlateExtensions.Instance.RecordUtility.CountDownTime);
            }

            if (m_IsReOrder)
            {
                for (int i = 0; i < DirectorRoomRecorderManager.Instance.CameraMarkerDirectorList.Count; i++)
                {
                    CameraMarkerDirectorInstance marker = DirectorRoomRecorderManager.Instance.CameraMarkerDirectorList[i];
                    if (marker.OrderFalg == -1 && i > 0)
                    {
                        marker.Marker.Order--;
                        DirectorRoomRecorderManager.Instance.CameraMarkerDirectorList[i - 1].Order++;
                        marker.ReOrder(0);
                        break;
                    }
                    else if (marker.OrderFalg == 1 && i != DirectorRoomRecorderManager.Instance.CameraMarkerDirectorList.Count - 1)
                    {
                        marker.Marker.Order++;
                        DirectorRoomRecorderManager.Instance.CameraMarkerDirectorList[i + 1].Order--;
                        marker.ReOrder(0);
                        break;
                    }
                }

                m_IsReOrder = false;

                DirectorRoomRecorderManager.Instance.CameraMarkerDirectorList.Sort((c1, c2) =>
                {
                    return c1.Marker.Order - c2.Marker.Order;
                });
            }
            GUI.skin.button.wordWrap = wordWrap;
            GUI.skin.label.fontSize = labelSize;
            GUI.skin.button.fontSize = buttonSize;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e +"导播台出现异常，也要设置字体格式还原");
            GUI.skin.button.wordWrap = wordWrap;
            GUI.skin.label.fontSize = labelSize;
            GUI.skin.button.fontSize = buttonSize;
        }

        Repaint();
    }

    private Vector2 m_GridListScrollPos = Vector2.zero;
    private static readonly float kGridTextureBorder = 2.0f;

    private void OnRenderCameraLayerGrid()
    {
        float width = Screen.width;
        float height = Screen.height;

        Rect rc = new Rect(0.0f, 0.0f, width, height - kSpace * m_HeightScale * 2.0f);

        GUILayout.BeginArea(rc);
        m_GridListScrollPos = GUILayout.BeginScrollView(m_GridListScrollPos, false, true);

        float gridWidth = ((rc.width - kSpace * m_WidthScale - 30.0f) - (kGridCountX - 1) * kGridSpace * m_WidthScale) / kGridCountX;
        float gridHeight = gridWidth / kDefaultCameraRatio;

        int index = 0;
        float offsetY = kSpace * m_HeightScale;
        GUILayout.Space(kSpace * m_HeightScale);

        for (int i = 0; i < DirectorRoomRecorderManager.Instance.CameraGateDirectorList.Count; i++)
        {
            if (index % kGridCountX == 0)
            {
                GUILayout.Space(gridHeight + (kGridSpace * m_HeightScale));
            }

            Rect gridRc = new Rect(kSpace * m_WidthScale + (index % kGridCountX) * (gridWidth + kGridSpace * m_WidthScale), offsetY, gridWidth, gridHeight);

            if (gridRc.min.y < m_GridListScrollPos.y + rc.max.y && gridRc.max.y > m_GridListScrollPos.y + rc.min.y)
            {
                OnRenderCameraDirector(gridRc, DirectorRoomRecorderManager.Instance.CameraGateDirectorList[i]);
            }

            if ((index + 1) % kGridCountX == 0)
            {
                offsetY += gridHeight + (kGridSpace * m_HeightScale);
            }

            index++;
        }

        for (int i = 0; i < DirectorRoomRecorderManager.Instance.CameraMarkerDirectorList.Count; i++)
        {
            if (index % kGridCountX == 0)
            {
                GUILayout.Space(gridHeight + (kGridSpace * m_HeightScale));
            }

            Rect gridRc = new Rect(kSpace * m_WidthScale + (index % kGridCountX) * (gridWidth + kGridSpace * m_WidthScale), offsetY, gridWidth, gridHeight);

            if (gridRc.min.y < m_GridListScrollPos.y + rc.max.y && gridRc.max.y > m_GridListScrollPos.y + rc.min.y)
            {
                OnRenderCameraDirector(gridRc, DirectorRoomRecorderManager.Instance.CameraMarkerDirectorList[i]);
            }

            if ((index + 1) % kGridCountX == 0)
            {
                offsetY += gridHeight + (kGridSpace * m_HeightScale);
            }

            index++;
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void OnRenderCameraLayerDetail()
    {
        float width = Screen.width;
        float height = Screen.height;

        float leftAreaWidth = m_IsMainCameraMaxShown ? width : width * 4.0f / 5.0f;
        float rightAreaWidth = width / 5.0f;

        if (Current == null && DirectorRoomRecorderManager.Instance.CameraGateDirectorList.Count > 0)
        {
            Current = DirectorRoomRecorderManager.Instance.CameraGateDirectorList[0];
        }

        if (Current == null && DirectorRoomRecorderManager.Instance.CameraMarkerDirectorList.Count > 0)
        {
            Current = DirectorRoomRecorderManager.Instance.CameraMarkerDirectorList[0];
        }

        if (Current != null)
        {
            OnRenderMainCameraDirector(new Rect(0.0f, 0.0f, leftAreaWidth, height), Current);
        }

        if (!m_IsMainCameraMaxShown)
        {
            OnRenderDetailCameraDirectorList(new Rect(leftAreaWidth, 0.0f, rightAreaWidth, height - kSpace * m_HeightScale));
        }
    }

    private void OnRenderMainCameraDirector(Rect rc, CameraDirectorInstance instance)
    {
        if (instance.Camera == null)
        {
            return;
        }

        float width = rc.width;
        float height = rc.height;

        float w, h;
        if (width / height > kDefaultCameraRatio)
        {
            w = width;
            h = width / kDefaultCameraRatio;
        }
        else
        {
            w = height * kDefaultCameraRatio;
            h = height;
        }

        if ((int)h != (int)instance.Camera.targetTexture.height)
        {
            GameObject.DestroyImmediate(instance.Camera.targetTexture);
            instance.SetRenderTargetSize((int)w, (int)h);
        }

        Rect uv;
        if (width / height > w / h)
        {
            float v = (h - (w * height / width)) * 0.5f / h;
            uv = new Rect(0.0f, v, 1.0f, 1.0f - 2.0f * v);
        }
        else
        {
            float u = (w - (h * width / height)) * 0.5f / w;
            uv = new Rect(u, 0.0f, 1.0f - 2.0f * u, 1.0f);
        }

        instance.Camera.Render();
        GUI.DrawTextureWithTexCoords(rc, instance.Camera.targetTexture, uv, false);

        bool isDeleted = OnRenderMainCameraRightBar(new Rect(rc.width - 60.0f * m_WidthScale, 0.0f, 60.0f * m_WidthScale, rc.height), instance);

        if (!isDeleted && IsCameraEditMode)
        {
            float sizeX = 270.0f * m_WidthScale;
            float sizeY = 160.0f * m_WidthScale;
            float controlSpaceX = 60.0f * m_WidthScale;
            float controlSpaceY = 60.0f * m_HeightScale;

            OnRenderDirectionKey(new Rect(controlSpaceX, height - controlSpaceY - sizeY, sizeX, sizeY), instance);

            float rightControlSize = 60.0f * m_WidthScale;
            OnRenderRightControl(new Rect(width - rightControlSize - controlSpaceX, height - controlSpaceY - rightControlSize, rightControlSize, rightControlSize), instance);
        }

        if (!isDeleted)
        {
            GUILayout.BeginArea(rc);
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.skin.label.fontSize = (int)(30 * m_WidthScale);
            GUILayout.Label(instance.name);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        if (m_IsEventTriggerMode)
        {
            OnRenderMainCameraLeftBar(new Rect(5.0f * m_WidthScale, 0.0f, 150.0f * m_WidthScale, rc.height));
        }
    }

    private List<Slate.Cutscene> m_EventCutsceneList = new List<Slate.Cutscene>();
    private int m_EventCutscenePage = 0;
    private static readonly int m_ItemCountPrePage = 8;
    private static readonly string EventTrackName = "Event_SubCutsceneTrack";
    private void UpdateEventCutsceneList()
    {
        m_EventCutscenePage = 0;
        m_EventCutsceneList.Clear();
        Slate.Cutscene[] cutsceneList = GameObject.FindObjectsOfType<Slate.Cutscene>();
        if (cutsceneList == null || cutsceneList.Length == 0)
        {
            return;
        }

        for (int i = 0; i < cutsceneList.Length; i++)
        {
            if (cutsceneList[i].EventSubCutscene)
            {
                m_EventCutsceneList.Add(cutsceneList[i]);
            }
        }

        if (m_EventCutsceneList.Count > 0)
        {
            m_EventCutsceneList.Sort((c1, c2) => string.Compare(c1.name, c2.name));
        }
    }

    private void OnRenderMainCameraLeftBar(Rect rc)
    {
        if (m_EventCutsceneList.Count == 0)
        {
            return;
        }

        int pageCount = Mathf.CeilToInt((float)m_EventCutsceneList.Count / m_ItemCountPrePage);

        GUI.skin.button.fontSize = (int)(18 * m_WidthScale);
        float buttonSize = rc.width;
        float buttonHeight = 50 * m_HeightScale;

        GUILayout.BeginArea(rc);
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<", GUILayout.Width(buttonSize * 0.5f), GUILayout.Height(buttonHeight)))
        {
            m_EventCutscenePage--;
        }

        if (GUILayout.Button(">", GUILayout.Width(buttonSize * 0.5f), GUILayout.Height(buttonHeight)))
        {
            m_EventCutscenePage++;
        }

        GUILayout.EndHorizontal();

        while (m_EventCutscenePage < 0)
        {
            m_EventCutscenePage += pageCount;
        }
        while (m_EventCutscenePage >= pageCount)
        {
            m_EventCutscenePage -= pageCount;
        }

        Slate.Cutscene cutscene = null;
        int maxIndex = Mathf.Min(m_EventCutsceneList.Count, (m_EventCutscenePage + 1) * m_ItemCountPrePage);
        for (int i = m_EventCutscenePage * m_ItemCountPrePage; i < maxIndex; i++)
        {
            cutscene = m_EventCutsceneList[i];

            if (cutscene && GUILayout.Button(cutscene.name, GUILayout.Width(buttonSize), GUILayout.Height(buttonHeight)))
            {
                TriggerEventCutsene(cutscene);
            }
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private void TriggerEventCutsene(Slate.Cutscene cutscene)
    {
        if (IsRecording)
        {
            Slate.CutsceneTrack subCutsenceTrack = Slate.CutsceneEditor.current.cutscene.GetTrackByName(EventTrackName);
            if (subCutsenceTrack == null)
            {
                subCutsenceTrack = Slate.CutsceneEditor.current.cutscene.directorGroup.AddTrack<Slate.DirectorActionTrack>(EventTrackName);
            }

            float startTime = Slate.SlateExtensions.Instance.RecordUtility.GetTotalTime();
            Slate.ActionClips.SubCutscene subCutscene = subCutsenceTrack.AddAction<Slate.ActionClips.SubCutscene>(startTime);
            subCutsenceTrack.name = EventTrackName;
            subCutscene.cutscene = cutscene;
            subCutscene.length = cutscene.length;

            Slate.CutsceneEditor.current.cutscene.DynamicAddTimePointers(subCutscene);
        }
        else
        {
            foreach (Slate.CutsceneGroup group in cutscene.groups)
            {
                foreach (Slate.CutsceneTrack track in group.tracks)
                {
                    track.BakeAllClip();
                }
            }
        }
    }

    private bool OnRenderMainCameraRightBar(Rect rc, CameraDirectorInstance instance)
    {
        if (instance.Camera == null)
        {
            return false;
        }

        bool isDeleted = false;
        GUILayout.BeginArea(rc);
        GUILayout.BeginVertical();

        GUI.skin.button.fontSize = (int)(18 * m_WidthScale);
        float buttonSize = rc.width;

        if (GUILayout.Button(m_IsMainCameraMaxShown ? "缩小" : "放大", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
        {
            m_IsMainCameraMaxShown = !m_IsMainCameraMaxShown;
        }


        if (GUILayout.Button(IsCameraEditMode ? "退出\n编辑" : "编辑\n模式", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
        {
            IsCameraEditMode = !IsCameraEditMode;
        }

        if (GUILayout.Button(m_IsEventTriggerMode ? "关闭\n触发" : "事件\n触发", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
        {
            m_IsEventTriggerMode = !m_IsEventTriggerMode;
            if (m_IsEventTriggerMode)
            {
                UpdateEventCutsceneList();
            }
        }

        if (!IsRecording)
        {
            if (instance.CanSave && GUILayout.Button("保存", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
            {
                instance.Save();
            }

            if (GUILayout.Button("复位", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
            {
                instance.Reset();
            }

            if (GUILayout.Button("水平", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
            {
                instance.HorizontalAdjust();
            }

            if (instance.Order > 0 && GUILayout.Button("上移", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
            {
                instance.ReOrder(-1);
                m_IsReOrder = true;
            }

            if (instance.Order < DirectorRoomRecorderManager.Instance.CameraMarkerDirectorList.Count - 1
                && GUILayout.Button("下移", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
            {
                instance.ReOrder(1);
                m_IsReOrder = true;
            }

            if (GUILayout.Button("克隆", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
            {
                GameObject obj = new GameObject(string.Format("Camera:{0:D3}",
                    + DirectorRoomRecorderManager.Instance.CameraGateDirectorList.Count
                    + DirectorRoomRecorderManager.Instance.CameraMarkerDirectorList.Count + 1));
                Camera camera = obj.AddComponent<Camera>();
                camera.CopyFrom(instance.Camera);
                camera.enabled = false;
                CameraDirectorMarker marker = obj.AddComponent<CameraDirectorMarker>();
                DirectorRoomRecorderManager.Instance.DynamicAddNew(marker, camera, true);
                var go = GameObject.FindObjectOfType<ConfigComponent>();
                if (go != null)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MoveGameObjectToScene(obj, go.gameObject.scene);
                }
            }

            if (instance.CanDelete && GUILayout.Button("删除", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
            {
                DirectorRoomRecorderManager.Instance.DynamicDelete(instance);
                isDeleted = true;
            }
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();

        return isDeleted;
    }

    private Vector2 m_DetailListScrollPos = Vector2.zero;

    private void OnRenderDetailCameraDirectorList(Rect rc)
    {
        GUILayout.BeginArea(rc);

        m_DetailListScrollPos = GUILayout.BeginScrollView(m_DetailListScrollPos, false, true);

        float offsetY = kSpace * m_HeightScale;
        GUILayout.Space(kSpace * m_HeightScale);

        float itemWidth = rc.width - kSpace * m_WidthScale - 30.0f;
        float itemHeight = itemWidth / kDefaultCameraRatio;

        for (int i = 0; i < DirectorRoomRecorderManager.Instance.CameraGateDirectorList.Count; i++)
        {
            GUILayout.Space(itemHeight + kSpace * m_HeightScale);

            Rect itemRc = new Rect(kSpace * m_WidthScale, offsetY, itemWidth, itemHeight);
            if (itemRc.min.y < m_DetailListScrollPos.y + rc.max.y && itemRc.max.y > m_DetailListScrollPos.y + rc.min.y)
            {
                OnRenderCameraDirector(itemRc, DirectorRoomRecorderManager.Instance.CameraGateDirectorList[i]);
            }

            offsetY += (itemHeight + kSpace * m_HeightScale);
        }

        for (int i = 0; i < DirectorRoomRecorderManager.Instance.CameraMarkerDirectorList.Count; i++)
        {
            GUILayout.Space(itemHeight + kSpace * m_HeightScale);

            Rect itemRc = new Rect(kSpace * m_WidthScale, offsetY, itemWidth, itemHeight);
            if (itemRc.min.y < m_DetailListScrollPos.y + rc.max.y && itemRc.max.y > m_DetailListScrollPos.y + rc.min.y)
            {
                OnRenderCameraDirector(itemRc, DirectorRoomRecorderManager.Instance.CameraMarkerDirectorList[i]);
            }

            offsetY += (itemHeight + kSpace * m_HeightScale);
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void OnRenderCameraDirector(Rect rc, CameraDirectorInstance instance)
    {
        if (instance.Camera == null)
        {
            return;
        }

        if (Event.current.type == EventType.MouseUp
            && Event.current.button == 0
            && rc.Contains(Event.current.mousePosition))
        {
            m_WillChange = instance;
        }

        float textureHeight = rc.height;
        if (textureHeight == 0.0f)
        {
            return;
        }
        float textureWidth = textureHeight * kDefaultCameraRatio;

        if (m_IsGridShown || instance != Current)
        {
            if (instance.Camera.targetTexture.height != (int)textureHeight)
            {
                GameObject.DestroyImmediate(instance.Camera.targetTexture);
                instance.SetRenderTargetSize((int)textureWidth, (int)textureHeight);
            }
        }

        instance.Camera.Render();

        Rect texRc = new Rect(rc.center.x - textureWidth * 0.5f, rc.center.y - textureHeight * 0.5f, textureWidth, textureHeight);

        if (Current == instance)
        {
            GUI.color = Color.green;
            GUI.DrawTexture(Rect.MinMaxRect(texRc.min.x - kGridTextureBorder, texRc.min.y - kGridTextureBorder, texRc.max.x + kGridTextureBorder, texRc.max.y + kGridTextureBorder), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }

        GUI.DrawTexture(texRc, instance.Camera.targetTexture, ScaleMode.StretchToFill, false);

        GUILayout.BeginArea(rc);
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUI.skin.label.fontSize = (int)(20 * m_WidthScale);
        GUILayout.Label(instance.name);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private void OnRenderFuncBar(Rect rc)
    {
        float space = kSpace * m_WidthScale;

        GUILayout.BeginArea(rc);
        GUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();

        GUI.skin.button.fontSize = (int)(18 * m_WidthScale);

        float buttonSize = rc.height * 0.8f;
        float funcButtonSize = rc.height;

        if (m_IsFuncShown)
        {
            if (GUILayout.Button("新建\n相机", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
            {
                GameObject obj = new GameObject(string.Format("Camera:{0:D3}",
                    DirectorRoomRecorderManager.Instance.CameraGateDirectorList.Count
    + DirectorRoomRecorderManager.Instance.CameraMarkerDirectorList.Count + 1));
                Camera camera = obj.AddComponent<Camera>();
                camera.enabled = false;
                CameraDirectorMarker marker = obj.AddComponent<CameraDirectorMarker>();
                DirectorRoomRecorderManager.Instance.DynamicAddNew(marker, camera, true);
                var go = GameObject.FindObjectOfType<ConfigComponent>();
                if (go != null)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MoveGameObjectToScene(obj, go.gameObject.scene);
                }

            }

            if (GUILayout.Button("录制\n显示", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
            {
                m_WillChange = null;
                m_IsRecordShown = !m_IsRecordShown;
            }

            GUILayout.Space(space);

            if (GUILayout.Button("刷新", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
            {
                m_WillChange = null;
                DirectorRoomRecorderManager.Instance.RefreshData();
            }

            GUILayout.Space(space);

            if (GUILayout.Button("连接\n中间件", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
            {
                m_WillChange = null;
                DirectorRoomRecorderManager.Instance.ConnectGateCameraData();
            }

            GUILayout.Space(space);

            if (GUILayout.Button(m_IsGridShown ? "列表" : "详细", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
            {
                m_WillChange = null;
                m_IsGridShown = !m_IsGridShown;
            }

            GUILayout.Space(space);
        }

        GUI.skin.button.fontSize = (int)(20 * m_HeightScale);

        if (GUILayout.Button("设置", GUILayout.Width(funcButtonSize), GUILayout.Height(funcButtonSize)))
        {
            m_WillChange = null;
            m_IsFuncShown = !m_IsFuncShown;
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    public static string HumanizeTimeString(float seconds)
    {
        System.TimeSpan ts = System.TimeSpan.FromSeconds(seconds);
        string timeStr = string.Format("{0:D2}:{1:D2}:{2:D2}", ts.Minutes, ts.Seconds, Mathf.RoundToInt((float)ts.Milliseconds / 10));
        return timeStr;
    }

    private void DrawCutsceneTime(float time)
    {
        if (Slate.Prefs.timeStepMode == Slate.Prefs.TimeStepMode.Seconds)
        {
            GUILayout.Label(HumanizeTimeString(Mathf.Round(time / Slate.Prefs.snapInterval) * Slate.Prefs.snapInterval));
        }
        else
        {
            GUILayout.Label(((int)(time * Slate.Prefs.frameRate)).ToString());
        }
    }

    private void OnRenderRecordBar(Rect rc)
    {
        float space = kSpace * m_WidthScale;
        float buttonSize = rc.height * 0.8f;

        GUILayout.BeginArea(rc);
        GUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();

        if (Slate.SlateExtensions.Instance != null && Slate.SlateExtensions.Instance.RecordUtility != null &&
            !(Slate.SlateExtensions.Instance.RecordUtility.IsRecording) && !(Slate.SlateExtensions.Instance.RecordUtility.IsCountDownRecordState)
            && Slate.SlateExtensions.Instance.RecordUtility.IsRecordActive == false && Slate.CutsceneEditor.current != null && Slate.CutsceneEditor.current.cutscene != null)
        {
            GUI.color = Color.white;
            GUI.skin.label.fontSize = (int)(60 * m_WidthScale);
            if (Slate.CutsceneEditor.current.cutscene != null)
            {
                DrawCutsceneTime(Slate.CutsceneEditor.current.cutscene.currentTime);
            }

            GUI.color = Color.white;

            if (GUILayout.Button("录制\n同步", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
            {
                m_WillChange = null;
                Slate.SlateExtensions.Instance.RecordUtility.SetRecordActive(true, Slate.CutsceneEditor.current.cutscene);
            }
        }
        else if (Slate.SlateExtensions.Instance != null
            && Slate.SlateExtensions.Instance.RecordUtility != null
            && Slate.SlateExtensions.Instance.RecordUtility.IsRecordEnable())
        {
            if (Slate.SlateExtensions.Instance.RecordUtility.IsWaittingRecordState)
            {
                GUI.color = Color.white;
                GUI.skin.label.fontSize = (int)(60 * m_WidthScale);
                DrawCutsceneTime(Slate.CutsceneEditor.current.cutscene.currentTime);
                GUI.color = Color.white;

                if (GUILayout.Button("录制", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
                {
                    m_WillChange = null;
                    Slate.CutsceneEditor.current.SetRecordCurrentStartTime();
                    Slate.CutsceneEditor.current.StartRecord();
                    Slate.CutsceneEditor.current.Play();
                }
            }
            else
            {
                GUI.color = Color.red;
                GUI.skin.label.fontSize = (int)(60 * m_WidthScale);
                DrawCutsceneTime(Slate.SlateExtensions.Instance.RecordUtility.GetRecordTime());
                GUI.color = Color.white;

                if (GUILayout.Button("停止\n录制", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
                {
                    m_WillChange = null;
                    Slate.CutsceneEditor.current.StopRecord();
                }
            }
        }

        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void OnRenderDirectionKey(Rect rc, CameraDirectorInstance instance)
    {
        GUILayout.BeginArea(rc);

        GUI.skin.button.fontSize = (int)(18 * m_WidthScale);

        float w = rc.width / 5 - 5 * m_WidthScale;
        float h = rc.height / 3;

        bool isHide = IsRecording && !instance.IsDynamic;

        if (!isHide)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.RepeatButton("◀", GUILayout.Width(w), GUILayout.Height(h)))
            {
                instance.Yaw(-m_RotationSpeed * Time.deltaTime);
            }

            if (GUILayout.RepeatButton("↑", GUILayout.Width(w), GUILayout.Height(h)))
            {
                instance.Forward(m_ControlSpeed * Time.deltaTime);
            }

            if (GUILayout.RepeatButton("▶", GUILayout.Width(w), GUILayout.Height(h)))
            {
                instance.Yaw(m_RotationSpeed * Time.deltaTime);
            }

            if (m_IsAddtiveFunc)
            {
                if (GUILayout.RepeatButton("㊤", GUILayout.Width(w), GUILayout.Height(h)))
                {
                    instance.Up(m_ControlSpeed * Time.deltaTime);
                }

                if (GUILayout.RepeatButton("㊦", GUILayout.Width(w), GUILayout.Height(h)))
                {
                    instance.Up(-m_ControlSpeed * Time.deltaTime);
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.RepeatButton("←", GUILayout.Width(w), GUILayout.Height(h)))
            {
                instance.Right(-m_ControlSpeed * Time.deltaTime);
            }

            if (GUILayout.Button("+", GUILayout.Width(w), GUILayout.Height(h)))
            {
                m_IsAddtiveFunc = !m_IsAddtiveFunc;
            }

            if (GUILayout.RepeatButton("→", GUILayout.Width(w), GUILayout.Height(h)))
            {
                instance.Right(m_ControlSpeed * Time.deltaTime);
            }

            if (instance.IsFixed && m_IsAddtiveFunc)
            {
                if (GUILayout.Button(instance.IsDynamic ? "♦" : "♢", GUILayout.Width(w), GUILayout.Height(h)))
                {
                    instance.IsDynamic = !instance.IsDynamic;
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.RepeatButton("▲", GUILayout.Width(w), GUILayout.Height(h)))
            {
                instance.Pitch(-m_RotationSpeed * Time.deltaTime);
            }

            if (GUILayout.RepeatButton("↓", GUILayout.Width(w), GUILayout.Height(h)))
            {
                instance.Forward(-m_ControlSpeed * Time.deltaTime);
            }

            if (GUILayout.RepeatButton("▼", GUILayout.Width(w), GUILayout.Height(h)))
            {
                instance.Pitch(m_RotationSpeed * Time.deltaTime);
            }

            if (m_IsAddtiveFunc)
            {
                if (GUILayout.RepeatButton("┓", GUILayout.Width(w), GUILayout.Height(h)))
                {
                    instance.Roll(m_RotationSpeed * Time.deltaTime);
                }

                if (GUILayout.RepeatButton("┗", GUILayout.Width(w), GUILayout.Height(h)))
                {
                    instance.Roll(-m_RotationSpeed * Time.deltaTime);
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        GUILayout.EndArea();
    }

    private void OnRenderRightControl(Rect rc, CameraDirectorInstance instance)
    {
        GUI.skin.button.fontSize = (int)(18 * m_WidthScale);

        string text = string.Empty;
        switch (m_ControlMode)
        {
            case ControlMode.None:
                text = "操作\n选择";
                break;
            case ControlMode.FOV:
                text = "FOV";
                break;
            case ControlMode.FocalLength:
                text = "焦距";
                break;
            case ControlMode.Speed:
                text = "移动\n速度";
                break;
            case ControlMode.Rot:
                text = "旋转\n速度";
                break;
        }

        float buttonSize = rc.width;
        GUILayout.BeginArea(rc);

        if (GUILayout.Button(text, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
        {
            int index = (int)m_ControlMode + 1;
            while (index >= (int)ControlMode.Count)
            {
                index -= (int)ControlMode.Count;
            }
            m_ControlMode = (ControlMode)index;
            ;
        }

        GUILayout.EndArea();

        if (m_ControlMode == ControlMode.FOV)
        {
            OnRenderFOVControl(new Rect(rc.xMin, rc.yMin - kSpace * m_HeightScale - kControlSliderHeight * m_HeightScale, rc.width, kControlSliderHeight * m_HeightScale), instance);
        }
        else if (m_ControlMode == ControlMode.FocalLength)
        {
            OnRenderFocalLengthControl(new Rect(rc.xMin, rc.yMin - kSpace * m_HeightScale - kControlSliderHeight * m_HeightScale, rc.width, kControlSliderHeight * m_HeightScale), instance);
        }
        else if (m_ControlMode == ControlMode.Speed)
        {
            OnRenderSpeedControl(new Rect(rc.xMin, rc.yMin - kSpace * m_HeightScale - kControlSliderHeight * m_HeightScale, rc.width, kControlSliderHeight * m_HeightScale), instance);
        }
        else if (m_ControlMode == ControlMode.Rot)
        {
            OnRenderRotControl(new Rect(rc.xMin, rc.yMin - kSpace * m_HeightScale - kControlSliderHeight * m_HeightScale, rc.width, kControlSliderHeight * m_HeightScale), instance);
        }
    }

    private void OnRenderFOVControl(Rect rc, CameraDirectorInstance instance)
    {
        if (Event.current.type == EventType.ScrollWheel && rc.Contains(Event.current.mousePosition))
        {
            instance.Camera.fieldOfView = Mathf.Clamp(instance.Camera.fieldOfView + Event.current.delta.y / 3.0f, 1.0f, 179.0f);
        }

        GUILayout.BeginArea(rc);
        GUILayout.BeginVertical();
        instance.Camera.fieldOfView = GUILayout.VerticalSlider(instance.Camera.fieldOfView, 1.0f, 179.0f, GUILayout.Width(rc.width));
        GUILayout.Space(kSpace * m_HeightScale);
        instance.Camera.fieldOfView = Mathf.Clamp(GUILayout.TextField(string.Format("{0:N1}", instance.Camera.fieldOfView), GUILayout.Width(rc.width)).ToFloat(), 1.0f, 179.0f);
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private void OnRenderFocalLengthControl(Rect rc, CameraDirectorInstance instance)
    {
        const float kFilmHeight = 35f;
        float fFoucsLength = (kFilmHeight * 0.5f) / Mathf.Tan(instance.Camera.fieldOfView / 180.0f * Mathf.PI / 2.0f);
        float newFoucsLength = fFoucsLength;

        if (Event.current.type == EventType.ScrollWheel && rc.Contains(Event.current.mousePosition))
        {
            newFoucsLength = Mathf.Clamp(newFoucsLength + Event.current.delta.y / 3.0f, 1.0f, 2000f);
        }

        GUILayout.BeginArea(rc);
        GUILayout.BeginVertical();
        newFoucsLength = GUILayout.VerticalSlider(fFoucsLength, 1.0f, 2000f, GUILayout.Width(rc.width));
        GUILayout.Space(kSpace * m_HeightScale);
        newFoucsLength = Mathf.Clamp(GUILayout.TextField(string.Format("{0:N1}", newFoucsLength), GUILayout.Width(rc.width)).ToFloat(), 1.0f, 2000f);
        GUILayout.EndVertical();
        GUILayout.EndArea();
        
        if (newFoucsLength != fFoucsLength)
        {
            instance.Camera.fieldOfView = Mathf.Clamp(2.0f * Mathf.Atan((kFilmHeight * 0.5f) / newFoucsLength) / Mathf.PI * 180.0f, 1, 179);
        }
    }

    private void OnRenderSpeedControl(Rect rc, CameraDirectorInstance instance)
    {
        if (Event.current.type == EventType.ScrollWheel && rc.Contains(Event.current.mousePosition))
        {
            m_ControlSpeed = Mathf.Clamp(m_ControlSpeed + Event.current.delta.y / 300.0f, 0.01f, 10.0f);
        }

        GUILayout.BeginArea(rc);
        GUILayout.BeginVertical();
        m_ControlSpeed = GUILayout.VerticalSlider(m_ControlSpeed, 0.01f, 10.0f, GUILayout.Width(rc.width));
        GUILayout.Space(kSpace * m_HeightScale);
        m_ControlSpeed = Mathf.Clamp(GUILayout.TextField(string.Format("{0:N2}", m_ControlSpeed), GUILayout.Width(rc.width)).ToFloat(), 0.01f, 10.0f);
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private void OnRenderRotControl(Rect rc, CameraDirectorInstance instance)
    {
        if (Event.current.type == EventType.ScrollWheel && rc.Contains(Event.current.mousePosition))
        {
            m_RotationSpeed = Mathf.Clamp(m_RotationSpeed + Event.current.delta.y / 300.0f, 1.0f, 360.0f);
        }

        GUILayout.BeginArea(rc);
        GUILayout.BeginVertical();
        m_RotationSpeed = GUILayout.VerticalSlider(m_RotationSpeed, 1.0f, 360.0f, GUILayout.Width(rc.width));
        GUILayout.Space(kSpace * m_HeightScale);
        m_RotationSpeed = Mathf.Clamp(GUILayout.TextField(string.Format("{0:N1}", m_RotationSpeed), GUILayout.Width(rc.width)).ToFloat(), 1.0f, 360.0f);
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private void OnRenderCountDown(float countDownTime)
    {
        int saveFontSize = GUI.skin.label.fontSize;
        Rect rc = new Rect(0, 0, Screen.width, Screen.height);

        GUI.skin.label.fontSize = 100;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;

        GUI.color = new Color(0.3F, 0.3F, 0.3F, 0.3F);
        GUI.DrawTexture(rc, Texture2D.whiteTexture, ScaleMode.StretchToFill, false);

        GUI.color = Color.white * 10;

        GUILayout.BeginArea(rc);

        GUILayout.BeginVertical();

        GUILayout.FlexibleSpace();
        int count = Mathf.FloorToInt(countDownTime);
        if (count == 0)
        {
            GUILayout.Label("Action!!", GUILayout.Height(Screen.height));
        }
        else
        {
            GUILayout.Label(count.ToString(), GUILayout.Height(Screen.height));
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.EndArea();

        GUI.skin.label.fontSize = saveFontSize;
        GUI.color = Color.white;
    }
}