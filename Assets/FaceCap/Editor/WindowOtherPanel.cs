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

            BlendGridController controllerTeethUp = new BlendGridController();
            controllerTeethUp.windowPosition = new Vector2(80 , 22);
            controllerTeethUp.windowSize = new Vector2(panelSize, panelSize);
            teethControllerUp = new BlendControllerPanel(this, new Rect(controllerTeethUp.windowPosition, controllerTeethUp.windowSize), controllerTeethUp);
            teethControllerUp.Init();

            BlendGridController controllerTeethDown = new BlendGridController();
            controllerTeethDown.windowPosition = new Vector2(80 , 128);
            controllerTeethDown.windowSize = new Vector2(panelSize, panelSize);
            teethControllerDown = new BlendControllerPanel(this, new Rect(controllerTeethDown.windowPosition, controllerTeethDown.windowSize), controllerTeethDown);
            teethControllerDown.Init();

            BlendXController controllerAdd1 = new BlendXController();
            controllerPanelAdd1 = new BlendSlideControllerPanel(this, Rect.zero, controllerAdd1, null);
            controllerPanelAdd1.Init();

            BlendXController controllerAdd2 = new BlendXController();
            controllerPanelAdd2 = new BlendSlideControllerPanel(this, Rect.zero, controllerAdd2, null);
            controllerPanelAdd2.Init();

            BlendXController controllerAdd3 = new BlendXController();
            controllerPanelAdd3 = new BlendSlideControllerPanel(this, Rect.zero, controllerAdd3, null);
            controllerPanelAdd3.Init();

            BlendXController controllerAdd4 = new BlendXController();
            controllerPanelAdd4 = new BlendSlideControllerPanel(this, Rect.zero, controllerAdd4, null);
            controllerPanelAdd4.Init();


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
                teethControllerUp.OnDraw();
                EditorGUIUtility.AddCursorRect(teethControllerUp.centerPanel, MouseCursor.Link);

                teethControllerDown.OnDraw();
                EditorGUIUtility.AddCursorRect(teethControllerDown.centerPanel, MouseCursor.Link);

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
                controllerPanelAdd1.OnDraw(new Vector2(120, 20));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                ///GUILayout.HorizontalSlider(0, 0, 1.00f, GUILayout.Width(120), GUILayout.Height(20));
                controllerPanelAdd2.OnDraw(new Vector2(120, 20));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                ///GUILayout.HorizontalSlider(0, 0, 1.00f, GUILayout.Width(120), GUILayout.Height(20));
                controllerPanelAdd3.OnDraw(new Vector2(120, 20));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                //GUILayout.HorizontalSlider(0, 0, 1.00f, GUILayout.Width(120), GUILayout.Height(20));
                controllerPanelAdd4.OnDraw(new Vector2(120, 20));
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
