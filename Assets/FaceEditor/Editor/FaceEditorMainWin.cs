using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;

namespace FaceCapEditor
{
    public class FaceEditorMainWin : EditorWindow
    {

        public static FaceEditorMainWin window;
        public const float topBarHeight = 20f;

        /// <summary>
        /// the min subcut panel width
        /// </summary>
        public const float minSubCutWidth = 250f;
        /// <summary>
        /// the min resource panel width
        /// </summary>
        public const float minResourceWidth = 0f;
        /// <summary>
        /// the min timeline panel height 
        /// </summary>
        public const float minTimelineHeight = 400f;


        public const float minGroupSubCutWidth = 250f;


        private GUIStyle _resizerStyle;

        /// <summary>
        /// subcut resizer
        /// </summary>
        private Rect _subCutResizerRect;
        private bool _subCutIsResizing;
        private float _subCutResizerSize = 2f;


        /// <summary>
        /// groupSubcut resizer
        /// </summary>
        private Rect _groupSubCutResizerRect;
        private bool _groupSubCutIsResizing;
        private float _groupSubCutResizerSize = 2f;

        /// <summary>
        /// timeline resizer
        /// </summary>
        private Rect _timelineResizerRect;
        private bool _timelineIsResizing;
        private float _timelineResizerSize = 2f;


        /// <summary>
        /// privew cut resizer
        /// </summary>
        private Rect _previewCutResizerRect;
        private bool _previewCutResizing;
        private float _previewCutResizerSize = 2f;

        /// <summary>
        /// resource resizer
        /// </summary>
        private Rect _resourceResizerRect;
        private bool _resourceIsResizing;
        private float _resourceResizerSize = 2f;

        private float _subCutWidth = minSubCutWidth;
        private float _resourceWidth = minResourceWidth;
        private float _timelineHeight = minTimelineHeight;

        private WindowPanel _topbarPanel;
        public WindowTopPanel topbarPanel
        {
            get { return (WindowTopPanel)_topbarPanel; }
        }

        private WindowPanel _browPanel;
        public WindowBrowPanel browPanel
        {
            get { return (WindowBrowPanel) _browPanel; }
        }

        private WindowPanel _eyePanel;
        public WindowEyePanel eyePanel
        {
            get { return (WindowEyePanel)_eyePanel; }
        }

        private WindowPanel _cheekPanel;
        public WindowCheekPanel cheekPanel
        {
            get { return (WindowCheekPanel)_cheekPanel; }
        }

        private WindowPanel _mShapePanel;
        public WindowMShapePanel mShapePanel
        {
            get { return (WindowMShapePanel)_mShapePanel; }
        }

        private WindowPanel _otherPanel;
        public WindowOtherPanel otherPanel
        {
            get { return (WindowOtherPanel)_otherPanel; }
        }

        private WindowPanel _mouthPanel;
        public WindowMouthPanel mouthPanel
        {
            get { return (WindowMouthPanel)_mouthPanel; }
        }
        [MenuItem("剧情工具/表情编辑", false, 1)]
        public static void OpenEditorMainWin()
        {
            window = EditorWindow.GetWindow<FaceEditorMainWin>(false, "导演编辑器", true);
            window.Show();

            // OnInit is called after OnPanelEnable
            window.Init();
        }

        public void Init()
        {

            window.browPanel.OnInit();
            window.eyePanel.OnInit();
            window.cheekPanel.OnInit();
            window.mShapePanel.OnInit();
            window.mouthPanel.OnInit();
            window.otherPanel.OnInit();           
        }

        public static void StopClose()
        {
            if (window != null)
            {
                window.Stop();
                window.Close();
            }
        }

        public void Stop()
        {
           
        }

