using UnityEditor;
using UnityEngine;

namespace FaceCapEditor
{
    public class BlendSlideControllerPanel
    {
        public const int panelPadding = 6;
        public const int dragButtonSize = 10;

        public Rect centerPanel;
        public WindowPanel parent;

        // 是否当前被选中
        public bool isSelected = false;
        private GUIStyle _boxStyle = null;

        // dragable area rect
        private Rect _centerPanelRect;
        // drag button rect
        private Rect _dragButtonRect;
        // the truely valid drag rect for blend controller's value 
        private Rect _validDragRect;
        // blend controller map button, from top, left, bottom, right 
        private Rect[] _controlBtnRect;

        private BlendXController _blendControllerX;
        public BlendXController blendControllerX
        {
            get { return _blendControllerX; }
        }

        private BlendYController _blendControllerY;
        public BlendYController blendControllerY
        {
            get { return _blendControllerY; }
        }

        private float _HorizontalSliderValue = float.PositiveInfinity;
        public float HorizontalSliderValue
        {
            set { _HorizontalSliderValue = value; }
            get { return _HorizontalSliderValue;  }
        }

        private float _VerticalSliderValue = float.PositiveInfinity;
        public float VerticalSliderValue
        {
            set { _VerticalSliderValue = value; }
            get { return _VerticalSliderValue;  }
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
            get { return _dragButtonPressed; }
        }

        public BlendSlideControllerPanel(WindowPanel parent, Rect rect, BlendXController blendControllerX = null, BlendYController blendControllerY = null)
        {
            this.parent = parent;
            this.centerPanel = rect;
            _blendControllerX = blendControllerX;
            _blendControllerY = blendControllerY;
        }

        public void Init()
        {
            _resizerStyle = new GUIStyle();
            _resizerStyle.normal.background = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;

            
        }

        

        public void OnUpdate(bool onfocus)
        {
            if (onfocus == false)
            {
                _dragButtonPressed = false;
                _resizeHorPressed = false;
                _resizeVerPressed = false;
            }
           
        }

