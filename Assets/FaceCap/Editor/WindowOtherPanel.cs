﻿using UnityEngine;
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

        public override void OnPanelEnable()
        {
            base.OnPanelEnable();

            BlendGridController controllerTeethUp = BlenderShapesManager.CreateBlendGridCtrl(FaceEditHelper.OtherCtrlName[(int)FaceEditHelper.OtherCtrl.upper_teeth_facialControl], panelSize, panelSize, 80, 22);           
            if (controllerTeethUp != null)
            {
                teethControllerUp = new BlendControllerPanel(this, new Rect(controllerTeethUp.windowPosition, controllerTeethUp.windowSize), controllerTeethUp);
                teethControllerUp.Init();
            }

            BlendGridController controllerTeethDown = BlenderShapesManager.CreateBlendGridCtrl(FaceEditHelper.OtherCtrlName[(int)FaceEditHelper.OtherCtrl.lower_teeth_facialControl], panelSize, panelSize, 80, 128);            
            if (controllerTeethDown != null)
            {
                teethControllerDown = new BlendControllerPanel(this, new Rect(controllerTeethDown.windowPosition, controllerTeethDown.windowSize), controllerTeethDown);
                teethControllerDown.Init();
            }

            BlendXController controllerAdd1 = BlenderShapesManager.CreateBlendXCtrl(FaceEditHelper.OtherCtrlName[(int)FaceEditHelper.OtherCtrl.add_facialControl]);
            if (controllerAdd1 != null)
            {
                controllerPanelAdd1 = new BlendSlideControllerPanel(this, Rect.zero, controllerAdd1, null);
                controllerPanelAdd1.Init();
            }

            BlendXController controllerAdd2 = BlenderShapesManager.CreateBlendXCtrl(FaceEditHelper.OtherCtrlName[(int)FaceEditHelper.OtherCtrl.add01_facialControl]);
            if (controllerAdd2 != null)
            {
                controllerPanelAdd2 = new BlendSlideControllerPanel(this, Rect.zero, controllerAdd2, null);
                controllerPanelAdd2.Init();
            }

            BlendXController controllerAdd3 = BlenderShapesManager.CreateBlendXCtrl(FaceEditHelper.OtherCtrlName[(int)FaceEditHelper.OtherCtrl.add02_facialControl]);
            if (controllerAdd3 != null)
            {
                controllerPanelAdd3 = new BlendSlideControllerPanel(this, Rect.zero, controllerAdd3, null);
                controllerPanelAdd3.Init();
            }

            BlendXController controllerAdd4 = BlenderShapesManager.CreateBlendXCtrl(FaceEditHelper.OtherCtrlName[(int)FaceEditHelper.OtherCtrl.add03_facialControl]);
            if (controllerAdd4 != null)
            {
                controllerPanelAdd4 = new BlendSlideControllerPanel(this, Rect.zero, controllerAdd4, null);
                controllerPanelAdd4.Init();
            }
        }

        public override void OnPanelDisable()
        {
            base.OnPanelDisable();          
        }

        public void Update()
        {
            if (controllerPanelAdd1 != null)
            {
                controllerPanelAdd1.OnUpdate(false);
            }
            if (controllerPanelAdd2 != null)
            {
                controllerPanelAdd2.OnUpdate(false);
            }            
            if (controllerPanelAdd3 != null)
            {
                controllerPanelAdd3.OnUpdate(false);
            }
            if (controllerPanelAdd4 != null)
            {
                controllerPanelAdd4.OnUpdate(false);
            }

            if (teethControllerUp != null)
                teethControllerUp.OnUpdate(true);
            if (teethControllerDown != null)
                teethControllerDown.OnUpdate(true);
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
                    controllerPanelAdd1.OnDraw(new Vector2(120, 20));
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                ///GUILayout.HorizontalSlider(0, 0, 1.00f, GUILayout.Width(120), GUILayout.Height(20));
                if (controllerPanelAdd2 != null)
                {
                    controllerPanelAdd2.OnDraw(new Vector2(120, 20));
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                ///GUILayout.HorizontalSlider(0, 0, 1.00f, GUILayout.Width(120), GUILayout.Height(20));
                if (controllerPanelAdd3 != null)
                {
                    controllerPanelAdd3.OnDraw(new Vector2(120, 20));
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                //GUILayout.HorizontalSlider(0, 0, 1.00f, GUILayout.Width(120), GUILayout.Height(20));
                if (controllerPanelAdd4 != null)
                {
                    controllerPanelAdd4.OnDraw(new Vector2(120, 20));
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
