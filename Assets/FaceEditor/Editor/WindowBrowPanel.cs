using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

namespace FaceCapEditor
{
    public class WindowBrowPanel : WindowPanel
    {

        public WindowBrowPanel(EditorWindow window, Rect rect)
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
            GUILayout.BeginHorizontal();
            {  
                GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(panelRect.height - 36));
                GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(panelRect.height - 36));
                GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(panelRect.height - 36));
                GUILayout.FlexibleSpace();
                GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(panelRect.height - 36));
                GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(panelRect.height - 36));
                GUILayout.VerticalSlider(0, -1.00f, 1.00f, GUILayout.Width(30), GUILayout.Height(panelRect.height - 36));
            }
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}
