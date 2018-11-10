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

        }

        public override void OnPanelEnable()
        {
            base.OnPanelEnable();

            BlendXController controllerA = BlenderShapesManager.CreateBlendXCtrl(FaceEditHelper.MouthShapeCtrlName[(int)FaceEditHelper.MouthShape.A_facialControl]);
            if (controllerA != null)
            {
                controllerPanelA = new BlendSlideControllerPanel(this, Rect.zero, controllerA, null);
                controllerPanelA.Init();

            }

            BlendXController controllerE = BlenderShapesManager.CreateBlendXCtrl(FaceEditHelper.MouthShapeCtrlName[(int)FaceEditHelper.MouthShape.E_facialControl]);
            if (controllerE != null)
            {
                controllerPanelE = new BlendSlideControllerPanel(this, Rect.zero, controllerE, null);
                controllerPanelE.Init();
            }


            BlendXController controllerI = BlenderShapesManager.CreateBlendXCtrl(FaceEditHelper.MouthShapeCtrlName[(int)FaceEditHelper.MouthShape.I_facialControl]);
            if (controllerI != null)
            {
                controllerPanelI = new BlendSlideControllerPanel(this, Rect.zero, controllerI, null);
                controllerPanelI.Init();
            }


            BlendXController controllerO = BlenderShapesManager.CreateBlendXCtrl(FaceEditHelper.MouthShapeCtrlName[(int)FaceEditHelper.MouthShape.O_facialControl]);
            if (controllerO != null)
            {
                controllerPanelO = new BlendSlideControllerPanel(this, Rect.zero, controllerO, null);
                controllerPanelO.Init();
            }
            BlendXController controllerU = BlenderShapesManager.CreateBlendXCtrl(FaceEditHelper.MouthShapeCtrlName[(int)FaceEditHelper.MouthShape.U_facialControl]);
            if (controllerU != null)
            {
                controllerPanelU = new BlendSlideControllerPanel(this, Rect.zero, controllerU, null);
                controllerPanelU.Init();
            }

            BlendXController controllerF = BlenderShapesManager.CreateBlendXCtrl(FaceEditHelper.MouthShapeCtrlName[(int)FaceEditHelper.MouthShape.F_facialControl]);
            if (controllerF != null)
            {
                controllerPanelF = new BlendSlideControllerPanel(this, Rect.zero, controllerF, null);
                controllerPanelF.Init();
            }

            BlendXController controllerM = BlenderShapesManager.CreateBlendXCtrl(FaceEditHelper.MouthShapeCtrlName[(int)FaceEditHelper.MouthShape.M_facialControl]);
            if (controllerM != null)
            {
                controllerPanelM = new BlendSlideControllerPanel(this, Rect.zero, controllerM, null);
                controllerPanelM.Init();
            }
        }

        public override void OnPanelDisable()
        {
            base.OnPanelDisable();
        }

        public override void Update(bool focus)
        {
            if (BlenderShapesManager.controllerList.Count <= 0)
            {
                return;
            }
            if (controllerPanelA != null)
                controllerPanelA.OnUpdate(focus);
            if (controllerPanelE != null)
                controllerPanelE.OnUpdate(focus);
            if (controllerPanelI != null)
                controllerPanelI.OnUpdate(focus);
            if (controllerPanelO != null)
                controllerPanelO.OnUpdate(focus);
            if (controllerPanelU != null)
                controllerPanelU.OnUpdate(focus);

            if (controllerPanelF != null)
                controllerPanelF.OnUpdate(focus);
            if (controllerPanelM != null)
                controllerPanelM.OnUpdate(focus);
            
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
            if(controllerPanelA != null)
                controllerPanelA.OnDraw(new Vector2(panelRect.width - 45, defaultHeight));
            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("E", myStyle);   
            if(controllerPanelE!=null)
                controllerPanelE.OnDraw(new Vector2(panelRect.width - 45, defaultHeight));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("I", myStyle);
            if (controllerPanelI != null)
                controllerPanelI.OnDraw(new Vector2(panelRect.width - 45, defaultHeight));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("O", myStyle);
            if (controllerPanelO != null)
                controllerPanelO.OnDraw(new Vector2(panelRect.width - 45, defaultHeight));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("U", myStyle);
            if (controllerPanelU != null)
                controllerPanelU.OnDraw(new Vector2(panelRect.width - 45, defaultHeight));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("F", myStyle);
            if (controllerPanelF != null)
                controllerPanelF.OnDraw(new Vector2(panelRect.width - 45, defaultHeight));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("M", myStyle);
            if (controllerPanelM != null)
                controllerPanelM.OnDraw(new Vector2(panelRect.width - 45, defaultHeight));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}
