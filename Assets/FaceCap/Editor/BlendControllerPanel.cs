using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FaceCapEditor
{
    public class BlendControllerPanel: IAddKeyEnable
    {
        public const int panelPadding = 6;
        public const int dragButtonSize = 10;
        public Rect centerPanel;
        public WindowPanel parent;
        public Color highlighColor
        {
            get { return EditorGUIUtility.isProSkin ? new Color(0.65f, 0.65f, 1,0.3f) : new Color(0.1f, 0.1f, 0.1f, 0.3f); }
        }
        // 是否当前被选中
        ///public bool isSelected = false;
        private GUIStyle _boxStyle = null;

        // dragable area rect
        private Rect _centerPanelRect;
        // drag button rect
        private Rect _dragButtonRect;
        // the truely valid drag rect for blend controller's value 
        private Rect _validDragRect;
        // blend controller map button, from top, left, bottom, right 
        private Rect _selRect;
        private Rect[] _controlBtnRect;

        private BlendGridController _blendController;
        public BlendGridController blendController
        {
            get { return _blendController; }
        }

        // temp weights
        private float[] _weights;

        // 缩放相关
        private Rect _resizeRightRect;
        private Rect _resizeBottomRect;
        private GUIStyle _resizerStyle;

        // 是否在缩放大小
        private bool _resizeHorPressed = false;
        private bool _resizeVerPressed = false;
        public bool resizePressed
        {
            get { return _resizeHorPressed || _resizeVerPressed; }
        }

        // 是否在拖动控制点
        private bool _dragButtonPressed = false;
        public bool dragButtonPressed
        {
            set { _dragButtonPressed = value; }
            get { return _dragButtonPressed; }
        }

        public bool GetIsSelect
        {
            get { return blendController.GetIsSelect;  }
            set { blendController.GetIsSelect = value; }
        }

        public BlendControllerPanel(WindowPanel parent, Rect rect, BlendGridController blendController)
        {
            this.parent = parent;
            this.centerPanel = rect;
            _blendController = blendController;
        }

        public void Init()
        {
            _resizerStyle = new GUIStyle();
            _resizerStyle.normal.background = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;

            // init panel rect
            _dragButtonRect = new Rect(0, 0, dragButtonSize, dragButtonSize);
            _centerPanelRect = new Rect(panelPadding, panelPadding, centerPanel.width - panelPadding * 2, centerPanel.height - panelPadding * 2);
            _validDragRect = new Rect(_dragButtonRect.width / 2, _dragButtonRect.height / 2, _centerPanelRect.width - _dragButtonRect.width, _centerPanelRect.height - _dragButtonRect.height);
            _dragButtonRect.center = _validDragRect.center;
            _selRect = new Rect(_validDragRect.x + 3, _validDragRect.y + 3, _validDragRect.width + 5, _validDragRect.height + 5);
            // init controller buttons rects
            _controlBtnRect = new Rect[blendController.blendShapeIndexs.Count];

            // init _weights from shape 
            _weights = new float[blendController.blendShapeIndexs.Count];

            //if (parent.editKey != null)
            //{
            //    Vector2 pos = parent.editKey.GetControllerParamValue(blendController.controllerIndex);
            //    if (!float.IsPositiveInfinity(pos.x) || !float.IsPositiveInfinity(pos.y))
            //        _dragButtonRect.center = GetWindowPosFromNormalizedPos(pos);
            //}
        }

        public Vector2 GetNormalizedPos()
        {
            return GetNormalizedPosFromWindowPos(_dragButtonRect);
        }

        public void SetNormalizedPos(Vector2 pos)
        {
            _dragButtonRect.center = GetWindowPosFromNormalizedPos(pos);
        }

        public void OnUpdate(bool onfocus)
        {
            if (onfocus == false)
            {
                _dragButtonPressed = false;
                _resizeHorPressed = false;
                _resizeVerPressed = false;
            }

            if (FaceEditorMainWin.window.editKey != null)
            {
                // 编辑关键帧模式下
                if (_dragButtonPressed == false && onfocus == false)
                {
                    // 如果未进行拖拽并且需要更新控制点位置， 未进入动画区域的话，则不更新
                    /*if (!parent.editKey.HasEnterAnimatableRange())
                        return;*/

                    Vector2 pos = FaceEditorMainWin.window.editKey.GetControllerParamValue(blendController.controllerName);
                    if (!float.IsPositiveInfinity(pos.x) || !float.IsPositiveInfinity(pos.y))
                        _dragButtonRect.center = GetWindowPosFromNormalizedPos(pos);

                    
                }
                else
                {
                    PreviewBlendController();
                   
                }
            }
            else
            {
                // 编辑模板模式下
                PreviewBlendController();
            }

            
        }

        public void OnDraw()
        {
  
            if (_boxStyle == null)
                _boxStyle = GUI.skin.FindStyle("box");

            _centerPanelRect = new Rect(panelPadding, panelPadding, centerPanel.width - panelPadding * 2, centerPanel.height - panelPadding * 2);
            _validDragRect = new Rect(_dragButtonRect.width / 2, _dragButtonRect.height / 2, _centerPanelRect.width - _dragButtonRect.width, _centerPanelRect.height - _dragButtonRect.height);
            
            _controlBtnRect[(int)BlendGridController.ControllerDirection.Top] = new Rect(centerPanel.width / 2 - panelPadding / 2, 0, panelPadding, panelPadding);
            _controlBtnRect[(int)BlendGridController.ControllerDirection.Left] = new Rect(0, centerPanel.height / 2 - panelPadding / 2, panelPadding, panelPadding);
            _controlBtnRect[(int)BlendGridController.ControllerDirection.Bottom] = new Rect(centerPanel.width / 2 - panelPadding / 2, centerPanel.height - panelPadding, panelPadding, panelPadding);
            _controlBtnRect[(int)BlendGridController.ControllerDirection.Right] = new Rect(centerPanel.width - panelPadding, centerPanel.height / 2 - panelPadding / 2, panelPadding, panelPadding);

            GUILayout.BeginArea(centerPanel, _boxStyle);

     
            DrawDragButton();
            


            //draw selected rect
            
            ProcessMouseEvent(_selRect);
            ProcessMenuEvent(_selRect);
            if (blendController.GetIsSelect)
            {                     
                GUI.color = highlighColor;
                GUI.DrawTexture(_selRect, Slate.Styles.whiteTexture);
                GUI.color = Color.white;                
            }


            GUILayout.EndArea();
           



        }


        void ProcessMenuEvent(Rect rect)
        {
        
            if (Event.current.type == EventType.ContextClick && rect.Contains(Event.current.mousePosition))
            {
                // panel 右击鼠标弹出菜单
                var menu = new UnityEditor.GenericMenu();

                menu.AddItem(new GUIContent("设置上"), false, OnSetPanelMenu, new List<int> { 0 });
                menu.AddItem(new GUIContent("设置左"), false, OnSetPanelMenu, new List<int> { 1 });
                menu.AddItem(new GUIContent("设置下"), false, OnSetPanelMenu, new List<int> { 2 });
                menu.AddItem(new GUIContent("设置右"), false, OnSetPanelMenu, new List<int> { 3 });
                menu.AddItem(new GUIContent("还原"), false, OnSetPanelMenu, new List<int> { 4 });
                menu.ShowAsContext();

                Event.current.Use();
            }
            
        }


        void OnSetPanelMenu(object userData)
        {
            List<int> menuOption = (List<int>)userData;
            int value = menuOption[0];

            switch (value)
            {
                case 0:
                    SetNormalizedPos(new Vector2(0, blendController.upValue));
                    break;

                case 1:
                    SetNormalizedPos(new Vector2(blendController.leftValue,0));
                    break;

                case 2:
                    SetNormalizedPos(new Vector2(0, blendController.downValue));
                    break;

                case 3:
                    SetNormalizedPos(new Vector2(blendController.rightValue, 0));
                    break;

                case 4:
                    SetNormalizedPos(new Vector2(0, 0));
                    break;
            }
        }


        void ProcessMouseEvent(Rect rect)
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                if (rect.Contains(Event.current.mousePosition))
                {
           
                    blendController.GetIsSelect = !blendController.GetIsSelect;
                    if (FaceEditorMainWin.window != null && FaceEditorMainWin.window.editKey != null)
                    {
                        FaceEditorMainWin.window.editKey.ChangeAnimParamState(blendController.GetControllerName(), !blendController.GetIsSelect);
                    }
                    Event.current.Use();
                    
                }
            }                    
        }

        void DrawResizer()
        {
            // draw story panel resizer
            _resizeBottomRect = new Rect(0, centerPanel.height - 2, centerPanel.width, 4);
            GUILayout.BeginArea(_resizeBottomRect, _resizerStyle);
            GUILayout.EndArea();
            EditorGUIUtility.AddCursorRect(_resizeBottomRect, MouseCursor.ResizeVertical);

            _resizeRightRect = new Rect(centerPanel.width - 2, 0, 4, centerPanel.height);
            GUILayout.BeginArea(_resizeRightRect, _resizerStyle);
            GUILayout.EndArea();
            EditorGUIUtility.AddCursorRect(_resizeRightRect, MouseCursor.ResizeHorizontal);

            ProcessResizeEvent();
        }

        void ProcessResizeEvent()
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    _resizeVerPressed = false;
                    _resizeHorPressed = false;

                    if (Event.current.button == 0 && _resizeBottomRect.Contains(Event.current.mousePosition))
                    {
                        //Debug.Log("_resize BottomRect MouseDown");
                        _resizeVerPressed = true;
                        _resizeHorPressed = false;
                        _dragButtonPressed = false;
                        //Event.current.Use();
                    }
                    else if (Event.current.button == 0 && _resizeRightRect.Contains(Event.current.mousePosition))
                    {
                        //Debug.Log("_resize RightRect MouseDown");
                        _resizeHorPressed = true;
                        _resizeVerPressed = false;
                        _dragButtonPressed = false;
                        // Event.current.Use();
                    }
                    break;

                case EventType.MouseUp:
                    //Debug.Log("_resize MouseUp");
                    _resizeHorPressed = false;
                    _resizeVerPressed = false;
                    _dragButtonPressed = false;
                    break;

                case EventType.MouseDrag:
                    {
                        if (_resizeVerPressed && Event.current.mousePosition.y > 60)
                        {
                            centerPanel.height = Event.current.mousePosition.y;
                            _dragButtonRect.center = _validDragRect.center;
                            //parent.Repaint();
                            //Event.current.Use();
                        }
                        else if (_resizeHorPressed && Event.current.mousePosition.x > 60)
                        {
                            centerPanel.width = Event.current.mousePosition.x;
                            _dragButtonRect.center = _validDragRect.center;
                            //parent.Repaint();
                            //Event.current.Use();
                        }
                    }
                    break;
            }
        }

        void DrawDragButton()
        {
            GUILayout.BeginArea(_centerPanelRect, _boxStyle);

            ProcessDragEvents();
            GUI.Button(_dragButtonRect, "");

            GUILayout.EndArea();
        }

        void ProcessDragEvents()
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    if (Event.current.button == 0 && _dragButtonRect.Contains(Event.current.mousePosition))
                    {
                        //Debug.Log("mousue down: " + Event.current.mousePosition);
                        if (FaceEditorMainWin.window!= null)
                        {
                            FaceEditorMainWin.window.resetDragPressed();
                        }

                        _dragButtonPressed = true;
                        _resizeHorPressed = false;
                        _resizeVerPressed = false;
                        //Event.current.Use();
                    }
                    break;

                case EventType.MouseUp:
                    _dragButtonPressed = false;
                    _resizeHorPressed = false;
                    _resizeVerPressed = false;
                    break;

                case EventType.MouseDrag:
                    {
                        if (_dragButtonPressed)
                        {
                            // because the GUI's orignal position is Upper-Left Corner, so we use center pos
                            Vector2 centerDragPos = _dragButtonRect.center;

                            // do drag opertaion and set drag value for button
                            if (Event.current.mousePosition.x >= _validDragRect.x && Event.current.mousePosition.x <= _validDragRect.x + _validDragRect.width)
                                centerDragPos.x = Event.current.mousePosition.x;
                            else
                            {
                                if (Event.current.mousePosition.x < _validDragRect.x)
                                    centerDragPos.x = _validDragRect.x;

                                if (Event.current.mousePosition.x > _validDragRect.x + _validDragRect.width)
                                    centerDragPos.x = _validDragRect.x + _validDragRect.width;
                            }

                            if (Event.current.mousePosition.y >= _validDragRect.y && Event.current.mousePosition.y <= _validDragRect.y + _validDragRect.height)
                                centerDragPos.y = Event.current.mousePosition.y;
                            else
                            {
                                if (Event.current.mousePosition.y < _validDragRect.y)
                                    centerDragPos.y = _validDragRect.y;

                                if (Event.current.mousePosition.y > _validDragRect.y + _validDragRect.height)
                                    centerDragPos.y = _validDragRect.y + _validDragRect.height;
                            }

                            _dragButtonRect.center = centerDragPos;
                            //Event.current.Use();
                        }
                    }
                    break;
            }
        }

        public void UpdateDragButton(Vector2 delta)
        {
            Vector2 centerDragPos = _dragButtonRect.center;
            centerDragPos += delta;

            // do drag opertaion and set drag value for button
            if (centerDragPos.x + delta.x >= _validDragRect.x && centerDragPos.x + delta.x <= _validDragRect.x + _validDragRect.width)
                centerDragPos.x = centerDragPos.x + delta.x;
            else
            {
                if (centerDragPos.x + delta.x < _validDragRect.x)
                    centerDragPos.x = _validDragRect.x;

                if (centerDragPos.x + delta.x > _validDragRect.x + _validDragRect.width)
                    centerDragPos.x = _validDragRect.x + _validDragRect.width;
            }

            if (centerDragPos.y + delta.y >= _validDragRect.y && centerDragPos.y + delta.y <= _validDragRect.y + _validDragRect.height)
                centerDragPos.y = centerDragPos.y + delta.y;
            else
            {
                if (centerDragPos.y + delta.y < _validDragRect.y)
                    centerDragPos.y = _validDragRect.y;

                if (centerDragPos.y + delta.y > _validDragRect.y + _validDragRect.height)
                    centerDragPos.y = _validDragRect.y + _validDragRect.height;
            }

            _dragButtonRect.center = centerDragPos;
        }

    
        void CalculateBlendShapeValue(Vector2 normalizedPos)
        {
            // set drag value for top controller
            if (blendController.top != -1)
            {
                _weights[(int)BlendGridController.ControllerDirection.Top] = BlendGridController.GetWeightFromPosition(BlendGridController.ControllerDirection.Top, normalizedPos);
            }

            // set drag value for left controller
            if (blendController.left != -1)
            {
                _weights[(int)BlendGridController.ControllerDirection.Left] = BlendGridController.GetWeightFromPosition(BlendGridController.ControllerDirection.Left, normalizedPos);
            }

            // set drag value for bottom controller
            if (blendController.bottom != -1)
            {
                _weights[(int)BlendGridController.ControllerDirection.Bottom] = BlendGridController.GetWeightFromPosition(BlendGridController.ControllerDirection.Bottom, normalizedPos);
            }

            // set drag value for right controller
            if (blendController.right != -1)
            {
                _weights[(int)BlendGridController.ControllerDirection.Right] = BlendGridController.GetWeightFromPosition(BlendGridController.ControllerDirection.Right, normalizedPos);
            }
        }

        Vector2 GetWindowPosFromNormalizedPos(Vector2 normalizedPos)
        {
            if (blendController.leftValue > blendController.rightValue)
            {
                float x = _validDragRect.center.x + ((-normalizedPos.x * _validDragRect.width) / 2);
                float y = _validDragRect.center.y + (((-normalizedPos.y) * _validDragRect.height) / 2);
                return new Vector2(x, y);
            }
            else
            {
                float x = _validDragRect.center.x + ((normalizedPos.x * _validDragRect.width) / 2);
                float y = _validDragRect.center.y + (((-normalizedPos.y) * _validDragRect.height) / 2);
                return new Vector2(x, y);
            }
        }

        Vector2 GetNormalizedPosFromWindowPos(Rect dragRect)
        {
            float x = (dragRect.center.x - _validDragRect.center.x) / (_validDragRect.width / 2);
            float y = (dragRect.center.y - _validDragRect.center.y) / (_validDragRect.height / 2);
            if (blendController.leftValue > blendController.rightValue)
            {
                return new Vector2(-x, -y);
            }

            return new Vector2(x, -y);
        }

        void PreviewBlendController()
        {
            Vector2 normalizedPos = GetNormalizedPosFromWindowPos(_dragButtonRect);
            if (FaceEditorMainWin.window.currentHandler != null)
            {
                if (blendController != null)
                {                    
                    FaceEditorMainWin.window.currentHandler.SetBlenderShapeByCtrlName(blendController.controllerName, normalizedPos);
                }
            }
            
        }

        public void Reset()
        {
            _dragButtonRect.center = GetWindowPosFromNormalizedPos(Vector2.zero);
            PreviewBlendController();
        }

        public string GetPanelControllerName()
        {
            return blendController.controllerName;
        }
    }

}