        public void OnDraw(Vector2 size)
        {
            if (blendControllerX != null)
            {

                HorizontalSliderValue = GUILayout.HorizontalSlider(0, -1.00f, 1.00f, GUILayout.Width(size.x), GUILayout.Height(size.y));
            }     
            
            if(blendControllerY != null)
            {
               
                VerticalSliderValue = GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(size.x), GUILayout.Height(size.y));
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

        void DrawSelectionBtns()
        {
            //for (int i = (int)BlendGridController.ControllerDirection.Top; i <= (int)BlendGridController.ControllerDirection.Right; i++)
            //{
            //    GUIStyle btnStyle = parent.unsetStyle;
            //    string btnName = "";

            //    if (blendController.blendShapeIndexs[i] != -1)
            //    {
            //        btnStyle = parent.setStyle;
            //        btnName = blendController.blendShapeIndexs[i].ToString();
            //    }

            //    if (GUI.Button(_controlBtnRect[i], btnName, btnStyle))
            //    {
            //        GenericMenu markerMenu = new GenericMenu();

            //        for (int b = 0; b < parent.shape.blendShapes.Count; b++)
            //        {
            //            // 如果当前是编辑模板模式，才能绑定blendshape
            //            if (parent.editKey == null)
            //            {
            //                // i是blendshape在BlendController里面的index, b是blendshape在shape里面的index 
            //                if (blendController.blendShapeIndexs[i] == b)
            //                    markerMenu.AddItem(new GUIContent(parent.shape.blendShapes[b].blendableName), true, OnBindBlendShapeMenu, new List<int> { i, b });
            //                else
            //                    markerMenu.AddItem(new GUIContent(parent.shape.blendShapes[b].blendableName), false, OnBindBlendShapeMenu, new List<int> { i, b });
            //            }
            //            else
            //            {
            //                markerMenu.AddDisabledItem(new GUIContent(parent.shape.blendShapes[b].blendableName));
            //            }
            //        }

            //        markerMenu.ShowAsContext();
            //    }
            //}
        }

        /// <summary>
        /// callback for GenericMenu selection
        /// </summary>
        /// <param name="userData">the userData is ControllerOption instance</param>
        void OnBindBlendShapeMenu(object userData)
        {
            //List<int> menuOption = (List<int>)userData;
            //int index = menuOption[0];
            //int value = menuOption[1];

            //Undo.RecordObject(parent.lipSync, "Change LipSync");

            //if (blendController.blendShapeIndexs[index] == value)
            //    blendController.blendShapeIndexs[index] = -1;
            //else
            //    blendController.blendShapeIndexs[index] = value;

            //// 保存模板
            //EditorUtility.SetDirty(parent.lipSync);
        }

        void CalculateBlendShapeValue(float sliderValue)
        {

            if (blendControllerX != null)
            {                
                // set drag value for left controller
                if (blendControllerX.left != -1)
                {
                    _weights[(int)BlendXController.ControllerDirection.Left] = BlendXController.GetWeightFromPosition(BlendXController.ControllerDirection.Left, sliderValue);
                }

                // set drag value for right controller
                if (blendControllerX.right != -1)
                {
                    _weights[(int)BlendXController.ControllerDirection.Right] = BlendXController.GetWeightFromPosition(BlendXController.ControllerDirection.Right, sliderValue);
                }
                
            }
            if(blendControllerY != null)
            {
                // set drag value for top controller
                if (blendControllerY.top != -1)
                {
                    _weights[(int)BlendYController.ControllerDirection.Top] = BlendYController.GetWeightFromPosition(BlendYController.ControllerDirection.Top, sliderValue);
                }

                // set drag value for bottom controller
                if (blendControllerY.bottom != -1)
                {
                    _weights[(int)BlendYController.ControllerDirection.Bottom] = BlendYController.GetWeightFromPosition(BlendYController.ControllerDirection.Bottom, sliderValue);
                }
            }           
        }

        Vector2 GetWindowPosFromNormalizedPos(Vector2 normalizedPos)
        {
            float x = _validDragRect.center.x + ((normalizedPos.x * _validDragRect.width) / 2);
            float y = _validDragRect.center.y + ((normalizedPos.y * _validDragRect.height) / 2);
            return new Vector2(x, y);
        }

        Vector2 GetNormalizedPosFromWindowPos(Rect dragRect)
        {
            float x = (dragRect.center.x - _validDragRect.center.x) / (_validDragRect.width / 2);
            float y = (dragRect.center.y - _validDragRect.center.y) / (_validDragRect.height / 2);
            return new Vector2(x, y);
        }

        public float GetSliderValueFromWindow()
        {
            if (blendControllerX != null)
            {
                return VerticalSliderValue;
            }
            if (blendControllerY != null)
            {
                return HorizontalSliderValue;
            }
            return float.PositiveInfinity;
        }

        void PreviewBlendController()
        {
            float sliderValue = GetSliderValueFromWindow();
            CalculateBlendShapeValue(sliderValue);

            if (blendControllerX != null)
            {
                for (int i = 0; i < blendControllerX.blendShapeIndexs.Count; i++)
                {
                    int blendShapeIndex = blendControllerX.blendShapeIndexs[i];
                    if (blendShapeIndex != -1)
                    {
                        // 使用BlendController面板映射的值
                        float weight = _weights[i];

                        //// 对于PositiveInfinity值，使用原始shape里面的weight
                        //if (float.IsPositiveInfinity(_weights[i]))
                        //    weight = parent.shape.blendShapes[blendShapeIndex].weight;

                        //// 对于编辑关键帧模式， 如果是PositiveInfinity的话，还需要乘以marker的强度系数
                        //if (parent.editKey != null && float.IsPositiveInfinity(_weights[i]))
                        //{
                        //    weight = weight * parent.editKey.marker.intensity;
                        //}

                        //parent.lipSync.blendSystem.SetBlendableValue(parent.shape.blendShapes[blendShapeIndex].blendableIndex, weight);
                    }
                }
            }

            if (blendControllerY != null)
            {
                for (int i = 0; i < blendControllerY.blendShapeIndexs.Count; i++)
                {
                    int blendShapeIndex = blendControllerY.blendShapeIndexs[i];
                    if (blendShapeIndex != -1)
                    {
                        // 使用BlendController面板映射的值
                        float weight = _weights[i];

                        //// 对于PositiveInfinity值，使用原始shape里面的weight
                        //if (float.IsPositiveInfinity(_weights[i]))
                        //    weight = parent.shape.blendShapes[blendShapeIndex].weight;

                        //// 对于编辑关键帧模式， 如果是PositiveInfinity的话，还需要乘以marker的强度系数
                        //if (parent.editKey != null && float.IsPositiveInfinity(_weights[i]))
                        //{
                        //    weight = weight * parent.editKey.marker.intensity;
                        //}

                        //parent.lipSync.blendSystem.SetBlendableValue(parent.shape.blendShapes[blendShapeIndex].blendableIndex, weight);
                    }
                }
            }
        }

        public void Reset()
        {
            _dragButtonRect.center = GetWindowPosFromNormalizedPos(Vector2.zero);
            PreviewBlendController();
        }
    }

}