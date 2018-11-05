using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

namespace FaceCapEditor
{
    public class WindowMShapePanel : WindowPanel
    {

        private BlendSlideControllerPanel controllerPanelA;
        private BlendSlideControllerPanel controllerPanelE;
        private BlendSlideControllerPanel controllerPanelI;
        private BlendSlideControllerPanel controllerPanelO;
        private BlendSlideControllerPanel controllerPanelU;
        private BlendSlideControllerPanel controllerPanelF;
        private BlendSlideControllerPanel controllerPanelM;

        public WindowMShapePanel(EditorWindow window, Rect rect)
        {
            panelRect = rect;
            panelWindow = window;

            BlendXController controllerA = new BlendXController();
            controllerPanelA = new BlendSlideControllerPanel(this, Rect.zero , controllerA, null);
            controllerPanelA.Init();

            BlendXController controllerE = new BlendXController();
            controllerPanelE = new BlendSlideControllerPanel(this, Rect.zero, controllerE, null);
            controllerPanelE.Init();


            BlendXController controllerI = new BlendXController();
            controllerPanelI = new BlendSlideControllerPanel(this, Rect.zero, controllerI, null);
            controllerPanelI.Init();


            BlendXController controllerO = new BlendXController();
            controllerPanelO = new BlendSlideControllerPanel(this, Rect.zero, controllerO, null);
            controllerPanelO.Init();

            BlendXController controllerU = new BlendXController();
            controllerPanelU = new BlendSlideControllerPanel(this, Rect.zero, controllerO, null);
            controllerPanelU.Init();

            BlendXController controllerF = new BlendXController();
            controllerPanelF = new BlendSlideControllerPanel(this, Rect.zero, controllerF, null);
            controllerPanelF.Init();

            BlendXController controllerM = new BlendXController();
            controllerPanelM = new BlendSlideControllerPanel(this, Rect.zero, controllerM, null);
            controllerPanelM.Init();
   
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
            controllerPanelA.OnDraw(new Vector2(panelRect.width - 45, defaultHeight));
            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("E", myStyle);        
            controllerPanelE.OnDraw(new Vector2(panelRect.width - 45, defaultHeight));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("I", myStyle);
            controllerPanelI.OnDraw(new Vector2(panelRect.width - 45, defaultHeight));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("O", myStyle);
            controllerPanelO.OnDraw(new Vector2(panelRect.width - 45, defaultHeight));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("U", myStyle);
            controllerPanelU.OnDraw(new Vector2(panelRect.width - 45, defaultHeight));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("F", myStyle);
            controllerPanelF.OnDraw(new Vector2(panelRect.width - 45, defaultHeight));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("M", myStyle);
            controllerPanelM.OnDraw(new Vector2(panelRect.width - 45, defaultHeight));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}
