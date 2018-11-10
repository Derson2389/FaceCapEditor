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
        private FaceControllerComponent _faceCtrlComp = null;
        public FaceControllerComponent FaceCtrlComp
        {
            get { return _faceCtrlComp;  }
            set { _faceCtrlComp = value; }
        }

        private bool _onFocus = false;

        public const float topBarHeight = 20f;
        /// <summary>
        /// the min subcut panel width
        /// </summary>
        public const float minBrowWidth = 250f;
        /// <summary>
        /// the min resource panel width
        /// </summary>
        public const float minResourceWidth = 0f;
        /// <summary>
        /// the min timeline panel height 
        /// </summary>
        public const float minMouthHeight = 400f;


        public const float minGroupSubCutWidth = 250f;


        private GUIStyle _resizerStyle;

        /// <summary>
        /// top resizer
        /// </summary>
        private Rect _topResizerRect;
        private bool _topIsResizing;
        private float _topResizerSize = 2f;

        private BlendShapeCtrlClip.BSEditKey _editKey = null;
        public BlendShapeCtrlClip.BSEditKey editKey
        {
            get { return _editKey;  } 
            set { _editKey = value; }
        }


        /// <summary>
        /// brow resizer
        /// </summary>
        private Rect _browResizerRect;
        private bool _browIsResizing;
        private float _browResizerSize = 2f;

        /// <summary>
        /// eye resizer
        /// </summary>
        private Rect _eyeResizerRect;
        private bool _eyeIsResizing;
        private float _eyeResizerSize = 2f;

        /// <summary>
        /// cheek cut resizer
        /// </summary>
        private Rect _cheekResizerRect;
        private bool _cheekResizing;
        private float _cheekResizerSize = 2f;

        /// <summary>
        /// Mouth resizer
        /// </summary>
        private Rect _mouthResizerRect;
        private bool _mouthIsResizing;
        private float _mouthResizerSize = 2f;


        /// <summary>
        /// Mouth Shape resizer
        /// </summary>
        private Rect _mShapeResizerRect;
        private bool _mShapeResizing;
        private float _mShapeResizerSize = 2f;

        /// <summary>
        /// Other resizer
        /// </summary>
        private Rect _otherResizerRect;
        private bool _otherResizing;
        private float _otherResizerSize = 2f;


        private float _browWidth = minBrowWidth;
        private float _mShapeWidth = 260;
          
        private const float _brawHeight = 160;
        private const float _cheekHeight = 160;
        private const float _eyeHeight = 260;

        private const float _mShapeHeight = _brawHeight + _cheekHeight + _eyeHeight;

        private float _resourceWidth = minResourceWidth;
        private float _MouthHeight = minMouthHeight;

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
        public static void OpenEditorMainWin(FaceControllerComponent comp, BlendShapeCtrlClip.BSEditKey newEditKey)
        {
            window = EditorWindow.GetWindow<FaceEditorMainWin>(false, "表情编辑器", true);
            window.minSize = new Vector2(760 , 680);
            window.Show();
            // OnInit is called after OnPanelEnable
            window.FaceCtrlComp = comp;
            window.editKey = newEditKey;
            window.Init();
        }

        public void Init()
        {
            window.topbarPanel.OnInit();
            window.browPanel.OnInit();
            window.eyePanel.OnInit();
            window.cheekPanel.OnInit();
            window.mShapePanel.OnInit();
            window.mouthPanel.OnInit();
            window.otherPanel.OnInit();           
        }


        public void ResetFaceCompoent(FaceControllerComponent faceCtrl)
        {
            if (faceCtrl == null)
            {
                return;
            }
            _faceCtrlComp = faceCtrl;
          
        }

        public static void StopClose()
        {
            if (window != null)
            {
                //window.Stop();
                window.Close();
            }
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

            topbarPanel.panelRect = new Rect(0, 0, position.width, topBarHeight);
            browPanel.panelRect = new Rect(0, topBarHeight, position.width - _mShapeWidth, _brawHeight);

            eyePanel.panelRect = new Rect(0, topBarHeight + browPanel.panelRect.height, position.width - _mShapeWidth, _eyeHeight);
            cheekPanel.panelRect = new Rect(0, topBarHeight + browPanel.panelRect.height + eyePanel.panelRect.height, position.width - _mShapeWidth, _cheekHeight);
            mouthPanel.panelRect = new Rect(0, topBarHeight + _mShapeHeight, position.width - _mShapeWidth, _MouthHeight);

            mShapePanel.panelRect = new Rect(position.width - _mShapeWidth + _mouthResizerSize, topBarHeight, _mShapeWidth, _mShapeHeight);
            otherPanel.panelRect = new Rect(position.width - _mShapeWidth + _mouthResizerSize, topBarHeight + _mShapeHeight, _mShapeWidth, position.height - _mShapeHeight);

            topbarPanel.OnPanelEnable();
            browPanel.OnPanelEnable();
            cheekPanel.OnPanelEnable();
            eyePanel.OnPanelEnable();
            mouthPanel.OnPanelEnable();
            otherPanel.OnPanelEnable();
            mShapePanel.OnPanelEnable();

            Slate.CutsceneUtility.onSelectionChange += OnCutsceneSelectChanged;
        }

        private void OnDisable()
        {
            browPanel.OnPanelDisable();
            cheekPanel.OnPanelDisable();
            eyePanel.OnPanelDisable();
            mouthPanel.OnPanelDisable();
            otherPanel.OnPanelDisable();
            mShapePanel.OnPanelDisable();
            topbarPanel.OnPanelDisable();

            Slate.CutsceneUtility.onSelectionChange -= OnCutsceneSelectChanged;
        }


        void OnCutsceneSelectChanged(Slate.IDirectable target)
        {
            if (target is BlendShapeCtrlClip)
            {
                BlendShapeCtrlClip clip = (BlendShapeCtrlClip)target;
                clip.EditKeyable(0);
            }
        }



        private void OnSelectionChange()
        {

            browPanel.OnPanelSelectionChange();
            cheekPanel.OnPanelSelectionChange();
            eyePanel.OnPanelSelectionChange();
            mouthPanel.OnPanelSelectionChange();
            otherPanel.OnPanelSelectionChange();
            mShapePanel.OnPanelSelectionChange();
            topbarPanel.OnPanelSelectionChange();
        }

        private void OnDestroy()
        {
            browPanel.OnPanelDestory();
            cheekPanel.OnPanelDestory();
            eyePanel.OnPanelDestory();
            mouthPanel.OnPanelDestory();
            otherPanel.OnPanelDestory();
            mShapePanel.OnPanelDestory();
            topbarPanel.OnPanelDestory();

            _browPanel = null;
            _cheekPanel = null;
            _eyePanel = null;
            _mouthPanel = null;
            _otherPanel = null;
            _mShapePanel = null;
            _topbarPanel = null;
        }

        private void OnGUI()
        {
            if (FaceCtrlComp!= null && BlenderShapesManager.controllerList.Count>0)
            {
                topbarPanel.panelRect = new Rect(0, 0, position.width, topBarHeight);
                browPanel.panelRect = new Rect(0, topBarHeight, position.width - _mShapeWidth, _brawHeight);

                eyePanel.panelRect = new Rect(0, topBarHeight + browPanel.panelRect.height, position.width - _mShapeWidth, _eyeHeight);
                cheekPanel.panelRect = new Rect(0, topBarHeight + browPanel.panelRect.height + eyePanel.panelRect.height, position.width - _mShapeWidth, _cheekHeight);
                mouthPanel.panelRect = new Rect(0, topBarHeight + _mShapeHeight, position.width - _mShapeWidth, _MouthHeight);

                mShapePanel.panelRect = new Rect(position.width - _mShapeWidth + _mouthResizerSize, topBarHeight, _mShapeWidth, _mShapeHeight);
                otherPanel.panelRect = new Rect(position.width - _mShapeWidth + _mouthResizerSize, topBarHeight + _mShapeHeight, _mShapeWidth, position.height - _mShapeHeight);
                topbarPanel.OnDraw();
                browPanel.OnDraw();
                cheekPanel.OnDraw();
                eyePanel.OnDraw();
                mouthPanel.OnDraw();
                if (otherPanel != null)
                    otherPanel.OnDraw();

                if (mShapePanel != null)
                    mShapePanel.OnDraw();

                DrawResizer();
                Repaint();

            }
            else
            {
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("请先进行加载配置操作！");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
            }


            

            ProcessEvents(Event.current);
            if (GUI.changed)
                Repaint();
        }

        private void DrawResizer()
        {
            // draw top panel resizer
            _topResizerRect = new Rect(0, topBarHeight, position.width , _topResizerSize);
            GUILayout.BeginArea(_topResizerRect, _resizerStyle);
            GUILayout.EndArea();
            EditorGUIUtility.AddCursorRect(_topResizerRect, MouseCursor.ResizeHorizontal);

            //// draw Mouth Shape panel resizer
            _mShapeResizerRect = new Rect(position.width - _mShapeWidth + _mouthResizerSize, topBarHeight, _mouthResizerSize, mShapePanel.panelRect.height);
            GUILayout.BeginArea(_mShapeResizerRect, _resizerStyle);
            GUILayout.EndArea();
            EditorGUIUtility.AddCursorRect(_mShapeResizerRect, MouseCursor.ResizeHorizontal);

            _mShapeResizerRect = new Rect(position.width - _mShapeWidth + _mouthResizerSize, topBarHeight + mShapePanel.panelRect.height,  mShapePanel.panelRect.width, _mouthResizerSize);
            GUILayout.BeginArea(_mShapeResizerRect, _resizerStyle);
            GUILayout.EndArea();
            EditorGUIUtility.AddCursorRect(_mShapeResizerRect, MouseCursor.ResizeHorizontal);


            //// draw other resizer
            _otherResizerRect = new Rect(position.width - _mShapeWidth + _mouthResizerSize, otherPanel.panelRect.height , _mouthResizerSize, mShapePanel.panelRect.height);
            GUILayout.BeginArea(_otherResizerRect, _resizerStyle);
            GUILayout.EndArea();
            EditorGUIUtility.AddCursorRect(_otherResizerRect, MouseCursor.ResizeHorizontal);

            //// draw Mouth panel resizer
            _mouthResizerRect = new Rect(0, topBarHeight + mShapePanel.panelRect.height, mouthPanel.panelRect.width, _mouthResizerSize);
            GUILayout.BeginArea(_mouthResizerRect, _resizerStyle);
            GUILayout.EndArea();
            EditorGUIUtility.AddCursorRect(_mouthResizerRect, MouseCursor.ResizeVertical);

            //// draw cheek panel resizer
            _cheekResizerRect = new Rect(0, topBarHeight + browPanel.panelRect.height + _eyeResizerSize + eyePanel.panelRect.height, eyePanel.panelRect.width, _cheekResizerSize);
            GUILayout.BeginArea(_cheekResizerRect, _resizerStyle);
            GUILayout.EndArea();
            EditorGUIUtility.AddCursorRect(_cheekResizerRect, MouseCursor.ResizeVertical);

            _eyeResizerRect = new Rect(0, topBarHeight + browPanel.panelRect.height + _eyeResizerSize , browPanel.panelRect.width, _eyeResizerSize);
            GUILayout.BeginArea(_eyeResizerRect, _resizerStyle);
            GUILayout.EndArea();
            EditorGUIUtility.AddCursorRect(_eyeResizerRect, MouseCursor.ResizeHorizontal);
        }

        void OnFocus()
        {
            _onFocus = true;           
        }

        void OnLostFocus()
        {
            _onFocus = false;
        }


        private void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    {
                        //if (e.button == 0 && _subCutResizerRect.Contains(e.mousePosition))
                        //{
                        //    _subCutIsResizing = true;
                        //}
                        //else if (e.button == 0 && _timelineResizerRect.Contains(e.mousePosition))
                        //{
                        //    _timelineIsResizing = true;
                        //}
                        //else if (e.button == 0 && _resourceResizerRect.Contains(e.mousePosition))
                        //{
                        //    _resourceIsResizing = true;
                        //}
                    }
                    break;

                case EventType.MouseUp:
                    //_subCutIsResizing = false;
                    //_timelineIsResizing = false;
                    //_resourceIsResizing = false;
                    break;
            }

            ProcessResizeEvent(e);
        }

        private void ProcessResizeEvent(Event e)
        {
            //if (_timelineIsResizing) // handle timeline panel resize event
            //{
            //    float scaleHeight = position.height - e.mousePosition.y;
            //    if (scaleHeight > minTimelineHeight && scaleHeight < position.height - topBarHeight - 50)
            //    {
            //        _timelineHeight = scaleHeight;
            //        Repaint();
            //    }
            //}
            //else if (_subCutIsResizing) // 
            //{
            //    float scaleWidth = e.mousePosition.x;
            //    if (scaleWidth > minSubCutWidth && scaleWidth < minSubCutWidth + 100)
            //    {
            //        _subCutWidth = scaleWidth;
            //        Repaint();
            //    }
            //}
            //else if (_resourceIsResizing) // handle resource panel resize event
            //{
            //    float scaleWidth = position.width - e.mousePosition.x;
            //    if (scaleWidth > minResourceWidth && scaleWidth < minResourceWidth + 200)
            //    {
            //        _resourceWidth = scaleWidth;
            //        Repaint();
            //    }
            //}
        }

        public void Update()
        {
            if (FaceCtrlComp == null && Selection.activeGameObject != null)               
            {
                var shapesCtrl = Selection.activeGameObject.GetComponent<FaceControllerComponent>();
                if (shapesCtrl != null && shapesCtrl != FaceCtrlComp)
                {
                    ResetFaceCompoent(shapesCtrl);
                }
            }
           
            if (window!= null && window.browPanel != null)
            {
                window.browPanel.Update(_onFocus);
            }

            if (window != null && window.mShapePanel != null)
            {
                window.mShapePanel.Update(_onFocus);
            }

            if (window != null && window.otherPanel != null)
            {
                window.otherPanel.Update(_onFocus);
            }
            if (window != null && window.otherPanel != null)
            {
                window.eyePanel.Update(_onFocus);
            }

            if (window != null && window.mouthPanel != null)
            {
                window.mouthPanel.Update(_onFocus);
            }

            if (window != null && window.cheekPanel != null)
            {
                window.cheekPanel.Update(_onFocus);
            }

        }

    }
}