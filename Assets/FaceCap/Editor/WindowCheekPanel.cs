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

            //Nose
            BlendGridController _noseController = new BlendGridController();
            _noseController.windowPosition = new Vector2(10, 10);
            _noseController.windowSize = new Vector2(panelSizeMax, panelSizeMax);
            noseController = new BlendControllerPanel(this, new Rect(_noseController.windowPosition, _noseController.windowSize), _noseController);
            noseController.Init();

            BlendYController controllerNoseL = new BlendYController();
            noseSliderPanelLeft = new BlendSlideControllerPanel(this, Rect.zero, null, controllerNoseL);
            noseSliderPanelLeft.Init();

            BlendYController controllerNose1R = new BlendYController();
            noseSliderPanelRight = new BlendSlideControllerPanel(this, Rect.zero, null, controllerNose1R);
            noseSliderPanelRight.Init();

            BlendXController controllerCheekLeft = new BlendXController();
            cheekSliderPanelLeft = new BlendSlideControllerPanel(this, Rect.zero, controllerCheekLeft, null);
            cheekSliderPanelLeft.Init();
            BlendXController controllerCheekRight = new BlendXController();
            cheekSliderPanelRight = new BlendSlideControllerPanel(this, Rect.zero, controllerCheekRight, null);
            cheekSliderPanelRight.Init();

        }

        public override void OnPanelEnable()
        {
            base.OnPanelEnable();          
        }

        public override void OnPanelDisable()
        {
            base.OnPanelDisable();
            noseController.Reset();
        }

        public override void OnDraw()
        {
            GUILayout.BeginArea(panelRect);

            var newRect = new Rect(0, 20, panelRect.width/2, panelRect.height);
            GUILayout.BeginArea(newRect);
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            //GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(120));
            noseSliderPanelLeft.OnDraw(new Vector2(30, 120));
            GUILayout.FlexibleSpace();
            newRect = new Rect(panelRect.width / 4 - panelSizeMax / 2 - 20, 0, 260, panelRect.height);
            GUILayout.BeginArea(newRect);
            noseController.OnDraw();
            GUILayout.EndArea();
            GUILayout.FlexibleSpace();
            //GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(120));
            noseSliderPanelRight.OnDraw(new Vector2(30, 120));
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
            cheekSliderPanelLeft.OnDraw(new Vector2(120, 20));
            GUILayout.Space(6);      
            cheekSliderPanelRight.OnDraw(new Vector2(120, 20));
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.EndArea();

            GUILayout.EndArea();
        }
    }
}
