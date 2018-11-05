using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

namespace FaceCapEditor
{
    public class WindowEyePanel : WindowPanel
    {
        //Eyelid
        private BlendControllerPanel eyeLidLeftUpController;
        private BlendControllerPanel eyeLidRightUpController;
        private BlendControllerPanel eyeLidLeftDownController;
        private BlendControllerPanel eyeLidRightDownController;

        private BlendSlideControllerPanel eyeLidSliedLeftCtrl;
        private BlendSlideControllerPanel eyeLidSliedRightCtrl;

        //Eye
        private BlendControllerPanel eyeLeftUpController;
        private BlendControllerPanel eyeRightUpController;
        private BlendControllerPanel eyeLeftDownController;
        private BlendControllerPanel eyeRightDownController;

        private BlendSlideControllerPanel eyeLeftCtrl1;
        private BlendSlideControllerPanel eyeLeftCtrl2;
        private BlendSlideControllerPanel eyeRightCtrl1;
        private BlendSlideControllerPanel eyeRightCtrl2;
        
        public const int panelSizeMax = 100;
        public const int panelSizeMin = 70;

        public WindowEyePanel(EditorWindow window, Rect rect)
        {
            panelRect = rect;
            panelWindow = window;
            //Eyelid
            BlendGridController controllerLeft = new BlendGridController();
            controllerLeft.windowPosition = new Vector2(0, 10);
            controllerLeft.windowSize = new Vector2(panelSizeMax, panelSizeMax);
            eyeLidLeftUpController = new BlendControllerPanel(this, new Rect(controllerLeft.windowPosition, controllerLeft.windowSize), controllerLeft);
            eyeLidLeftUpController.Init();

            BlendGridController controllerRight = new BlendGridController();
            controllerRight.windowPosition = new Vector2(110, 10);
            controllerRight.windowSize = new Vector2(panelSizeMax, panelSizeMax);
            eyeLidRightUpController = new BlendControllerPanel(this, new Rect(controllerRight.windowPosition, controllerRight.windowSize), controllerRight);
            eyeLidRightUpController.Init();
       
            BlendGridController controllerLeftDown = new BlendGridController();
            controllerLeftDown.windowPosition = new Vector2(0, 120);
            controllerLeftDown.windowSize = new Vector2(panelSizeMax, panelSizeMax);
            eyeLidLeftDownController = new BlendControllerPanel(this, new Rect(controllerLeftDown.windowPosition, controllerLeftDown.windowSize), controllerLeftDown);
            eyeLidLeftDownController.Init();

            BlendGridController controllerRightDown = new BlendGridController();
            controllerRightDown.windowPosition = new Vector2(110, 120);
            controllerRightDown.windowSize = new Vector2(panelSizeMax, panelSizeMax);
            eyeLidRightDownController = new BlendControllerPanel(this, new Rect(controllerRightDown.windowPosition, controllerRightDown.windowSize), controllerRightDown);
            eyeLidRightDownController.Init();

            BlendYController controllerEyeLidLeft1 = new BlendYController();
            eyeLidSliedLeftCtrl = new BlendSlideControllerPanel(this, Rect.zero, null, controllerEyeLidLeft1);
            eyeLidSliedLeftCtrl.Init();
            BlendYController controllerEyeLidLeft2 = new BlendYController();
            eyeLidSliedRightCtrl = new BlendSlideControllerPanel(this, Rect.zero, null, controllerEyeLidLeft2);
            eyeLidSliedRightCtrl.Init();


            //Eye
            BlendGridController eyecontrollerLeft = new BlendGridController();
            eyecontrollerLeft.windowPosition = new Vector2(10, 10);
            eyecontrollerLeft.windowSize = new Vector2(panelSizeMax, panelSizeMax);
            eyeLeftUpController = new BlendControllerPanel(this, new Rect(eyecontrollerLeft.windowPosition, eyecontrollerLeft.windowSize), eyecontrollerLeft);
            eyeLeftUpController.Init();

            BlendGridController eyecontrollerRight = new BlendGridController();
            eyecontrollerRight.windowPosition = new Vector2(120, 10);
            eyecontrollerRight.windowSize = new Vector2(panelSizeMax, panelSizeMax);
            eyeRightUpController = new BlendControllerPanel(this, new Rect(eyecontrollerRight.windowPosition, eyecontrollerRight.windowSize), eyecontrollerRight);
            eyeRightUpController.Init();

            BlendGridController eyecontrollerLeftDown = new BlendGridController();
            eyecontrollerLeftDown.windowPosition = new Vector2(40, 120);
            eyecontrollerLeftDown.windowSize = new Vector2(panelSizeMin, panelSizeMin);
            eyeLeftDownController = new BlendControllerPanel(this, new Rect(eyecontrollerLeftDown.windowPosition, eyecontrollerLeftDown.windowSize), eyecontrollerLeftDown);
            eyeLeftDownController.Init();

            BlendGridController eyecontrollerRightDown = new BlendGridController();
            eyecontrollerRightDown.windowPosition = new Vector2(120, 120);
            eyecontrollerRightDown.windowSize = new Vector2(panelSizeMin, panelSizeMin);
            eyeRightDownController = new BlendControllerPanel(this, new Rect(eyecontrollerRightDown.windowPosition, eyecontrollerRightDown.windowSize), eyecontrollerRightDown);
            eyeRightDownController.Init();

            BlendYController controllerEyeLeft1 = new BlendYController();
            eyeLeftCtrl1 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerEyeLeft1);
            eyeLeftCtrl1.Init();

