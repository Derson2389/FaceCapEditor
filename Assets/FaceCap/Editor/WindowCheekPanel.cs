using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

namespace FaceCapEditor
{
    public class WindowCheekPanel : WindowPanel
    {
        private const int panelSizeMax = 100;
        private BlendControllerPanel noseController;
        private BlendSlideControllerPanel noseSliderPanelLeft;
        private BlendSlideControllerPanel noseSliderPanelRight;

        private BlendSlideControllerPanel cheekSliderPanelLeft;
        private BlendSlideControllerPanel cheekSliderPanelRight;


        public WindowCheekPanel(EditorWindow window, Rect rect)
        {
            panelRect = rect;
            panelWindow = window;

        }

        public override void OnInit()
        {
            base.OnInit();
            OnPanelEnable();
        }

        public override void OnPanelEnable()
        {
            base.OnPanelEnable();
            //Nose
            BlendGridController _noseController = FaceEditorMainWin.window.currentHandler.CreateBlendGridCtrl(FaceEditHelper.CheekListCtrlName[(int)FaceEditHelper.CheekListCtrl.nose_facialControl], panelSizeMax, panelSizeMax, 10, 10);
            if (_noseController != null)
            {
                noseController = new BlendControllerPanel(this, new Rect(_noseController.windowPosition, _noseController.windowSize), _noseController);
                noseController.Init();
                FaceEditorMainWin.window.InserPanelList(noseController);
            }


            BlendYController controllerNoseL = FaceEditorMainWin.window.currentHandler.CreateBlendYCtrl(FaceEditHelper.CheekListCtrlName[(int)FaceEditHelper.CheekListCtrl.r_nose_facialControl]);
            if (controllerNoseL != null)
            {
                noseSliderPanelLeft = new BlendSlideControllerPanel(this, Rect.zero, null, controllerNoseL);
                noseSliderPanelLeft.Init();
                FaceEditorMainWin.window.InserPanelList(noseSliderPanelLeft);
            }

            BlendYController controllerNose1R = FaceEditorMainWin.window.currentHandler.CreateBlendYCtrl(FaceEditHelper.CheekListCtrlName[(int)FaceEditHelper.CheekListCtrl.l_nose_facialControl]);
            if (controllerNose1R != null)
            {
                noseSliderPanelRight = new BlendSlideControllerPanel(this, Rect.zero, null, controllerNose1R);
                noseSliderPanelRight.Init();
                FaceEditorMainWin.window.InserPanelList(noseSliderPanelRight);
            }

            BlendXController controllerCheekLeft = FaceEditorMainWin.window.currentHandler.CreateBlendXCtrl(FaceEditHelper.CheekListCtrlName[(int)FaceEditHelper.CheekListCtrl.r_cheek_facialControl]);
            if (controllerCheekLeft != null)
            {
                cheekSliderPanelLeft = new BlendSlideControllerPanel(this, Rect.zero, controllerCheekLeft, null);
                cheekSliderPanelLeft.Init();
                FaceEditorMainWin.window.InserPanelList(cheekSliderPanelLeft);
            }
            BlendXController controllerCheekRight = FaceEditorMainWin.window.currentHandler.CreateBlendXCtrl(FaceEditHelper.CheekListCtrlName[(int)FaceEditHelper.CheekListCtrl.l_cheek_facialControl]);
            if (controllerCheekRight != null)
            {
                cheekSliderPanelRight = new BlendSlideControllerPanel(this, Rect.zero, controllerCheekRight, null);
                cheekSliderPanelRight.Init();
                FaceEditorMainWin.window.InserPanelList(cheekSliderPanelRight);
            }
        }

        public override void OnPanelDisable()
        {
            base.OnPanelDisable();
            if(noseController!= null)
                noseController.Reset();
        }


        public override void Update(bool focus)
        {
            if (noseSliderPanelLeft != null)
                noseSliderPanelLeft.OnUpdate(focus);
            if (noseSliderPanelRight != null)
                noseSliderPanelRight.OnUpdate(focus);
            if (cheekSliderPanelLeft != null)
                cheekSliderPanelLeft.OnUpdate(focus);
            if (cheekSliderPanelRight != null)
                cheekSliderPanelRight.OnUpdate(focus);

            if (noseController != null)
                noseController.OnUpdate(focus);
        }

        public override void OnDraw()
        {
            GUILayout.BeginArea(panelRect);

            var newRect = new Rect(0, 20, panelRect.width/2, panelRect.height);
            GUILayout.BeginArea(newRect);
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            if(noseSliderPanelLeft!= null)
                noseSliderPanelLeft.OnDraw(new Vector2(10, 120));
            GUILayout.FlexibleSpace();
            newRect = new Rect(panelRect.width / 4 - panelSizeMax / 2 - 20, 0, 260, panelRect.height);
            GUILayout.BeginArea(newRect);
            if(noseController!= null)
                noseController.OnDraw();
            GUILayout.EndArea();
            GUILayout.FlexibleSpace();
            if (noseSliderPanelRight != null)
                noseSliderPanelRight.OnDraw(new Vector2(10, 120));
            GUILayout.Space(20);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            newRect = new Rect(panelRect.width / 2, 20, panelRect.width / 2, panelRect.height);

            GUILayout.BeginArea(newRect);
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIStyle myStyle = new GUIStyle();
            myStyle.fontSize = 22;
            myStyle.normal.textColor = Color.white;
            GUILayout.Label("Cheek", myStyle);           
            GUILayout.FlexibleSpace();
            GUILayout.Space(60);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (cheekSliderPanelLeft != null)
                cheekSliderPanelLeft.OnDraw(new Vector2(120, 10));
            GUILayout.Space(20);
            if (cheekSliderPanelRight != null)
                cheekSliderPanelRight.OnDraw(new Vector2(120, 10));
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.EndArea();

            GUILayout.EndArea();
        }
    }
}
