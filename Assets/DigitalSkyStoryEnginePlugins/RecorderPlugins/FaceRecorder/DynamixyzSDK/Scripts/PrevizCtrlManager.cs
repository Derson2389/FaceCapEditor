using System.Collections;
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

    public void LoadConfig(TextAsset configuration, GameObject gameObject)
    {
        var faceCtrlComponent = gameObject.GetComponentInChildren<FaceControllerComponent>();
        if (faceCtrlComponent == null)
        {
            Debug.LogError("请先在表情blendshape上挂载FaceControllerComponent");
            return;
        }

        controllerList.Clear();
        string text = configuration.text;
        string[] lines = text.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line))
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

                string[] bs3 = parseStr[4].Split('|');
                addDummy.upValue = float.Parse(bs3[1]);
                BlendShape newBs3 = new BlendShape();
                newBs3.blendableName = bs3[0];
                newBs3.blendableIndex = faceCtrlComponent.GetMeshRenderIdxByName(newBs3.blendableName);
                addDummy.ctrlBlendShapes.Add(newBs3);

                string[] bs4 = parseStr[5].Split('|');
                addDummy.downValue = float.Parse(bs4[1]);
                BlendShape newBs4 = new BlendShape();
                newBs4.blendableName = bs4[0];
                newBs4.blendableIndex = faceCtrlComponent.GetMeshRenderIdxByName(newBs4.blendableName);

                addDummy.ctrlBlendShapes.Add(newBs4);
                controllerList.Add(addDummy);
            }
        }
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


}
