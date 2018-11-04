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


        public WindowMouthPanel(EditorWindow window, Rect rect)
        {
            panelRect = rect;
            panelWindow = window;

            BlendGridController controllerTogue = new BlendGridController();
            controllerTogue.windowPosition = new Vector2(10, 22);
            controllerTogue.windowSize = new Vector2(panelSize, panelSize);
            togueController = new BlendControllerPanel(this, new Rect(controllerTogue.windowPosition, controllerTogue.windowSize), controllerTogue);
            togueController.Init();


            BlendGridController controllerJaw = new BlendGridController();
            controllerJaw.windowPosition = new Vector2(10, 28);
            controllerJaw.windowSize = new Vector2(panelSize, panelSize);
            jawController = new BlendControllerPanel(this, new Rect(controllerJaw.windowPosition, controllerJaw.windowSize), controllerJaw);
            jawController.Init();

            BlendGridController controllerMouthUp= new BlendGridController();
            controllerMouthUp.windowPosition = new Vector2(180, 0);
            controllerMouthUp.windowSize = new Vector2(panelSize, panelSize);
            mouthUpController = new BlendControllerPanel(this, new Rect(controllerMouthUp.windowPosition, controllerMouthUp.windowSize), controllerMouthUp);
            mouthUpController.Init();


            BlendGridController controllerMouthDown = new BlendGridController();
            controllerMouthDown.windowPosition = new Vector2(180, 0);
            controllerMouthDown.windowSize = new Vector2(panelSize, panelSize);
            mouthDownController = new BlendControllerPanel(this, new Rect(controllerMouthDown.windowPosition, controllerMouthDown.windowSize), controllerMouthDown);
            mouthDownController.Init();


            BlendGridController controllerCenter = new BlendGridController();
            controllerCenter.windowPosition = new Vector2(180, 0);
            controllerCenter.windowSize = new Vector2(panelSize, panelSize);
            mouthCenterController = new BlendControllerPanel(this, new Rect(controllerCenter.windowPosition, controllerCenter.windowSize), controllerCenter);
            mouthCenterController.Init();


            BlendGridController controllerLeft = new BlendGridController();
            controllerLeft.windowPosition = new Vector2(65, 0);
            controllerLeft.windowSize = new Vector2(panelSize, panelSize);
            mouthLeftController = new BlendControllerPanel(this, new Rect(controllerLeft.windowPosition, controllerLeft.windowSize), controllerLeft);
            mouthLeftController.Init();


            BlendGridController controllerRight = new BlendGridController();
            controllerRight.windowPosition = new Vector2(300, 0);
            controllerRight.windowSize = new Vector2(panelSize, panelSize);
            mouthRightController = new BlendControllerPanel(this, new Rect(controllerRight.windowPosition, controllerRight.windowSize), controllerRight);
            mouthRightController.Init();

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
                        GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(80));
                        GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(80));
                        GUILayout.FlexibleSpace();
                        GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(80));
                        GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(80));
                        GUILayout.EndHorizontal();

                        mouthUpController.OnDraw();
                        EditorGUIUtility.AddCursorRect(mouthUpController.centerPanel, MouseCursor.Link);
                    }
                    GUILayout.EndArea();

                    var newRect2 = new Rect(110, 154, newRect.width - 110, 100);
                    GUILayout.BeginArea(newRect2);
                    {
                        mouthLeftController.OnDraw();
                        EditorGUIUtility.AddCursorRect(mouthLeftController.centerPanel, MouseCursor.Link);

                        mouthRightController.OnDraw();
                        EditorGUIUtility.AddCursorRect(mouthRightController.centerPanel, MouseCursor.Link);

                        mouthCenterController.OnDraw();
                        EditorGUIUtility.AddCursorRect(mouthCenterController.centerPanel, MouseCursor.Link);
                    }
                    GUILayout.EndArea();

                    var newRect3 = new Rect(110, 258, newRect.width - 110, 135);
                    GUILayout.BeginArea(newRect3);
                    {
                        GUILayout.BeginVertical();
                        GUILayout.BeginHorizontal();
                        GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(80));
                        GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(80));
                        GUILayout.FlexibleSpace();
                        GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(80));
                        GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(80));
                        GUILayout.EndHorizontal();
                        GUILayout.FlexibleSpace();
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        GUILayout.HorizontalSlider(0, -1.00f, 1.00f, GUILayout.Width(80), GUILayout.Height(30));
                        GUILayout.Space(20);
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();

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
