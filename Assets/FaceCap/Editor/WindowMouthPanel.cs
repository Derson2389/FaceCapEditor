using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

namespace FaceCapEditor
{
    public class WindowMouthPanel : WindowPanel
    {

        private const int panelSize = 100;
        //Togue
        private BlendControllerPanel togueController;

        //Jaw
        private BlendControllerPanel jawController;

        //Mouth
        private BlendControllerPanel mouthLeftController;
        private BlendControllerPanel mouthRightController;
        private BlendControllerPanel mouthUpController;
        private BlendControllerPanel mouthDownController;
        private BlendControllerPanel mouthCenterController;

        //slide left up
        private BlendSlideControllerPanel mouthLeftUpPanel1;
        private BlendSlideControllerPanel mouthLeftUpPanel2;
        //slide right up
        private BlendSlideControllerPanel mouthRightUpPanel1;
        private BlendSlideControllerPanel mouthRightUpPanel2;
        //slide left down
        private BlendSlideControllerPanel mouthLeftDownPanel1;
        private BlendSlideControllerPanel mouthLeftDownPanel2;
        //slide right down
        private BlendSlideControllerPanel mouthRightDownPanel1;
        private BlendSlideControllerPanel mouthRightDownPanel2;
        //slide center down
        private BlendSlideControllerPanel mouthCenterDownPanel;

        public WindowMouthPanel(EditorWindow window, Rect rect)
        {
            panelRect = rect;
            panelWindow = window;

            BlendGridController controllerTogue = BlenderShapesManager.CreateBlendGridCtrl(FaceEditHelper.MouthCtrlName[(int)FaceEditHelper.MouthCtrl.tongue_facialControl], panelSize, panelSize, 10, 22);
            
            if (controllerTogue != null)
            {
                togueController = new BlendControllerPanel(this, new Rect(controllerTogue.windowPosition, controllerTogue.windowSize), controllerTogue);
                togueController.Init();
            }

            BlendGridController controllerJaw = BlenderShapesManager.CreateBlendGridCtrl(FaceEditHelper.MouthCtrlName[(int)FaceEditHelper.MouthCtrl.jaw_facialControl], panelSize, panelSize, 10, 28);
            if (controllerJaw != null)
            {
                jawController = new BlendControllerPanel(this, new Rect(controllerJaw.windowPosition, controllerJaw.windowSize), controllerJaw);
                jawController.Init();
            }

            BlendGridController controllerMouthUp = BlenderShapesManager.CreateBlendGridCtrl(FaceEditHelper.MouthCtrlName[(int)FaceEditHelper.MouthCtrl.upper_lip_facialControl], panelSize, panelSize, 180, 0);
            if (controllerMouthUp != null)
            {
                mouthUpController = new BlendControllerPanel(this, new Rect(controllerMouthUp.windowPosition, controllerMouthUp.windowSize), controllerMouthUp);
                mouthUpController.Init();
            }

            BlendGridController controllerMouthDown = BlenderShapesManager.CreateBlendGridCtrl(FaceEditHelper.MouthCtrlName[(int)FaceEditHelper.MouthCtrl.lower_lip_facialControl], panelSize, panelSize, 180, 0);
            if (controllerMouthDown != null)
            {
                mouthDownController = new BlendControllerPanel(this, new Rect(controllerMouthDown.windowPosition, controllerMouthDown.windowSize), controllerMouthDown);
                mouthDownController.Init();
            }

            BlendGridController controllerCenter = BlenderShapesManager.CreateBlendGridCtrl(FaceEditHelper.MouthCtrlName[(int)FaceEditHelper.MouthCtrl.mouth_move_facialControl], panelSize, panelSize, 180, 0);

            if (controllerCenter != null)
            {
                mouthCenterController = new BlendControllerPanel(this, new Rect(controllerCenter.windowPosition, controllerCenter.windowSize), controllerCenter);
                mouthCenterController.Init();
            }


            BlendGridController controllerLeft = BlenderShapesManager.CreateBlendGridCtrl(FaceEditHelper.MouthCtrlName[(int)FaceEditHelper.MouthCtrl.r_corners_facialControl], panelSize, panelSize, 65, 0);

            if (controllerLeft != null)
            {
                mouthLeftController = new BlendControllerPanel(this, new Rect(controllerLeft.windowPosition, controllerLeft.windowSize), controllerLeft);
                mouthLeftController.Init();
            }

            BlendGridController controllerRight = BlenderShapesManager.CreateBlendGridCtrl(FaceEditHelper.MouthCtrlName[(int)FaceEditHelper.MouthCtrl.l_corners_facialControl], panelSize, panelSize, 300, 0);

            if (controllerRight != null)
            {
                mouthRightController = new BlendControllerPanel(this, new Rect(controllerRight.windowPosition, controllerRight.windowSize), controllerRight);
                mouthRightController.Init();
            }

            //slider controller
            //up
            BlendYController controllerMouthUpL = BlenderShapesManager.CreateBlendYCtrl(FaceEditHelper.MouthCtrlName[(int)FaceEditHelper.MouthCtrl.r_upper_corners_facialControl]);
            if (controllerMouthUpL != null)
            {
                mouthLeftUpPanel1 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerMouthUpL);
                mouthLeftUpPanel1.Init();
            }

