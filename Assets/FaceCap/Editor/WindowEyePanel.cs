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

            //Eyelid
            BlendGridController controllerLeft = FaceEditorMainWin.window.currentHandler.CreateBlendGridCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.r_upper_eyelid_facialControl], panelSizeMax, panelSizeMax, 0, 10);
            if (controllerLeft != null)
            {
                eyeLidLeftUpController = new BlendControllerPanel(this, new Rect(controllerLeft.windowPosition, controllerLeft.windowSize), controllerLeft);
                eyeLidLeftUpController.Init();
                FaceEditorMainWin.window.InserPanelList(eyeLidLeftUpController);
            }

            BlendGridController controllerRight = FaceEditorMainWin.window.currentHandler.CreateBlendGridCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.l_upper_eyelid_facialControl], panelSizeMax, panelSizeMax, 110, 10);
            if (controllerRight != null)
            {
                eyeLidRightUpController = new BlendControllerPanel(this, new Rect(controllerRight.windowPosition, controllerRight.windowSize), controllerRight);
                eyeLidRightUpController.Init();
                FaceEditorMainWin.window.InserPanelList(eyeLidRightUpController);
            }

            BlendGridController controllerLeftDown = FaceEditorMainWin.window.currentHandler.CreateBlendGridCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.r_lower_eyelid_facialControl], panelSizeMax, panelSizeMax, 0, 120);
            if (controllerLeftDown != null)
            {
                eyeLidLeftDownController = new BlendControllerPanel(this, new Rect(controllerLeftDown.windowPosition, controllerLeftDown.windowSize), controllerLeftDown);
                eyeLidLeftDownController.Init();
                FaceEditorMainWin.window.InserPanelList(eyeLidLeftDownController);
            }

            BlendGridController controllerRightDown = FaceEditorMainWin.window.currentHandler.CreateBlendGridCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.l_lower_eyelid_facialControl], panelSizeMax, panelSizeMax, 110, 120);
            if (controllerRightDown != null)
            {
                eyeLidRightDownController = new BlendControllerPanel(this, new Rect(controllerRightDown.windowPosition, controllerRightDown.windowSize), controllerRightDown);
                eyeLidRightDownController.Init();
                FaceEditorMainWin.window.InserPanelList(eyeLidRightDownController);
            }

            BlendYController controllerEyeLidLeft1 = FaceEditorMainWin.window.currentHandler.CreateBlendYCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.r_eyelid_blink_facialControl]);
            if (controllerEyeLidLeft1 != null)
            {
                eyeLidSliedLeftCtrl = new BlendSlideControllerPanel(this, Rect.zero, null, controllerEyeLidLeft1);
                eyeLidSliedLeftCtrl.Init();
                FaceEditorMainWin.window.InserPanelList(eyeLidSliedLeftCtrl);
            }
            BlendYController controllerEyeLidLeft2 = FaceEditorMainWin.window.currentHandler.CreateBlendYCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.l_eyelid_blink_facialControl]);
            if (controllerEyeLidLeft2 != null)
            {
                eyeLidSliedRightCtrl = new BlendSlideControllerPanel(this, Rect.zero, null, controllerEyeLidLeft2);
                eyeLidSliedRightCtrl.Init();
                FaceEditorMainWin.window.InserPanelList(eyeLidSliedRightCtrl);
            }


            //Eye
            BlendGridController eyecontrollerLeft = FaceEditorMainWin.window.currentHandler.CreateBlendGridCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.r_eyeBall_facialControl], panelSizeMax, panelSizeMax, 10, 10);
            if (eyecontrollerLeft != null)
            {
                eyeLeftUpController = new BlendControllerPanel(this, new Rect(eyecontrollerLeft.windowPosition, eyecontrollerLeft.windowSize), eyecontrollerLeft);
                eyeLeftUpController.Init();
                FaceEditorMainWin.window.InserPanelList(eyeLeftUpController);
            }

            BlendGridController eyecontrollerRight = FaceEditorMainWin.window.currentHandler.CreateBlendGridCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.l_eyeBall_facialControl], panelSizeMax, panelSizeMax, 120, 10);
            if (eyecontrollerRight != null)
            {
                eyeRightUpController = new BlendControllerPanel(this, new Rect(eyecontrollerRight.windowPosition, eyecontrollerRight.windowSize), eyecontrollerRight);
                eyeRightUpController.Init();
                FaceEditorMainWin.window.InserPanelList(eyeRightUpController);
            }

            BlendGridController eyecontrollerLeftDown = FaceEditorMainWin.window.currentHandler.CreateBlendGridCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.r_eyeHightLight_move_facialControl], panelSizeMin, panelSizeMin, 40, 120);
            if (eyecontrollerLeftDown != null)
            {
                eyeLeftDownController = new BlendControllerPanel(this, new Rect(eyecontrollerLeftDown.windowPosition, eyecontrollerLeftDown.windowSize), eyecontrollerLeftDown);
                eyeLeftDownController.Init();
                FaceEditorMainWin.window.InserPanelList(eyeLeftDownController);
            }

            BlendGridController eyecontrollerRightDown = FaceEditorMainWin.window.currentHandler.CreateBlendGridCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.l_eyeHightLight_move_facialControl], panelSizeMin, panelSizeMin, 120, 120);
            if (eyecontrollerRightDown != null)
            {
                eyeRightDownController = new BlendControllerPanel(this, new Rect(eyecontrollerRightDown.windowPosition, eyecontrollerRightDown.windowSize), eyecontrollerRightDown);
                eyeRightDownController.Init();
                FaceEditorMainWin.window.InserPanelList(eyeRightDownController);
            }

            BlendYController controllerEyeLeft1 = FaceEditorMainWin.window.currentHandler.CreateBlendYCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.r_pupil_scale_facialControl]);
            if (controllerEyeLeft1 != null)
            {
                eyeLeftCtrl1 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerEyeLeft1);
                eyeLeftCtrl1.Init();
                FaceEditorMainWin.window.InserPanelList(eyeLeftCtrl1);
            }


            BlendYController controllerEyeLeft2 = FaceEditorMainWin.window.currentHandler.CreateBlendYCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.l_pupil_scale_facialControl]);
            if (controllerEyeLeft2 != null)
            {
                eyeLeftCtrl2 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerEyeLeft2);
                eyeLeftCtrl2.Init();
                FaceEditorMainWin.window.InserPanelList(eyeLeftCtrl2);
            }


            BlendYController controllerEyeRightCtrl1 = FaceEditorMainWin.window.currentHandler.CreateBlendYCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.r_eyeHightLight_scale_facialControl]);
            if (controllerEyeLeft2 != null)
            {
                eyeRightCtrl1 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerEyeRightCtrl1);
                eyeRightCtrl1.Init();
                FaceEditorMainWin.window.InserPanelList(eyeRightCtrl1);
            }

            BlendYController controllerEyeRightCtrl2 = FaceEditorMainWin.window.currentHandler.CreateBlendYCtrl(FaceEditHelper.EyeListCtrlName[(int)FaceEditHelper.EyeListCtrl.l_eyeHightLight_scale_facialControl]);
            if (controllerEyeLeft2 != null)
            {
                eyeRightCtrl2 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerEyeRightCtrl2);
                eyeRightCtrl2.Init();
                FaceEditorMainWin.window.InserPanelList(eyeRightCtrl2);
            }
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
                    GUILayout.Space(10);
                    if (eyeLidSliedLeftCtrl!= null)
                        eyeLidSliedLeftCtrl.OnDraw(new Vector2(10,180));
                    GUILayout.Space(10);
                    if (eyeLidSliedRightCtrl!= null)
                        eyeLidSliedRightCtrl.OnDraw(new Vector2(10, 180));
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

            newRect = new Rect(panelRect.width - 260, offset_y, 360, panelRect.height);
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

                var newRect1 = new Rect(-60, 120, 160, 100);
                GUILayout.BeginArea(newRect1);
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    GUILayout.FlexibleSpace(); 
                    if (eyeLeftCtrl1 != null)
                        eyeLeftCtrl1.OnDraw(new Vector2(10, 80));
                    GUILayout.Space(10);
                    if (eyeLeftCtrl2 != null)
                        eyeLeftCtrl2.OnDraw(new Vector2(10, 80));
                    GUILayout.FlexibleSpace();
                    GUILayout.Space(10);
                    GUILayout.EndHorizontal();
                    GUILayout.EndArea();
                }

                var newRect2 = new Rect(130, 120, 160, 100);
                GUILayout.BeginArea(newRect2);
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Space(10);
                    if (eyeRightCtrl1 != null)
                        eyeRightCtrl1.OnDraw(new Vector2(10, 80));
                    GUILayout.Space(10);
                    if (eyeRightCtrl2 != null)
                        eyeRightCtrl2.OnDraw(new Vector2(10, 80));
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
