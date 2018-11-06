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
        public int leftValue = -1;
        public int rightValue = 1;
        public int upValue = 1;
        public int downValue = -1;

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
                    }
                    else if (type == "X")
                    {
                        intType = 0;
                    }
                    else if (type == "XY")
                    {
                        intType = 2;
                    }



                }
            }
            sr.Close();
        }


    }



}