        private void OnEnable()
        {
            Rect rect = new Rect();
            _topbarPanel = new WindowTopPanel(this, rect);
            _browPanel = new WindowBrowPanel(this, rect);
            _cheekPanel = new WindowCheekPanel(this, rect);
            _eyePanel = new WindowEyePanel(this, rect);
            _mouthPanel = new WindowMouthPanel(this, rect);
            _otherPanel = new WindowOtherPanel(this, rect);
            _mShapePanel = new WindowMShapePanel(this, rect);
            _resizerStyle = new GUIStyle();
            _resizerStyle.normal.background = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;

            _topbarPanel.panelRect = new Rect(0, 0, position.width, topBarHeight);
            _browPanel.panelRect = new Rect(0, 0, position.width, topBarHeight);
            _cheekPanel.panelRect = new Rect(0, topBarHeight, _subCutWidth, position.height - topBarHeight - _timelineHeight);
            _eyePanel.panelRect = new Rect(_subCutWidth + _subCutResizerSize, topBarHeight, minGroupSubCutWidth/*position.width - _subCutWidth - _subCutResizerSize - _resourceWidth*/, position.height - topBarHeight - _timelineResizerSize - _timelineHeight);
            _mouthPanel.panelRect = new Rect(_subCutWidth + _subCutResizerSize, topBarHeight + groupSubCutPanel.panelRect.height + _timelineResizerSize, position.width - _subCutWidth - _subCutResizerSize - _resourceWidth, _timelineHeight);
            _otherPanel.panelRect = new Rect(0, topBarHeight + groupSubCutPanel.panelRect.height + _timelineResizerSize, _subCutWidth, _timelineHeight);
            _mShapePanel.panelRect = new Rect(_subCutWidth + minGroupSubCutWidth + _subCutResizerSize, topBarHeight, position.width - _subCutWidth - _subCutResizerSize - _resourceWidth - groupSubCutPanel.panelRect.width, position.height - topBarHeight - _timelineResizerSize - _timelineHeight);


            browPanel.OnPanelEnable();
            cheekPanel.OnPanelEnable();
            eyePanel.OnPanelEnable();
            mouthPanel.OnPanelEnable();
            otherPanel.OnPanelEnable();
            mShapePanel.OnPanelEnable();
        }

        private void OnDisable()
        {
            browPanel.OnPanelDisable();
            cheekPanel.OnPanelDisable();
            eyePanel.OnPanelDisable();
            mouthPanel.OnPanelDisable();
            otherPanel.OnPanelDisable();
            mShapePanel.OnPanelDisable();
        }

        private void OnSelectionChange()
        {
            browPanel.OnPanelSelectionChange();
            cheekPanel.OnPanelSelectionChange();
            eyePanel.OnPanelSelectionChange();
            mouthPanel.OnPanelSelectionChange();
            otherPanel.OnPanelSelectionChange();
            mShapePanel.OnPanelSelectionChange();
        }

        private void OnDestroy()
        {
            browPanel.OnPanelDestory();
            cheekPanel.OnPanelDestory();
            eyePanel.OnPanelDestory();
            mouthPanel.OnPanelDestory();
            otherPanel.OnPanelDestory();
            mShapePanel.OnPanelDestory();

            _browPanel = null;
            _cheekPanel = null;
            _eyePanel = null;
            _mouthPanel = null;
            _otherPanel = null;
            _mShapePanel = null;
        }

        private void OnGUI()
        {

            browPanel.panelRect = new Rect(0, 0, position.width, topBarHeight);
            cheekPanel.panelRect = new Rect(0, topBarHeight, _subCutWidth, position.height - topBarHeight - _timelineHeight);
            eyePanel.panelRect = new Rect(_subCutWidth + _subCutResizerSize, topBarHeight, minGroupSubCutWidth/*position.width - _subCutWidth - _subCutResizerSize - _resourceWidth*/, position.height - topBarHeight - _timelineResizerSize - _timelineHeight);
            mouthPanel.panelRect = new Rect(_subCutWidth + _subCutResizerSize, topBarHeight + groupSubCutPanel.panelRect.height + _timelineResizerSize, position.width - _subCutWidth - _subCutResizerSize - _resourceWidth, _timelineHeight);
            otherPanel.panelRect = new Rect(0, topBarHeight + groupSubCutPanel.panelRect.height + _timelineResizerSize, _subCutWidth, _timelineHeight);
            mShapePanel.panelRect = new Rect(_subCutWidth + minGroupSubCutWidth + _subCutResizerSize, topBarHeight, position.width - _subCutWidth - _subCutResizerSize - _resourceWidth - groupSubCutPanel.panelRect.width, position.height - topBarHeight - _timelineResizerSize - _timelineHeight);

            browPanel.OnDraw();
            cheekPanel.OnDraw();
            eyePanel.OnDraw();
            mouthPanel.OnDraw();
            if(otherPanel != null)
                otherPanel.OnDraw();
            DrawResizer();
            if(mShapePanel != null)
                mShapePanel.OnDraw();
            Repaint();

            ProcessEvents(Event.current);
            if (GUI.changed)
                Repaint();
        }


