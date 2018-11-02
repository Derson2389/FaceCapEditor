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
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("brow "), EditorStyles.toolbarButton, GUILayout.Width(145)))
            {

            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}
