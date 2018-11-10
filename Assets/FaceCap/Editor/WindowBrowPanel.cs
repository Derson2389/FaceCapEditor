using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

namespace FaceCapEditor
{
    public class WindowBrowPanel : WindowPanel
    {
        public GUIStyle unsetStyle = null;
        public GUIStyle setStyle = null;
        public const int panelSize = 100;
        private BlendControllerPanel leftController;
        private BlendControllerPanel rightController;

        private BlendSlideControllerPanel leftSlider1;
        private BlendSlideControllerPanel leftSlider2;
        private BlendSlideControllerPanel leftSlider3;

        private BlendSlideControllerPanel rightSlider1;
        private BlendSlideControllerPanel rightSlider2;
        private BlendSlideControllerPanel rightSlider3;

        public WindowBrowPanel(EditorWindow window, Rect rect)
        {
            panelRect = rect;
            panelWindow = window; 
           
        }

        public override void OnPanelEnable()
        {
            base.OnPanelEnable();

            Texture2D unsetIcon = (Texture2D)AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/FaceCap/Png/default_icon.png");
            Texture2D setIcon = (Texture2D)AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/FaceCap/Png/white.png");

            unsetStyle = new GUIStyle();
            unsetStyle.normal.background = unsetIcon;
            setStyle = new GUIStyle();
            setStyle.normal.background = setIcon;
            setStyle.alignment = TextAnchor.MiddleCenter;


            BlendGridController controllerLeft = BlenderShapesManager.CreateBlendGridCtrl("r_brow_move_facialControl", panelSize, panelSize, 20, 10);
            if (controllerLeft != null)
            {
                leftController = new BlendControllerPanel(this, new Rect(controllerLeft.windowPosition, controllerLeft.windowSize), controllerLeft);
                leftController.Init();
            }

            BlendGridController controllerRight = BlenderShapesManager.CreateBlendGridCtrl("l_brow_move_facialControl", panelSize, panelSize, 20, 10);
            if (controllerRight != null)
            {
                rightController = new BlendControllerPanel(this, new Rect(controllerRight.windowPosition, controllerRight.windowSize), controllerRight);
                rightController.Init();
            }

            BlendYController controllerLeft1 = BlenderShapesManager.CreateBlendYCtrl("r_out_brow_facialControl");
            if (controllerLeft1 != null)
            {
                leftSlider1 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerLeft1);
                leftSlider1.Init();
            }
            
            BlendYController controllerLeft2 = BlenderShapesManager.CreateBlendYCtrl("r_mid_brow_facialControl");
            if (controllerLeft2 != null)
            {
                leftSlider2 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerLeft2);
                leftSlider2.Init();
            }
            BlendYController controllerLeft3 = BlenderShapesManager.CreateBlendYCtrl("r_in_brow_facialControl");
            if (controllerLeft3 != null)
            {
                leftSlider3 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerLeft3);
                leftSlider3.Init();
            }
            
            BlendYController controllerRight1 = BlenderShapesManager.CreateBlendYCtrl("l_in_brow_facialControl");
            if (controllerRight1 != null)
            {
                rightSlider1 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerRight1);
                rightSlider1.Init();
            }
            BlendYController controllerRight2 = BlenderShapesManager.CreateBlendYCtrl("l_mid_brow_facialControl");
            if (controllerRight2 != null)
            {
                rightSlider2 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerRight2);
                rightSlider2.Init();
            }
            BlendYController controllerRight3 = BlenderShapesManager.CreateBlendYCtrl("l_out_brow_facialControl");
            if (controllerRight3 != null)
            {
                rightSlider3 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerRight3);
                rightSlider3.Init();
            }

        }

        public override void OnPanelDisable()
        {
            base.OnPanelDisable();
            //leftController.Reset();
           // rightController.Reset();
        }
   
        public override void OnPanelSelectionChange()
        {
            base.OnPanelSelectionChange();

            if(leftController!= null)
                leftController.OnUpdate(true);
            if(rightController!= null)
                rightController.OnUpdate(true);

            if(leftSlider1!= null)
                leftSlider1.OnUpdate(false);
            if (leftSlider2 != null)
                leftSlider2.OnUpdate(false);
            if (leftSlider3 != null)
                leftSlider3.OnUpdate(false);

            if (rightSlider1 != null)
                rightSlider1.OnUpdate(false);
            if (rightSlider2 != null)
                rightSlider2.OnUpdate(false);
            if (rightSlider3 != null)
                rightSlider3.OnUpdate(false);
        }

        public override void Update(bool focus)
        {
            if (BlenderShapesManager.controllerList.Count <= 0)
            {
                return;
            }
            if (leftController != null)
                leftController.OnUpdate(true);
            if (rightController != null)
                rightController.OnUpdate(true);
            if (leftSlider1 != null)
                leftSlider1.OnUpdate(false);
            if (leftSlider2 != null)
                leftSlider2.OnUpdate(false);
            if (leftSlider3 != null)
                leftSlider3.OnUpdate(false);

            if (rightSlider1 != null)
                rightSlider1.OnUpdate(false);
            if (rightSlider2 != null)
                rightSlider2.OnUpdate(false);
            if (rightSlider3 != null)
                rightSlider3.OnUpdate(false);
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
            GUILayout.BeginHorizontal();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            var newRect = new Rect(panelRect.x, panelRect.y, panelRect.width, panelRect.height);
            GUILayout.BeginArea(newRect);
            GUILayout.BeginHorizontal();
            {
                if (leftController != null)
                {
                    leftController.OnDraw();
                    EditorGUIUtility.AddCursorRect(leftController.centerPanel, MouseCursor.Link);
                }
            }
            GUILayout.EndArea();
            newRect = new Rect(panelRect.x + 160, panelRect.y, panelRect.width, panelRect.height);
            GUILayout.BeginArea(newRect);         
            {
                GUILayout.BeginHorizontal();
                if(leftSlider1 != null)
                    leftSlider1.OnDraw(new Vector2(30, panelRect.height - 36));
                if (leftSlider2 != null)
                    leftSlider2.OnDraw(new Vector2(30, panelRect.height - 36));
                if (leftSlider3 != null)
                    leftSlider3.OnDraw(new Vector2(30, panelRect.height - 36));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();

            newRect = new Rect(panelRect.width - 230, panelRect.y, panelRect.width, panelRect.height);
            GUILayout.BeginArea(newRect);
            {
                GUILayout.BeginHorizontal();

                if (leftSlider1 != null)
                    rightSlider1.OnDraw(new Vector2(30, panelRect.height - 36));
                if (rightSlider2 != null)
                    rightSlider2.OnDraw(new Vector2(30, panelRect.height - 36));
                if (rightSlider3 != null)
                    rightSlider3.OnDraw(new Vector2(30, panelRect.height - 36));
                GUILayout.EndHorizontal();
                
            }
            GUILayout.EndArea();

            newRect = new Rect(panelRect.width - 140, panelRect.y, panelRect.width, panelRect.height);
            GUILayout.BeginArea(newRect);
            GUILayout.BeginHorizontal();
            {
                if (rightController != null)
                {
                    rightController.OnDraw();
                    EditorGUIUtility.AddCursorRect(rightController.centerPanel, MouseCursor.Link);
                }
            }
            GUILayout.EndArea();

            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}