            BlendYController controllerMouthUpR = BlenderShapesManager.CreateBlendYCtrl(FaceEditHelper.MouthCtrlName[(int)FaceEditHelper.MouthCtrl.r_upper_lip_facialControl]);
            if (controllerMouthUpR != null)
            {
                mouthLeftUpPanel2 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerMouthUpR);
                mouthLeftUpPanel2.Init();
            }

            BlendYController controllerMouthL = BlenderShapesManager.CreateBlendYCtrl(FaceEditHelper.MouthCtrlName[(int)FaceEditHelper.MouthCtrl.l_upper_lip_facialControl]);
            if (controllerMouthL != null)
            {
                mouthRightUpPanel1 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerMouthL);
                mouthRightUpPanel1.Init();
            }

            BlendYController controllerMouthR = BlenderShapesManager.CreateBlendYCtrl(FaceEditHelper.MouthCtrlName[(int)FaceEditHelper.MouthCtrl.l_upper_corners_facialControl]);
            if (controllerMouthR != null)
            {
                mouthRightUpPanel2 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerMouthR);
                mouthRightUpPanel2.Init();
            }
            //down
            BlendYController controllerMouthDownL1 = BlenderShapesManager.CreateBlendYCtrl(FaceEditHelper.MouthCtrlName[(int)FaceEditHelper.MouthCtrl.r_lower_corners_facialControl]);
            if (controllerMouthDownL1 != null)
            {
                mouthLeftDownPanel1 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerMouthDownL1);
                mouthLeftDownPanel1.Init();
            }

            BlendYController controllerMouthDownL2 = BlenderShapesManager.CreateBlendYCtrl(FaceEditHelper.MouthCtrlName[(int)FaceEditHelper.MouthCtrl.r_lower_lip_facialControl]);
            if (controllerMouthDownL2 != null)
            {
                mouthLeftDownPanel2 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerMouthDownL2);
                mouthLeftDownPanel2.Init();
            }

            BlendYController controllerMouthDownR1 = BlenderShapesManager.CreateBlendYCtrl(FaceEditHelper.MouthCtrlName[(int)FaceEditHelper.MouthCtrl.l_lower_lip_facialControl]);
            if (controllerMouthDownR1 != null)
            {
                mouthRightDownPanel1 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerMouthDownR1);
                mouthRightDownPanel1.Init();
            }
            BlendYController controllerMouthDownR2 = BlenderShapesManager.CreateBlendYCtrl(FaceEditHelper.MouthCtrlName[(int)FaceEditHelper.MouthCtrl.l_lower_corners_facialControl]);
            if (controllerMouthDownR2 != null)
            {
                mouthRightDownPanel2 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerMouthDownR2);
                mouthRightDownPanel2.Init();
            }

            BlendXController controllerCenterDown = BlenderShapesManager.CreateBlendXCtrl(FaceEditHelper.MouthCtrlName[(int)FaceEditHelper.MouthCtrl.mouth_rotate_facialControl]);
            if (controllerCenterDown != null)
            {
                mouthCenterDownPanel = new BlendSlideControllerPanel(this, Rect.zero, controllerCenterDown, null);
                mouthCenterDownPanel.Init();
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

        public void Update()
        {
            if (togueController != null)
                togueController.OnUpdate(true);

            if (jawController != null)
                jawController.OnUpdate(true);

            //Mouth
            if (mouthLeftController != null)
                mouthLeftController.OnUpdate(true);
            if (mouthRightController != null)
                mouthRightController.OnUpdate(true);
            if (mouthUpController != null)
                mouthUpController.OnUpdate(true);
            if (mouthDownController != null)
                mouthDownController.OnUpdate(true);
            if (mouthCenterController != null)
                mouthCenterController.OnUpdate(true);

            //slide left up
            if (mouthLeftUpPanel1 != null)
                mouthLeftUpPanel1.OnUpdate(true);
            if (mouthLeftUpPanel2 != null)
                mouthLeftUpPanel2.OnUpdate(true);
            //slide right up
            if (mouthRightUpPanel1 != null)
                mouthRightUpPanel1.OnUpdate(true);
            if (mouthRightUpPanel2 != null)
                mouthRightUpPanel2.OnUpdate(true);
            //slide left down
            if (mouthLeftDownPanel1 != null)
                mouthLeftDownPanel1.OnUpdate(true);
            if (mouthLeftDownPanel2 != null)
                mouthLeftDownPanel2.OnUpdate(true);
            //slide right down
            if (mouthRightDownPanel1 != null)
                mouthRightDownPanel1.OnUpdate(true);
            if (mouthRightDownPanel2 != null)
                mouthRightDownPanel2.OnUpdate(true);
            //slide center down
            if (mouthCenterDownPanel != null)
                mouthCenterDownPanel.OnUpdate(true);

    }


        public override void OnDraw()
        {
            GUILayout.BeginArea(panelRect);
            {
                //left Tongue and jaw
                var newRect = new Rect(0, 20, 110, panelRect.height);
                GUILayout.BeginArea(newRect);
                {
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUIStyle myStyle = new GUIStyle();
                    myStyle.fontSize = 22;
                    myStyle.normal.textColor = Color.white;
                    GUILayout.Label("Tongue", myStyle);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.FlexibleSpace();
                    newRect = new Rect(0, 20, 110, panelRect.height - 20);
                    GUILayout.BeginArea(newRect);
                    {
                        if (togueController != null)
                            togueController.OnDraw();
                        EditorGUIUtility.AddCursorRect(togueController.centerPanel, MouseCursor.Link);

                    }
                    GUILayout.EndArea();
                    newRect = new Rect(0, 180, 110, 200);
                    GUILayout.BeginArea(newRect);
                    {
                        GUILayout.BeginVertical();
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();                        
                        myStyle.fontSize = 22;
                        myStyle.normal.textColor = Color.white;
                        GUILayout.Label("Jaw", myStyle);
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        GUILayout.Space(20);
                        GUILayout.FlexibleSpace();
                        if (jawController != null)
                            jawController.OnDraw();
                       EditorGUIUtility.AddCursorRect(jawController.centerPanel, MouseCursor.Link);

                    }
                    GUILayout.EndArea();
                    GUILayout.EndVertical();
                }
                GUILayout.EndArea();

                //Mouth
                newRect = new Rect(110, 0, panelRect.width - 110,panelRect.height);
                GUILayout.BeginArea(newRect);
                {
                    GUILayout.BeginHorizontal();                   
                    GUILayout.FlexibleSpace();
                    GUILayout.Space(68);
;                   GUILayout.BeginVertical();
                    GUILayout.Space(18);
                    GUIStyle myStyle = new GUIStyle();
                    myStyle.fontSize = 22;
                    myStyle.normal.textColor = Color.white;
                    GUILayout.Label("Mouth", myStyle);
                    GUILayout.EndVertical();
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    var newRect1 = new Rect(110, 48, newRect.width - 110, 100);
                    GUILayout.BeginArea(newRect1);
                    {
                        GUILayout.BeginHorizontal();
                        if (mouthLeftUpPanel1 != null)
                            mouthLeftUpPanel1.OnDraw(new Vector2(30, 80));
                        if (mouthLeftUpPanel2 != null)
                            mouthLeftUpPanel2.OnDraw(new Vector2(30, 80));
                        GUILayout.FlexibleSpace();

                        if (mouthRightUpPanel1 != null)
                            mouthRightUpPanel1.OnDraw(new Vector2(30, 80));
                        if (mouthRightUpPanel2 != null)
                            mouthRightUpPanel2.OnDraw(new Vector2(30, 80));

                        GUILayout.EndHorizontal();

                        if (mouthUpController != null)
                            mouthUpController.OnDraw();
                        EditorGUIUtility.AddCursorRect(mouthUpController.centerPanel, MouseCursor.Link);
                    }
                    GUILayout.EndArea();

                    var newRect2 = new Rect(110, 154, newRect.width - 110, 100);
                    GUILayout.BeginArea(newRect2);
                    {
                        if (mouthLeftController != null)
                            mouthLeftController.OnDraw();
                        EditorGUIUtility.AddCursorRect(mouthLeftController.centerPanel, MouseCursor.Link);

                        if (mouthRightController != null)
                            mouthRightController.OnDraw();
                        EditorGUIUtility.AddCursorRect(mouthRightController.centerPanel, MouseCursor.Link);

                        if (mouthCenterController != null)
                            mouthCenterController.OnDraw();
                        EditorGUIUtility.AddCursorRect(mouthCenterController.centerPanel, MouseCursor.Link);
                    }
                    GUILayout.EndArea();

                    var newRect3 = new Rect(110, 258, newRect.width - 110, 135);
                    GUILayout.BeginArea(newRect3);
                    {
                        GUILayout.BeginVertical();
                        GUILayout.BeginHorizontal();

                        if (mouthLeftDownPanel1 != null)
                            mouthLeftDownPanel1.OnDraw(new Vector2(30, 80));
                        if (mouthLeftDownPanel2 != null)
                            mouthLeftDownPanel2.OnDraw(new Vector2(30, 80));

                        GUILayout.FlexibleSpace();
                        if (mouthRightDownPanel1 != null)
                            mouthRightDownPanel1.OnDraw(new Vector2(30, 80));
                        if (mouthRightDownPanel2 != null)
                            mouthRightDownPanel2.OnDraw(new Vector2(30, 80));
                        GUILayout.EndHorizontal();
                        GUILayout.FlexibleSpace();
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        ///GUILayout.HorizontalSlider(0, -1.00f, 1.00f, GUILayout.Width(80), GUILayout.Height(30));
                        if (mouthCenterDownPanel != null)
                            mouthCenterDownPanel.OnDraw(new Vector2(80, 30));
                        GUILayout.Space(20);
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();

                        if (mouthDownController != null)
                            mouthDownController.OnDraw();
                        EditorGUIUtility.AddCursorRect(mouthDownController.centerPanel, MouseCursor.Link);
                    }
                    GUILayout.EndArea();


                }
                GUILayout.EndArea();

            }
            GUILayout.EndArea();
        }
    }
}