        private void DrawResizer()
        {

                // draw subCut panel resizer
            _subCutResizerRect = new Rect(_subCutWidth, topBarHeight, _subCutResizerSize, position.height - topBarHeight);
            GUILayout.BeginArea(_subCutResizerRect, _resizerStyle);
            GUILayout.EndArea();
            EditorGUIUtility.AddCursorRect(_subCutResizerRect, MouseCursor.ResizeHorizontal);


            // draw group Cut panel resizer
            _groupSubCutResizerRect = new Rect(_subCutWidth + groupSubCutPanel.panelRect.width, topBarHeight, _subCutResizerSize, groupSubCutPanel.panelRect.height);
            GUILayout.BeginArea(_groupSubCutResizerRect, _resizerStyle);
            GUILayout.EndArea();
            EditorGUIUtility.AddCursorRect(_groupSubCutResizerRect, MouseCursor.ResizeHorizontal);

            // draw PreivewCut panel resizer
            _previewCutResizerRect = new Rect(0, topBarHeight + groupSubCutPanel.panelRect.height, _subCutWidth, _previewCutResizerSize);
            GUILayout.BeginArea(_previewCutResizerRect, _resizerStyle);
            GUILayout.EndArea();
            EditorGUIUtility.AddCursorRect(_previewCutResizerRect, MouseCursor.ResizeVertical);


            // draw timeline panel resizer
            _timelineResizerRect = new Rect(_subCutWidth, topBarHeight + groupSubCutPanel.panelRect.height, position.width - _subCutWidth - _resourceWidth, _timelineResizerSize);
            GUILayout.BeginArea(_timelineResizerRect, _resizerStyle);
            GUILayout.EndArea();
            EditorGUIUtility.AddCursorRect(_timelineResizerRect, MouseCursor.ResizeVertical);
      
            //_resourceResizerRect = new Rect(position.width - _resourceWidth - _resourceResizerSize, topBarHeight, _resourceResizerSize, position.height - topBarHeight);
            //GUILayout.BeginArea(_resourceResizerRect, _resizerStyle);
            //GUILayout.EndArea();
            //EditorGUIUtility.AddCursorRect(_resourceResizerRect, MouseCursor.ResizeHorizontal);
        }


        private void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    {
                        if (e.button == 0 && _subCutResizerRect.Contains(e.mousePosition))
                        {
                            _subCutIsResizing = true;
                        }
                        else if (e.button == 0 && _timelineResizerRect.Contains(e.mousePosition))
                        {
                            _timelineIsResizing = true;
                        }
                        else if (e.button == 0 && _resourceResizerRect.Contains(e.mousePosition))
                        {
                            _resourceIsResizing = true;
                        }
                    }
                    break;

                case EventType.MouseUp:
                    _subCutIsResizing = false;
                    _timelineIsResizing = false;
                    _resourceIsResizing = false;
                    break;
            }

            ProcessResizeEvent(e);
        }

        private void ProcessResizeEvent(Event e)
        {
            if (_timelineIsResizing) // handle timeline panel resize event
            {
                float scaleHeight = position.height - e.mousePosition.y;
                if (scaleHeight > minTimelineHeight && scaleHeight < position.height - topBarHeight - 50)
                {
                    _timelineHeight = scaleHeight;
                    Repaint();
                }
            }
            else if (_subCutIsResizing) // 
            {
                float scaleWidth = e.mousePosition.x;
                if (scaleWidth > minSubCutWidth && scaleWidth < minSubCutWidth + 100)
                {
                    _subCutWidth = scaleWidth;
                    Repaint();
                }
            }
            else if (_resourceIsResizing) // handle resource panel resize event
            {
                float scaleWidth = position.width - e.mousePosition.x;
                if (scaleWidth > minResourceWidth && scaleWidth < minResourceWidth + 200)
                {
                    _resourceWidth = scaleWidth;
                    Repaint();
                }
            }
        }


        public void Update()
        {           
           /// viewPanel.onUpdate();
        }

        

    }
}