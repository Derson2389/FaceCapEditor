using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

namespace FaceCapEditor
{
    public class WindowBrowPanel : WindowPanel
    {

        public GUIStyle unsetStyle = null;
        public GUIStyle setStyle = null;
        public const int panelSize = 100;
        private BlendControllerPanel leftController;
        private BlendControllerPanel rightController;

        public WindowBrowPanel(EditorWindow window, Rect rect)
        {
            panelRect = rect;
            panelWindow = window;
            BlendGridController controllerLeft = new BlendGridController();
            controllerLeft.windowPosition = new Vector2(20, 10);
            controllerLeft.windowSize = new Vector2(panelSize, panelSize);
            leftController  = new BlendControllerPanel(this, new Rect(controllerLeft.windowPosition, controllerLeft.windowSize), controllerLeft);
            leftController.Init();

            BlendGridController controllerRight = new BlendGridController();
            controllerRight.windowPosition = new Vector2(20, 10);
            controllerRight.windowSize = new Vector2(panelSize, panelSize);
            rightController = new BlendControllerPanel(this, new Rect(controllerRight.windowPosition, controllerRight.windowSize), controllerRight);
            rightController.Init();
        }

        public override void OnPanelEnable()
        {
            base.OnPanelEnable();

            Texture2D unsetIcon = (Texture2D)AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/FaceCap/Png/default_icon.png");
            Texture2D setIcon = (Texture2D)AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/FaceCap/Png/white.png");

            unsetStyle = new GUIStyle();
            unsetStyle.normal.background = unsetIcon;
            setStyle = new GUIStyle();
            setStyle.normal.background = setIcon;
            setStyle.alignment = TextAnchor.MiddleCenter;
        }

        public override void OnPanelDisable()
        {
            base.OnPanelDisable();
            leftController.Reset();
            rightController.Reset();
        }

        public override void OnDraw()
        {
            
            GUILayout.BeginArea(panelRect);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUIStyle myStyle = new GUIStyle();
                myStyle.fontSize = 22;
                myStyle.normal.textColor = Color.white;
                GUILayout.Label("Brow", myStyle);
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            var newRect = new Rect(panelRect.x, panelRect.y, panelRect.width, panelRect.height);
            GUILayout.BeginArea(newRect);
            GUILayout.BeginHorizontal();
            {                
                leftController.OnDraw();
                EditorGUIUtility.AddCursorRect(leftController.centerPanel, MouseCursor.Link);                
            }
            GUILayout.EndArea();
            newRect = new Rect(panelRect.x + 160, panelRect.y, panelRect.width, panelRect.height);
            GUILayout.BeginArea(newRect);         
            {
                GUILayout.BeginHorizontal();
                GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(panelRect.height - 36));
                GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(panelRect.height - 36));
                GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(panelRect.height - 36));
                GUILayout.EndHorizontal();
                //GUILayout.FlexibleSpace();
                //GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(panelRect.height - 36));
                //GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(panelRect.height - 36));
                //GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(panelRect.height - 36));
            }
            GUILayout.EndArea();

            newRect = new Rect(panelRect.width - 230, panelRect.y, panelRect.width, panelRect.height);
            GUILayout.BeginArea(newRect);
            {
                GUILayout.BeginHorizontal();
                GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(panelRect.height - 36));
                GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(panelRect.height - 36));
                GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(panelRect.height - 36));
                GUILayout.EndHorizontal();
                
            }
            GUILayout.EndArea();

            newRect = new Rect(panelRect.width - 140, panelRect.y, panelRect.width, panelRect.height);
            GUILayout.BeginArea(newRect);
            GUILayout.BeginHorizontal();
            {
                rightController.OnDraw();
                EditorGUIUtility.AddCursorRect(rightController.centerPanel, MouseCursor.Link);
            }
            GUILayout.EndArea();

            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}
