using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

namespace FaceCapEditor
{
    public class WindowOtherPanel : WindowPanel
    {

        //Others
        private BlendControllerPanel teethControllerUp;
        private BlendControllerPanel teethControllerDown;

        private BlendSlideControllerPanel controllerPanelAdd1;
        private BlendSlideControllerPanel controllerPanelAdd2;
        private BlendSlideControllerPanel controllerPanelAdd3;
        private BlendSlideControllerPanel controllerPanelAdd4;


        public const int panelSize = 100;

        public WindowOtherPanel(EditorWindow window, Rect rect)
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
            if (FaceEditorMainWin.window.currentHandler == null)
                return;

            BlendGridController controllerTeethUp = FaceEditorMainWin.window.currentHandler.CreateBlendGridCtrl(FaceEditHelper.OtherCtrlName[(int)FaceEditHelper.OtherCtrl.upper_teeth_facialControl], panelSize, panelSize, 80, 22);           
            if (controllerTeethUp != null)
            {
                teethControllerUp = new BlendControllerPanel(this, new Rect(controllerTeethUp.windowPosition, controllerTeethUp.windowSize), controllerTeethUp);
                teethControllerUp.Init();
                FaceEditorMainWin.window.InserPanelList(teethControllerUp);
            }

            BlendGridController controllerTeethDown = FaceEditorMainWin.window.currentHandler.CreateBlendGridCtrl(FaceEditHelper.OtherCtrlName[(int)FaceEditHelper.OtherCtrl.lower_teeth_facialControl], panelSize, panelSize, 80, 128);            
            if (controllerTeethDown != null)
            {
                teethControllerDown = new BlendControllerPanel(this, new Rect(controllerTeethDown.windowPosition, controllerTeethDown.windowSize), controllerTeethDown);
                teethControllerDown.Init();
                FaceEditorMainWin.window.InserPanelList(teethControllerDown);
            }

            BlendXController controllerAdd1 = FaceEditorMainWin.window.currentHandler.CreateBlendXCtrl(FaceEditHelper.OtherCtrlName[(int)FaceEditHelper.OtherCtrl.add_facialControl]);
            if (controllerAdd1 != null)
            {
                controllerPanelAdd1 = new BlendSlideControllerPanel(this, Rect.zero, controllerAdd1, null);
                controllerPanelAdd1.Init();
                FaceEditorMainWin.window.InserPanelList(controllerPanelAdd1);
            }

            BlendXController controllerAdd2 = FaceEditorMainWin.window.currentHandler.CreateBlendXCtrl(FaceEditHelper.OtherCtrlName[(int)FaceEditHelper.OtherCtrl.add01_facialControl]);
            if (controllerAdd2 != null)
            {
                controllerPanelAdd2 = new BlendSlideControllerPanel(this, Rect.zero, controllerAdd2, null);
                controllerPanelAdd2.Init();
                FaceEditorMainWin.window.InserPanelList(controllerPanelAdd2);
            }

            BlendXController controllerAdd3 = FaceEditorMainWin.window.currentHandler.CreateBlendXCtrl(FaceEditHelper.OtherCtrlName[(int)FaceEditHelper.OtherCtrl.add02_facialControl]);
            if (controllerAdd3 != null)
            {
                controllerPanelAdd3 = new BlendSlideControllerPanel(this, Rect.zero, controllerAdd3, null);
                controllerPanelAdd3.Init();
                FaceEditorMainWin.window.InserPanelList(controllerPanelAdd3);
            }

            BlendXController controllerAdd4 = FaceEditorMainWin.window.currentHandler.CreateBlendXCtrl(FaceEditHelper.OtherCtrlName[(int)FaceEditHelper.OtherCtrl.add03_facialControl]);
            if (controllerAdd4 != null)
            {
                controllerPanelAdd4 = new BlendSlideControllerPanel(this, Rect.zero, controllerAdd4, null);
                controllerPanelAdd4.Init();
                FaceEditorMainWin.window.InserPanelList(controllerPanelAdd4);
            }
        }

        public override void OnPanelDisable()
        {
            base.OnPanelDisable();          
        }

        public override void Update(bool focus)
        {
            if (controllerPanelAdd1 != null)
            {
                controllerPanelAdd1.OnUpdate(focus);
            }
            if (controllerPanelAdd2 != null)
            {
                controllerPanelAdd2.OnUpdate(focus);
            }            
            if (controllerPanelAdd3 != null)
            {
                controllerPanelAdd3.OnUpdate(focus);
            }
            if (controllerPanelAdd4 != null)
            {
                controllerPanelAdd4.OnUpdate(focus);
            }

            if (teethControllerUp != null)
                teethControllerUp.OnUpdate(focus);
            if (teethControllerDown != null)
                teethControllerDown.OnUpdate(focus);
        }


        public override void OnDraw()
        {
            GUILayout.BeginArea(panelRect);
            GUILayout.BeginVertical();
            GUILayout.Space(18);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();           
            GUIStyle myStyle = new GUIStyle();
            myStyle.fontSize = 22;
            myStyle.normal.textColor = Color.white;
            GUILayout.Label("Teeth", myStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            var newRect = new Rect(0,20, panelRect.width, panelRect.height -100);
            GUILayout.BeginArea(newRect);
            {
                if (teethControllerUp != null)
                {
                    teethControllerUp.OnDraw();
                    EditorGUIUtility.AddCursorRect(teethControllerUp.centerPanel, MouseCursor.Link);
                }

                if (teethControllerDown != null)
                {
                    teethControllerDown.OnDraw();
                    EditorGUIUtility.AddCursorRect(teethControllerDown.centerPanel, MouseCursor.Link);
                }

            }
            GUILayout.EndArea();
            newRect = new Rect(0, 250, panelRect.width,  200);
            GUILayout.BeginArea(newRect);
            {
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Add", myStyle);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                ///GUILayout.HorizontalSlider(0, 0, 1.00f, GUILayout.Width(120), GUILayout.Height(20));
                if (controllerPanelAdd1 != null)
                {
                    controllerPanelAdd1.OnDraw(new Vector2(120, 10));
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(15);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                ///GUILayout.HorizontalSlider(0, 0, 1.00f, GUILayout.Width(120), GUILayout.Height(20));
                if (controllerPanelAdd2 != null)
                {
                    controllerPanelAdd2.OnDraw(new Vector2(120, 10));
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(15);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                ///GUILayout.HorizontalSlider(0, 0, 1.00f, GUILayout.Width(120), GUILayout.Height(20));
                if (controllerPanelAdd3 != null)
                {
                    controllerPanelAdd3.OnDraw(new Vector2(120, 10));
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(15);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                //GUILayout.HorizontalSlider(0, 0, 1.00f, GUILayout.Width(120), GUILayout.Height(20));
                if (controllerPanelAdd4 != null)
                {
                    controllerPanelAdd4.OnDraw(new Vector2(120, 10));
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();

            }
            GUILayout.EndArea();
           
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}
