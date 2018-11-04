using UnityEngine;
using UnityEditor;
using System.Collections;

namespace FaceCapEditor
{
    public class WindowPanel
    {
        public float leftPanelMargin;
        public float rightPanelMargin;
        public float topPanelMargin;
        public float bottomPanelMargin; 

        /// <summary>
        /// the display rect of this panel
        /// </summary>
        public Rect panelRect;

        /// <summary>
        /// the window of this panel
        /// </summary>
        public EditorWindow panelWindow;

        /// <summary>
        /// interface for EditorWindow's OnEnable
        /// </summary>
        public virtual void OnPanelEnable() { }

        /// <summary>
        /// interface for EditorWindow's OnDisable
        /// </summary>
        public virtual void OnPanelDisable() { }

        /// <summary>
        /// interface for EditorWindow's OnSelectionChange
        /// </summary>
        public virtual void OnPanelSelectionChange() { }

        /// <summary>
        /// interface for EditorWindow's OnDestory
        /// </summary>
        public virtual void OnPanelDestory() { }

        /// <summary>
        /// Init
        /// </summary>
        public virtual void OnInit() { }

        /// <summary>
        /// interface for EditorWindow's OnGUI
        /// </summary>
        public virtual void OnDraw() { }

        /// <summary>
        /// handle event
        /// </summary>
        /// <param name="e"></param>
        public virtual void ProcessEvents(Event e) { }
    }
}

