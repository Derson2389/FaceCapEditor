using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class BlenderShapesManager  {

    public static string ConfigTxt = string.Empty;
    public static List<BlenderShapeCtrl> controllerList = new List<BlenderShapeCtrl>();

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
            left
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

    public static void LoadConfig(FaceControllerComponent faceCtrlComponent)
    {
        if (ConfigTxt == null)
        {
            Debug.LogWarning("ConfigTxt null");
            return;    
        }
        if (faceCtrlComponent == null)
        {
            Debug.LogWarning("faceCtrlComponent null");
            return;
        }
        controllerList.Clear();
        string finalPath = Application.dataPath + ConfigTxt.Replace("Assets","");

        if (File.Exists(finalPath))
        {
            StreamReader sr = new StreamReader(finalPath);
            while (!sr.EndOfStream)
            {
                string text = sr.ReadLine();
                if (!string.IsNullOrEmpty(text))
                {
                    string[] parseStr = text.Split(',');
                    string ctrName = parseStr[0];
                    string type = parseStr[1];
                    int intType = 0;
                    if (type == "Y")
                    {
                        intType = 1;

                        BlenderShapeCtrl addDummy = new BlenderShapeCtrl();
                        addDummy.ctrlName = ctrName;
                        addDummy.ctrlType = intType;
                        string[] bs1 = parseStr[2].Split('|');
                        addDummy.upValue = float.Parse(bs1[1]);
                        BlendShape newBs1 = new BlendShape();
                        newBs1.blendableName = bs1[0];
                        newBs1.blendableIndex = faceCtrlComponent.GetBlendShapeIdxByName(newBs1.blendableName).blendableIndex;

                        addDummy.ctrlBlendShapes.Add(newBs1);
                        string[] bs2 = parseStr[3].Split('|');
                        addDummy.downValue = float.Parse(bs2[1]);
                        BlendShape newBs2 = new BlendShape();
                        newBs2.blendableName = bs2[0];
                        newBs2.blendableIndex = faceCtrlComponent.GetBlendShapeIdxByName(newBs2.blendableName).blendableIndex;

                        addDummy.ctrlBlendShapes.Add(newBs2);
                        controllerList.Add(addDummy);
                    }
                    else if (type == "X")
                    {
                        intType = 0;
                        BlenderShapeCtrl addDummy = new BlenderShapeCtrl();
                        addDummy.ctrlName = ctrName;
                        addDummy.ctrlType = intType;
                        string[] bs1 = parseStr[2].Split('|');
                        addDummy.rightValue = float.Parse(bs1[1]);
                        BlendShape newBs1 = new BlendShape();
                        newBs1.blendableName = bs1[0];
                        newBs1.blendableIndex = faceCtrlComponent.GetBlendShapeIdxByName(newBs1.blendableName).blendableIndex;
                        addDummy.ctrlBlendShapes.Add(newBs1);

                        string[] bs2 = parseStr[3].Split('|');
                        addDummy.leftValue = float.Parse(bs2[1]);
                        BlendShape newBs2 = new BlendShape();
                        newBs2.blendableName = bs2[0];
                        newBs2.blendableIndex = faceCtrlComponent.GetBlendShapeIdxByName(newBs2.blendableName).blendableIndex;

                        addDummy.ctrlBlendShapes.Add(newBs2);
                        controllerList.Add(addDummy);

                    }
                    else if (type == "XY")
                    {
                        intType = 2;
                        BlenderShapeCtrl addDummy = new BlenderShapeCtrl();
                        addDummy.ctrlName = ctrName;
                        addDummy.ctrlType = intType;
                        string[] bs1 = parseStr[2].Split('|');
                        addDummy.rightValue = float.Parse(bs1[1]);
                        BlendShape newBs1 = new BlendShape();
                        newBs1.blendableName = bs1[0];
                        newBs1.blendableIndex = faceCtrlComponent.GetBlendShapeIdxByName(newBs1.blendableName).blendableIndex;
                        addDummy.ctrlBlendShapes.Add(newBs1);

                        string[] bs2 = parseStr[3].Split('|');
                        addDummy.leftValue = float.Parse(bs2[1]);
                        BlendShape newBs2 = new BlendShape();
                        newBs2.blendableName = bs2[0];
                        newBs2.blendableIndex = faceCtrlComponent.GetBlendShapeIdxByName(newBs2.blendableName).blendableIndex;
                        addDummy.ctrlBlendShapes.Add(newBs2);

                        string[] bs3 = parseStr[4].Split('|');
                        addDummy.upValue = float.Parse(bs3[1]);
                        BlendShape newBs3 = new BlendShape();
                        newBs3.blendableName = bs3[0];
                        newBs3.blendableIndex = faceCtrlComponent.GetBlendShapeIdxByName(newBs3.blendableName).blendableIndex;
                        addDummy.ctrlBlendShapes.Add(newBs3);

                        string[] bs4 = parseStr[5].Split('|');
                        addDummy.downValue = float.Parse(bs4[1]);
                        BlendShape newBs4 = new BlendShape();
                        newBs4.blendableName = bs4[0];
                        newBs4.blendableIndex = faceCtrlComponent.GetBlendShapeIdxByName(newBs4.blendableName).blendableIndex;

                        addDummy.ctrlBlendShapes.Add(newBs4);
                        controllerList.Add(addDummy);
                    }
                }
            }
            sr.Close();
        }
    }

    public static BlendGridController CreateBlendGridCtrl(string name, int width, int height, int posX, int posY)
    {

        BlendGridController dummy = new BlendGridController();
        dummy.windowPosition = new Vector2(posX, posY);
        dummy.windowSize = new Vector2(width, width);
        dummy.controllerName = name;

        BlenderShapesManager.BlenderShapeCtrl crl = BlenderShapesManager.getControllerByName(dummy.controllerName);
        if (crl != null)
        {
            dummy.top = crl.ctrlBlendShapes[(int)BlenderShapesManager.BlenderShapeCtrl.blendIdx.top].blendableIndex;
            dummy.bottom = crl.ctrlBlendShapes[(int)BlenderShapesManager.BlenderShapeCtrl.blendIdx.down].blendableIndex;
            dummy.left = crl.ctrlBlendShapes[(int)BlenderShapesManager.BlenderShapeCtrl.blendIdx.left].blendableIndex;
            dummy.right = crl.ctrlBlendShapes[(int)BlenderShapesManager.BlenderShapeCtrl.blendIdx.right].blendableIndex;
            return dummy;
        }
        return null;   
    }

    public static BlendYController CreateBlendYCtrl(string name)
    {
        
        BlendYController dummy = new BlendYController();
        dummy.controllerName = name;

        BlenderShapesManager.BlenderShapeCtrl crl = BlenderShapesManager.getControllerByName(dummy.controllerName);
        if (crl != null)
        {
            dummy.top = crl.ctrlBlendShapes[(int)BlenderShapesManager.BlenderShapeCtrl.blendYIdx.top].blendableIndex;
            dummy.bottom = crl.ctrlBlendShapes[(int)BlenderShapesManager.BlenderShapeCtrl.blendYIdx.down].blendableIndex;
            dummy.upSliderValue = crl.upValue;
            dummy.downSliderValue = crl.downValue;
            return dummy;
        }
      
        return null;
    }

    public static BlendXController CreateBlendXCtrl(string name)
    {
        BlendXController dummy = new BlendXController();
        dummy.controllerName = name;

        BlenderShapesManager.BlenderShapeCtrl crl = BlenderShapesManager.getControllerByName(dummy.controllerName);
        if (crl != null)
        {
            dummy.left = crl.ctrlBlendShapes[(int)BlenderShapesManager.BlenderShapeCtrl.blendXIdx.left].blendableIndex;
            dummy.right = crl.ctrlBlendShapes[(int)BlenderShapesManager.BlenderShapeCtrl.blendXIdx.right].blendableIndex;
            dummy.leftSliderValue = crl.leftValue;
            dummy.rightSliderValue = crl.rightValue;
            return dummy;
        }

        return null;
    }


    public static BlenderShapeCtrl getControllerByName(string name)
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


}
