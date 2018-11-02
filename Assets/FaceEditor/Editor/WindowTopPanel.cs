﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

namespace FaceCapEditor
{
    public class WindowTopPanel : WindowPanel
    {
      
        public WindowTopPanel(EditorWindow window, Rect rect)
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
            GUILayout.BeginArea(panelRect, EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("表情编辑器"), EditorStyles.toolbarButton, GUILayout.Width(145)))
            {
                
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}
