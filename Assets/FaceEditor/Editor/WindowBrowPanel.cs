﻿using UnityEngine;
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
            GUILayout.FlexibleSpace();
            //if (GUILayout.Button(new GUIContent("brow "), EditorStyles.toolbarButton, GUILayout.Width(panelRect.width)))
            //{

            //}
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}
