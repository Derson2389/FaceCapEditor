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
            BlendGridController controllerLeft = BlenderShapesManager.CreateBlendGridCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.r_upper_eyelid_facialControl], panelSizeMax, panelSizeMax,0,10);
            if (controllerLeft != null)
            {
                eyeLidLeftUpController = new BlendControllerPanel(this, new Rect(controllerLeft.windowPosition, controllerLeft.windowSize), controllerLeft);
                eyeLidLeftUpController.Init();
            }

            BlendGridController controllerRight = BlenderShapesManager.CreateBlendGridCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.l_upper_eyelid_facialControl], panelSizeMax, panelSizeMax, 110, 10);
            if (controllerRight != null)
            {
                eyeLidRightUpController = new BlendControllerPanel(this, new Rect(controllerRight.windowPosition, controllerRight.windowSize), controllerRight);
                eyeLidRightUpController.Init();
            }
       
            BlendGridController controllerLeftDown  = BlenderShapesManager.CreateBlendGridCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.r_lower_eyelid_facialControl], panelSizeMax, panelSizeMax, 0, 120);
            if (controllerLeftDown != null)
            {
                eyeLidLeftDownController = new BlendControllerPanel(this, new Rect(controllerLeftDown.windowPosition, controllerLeftDown.windowSize), controllerLeftDown);
                eyeLidLeftDownController.Init();
            }

            BlendGridController controllerRightDown = BlenderShapesManager.CreateBlendGridCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.l_lower_eyelid_facialControl], panelSizeMax, panelSizeMax, 110, 120);
            if (controllerRightDown != null)
            {
                eyeLidRightDownController = new BlendControllerPanel(this, new Rect(controllerRightDown.windowPosition, controllerRightDown.windowSize), controllerRightDown);
                eyeLidRightDownController.Init();
            }

            BlendYController controllerEyeLidLeft1 = BlenderShapesManager.CreateBlendYCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.r_eyelid_blink_facialControl]);
            if (controllerEyeLidLeft1 != null)
            {
                eyeLidSliedLeftCtrl = new BlendSlideControllerPanel(this, Rect.zero, null, controllerEyeLidLeft1);
                eyeLidSliedLeftCtrl.Init();
            }
            BlendYController controllerEyeLidLeft2 = BlenderShapesManager.CreateBlendYCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.l_eyelid_blink_facialControl]);
            if (controllerEyeLidLeft2 != null)
            {
                eyeLidSliedRightCtrl = new BlendSlideControllerPanel(this, Rect.zero, null, controllerEyeLidLeft2);
                eyeLidSliedRightCtrl.Init();
            }


            //Eye
            BlendGridController eyecontrollerLeft = BlenderShapesManager.CreateBlendGridCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.r_eyeBall_facialControl], panelSizeMax, panelSizeMax, 10, 10);
            if (eyecontrollerLeft != null)
            {
                eyeLeftUpController = new BlendControllerPanel(this, new Rect(eyecontrollerLeft.windowPosition, eyecontrollerLeft.windowSize), eyecontrollerLeft);
                eyeLeftUpController.Init();
            }

            BlendGridController eyecontrollerRight = BlenderShapesManager.CreateBlendGridCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.l_eyeBall_facialControl], panelSizeMax, panelSizeMax, 120, 10);
            if (eyecontrollerRight != null)
            {
                eyeRightUpController = new BlendControllerPanel(this, new Rect(eyecontrollerRight.windowPosition, eyecontrollerRight.windowSize), eyecontrollerRight);
                eyeRightUpController.Init();
            }

            BlendGridController eyecontrollerLeftDown = BlenderShapesManager.CreateBlendGridCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.r_eyeHightLight_move_facialControl], panelSizeMin, panelSizeMin, 40, 120);
            if (eyecontrollerLeftDown != null)
            {
                eyeLeftDownController = new BlendControllerPanel(this, new Rect(eyecontrollerLeftDown.windowPosition, eyecontrollerLeftDown.windowSize), eyecontrollerLeftDown);
                eyeLeftDownController.Init();
            }

            BlendGridController eyecontrollerRightDown = BlenderShapesManager.CreateBlendGridCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.l_eyeHightLight_move_facialControl], panelSizeMin, panelSizeMin, 120, 120);
            if (eyecontrollerRightDown != null)
            {
                eyeRightDownController = new BlendControllerPanel(this, new Rect(eyecontrollerRightDown.windowPosition, eyecontrollerRightDown.windowSize), eyecontrollerRightDown);
                eyeRightDownController.Init();
            }

            BlendYController controllerEyeLeft1 = BlenderShapesManager.CreateBlendYCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.r_pupil_scale_facialControl]);
            if(controllerEyeLeft1 != null)
            {
                eyeLeftCtrl1 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerEyeLeft1);
                eyeLeftCtrl1.Init();
            }       

            
            BlendYController controllerEyeLeft2  = BlenderShapesManager.CreateBlendYCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.l_pupil_scale_facialControl]);
            if (controllerEyeLeft2 != null)
            {
                eyeLeftCtrl2 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerEyeLeft2);
                eyeLeftCtrl2.Init();
            }


            BlendYController controllerEyeRightCtrl1  = BlenderShapesManager.CreateBlendYCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.r_eyeHightLight_scale_facialControl]);
            if (controllerEyeLeft2 != null)
            {
                eyeRightCtrl1 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerEyeRightCtrl1);
                eyeRightCtrl1.Init();
            }

            BlendYController controllerEyeRightCtrl2 = BlenderShapesManager.CreateBlendYCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.l_eyeHightLight_scale_facialControl]);
            if (controllerEyeLeft2 != null)
            {
                eyeRightCtrl2 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerEyeRightCtrl2);
                eyeRightCtrl2.Init();
            }

        }

        public override void OnPanelEnable()
        {
            base.OnPanelEnable();          
        }

        public override void OnPanelDisable()
        {
            base.OnPanelDisable();          
        }

        public override void Update(bool focus)
        {        
            if (eyeLidSliedLeftCtrl != null)
                eyeLidSliedLeftCtrl.OnUpdate(focus);
            if(eyeLidSliedRightCtrl != null)
                eyeLidSliedRightCtrl.OnUpdate(focus);
            if (eyeLeftCtrl1 != null)
                eyeLeftCtrl1.OnUpdate(focus);
            if (eyeLeftCtrl2 != null)
                eyeLeftCtrl2.OnUpdate(focus);
            if (eyeRightCtrl1 != null)
                eyeRightCtrl1.OnUpdate(focus);
            if (eyeRightCtrl2 != null)
                eyeRightCtrl2.OnUpdate(focus);

            if (eyeLidLeftUpController != null)
                eyeLidLeftUpController.OnUpdate(focus);

            if (eyeLidRightUpController != null)
                eyeLidRightUpController.OnUpdate(focus);
            if (eyeLidLeftDownController != null)
                eyeLidLeftDownController.OnUpdate(focus);
            if (eyeLidRightDownController != null)
                eyeLidRightDownController.OnUpdate(focus);

            if (eyeLeftUpController != null)
                eyeLeftUpController.OnUpdate(focus);
            if (eyeRightUpController != null)
                eyeRightUpController.OnUpdate(focus);
            if (eyeLeftDownController != null)
                eyeLeftDownController.OnUpdate(focus);
            if (eyeRightDownController != null)
                eyeRightDownController.OnUpdate(focus);
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
                    if(eyeLidSliedLeftCtrl!= null)
                        eyeLidSliedLeftCtrl.OnDraw(new Vector2(30,180));
                    //GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(180));
                    if(eyeLidSliedRightCtrl!= null)
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
                    if (eyeLidLeftUpController != null)
                    {
                        eyeLidLeftUpController.OnDraw();
                        EditorGUIUtility.AddCursorRect(eyeLidLeftUpController.centerPanel, MouseCursor.Link);
                    }
                    if (eyeLidRightUpController != null)
                    {
                        eyeLidRightUpController.OnDraw();
                        EditorGUIUtility.AddCursorRect(eyeLidRightUpController.centerPanel, MouseCursor.Link);
                    }

                    if (eyeLidLeftDownController != null)
                    {
                        eyeLidLeftDownController.OnDraw();
                        EditorGUIUtility.AddCursorRect(eyeLidLeftDownController.centerPanel, MouseCursor.Link);
                    }

                    if (eyeLidRightDownController != null)
                    {
                        eyeLidRightDownController.OnDraw();
                        EditorGUIUtility.AddCursorRect(eyeLidRightDownController.centerPanel, MouseCursor.Link);
                    }

                    GUILayout.EndHorizontal();
                }
                GUILayout.EndArea();
            }

            newRect = new Rect(panelRect.width - 240, offset_y, 360, panelRect.height);
            GUILayout.BeginArea(newRect);
            {
                GUILayout.BeginHorizontal();
                {
                    if (eyeLeftUpController != null)
                    {
                        eyeLeftUpController.OnDraw();
                        EditorGUIUtility.AddCursorRect(eyeLeftUpController.centerPanel, MouseCursor.Link);
                    }

                    if (eyeRightUpController != null)
                    {
                        eyeRightUpController.OnDraw();
                        EditorGUIUtility.AddCursorRect(eyeRightUpController.centerPanel, MouseCursor.Link);
                    }

                    if (eyeLeftDownController != null)
                    {
                        eyeLeftDownController.OnDraw();
                        EditorGUIUtility.AddCursorRect(eyeLeftDownController.centerPanel, MouseCursor.Link);
                    }

                    if (eyeRightDownController != null)
                    {
                        eyeRightDownController.OnDraw();
                        EditorGUIUtility.AddCursorRect(eyeRightDownController.centerPanel, MouseCursor.Link);
                    }

                    GUILayout.EndHorizontal();
                }
                var newRect1 = new Rect(-56, 120, 160, 100);
                GUILayout.BeginArea(newRect1);
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    //GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(15), GUILayout.Height(80));
                    //GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(15), GUILayout.Height(80));
                    if (eyeLeftCtrl1 != null)
                        eyeLeftCtrl1.OnDraw(new Vector2(15, 80));
                    if (eyeLeftCtrl2 != null)
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
                    if (eyeRightCtrl1 != null)
                        eyeRightCtrl1.OnDraw(new Vector2(15, 80));
                    if (eyeRightCtrl2 != null)
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
