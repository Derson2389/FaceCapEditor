using UnityEditor;
using UnityEngine;

namespace FaceCapEditor
{
    public class BlendSlideControllerPanel
    {
        public const int panelPadding = 6;
        public const int dragButtonSize = 10;

        public float _leftValue = -1f;
        public float _rightValue = 1f;
        public float _upValue = 1f;
        public float _downValue = -1f;


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

        private float _HorizontalSliderValue = 0f;
        public float HorizontalSliderValue
        {
            set
            {
                _HorizontalSliderValue = value;
            }
            get { return _HorizontalSliderValue;  }
        }

        private float _VerticalSliderValue = 0f;
        public float VerticalSliderValue
        {
            set { _VerticalSliderValue = value; }
            get { return _VerticalSliderValue;  }
        }

        // temp weights
        public float[] _weights;
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

            if (_blendControllerX != null)
            {
                
                if (blendControllerX.leftSliderValue == blendControllerX.rightSliderValue)
                {
                    if (blendControllerX.leftSliderValue < 0)
                    {
                        _leftValue = blendControllerX.leftSliderValue;
                        _rightValue = 0f;
                    }
                    else if (blendControllerX.leftSliderValue >= 0)
                    {
                        _leftValue = 0;
                        _rightValue = blendControllerX.rightSliderValue;
                    }
                }
                else
                {
                    _leftValue = blendControllerX.leftSliderValue;
                    _rightValue = blendControllerX.rightSliderValue;
                }

                _weights = new float[_blendControllerX.blendShapeIndexs.Count];
            }
            if (_blendControllerY != null)
            {
                if (_blendControllerY.upSliderValue == _blendControllerY.downSliderValue)
                {
                    if (_blendControllerY.upSliderValue < 0)
                    {
                        _upValue = 0f;
                        _downValue = _blendControllerY.downSliderValue;
                    }
                    else if (_blendControllerY.upSliderValue >= 0)
                    {
                        _upValue = blendControllerY.upSliderValue;
                        _downValue = 0f;
                    }
                }
                else
                {
                    _upValue = _blendControllerY.upSliderValue;
                    _downValue = _blendControllerY.downSliderValue;
                }

                _weights = new float[_blendControllerY.blendShapeIndexs.Count];
            }
        }
       
        public void OnUpdate(bool onfocus)
        {
             PreviewBlendController();
        }

        public void OnDraw(Vector2 size)
        {

            if (blendControllerX != null)
            {                
                HorizontalSliderValue = GUILayout.HorizontalSlider(HorizontalSliderValue, _leftValue, _rightValue, GUILayout.Width(size.x), GUILayout.Height(size.y));
            }     
            
            if(blendControllerY != null)
            {              
                VerticalSliderValue = GUILayout.VerticalSlider(VerticalSliderValue, _upValue, _downValue, GUILayout.Width(size.x), GUILayout.Height(size.y));
            }

           
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
                       
                        //Event.current.Use();
                    }
                    else if (Event.current.button == 0 && _resizeRightRect.Contains(Event.current.mousePosition))
                    {
                        //Debug.Log("_resize RightRect MouseDown");
                        _resizeHorPressed = true;
                        _resizeVerPressed = false;
                        
                        // Event.current.Use();
                    }
                    break;

                case EventType.MouseUp:
                    //Debug.Log("_resize MouseUp");
                    _resizeHorPressed = false;
                    _resizeVerPressed = false;
                    
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

        void ProcessDragEvents()
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    if (Event.current.button == 0 && _dragButtonRect.Contains(Event.current.mousePosition))
                    {
                        //Debug.Log("mousue down: " + Event.current.mousePosition);
                        //_dragButtonPressed = true;
                        _resizeHorPressed = false;
                        _resizeVerPressed = false;
                        //Event.current.Use();
                    }
                    break;

                case EventType.MouseUp:
                    //_dragButtonPressed = false;
                    _resizeHorPressed = false;
                    _resizeVerPressed = false;
                    break;

                case EventType.MouseDrag:
                    {
                        
                    }
                    break;
            }
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

        public float GetSliderValueFromWindow()
        {
            if (blendControllerX != null)
            {
                return HorizontalSliderValue;
            }
            if (blendControllerY != null)
            {
                return VerticalSliderValue;
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
                        if (FaceEditorMainWin.window.FaceCtrlComp != null)
                            FaceEditorMainWin.window.FaceCtrlComp.SetFaceController(FaceEditorMainWin.window.FaceCtrlComp.blendShapeList[blendShapeIndex].blendableIndex, weight);

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
                        if (float.IsPositiveInfinity(weight))
                            weight = FaceEditorMainWin.window.FaceCtrlComp.blendShapeList[blendShapeIndex].weight;

                        //// 对于编辑关键帧模式， 如果是PositiveInfinity的话，还需要乘以marker的强度系数
                        //if (parent.editKey != null && float.IsPositiveInfinity(_weights[i]))
                        //{
                        //    weight = weight * parent.editKey.marker.intensity;
                        //}

                        //parent.lipSync.blendSystem.SetBlendableValue(parent.shape.blendShapes[blendShapeIndex].blendableIndex, weight);
                        if (FaceEditorMainWin.window.FaceCtrlComp != null )
                            FaceEditorMainWin.window.FaceCtrlComp.SetFaceController(FaceEditorMainWin.window.FaceCtrlComp.blendShapeList[blendShapeIndex].blendableIndex, weight);
                    }
                }
            }
        }

        public void Reset()
        {            
            PreviewBlendController();
        }
    }

}