            BlendYController controllerEyeLeft2 = new BlendYController();
            eyeLeftCtrl2 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerEyeLeft2);
            eyeLeftCtrl2.Init();


            BlendYController controllerEyeRightCtrl1 = new BlendYController();
            eyeRightCtrl1 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerEyeRightCtrl1);
            eyeRightCtrl1.Init();

            BlendYController controllerEyeRightCtrl2 = new BlendYController();
            eyeRightCtrl2 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerEyeRightCtrl2);
            eyeRightCtrl2.Init();

        }

        public override void OnPanelEnable()
        {
            base.OnPanelEnable();          
        }

        public override void OnPanelDisable()
        {
            base.OnPanelDisable();          
        }

        public override void OnDraw()
        {
            GUILayout.BeginArea(panelRect);
            GUILayout.BeginVertical();
            float offset_y = 10f;
            var newRect = new Rect(0, offset_y, 80, panelRect.height);
            GUILayout.BeginArea(newRect);
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    //GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(180));
                    eyeLidSliedLeftCtrl.OnDraw(new Vector2(30,180));
                    //GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(180));
                    eyeLidSliedRightCtrl.OnDraw(new Vector2(30, 180));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndArea();
            }

            newRect = new Rect(80, offset_y, 360, panelRect.height);
            GUILayout.BeginArea(newRect);
            {
                GUILayout.BeginHorizontal();
                {
                    eyeLidLeftUpController.OnDraw();
                    EditorGUIUtility.AddCursorRect(eyeLidLeftUpController.centerPanel, MouseCursor.Link);

                    eyeLidRightUpController.OnDraw();
                    EditorGUIUtility.AddCursorRect(eyeLidRightUpController.centerPanel, MouseCursor.Link);

                    eyeLidLeftDownController.OnDraw();
                    EditorGUIUtility.AddCursorRect(eyeLidLeftDownController.centerPanel, MouseCursor.Link);

                    eyeLidRightDownController.OnDraw();
                    EditorGUIUtility.AddCursorRect(eyeLidRightDownController.centerPanel, MouseCursor.Link);

                    GUILayout.EndHorizontal();
                }
                GUILayout.EndArea();
            }

            newRect = new Rect(panelRect.width - 240, offset_y, 360, panelRect.height);
            GUILayout.BeginArea(newRect);
            {
                GUILayout.BeginHorizontal();
                {
                    eyeLeftUpController.OnDraw();
                    EditorGUIUtility.AddCursorRect(eyeLeftUpController.centerPanel, MouseCursor.Link);

                    eyeRightUpController.OnDraw();
                    EditorGUIUtility.AddCursorRect(eyeRightUpController.centerPanel, MouseCursor.Link);

                    eyeLeftDownController.OnDraw();
                    EditorGUIUtility.AddCursorRect(eyeLeftDownController.centerPanel, MouseCursor.Link);

                    eyeRightDownController.OnDraw();
                    EditorGUIUtility.AddCursorRect(eyeRightDownController.centerPanel, MouseCursor.Link);

                    GUILayout.EndHorizontal();
                }
                var newRect1 = new Rect(-56, 120, 160, 100);
                GUILayout.BeginArea(newRect1);
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    //GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(15), GUILayout.Height(80));
                    //GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(15), GUILayout.Height(80));
                    eyeLeftCtrl1.OnDraw(new Vector2(15, 80));
                    eyeLeftCtrl2.OnDraw(new Vector2(15, 80));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.EndArea();
                }

                var newRect2 = new Rect(130, 120, 160, 100);
                GUILayout.BeginArea(newRect2);
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    // GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(15), GUILayout.Height(80));
                    // GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(15), GUILayout.Height(80));
                    eyeRightCtrl1.OnDraw(new Vector2(15, 80));
                    eyeRightCtrl2.OnDraw(new Vector2(15, 80));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.EndArea();
                }

                GUILayout.EndArea();
            }


            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}
