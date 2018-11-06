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

            BlendGridController controllerLeft = new BlendGridController();
            controllerLeft.windowPosition = new Vector2(20, 10);
            controllerLeft.windowSize = new Vector2(panelSize, panelSize);

            BlenderShapesManager.BlenderShapeCtrl crl = BlenderShapesManager.getControllerByName("l_brow_move_facialControl");

            if (crl != null)
            {
                controllerLeft.top = crl.ctrlBlendShapes[(int)BlenderShapesManager.BlenderShapeCtrl.blendIdx.top].blendableIndex;
                controllerLeft.bottom = crl.ctrlBlendShapes[(int)BlenderShapesManager.BlenderShapeCtrl.blendIdx.down].blendableIndex; ;
                controllerLeft.left = crl.ctrlBlendShapes[(int)BlenderShapesManager.BlenderShapeCtrl.blendIdx.left].blendableIndex; ;
                controllerLeft.right = crl.ctrlBlendShapes[(int)BlenderShapesManager.BlenderShapeCtrl.blendIdx.right].blendableIndex; ;

                controllerLeft.controllerName = "l_brow_move_facialControl";

            }
            leftController = new BlendControllerPanel(this, new Rect(controllerLeft.windowPosition, controllerLeft.windowSize), controllerLeft);
            leftController.Init();

            BlendGridController controllerRight = new BlendGridController();
            controllerRight.windowPosition = new Vector2(20, 10);
            controllerRight.windowSize = new Vector2(panelSize, panelSize);
            rightController = new BlendControllerPanel(this, new Rect(controllerRight.windowPosition, controllerRight.windowSize), controllerRight);
            rightController.Init();

            BlendYController controllerLeft1 = new BlendYController();
            controllerLeft1.top = 0;
            controllerLeft1.bottom = 1;
            controllerLeft1.controllerIndex = 0;
            controllerLeft1.controllerName = "browControllerLeft1";
            leftSlider1 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerLeft1);
            leftSlider1.Init();
            BlendYController controllerLeft2 = new BlendYController();
            leftSlider2 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerLeft2);
            leftSlider2.Init();
            BlendYController controllerLeft3 = new BlendYController();
            leftSlider3 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerLeft3);
            leftSlider3.Init();

            BlendYController controllerRight1 = new BlendYController();
            rightSlider1 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerRight1);
            rightSlider1.Init();
            BlendYController controllerRight2 = new BlendYController();
            rightSlider2 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerRight2);
            rightSlider2.Init();
            BlendYController controllerRight3 = new BlendYController();
            rightSlider3 = new BlendSlideControllerPanel(this, Rect.zero, null, controllerRight3);
            rightSlider3.Init();

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

            leftController.OnUpdate(true);
            rightController.OnUpdate(true);

            leftSlider1.OnUpdate(false);
            leftSlider2.OnUpdate(false);
            leftSlider3.OnUpdate(false);

            rightSlider1.OnUpdate(false);
            rightSlider2.OnUpdate(false);
            rightSlider3.OnUpdate(false);
        }

        public void Update()
        {
            if (BlenderShapesManager.controllerList.Count <= 0)
            {
                return;
            }
            leftController.OnUpdate(true);
            rightController.OnUpdate(true);

            leftSlider1.OnUpdate(false);
            leftSlider2.OnUpdate(false);
            leftSlider3.OnUpdate(false);

            rightSlider1.OnUpdate(false);
            rightSlider2.OnUpdate(false);
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
                leftController.OnDraw();
                EditorGUIUtility.AddCursorRect(leftController.centerPanel, MouseCursor.Link);                
            }
            GUILayout.EndArea();
            newRect = new Rect(panelRect.x + 160, panelRect.y, panelRect.width, panelRect.height);
            GUILayout.BeginArea(newRect);         
            {
                GUILayout.BeginHorizontal();
                leftSlider1.OnDraw(new Vector2(30, panelRect.height - 36));
                leftSlider2.OnDraw(new Vector2(30, panelRect.height - 36));
                leftSlider3.OnDraw(new Vector2(30, panelRect.height - 36));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();

            newRect = new Rect(panelRect.width - 230, panelRect.y, panelRect.width, panelRect.height);
            GUILayout.BeginArea(newRect);
            {
                GUILayout.BeginHorizontal();
                
                rightSlider1.OnDraw(new Vector2(30, panelRect.height - 36));
                rightSlider2.OnDraw(new Vector2(30, panelRect.height - 36));
                rightSlider3.OnDraw(new Vector2(30, panelRect.height - 36));
                GUILayout.EndHorizontal();
                
            }
            GUILayout.EndArea();

            newRect = new Rect(panelRect.width - 140, panelRect.y, panelRect.width, panelRect.height);
            GUILayout.BeginArea(newRect);
            GUILayout.BeginHorizontal();
            {
                rightController.OnDraw();
                EditorGUIUtility.AddCursorRect(rightController.centerPanel, MouseCursor.Link);
            }
            GUILayout.EndArea();

            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}
