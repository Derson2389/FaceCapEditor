﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PrevizCtrlHandler
{
    
    public List<BlenderShapeCtrl> controllerList = new List<BlenderShapeCtrl>();

    public class BlenderShapeCtrl
    {
        public string ctrlName = string.Empty;
        public int ctrlType = 0;
        public float leftValue = -1f;
        public float rightValue = 1f;
        public float upValue = 1f;
        public float downValue = -1f;

        public enum blendIdx
        {
            top,
            down,
            right,
            left,
                   
        }

        public enum blendYIdx
        {
            top,
            down,
            
        }

        public enum blendXIdx
        {
            left,
            right,

        }

        public List<BlendShape> ctrlBlendShapes = new List<BlendShape>();
    }
    public FaceControllerComponent faceCtrlComponent = null;
    public void LoadConfig(TextAsset configuration, GameObject gameObject)
    {
        faceCtrlComponent = gameObject.GetComponentInChildren<FaceControllerComponent>();
        if (faceCtrlComponent == null)
        {
            Debug.LogError("请先在表情blendshape上挂载FaceControllerComponent");
            return;
        }
        if (configuration == null)
        {
            Debug.LogWarning("请先加载在表情配置文件");
            return;
        }

        controllerList.Clear();
        string text = configuration.text;
        string[] lines = text.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line)|| line == "\r")
            {
                continue;
            }

            string[] parseStr = line.Split(',');
            string ctrName = parseStr[0];
            string type = parseStr[1];
            int intType = 0;
            if (type == "Y")///只有Y向的控制器
            {
                intType = 1;

                BlenderShapeCtrl addDummy = new BlenderShapeCtrl();
                addDummy.ctrlName = ctrName;
                addDummy.ctrlType = intType;
                string[] bs1 = parseStr[2].Split('|');
                addDummy.upValue = float.Parse(bs1[1]);
                BlendShape newBs1 = new BlendShape();
                newBs1.blendableName = bs1[0];
                newBs1.blendableIndex = faceCtrlComponent.GetMeshRenderIdxByName(newBs1.blendableName);

                addDummy.ctrlBlendShapes.Add(newBs1);
                string[] bs2 = parseStr[3].Split('|');
                addDummy.downValue = float.Parse(bs2[1]);
                BlendShape newBs2 = new BlendShape();
                newBs2.blendableName = bs2[0];
                newBs2.blendableIndex = faceCtrlComponent.GetMeshRenderIdxByName(newBs2.blendableName);

                addDummy.ctrlBlendShapes.Add(newBs2);
                controllerList.Add(addDummy);
            }
            else if (type == "X")///只有X向的控制器
            {
                intType = 0;
                BlenderShapeCtrl addDummy = new BlenderShapeCtrl();
                addDummy.ctrlName = ctrName;
                addDummy.ctrlType = intType;
                string[] bs1 = parseStr[2].Split('|');
                addDummy.rightValue = float.Parse(bs1[1]);
                BlendShape newBs1 = new BlendShape();
                newBs1.blendableName = bs1[0];
                newBs1.blendableIndex = faceCtrlComponent.GetMeshRenderIdxByName(newBs1.blendableName);
                addDummy.ctrlBlendShapes.Add(newBs1);

                string[] bs2 = parseStr[3].Split('|');
                addDummy.leftValue = float.Parse(bs2[1]);
                BlendShape newBs2 = new BlendShape();
                newBs2.blendableName = bs2[0];
                newBs2.blendableIndex = faceCtrlComponent.GetMeshRenderIdxByName(newBs2.blendableName);

                addDummy.ctrlBlendShapes.Add(newBs2);
                controllerList.Add(addDummy);

            }
            else if (type == "XY")///XY向都有的控制器
            {
                intType = 2;
                BlenderShapeCtrl addDummy = new BlenderShapeCtrl();
                addDummy.ctrlName = ctrName;
                addDummy.ctrlType = intType;
                string[] bs1 = parseStr[2].Split('|');
                addDummy.upValue = float.Parse(bs1[1]);
                BlendShape newBs1 = new BlendShape();
                newBs1.blendableName = bs1[0];
                newBs1.blendableIndex = faceCtrlComponent.GetMeshRenderIdxByName(newBs1.blendableName);
                addDummy.ctrlBlendShapes.Add(newBs1);

                string[] bs2 = parseStr[3].Split('|');
                addDummy.downValue = float.Parse(bs2[1]);
                BlendShape newBs2 = new BlendShape();
                newBs2.blendableName = bs2[0];
                newBs2.blendableIndex = faceCtrlComponent.GetMeshRenderIdxByName(newBs2.blendableName);
                addDummy.ctrlBlendShapes.Add(newBs2);

                string[] bs3 = parseStr[4].Split('|');
                addDummy.rightValue = float.Parse(bs3[1]);
                BlendShape newBs3 = new BlendShape();
                newBs3.blendableName = bs3[0];
                newBs3.blendableIndex = faceCtrlComponent.GetMeshRenderIdxByName(newBs3.blendableName);
                addDummy.ctrlBlendShapes.Add(newBs3);

                string[] bs4 = parseStr[5].Split('|');
                addDummy.leftValue = float.Parse(bs4[1]);
                BlendShape newBs4 = new BlendShape();
                newBs4.blendableName = bs4[0];
                newBs4.blendableIndex = faceCtrlComponent.GetMeshRenderIdxByName(newBs4.blendableName);

                addDummy.ctrlBlendShapes.Add(newBs4);
                controllerList.Add(addDummy);
            }
        }
    }

    public List<float> _weights = new List<float>();
    public void SetBlenderShapeByCtrlName(string ctrlName,  Vector2 vec)
    {
        if (faceCtrlComponent == null)
        {
            Debug.LogError("表情录制对象为空");
            return;
        }
        BlenderShapeCtrl ctrl = getControllerByName(ctrlName);
        
        if (ctrl != null)
        { 
            _weights.Clear();
            int cout = ctrl.ctrlBlendShapes.Count;
            for (int i = 0; i < cout; i++)
            {
                _weights.Add(0f);
            }

            if (ctrl.ctrlType == 1)//Y Controller
            {             
                float valueY = vec.y;
                var top = ctrl.ctrlBlendShapes[(int)BlenderShapeCtrl.blendXIdx.left].blendableIndex;
                var bottom = ctrl.ctrlBlendShapes[(int)BlenderShapeCtrl.blendXIdx.right].blendableIndex;
                               
                if (ctrl.downValue == ctrl.upValue)
                {
                    if (ctrl.downValue > 0)
                    {
                        if (top != -1)
                        {
                            _weights[(int)ControllerDirectionY.Top] = GetYtypeWeightFromCtrlValue(ControllerDirectionY.Top, valueY);
                        }
                        if (bottom != -1)
                        {
                            _weights[(int)ControllerDirectionY.Bottom] = _weights[(int)BlendYController.ControllerDirection.Top];
                        }
                    }
                    else
                    {
                        if (bottom != -1)
                        {
                            _weights[(int)ControllerDirectionY.Bottom] = GetYtypeWeightFromCtrlValue(ControllerDirectionY.Bottom, valueY);
                        }
                        // set drag value for top controller
                        if (top != -1)
                        {
                            _weights[(int)ControllerDirectionY.Top] = _weights[(int)ControllerDirectionY.Bottom];
                        }

                    }

                }
                else
                {
                    // set drag value for top controller
                    if (top != -1)
                    {
                        _weights[(int)ControllerDirectionY.Top] = GetYtypeWeightFromCtrlValue(ControllerDirectionY.Top, valueY);
                    }

                    // set drag value for bottom controller
                    if (bottom != -1)
                    {
                        _weights[(int)ControllerDirectionY.Bottom] = GetYtypeWeightFromCtrlValue(ControllerDirectionY.Bottom, valueY);
                    }
                }

            }
            else if (ctrl.ctrlType == 0)// X Ctrl
            {
                float valueX = vec.x;
                var left = ctrl.ctrlBlendShapes[(int)BlenderShapeCtrl.blendXIdx.left].blendableIndex;
                var right = ctrl.ctrlBlendShapes[(int)BlenderShapeCtrl.blendXIdx.right].blendableIndex;
            
                if (ctrl.leftValue == ctrl.rightValue)
                {
                    if (ctrl.leftValue > 0)
                    {
                        if (right != -1)
                        {
                            _weights[(int)ControllerDirectionX.Right] = GetXtypeWeightFromCtrlValue(ControllerDirectionX.Right, valueX);
                        }
                        if (right != -1)
                        {
                            _weights[(int)ControllerDirectionX.Left] = _weights[(int)ControllerDirectionX.Right];
                        }
                    }
                    else
                    {
                        if (left != -1)
                        {
                            _weights[(int)ControllerDirectionX.Left] = GetXtypeWeightFromCtrlValue(ControllerDirectionX.Left, valueX);
                        }
                        // set drag value for right controller
                        if (right != -1)
                        {
                            _weights[(int)ControllerDirectionX.Right] = _weights[(int)ControllerDirectionX.Left];
                        }
                    }
                }
                else if(ctrl.leftValue > ctrl.rightValue)
                {
                   
                    if (left != -1)
                    {
                        _weights[(int)ControllerDirectionX.Left] = GetXtypeWeightFromCtrlValue(ControllerDirectionX.Left, valueX);
                    }

                    // set drag value for right controller
                    if (right != -1)
                    {
                        _weights[(int)ControllerDirectionX.Right] = GetXtypeWeightFromCtrlValue(ControllerDirectionX.Right, valueX);
                    }
                }
                else 
                {

                    if (left != -1)
                    {
                        _weights[(int)ControllerDirectionX.Left] = GetXtypeWeightFromCtrlValue(ControllerDirectionX.Left, valueX, true);
                    }

                    // set drag value for right controller
                    if (right != -1)
                    {
                        _weights[(int)ControllerDirectionX.Right] = GetXtypeWeightFromCtrlValue(ControllerDirectionX.Right, valueX, true);
                    }
                }
            }
            else if (ctrl.ctrlType == 2)//XY Ctrl
            {
                Vector2 xy = new Vector2(vec.x, vec.y);
                var top = ctrl.ctrlBlendShapes[(int)BlenderShapeCtrl.blendIdx.top].blendableIndex;
                var bottom = ctrl.ctrlBlendShapes[(int)BlenderShapeCtrl.blendIdx.down].blendableIndex;
                var left = ctrl.ctrlBlendShapes[(int)BlenderShapeCtrl.blendIdx.left].blendableIndex;
                var right = ctrl.ctrlBlendShapes[(int)BlenderShapeCtrl.blendIdx.right].blendableIndex;

                if (ctrl.leftValue > ctrl.rightValue)
                {
                    if (top != -1)
                    {
                        _weights[(int)ControllerDirectionXY.Top] = GetXYtypeWeightFromPosition(ControllerDirectionXY.Top, xy, true);
                    }

                    if (left != -1)
                    {
                        _weights[(int)ControllerDirectionXY.Left] = GetXYtypeWeightFromPosition(ControllerDirectionXY.Left, xy, true);
                    }

                    if (bottom != -1)
                    {
                        _weights[(int)ControllerDirectionXY.Bottom] = GetXYtypeWeightFromPosition(ControllerDirectionXY.Bottom, xy, true);
                    }

                    if (right != -1)
                    {
                        _weights[(int)ControllerDirectionXY.Right] = GetXYtypeWeightFromPosition(ControllerDirectionXY.Right, xy, true);
                    }
                }
                else
                {
                    if (top != -1)
                    {
                        _weights[(int)ControllerDirectionXY.Top] = GetXYtypeWeightFromPosition(ControllerDirectionXY.Top, xy);
                    }

                    if (left != -1)
                    {
                        _weights[(int)ControllerDirectionXY.Left] = GetXYtypeWeightFromPosition(ControllerDirectionXY.Left, xy);
                    }

                    if (bottom != -1)
                    {
                        _weights[(int)ControllerDirectionXY.Bottom] = GetXYtypeWeightFromPosition(ControllerDirectionXY.Bottom, xy);
                    }

                    if (right != -1)
                    {
                        _weights[(int)ControllerDirectionXY.Right] = GetXYtypeWeightFromPosition(ControllerDirectionXY.Right, xy);
                    }
                }
            }

            ///set bendershape value
            for (int i = 0; i < ctrl.ctrlBlendShapes.Count; i++)
            {
                int blendShapeIndex = ctrl.ctrlBlendShapes[i].blendableIndex;
                if (blendShapeIndex != -1)
                {
                    // 使用BlendController面板映射的值
                    float weight = _weights[i];

                    //// 对于PositiveInfinity值，使用原始shape里面的weight
                    if (float.IsPositiveInfinity(weight))
                        weight = 0;
                    faceCtrlComponent.SetFaceController(blendShapeIndex, weight);
                }
            }
        }

    }


    public enum ControllerDirectionX
    {
        Left = 0,
        Right,
        Count
    }

    public enum ControllerDirectionY
    {
        Top = 0,
        Bottom,
        Count
    }

    public enum ControllerDirectionXY
    {
        Top = 0,
        Bottom,
        Right,
        Left,
    }


    public float GetXtypeWeightFromCtrlValue(ControllerDirectionX dir, float ctrlValue, bool isRMax = false)
    {
        float value = 0f;

        if (isRMax)
        {
            switch (dir)
            {
                case ControllerDirectionX.Left:
                    if (ctrlValue < 0f)
                        value = float.PositiveInfinity;
                    else
                        value = Mathf.Abs(ctrlValue) * 100;
                    break;

                case ControllerDirectionX.Right:
                    if (ctrlValue > 0f)
                        value = float.PositiveInfinity;
                    else
                        value = Mathf.Abs(ctrlValue) * 100;
                    break;
            }
        }
        else
        {
            switch (dir)
            {
                case ControllerDirectionX.Left:
                    if (ctrlValue > 0f)
                        value = float.PositiveInfinity;
                    else
                        value = Mathf.Abs(ctrlValue) * 100;
                    break;

                case ControllerDirectionX.Right:
                    if (ctrlValue < 0f)
                        value = float.PositiveInfinity;
                    else
                        value = Mathf.Abs(ctrlValue) * 100;
                    break;
            }
        }
        return value;
    }

    public float GetYtypeWeightFromCtrlValue(ControllerDirectionY dir, float ctrlValue)
    {
        float value = 0f;

        switch (dir)
        {
            case ControllerDirectionY.Top:
                if (ctrlValue < 0)
                    value = float.PositiveInfinity;
                else
                    value = Mathf.Abs(ctrlValue) * 100;
                break;

            case ControllerDirectionY.Bottom:
                if (ctrlValue > 0)
                    value = float.PositiveInfinity;
                else
                    value = Mathf.Abs(ctrlValue) * 100;
                break;
        }

        return value;
    }


    public  float GetXYtypeWeightFromPosition(ControllerDirectionXY dir, Vector2 pos, bool isLMax = false)
    {
        float value = 0f;
        if (isLMax)
        {
            switch (dir)
            {
                case ControllerDirectionXY.Top:
                    if (pos.y < 0f)
                        value = float.PositiveInfinity;
                    else
                        value = Mathf.Abs(pos.y) * 100;
                    break;

                case ControllerDirectionXY.Bottom:
                    if (pos.y > 0f)
                        value = float.PositiveInfinity;
                    else
                        value = Mathf.Abs(pos.y) * 100;
                    break;

                case ControllerDirectionXY.Left:
                    if (pos.x < 0f)
                        value = float.PositiveInfinity;
                    else
                        value = Mathf.Abs(pos.x) * 100;
                    break;

                case ControllerDirectionXY.Right:
                    if (pos.x > 0f)
                        value = float.PositiveInfinity;
                    else
                        value = Mathf.Abs(pos.x) * 100;
                    break;
            }
        }
        else
        {
            switch (dir)
            {
                case ControllerDirectionXY.Top:
                    if (pos.y < 0f)
                        value = float.PositiveInfinity;
                    else
                        value = Mathf.Abs(pos.y) * 100;
                    break;

                case ControllerDirectionXY.Bottom:
                    if (pos.y > 0f)
                        value = float.PositiveInfinity;
                    else
                        value = Mathf.Abs(pos.y) * 100;
                    break;

                case ControllerDirectionXY.Left:
                    if (pos.x > 0f)
                        value = float.PositiveInfinity;
                    else
                        value = Mathf.Abs(pos.x) * 100;
                    break;

                case ControllerDirectionXY.Right:
                    if (pos.x < 0f)
                        value = float.PositiveInfinity;
                    else
                        value = Mathf.Abs(pos.x) * 100;
                    break;
            }
        }
        return value;
    }


    public BlenderShapeCtrl getControllerByName(string name)
    {
        BlenderShapeCtrl ad = null;
        for (int i = 0; i < controllerList.Count; i++)
        {
            var ctrl = controllerList[i];
            if (/*ctrl.ctrlName.Contains(name)|| */ctrl.ctrlName == name)
            {
                ad = ctrl;
            }
        }
        return ad;
    }

    #region Panel


    public  BlendXController CreateBlendXCtrl( string name)
    {
        BlendXController dummy = new BlendXController();
        dummy.controllerName = name;

        BlenderShapeCtrl crl = getControllerByName(dummy.controllerName);
        if (crl != null)
        {
            dummy.left = crl.ctrlBlendShapes[(int)BlenderShapeCtrl.blendXIdx.left].blendableIndex;
            dummy.right = crl.ctrlBlendShapes[(int)BlenderShapeCtrl.blendXIdx.right].blendableIndex;
            dummy.leftSliderValue = crl.leftValue;
            dummy.rightSliderValue = crl.rightValue;
            return dummy;
        }

        return null;
    }

    public  BlendYController CreateBlendYCtrl(string name)
    {
        BlendYController dummy = new BlendYController();
        dummy.controllerName = name;

        BlenderShapeCtrl crl = getControllerByName(dummy.controllerName);
        if (crl != null)
        {
            dummy.top = crl.ctrlBlendShapes[(int)BlenderShapeCtrl.blendYIdx.top].blendableIndex;
            dummy.bottom = crl.ctrlBlendShapes[(int)BlenderShapeCtrl.blendYIdx.down].blendableIndex;
            dummy.upSliderValue = crl.upValue;
            dummy.downSliderValue = crl.downValue;
            return dummy;
        }

        return null;
    }


    public  BlendGridController CreateBlendGridCtrl(string name, int width, int height, int posX, int posY)
    {
        BlendGridController dummy = new BlendGridController();
        dummy.windowPosition = new Vector2(posX, posY);
        dummy.windowSize = new Vector2(width, width);
        dummy.controllerName = name;

        BlenderShapeCtrl crl = getControllerByName(dummy.controllerName);
        if (crl != null)
        {
            dummy.top = crl.ctrlBlendShapes[(int)BlenderShapeCtrl.blendIdx.top].blendableIndex;
            dummy.bottom = crl.ctrlBlendShapes[(int)BlenderShapeCtrl.blendIdx.down].blendableIndex;
            dummy.left = crl.ctrlBlendShapes[(int)BlenderShapeCtrl.blendIdx.left].blendableIndex;
            dummy.right = crl.ctrlBlendShapes[(int)BlenderShapeCtrl.blendIdx.right].blendableIndex;
            dummy.leftValue = crl.leftValue;
            dummy.rightValue = crl.rightValue;
            dummy.upValue = crl.upValue;
            dummy.downValue = crl.downValue;
            return dummy;
        }
        return null;
    }

    #endregion

}
