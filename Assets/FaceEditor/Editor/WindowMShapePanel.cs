using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

namespace FaceCapEditor
{
    public class WindowMShapePanel : WindowPanel
    {

        public WindowMShapePanel(EditorWindow window, Rect rect)
        {
            panelRect = rect;
            panelWindow = window;
        }

        public override void OnPanelEnable()
        {
            base.OnPanelEnable();
        }

        public override void OnPanelDisable()
        {
            base.OnPanelDisable();
        }


        private float defaultHeight = 66f;
        public override void OnDraw()
        {
            GUILayout.BeginArea(panelRect);
            GUILayout.BeginVertical();

            GUILayout.Space(defaultHeight/2);
            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();
            GUIStyle myStyle = new GUIStyle();
            myStyle.fontSize = 21;
            myStyle.normal.textColor = Color.white;
            GUILayout.Label("A", myStyle);
            GUILayout.HorizontalSlider(0, -1.00f, 1.00f, GUILayout.Width(panelRect.width - 45), GUILayout.Height(defaultHeight));
            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("E", myStyle);
            GUILayout.HorizontalSlider(0, -1.00f, 1.00f, GUILayout.Width(panelRect.width - 45), GUILayout.Height(defaultHeight));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("I", myStyle);
            GUILayout.HorizontalSlider(0, -1.00f, 1.00f, GUILayout.Width(panelRect.width - 45), GUILayout.Height(defaultHeight));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("O", myStyle);
            GUILayout.HorizontalSlider(0, -1.00f, 1.00f, GUILayout.Width(panelRect.width - 45), GUILayout.Height(defaultHeight));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("U", myStyle);
            GUILayout.HorizontalSlider(0, -1.00f, 1.00f, GUILayout.Width(panelRect.width - 45), GUILayout.Height(defaultHeight));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("F", myStyle);
            GUILayout.HorizontalSlider(0, -1.00f, 1.00f, GUILayout.Width(panelRect.width - 45), GUILayout.Height(defaultHeight));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("M", myStyle);
            GUILayout.HorizontalSlider(0, -1.00f, 1.00f, GUILayout.Width(panelRect.width - 45), GUILayout.Height(defaultHeight));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}
