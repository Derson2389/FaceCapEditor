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

        public static FaceEditorMainWin _window;
        private FaceControllerComponent _faceCtrlComp = null;
        public FaceControllerComponent FaceCtrlComp
        {
            get { return _faceCtrlComp;  }
            set { _faceCtrlComp = value; }
        }

        public BlendShapeCtrlClip Clip = null;
        public PrevizCtrlHandler currentHandler = null;

        private bool _onFocus = false;
        private float _panelEditTime = 0;
        private List<IAddKeyEnable> _panelList;
        private Vector2 _mousePos;

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

        public bool isAllSelected = false;
        
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
        public static void OpenEditorMainWin(BlendShapeCtrlClip clip, BlendShapeCtrlClip.BSEditKey newEditKey)  
        {
            _window = EditorWindow.GetWindow<FaceEditorMainWin>(false, "表情编辑器", true);   
            _window.minSize = new Vector2(850 , 680);   
            _window.Show();
            // OnInit is called after OnPanelEnable
            //BlenderShapesManager.ConfigTxt = clip.CtrlConfigDataFile;
            //BlenderShapesManager.LoadConfig(clip.FaceCtr);
            _window.Clip = clip;
            _window.currentHandler = clip.CtrlHandler;
            _window.FaceCtrlComp = clip.FaceCtr;
            _window.editKey = newEditKey; 
            _window.Init();   
            Debug.LogWarning("Init");   
        }

        public static FaceEditorMainWin window
        {
            get
            {
                if (_window == null)   
                {
                    _window = EditorWindow.GetWindow<FaceEditorMainWin>();   
                } 

                return _window;
            }
        }

        public void InserPanelList(IAddKeyEnable panel)
        {
            if (_panelList != null && !_panelList.Contains(panel))
            {
                _panelList.Add(panel);
            }
        }

        public void Init()
        {

            _panelList = new List<IAddKeyEnable>();
            _panelList.Clear();

            window.topbarPanel.OnInit();
            window.browPanel.OnInit();
            window.eyePanel.OnInit();
            window.cheekPanel.OnInit();
            window.mShapePanel.OnInit();
            window.mouthPanel.OnInit();
            window.otherPanel.OnInit();

            if (editKey != null)
            {
                _panelEditTime = editKey.GetCurrentTime();
            }

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
            Debug.LogWarning("OnEnable");   

          
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

            if (Clip != null)
            {
                currentHandler = Clip.CtrlHandler;
                if(Clip.editKey!= null)
                    editKey = Clip.editKey;
                else 
                {
                    Clip.EditKeyable(0);
                    editKey = Clip.editKey;
                } 
                topbarPanel.OnPanelEnable();          
                browPanel.OnPanelEnable();
                cheekPanel.OnPanelEnable(); 
                eyePanel.OnPanelEnable();
                mouthPanel.OnPanelEnable();
                otherPanel.OnPanelEnable();
                mShapePanel.OnPanelEnable();  
            }
            Slate.CutsceneUtility.onSelectionChange += OnCutsceneSelectChanged;

        }   

        private void OnDisable()
        {
            Debug.LogWarning("OnDisable");
            browPanel.OnPanelDisable();
            cheekPanel.OnPanelDisable();
            eyePanel.OnPanelDisable();
            mouthPanel.OnPanelDisable();
            otherPanel.OnPanelDisable();
            mShapePanel.OnPanelDisable();
            topbarPanel.OnPanelDisable();

            Slate.CutsceneUtility.onSelectionChange -= OnCutsceneSelectChanged;
            editKey = null;
              
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
            Debug.LogWarning("OnSelectionChange");
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
            Debug.LogWarning("OnDestroy");
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
            //avoid edit when compiling
            if (EditorApplication.isCompiling)
            {
                ShowNotification(new GUIContent("Compiling\n...Please wait..."));
                return;
            }

            if (FaceCtrlComp!= null && FaceEditorMainWin.window != null && FaceEditorMainWin.window.currentHandler != null && FaceEditorMainWin.window.currentHandler.controllerList.Count>0)
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

                ProcessShortKey();
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

            if (GUI.changed)
                Repaint();
        }


        


        void ProcessShortKey()
        {
            var e = Event.current;
            if (e.type == EventType.KeyDown && !e.shift)
            {

                if (e.keyCode == KeyCode.S)
                {
                    AddKeyframe();
                    e.Use();
                }
            }

            if (e.type == EventType.KeyDown )
            {
                if (e.keyCode == KeyCode.A)
                {
                    AddKeyframe(true);
                    e.Use();
                }
            }
            
        }


        public void resetDragPressed()
        {
            for (int i = 0; i < _panelList.Count; i++)
            {

                if (_panelList[i] is BlendControllerPanel)
                {
                    var panel = _panelList[i] as BlendControllerPanel;
                    panel.dragButtonPressed = false;


                }
                if (_panelList[i] is BlendSlideControllerPanel)
                {
                    var panel = _panelList[i] as BlendSlideControllerPanel;
                    panel.dragButtonPressed = false;
                }
            }
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

        /// <summary>
        /// 保存编辑后的关键帧数据
        /// </summary>
        public void AddKeyframe(bool all = false)
        {
            // 编辑模板模式下不能保存关键帧数据
            if (editKey == null)
                return;

            // 保存关键帧  
            for (int i = 0; i < _panelList.Count; i++)
            {
                // 保存动画关键帧blendControllerKey数据
                if ((_panelList[i].GetIsSelect || all) && _panelList[i] is BlendControllerPanel)
                {
                    var panel = _panelList[i] as BlendControllerPanel;
                    Vector2 normalizedPos = panel.GetNormalizedPos();
                    editKey.SetControllerParamValue(panel.GetPanelControllerName(), normalizedPos);
                }
                if ((_panelList[i].GetIsSelect|| all) && _panelList[i] is BlendSlideControllerPanel)
                {
                    var panel = _panelList[i] as BlendSlideControllerPanel;
                    Vector2 normalizedPos = panel.GetNormalizedPos();
                    editKey.SetControllerParamValue(panel.GetPanelControllerName(), normalizedPos);
                }
            }

            editKey.Save(all);

        }

        public void SelectAll()
        {
            isAllSelected = !isAllSelected;
            for (int i = 0; i < _panelList.Count;i++)
            {
                _panelList[i].GetIsSelect = isAllSelected;
                editKey.ChangeAnimParamState(_panelList[i].GetPanelControllerName(), !isAllSelected);
            }
            
        }
         
    
         
        public void Update()
        {
            
            if (editKey != null)
            {
                // 如果Timeline未进入当前编辑clip, 则不进行更新
                if (!editKey.HasEnterEditClip())
                    return;

                // 判断是否更新了编辑时间点
                if (Mathf.Abs(_panelEditTime - editKey.GetCurrentTime()) > FaceEditHelper.PROXIMITY_TOLERANCE)
                {
                    _panelEditTime = editKey.GetCurrentTime();
                   

                   /// _willRepaint = true;
                }
                _selfUpdate(_onFocus);

            }
            else
            {
                // 更新模型预览效果
                _selfUpdate(_onFocus);
            }
            
        }

        public void _selfUpdate(bool _onFocus)
        {

            if (window != null && window.browPanel != null)
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