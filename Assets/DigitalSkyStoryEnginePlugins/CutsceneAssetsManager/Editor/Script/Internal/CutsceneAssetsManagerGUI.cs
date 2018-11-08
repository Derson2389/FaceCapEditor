﻿using UnityEngine;
using UnityEditor;

public class CutsceneAssetsManagerGUI
{
    public static float LISTVIEW_TITLE_HEIGHT = 24;

    public static bool FlatButton(Rect rect, string text, Color color)
    {
        bool clicked = false;

        GUI.backgroundColor = color;
        clicked = GUI.Button(rect, text, CutsceneAssetsManagerStyles.flatButton);
        GUI.backgroundColor = Color.white;

        return clicked;
    }

    public enum MouseButton
    {
        None,
        Left,
        Right
    }

    public static void ListviewItem(Rect rect, Texture image, string text, Color backgroundColor, Color textColor, System.Action<CutsceneAssetsManagerUtility.PrefabListviewItem>  callbackAdd = null, System.Action<CutsceneAssetsManagerUtility.PrefabListviewItem> callbackReduce = null, int idx = 0, bool isAddItem = false, System.Action addfunc = null, CutsceneAssetsManagerUtility.PrefabListviewItem prefab = null)
    {
        GUI.color = backgroundColor;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = Color.white;

        if (isAddItem == false)
        {
            Rect imageRect = new Rect(rect.x + 6, rect.y + 6, rect.width - 12, rect.height - 32);
            GUI.DrawTexture(imageRect, image, ScaleMode.ScaleToFit);

            //Rect numberRect = new Rect(rect.x + 6, rect.y + 6, rect.width - 12, rect.height - 32);
            if (callbackAdd != null && callbackReduce != null && prefab!= null)
            {
                GUIStyle bs = new GUIStyle();
                bs.normal.background = Texture2D.blackTexture;     //这是设置背景填充的
                bs.normal.textColor = new Color(46f / 255f, 139f / 255f, 87f / 255f);
                bs.fontSize = 20;       //当然，这是字体颜色 
                Rect addRect = new Rect(rect.x + rect.width - 20, rect.y + 20, 20, 20);
                EditorGUIUtility.AddCursorRect(addRect, MouseCursor.Link);
                if (GUI.Button(addRect, "+", bs))
                {
                    if (callbackAdd != null)
                    {
                        callbackAdd(prefab);
                    }
                }
                Rect reduceRect = new Rect(rect.x + rect.width - 17, rect.y + 40, 20, 20);
                EditorGUIUtility.AddCursorRect(reduceRect, MouseCursor.Link);
                if (GUI.Button(reduceRect, "-", bs))
                {
                    if (callbackReduce != null)
                    {
                        callbackReduce(prefab);
                    }
                }
                GUI.color = new Color(46f / 255f, 139f / 255f, 87f / 255f);
                GUI.Label(new Rect(rect.x + rect.width - 24, rect.y, 20, 20), prefab.number.ToString(), CutsceneAssetsManagerStyles.itemText);
                GUI.color = Color.white;
            }
            
            GUI.color = textColor;
            GUI.Label(new Rect(rect.x, rect.y + rect.height - 24, rect.width, 20), text, CutsceneAssetsManagerStyles.itemText);
            GUI.color = Color.white;

            //if (callbackPre != null)
            //{
            //    GUI.color = new Color(0, 1f, 1f);
            //    if (GUI.Button(new Rect(rect.x, rect.y, 20, 20), "-"))
            //    {
            //        callbackPre(idx);
            //    }
            //    GUI.color = Color.white;
            //}

            //if (callbackNext != null)
            //{
            //    GUI.color = new Color(0, 1f, 1f);
            //    if (GUI.Button(new Rect(rect.x + rect.width - 20, rect.y, 20, 20), "+"))
            //    {
            //        callbackNext(idx);
            //    }
            //    GUI.color = Color.white;
            //}
        }
        else
        {
            GUIStyle bs = new GUIStyle();
            bs.normal.background = Texture2D.blackTexture;     //这是设置背景填充的
            bs.normal.textColor = Color.green;
            bs.fontSize = 50;       //当然，这是字体颜色 
            Rect addRect = new Rect(rect.x + 66, rect.y + 35, 60, 60);
            EditorGUIUtility.AddCursorRect(addRect, MouseCursor.Link);
            GUIStyle bs1 = new GUIStyle();
            bs1.normal.background = Texture2D.blackTexture;     //这是设置背景填充的
            bs1.normal.textColor = Color.green;
            bs1.fontSize = 20;       //当然，这是字体颜色
            Rect addtxtRect = new Rect(addRect.x, addRect.y + 60, addRect.width, addRect.height);
            GUI.Label(addtxtRect, "新建", bs1);
            if (GUI.Button(addRect, "+", bs))
            {
                if (addfunc != null)
                    addfunc();
            }
        }

    }

    public static void CutsceneGroupItem(Rect rect, Texture2D image, string text, string desc, Color backgroundColor, Color textColor, int ClipsCount = 0)
    {
        GUI.color = backgroundColor;
        Rect whiteRect = new Rect(rect.x, rect.y, rect.width, rect.height);
        //GUI.DrawTexture(whiteRect, Texture2D.whiteTexture);
        GUI.color = Color.white;

        Rect imageRect = new Rect(rect.x , rect.y , rect.width, rect.height );
        GUI.DrawTexture(imageRect, image, ScaleMode.ScaleToFit, false);

        GUI.color = Color.white;
        Rect textRect = new Rect(rect.x + rect.width - 17, rect.y, rect.width, rect.height);
        GUI.Label(textRect, ClipsCount.ToString());
        GUI.color = Color.white;

        if (ClipsCount==0)
        {
            GUI.color = Color.red;
            Rect textRect2 = new Rect((rect.x + rect.width) / 2 - 25, rect.y + 26, rect.width, rect.height);
            GUI.Label(textRect2, "暂无分镜");
            GUI.color = Color.white;
        }


    }

    public static void CutsceneGroupCtrPanel(Rect rect, string desc, Color backgroundColor, Color textColor, int cutNumber=0, string mCurrentProjectName = null, string actName = null)
    {
        GUI.color = backgroundColor;
        GUI.Box(rect, "");
        GUI.color = Color.white;

        GUILayout.BeginArea(rect);
        {
            GUILayout.FlexibleSpace();
            //GUILayout.BeginHorizontal();

            //if (GUILayout.Button("详细", GUILayout.Height(30)))
            //{
            //    if (mCurrentProjectName != null)
            //    {
            //        CutsceneBrowerView2.ShowWindow(mCurrentProjectName, actName, null, desc);
            //    }          
            //}
          
            GUI.enabled = cutNumber != 0;
            if (GUILayout.Button("动捕室上传", GUILayout.Height(20)))
            {
                AssetDatabase.Refresh();
                SvnForUnity.SvnPreUpdate(mCurrentProjectName, actName);
                SvnForUnity.SvnAddFolderAndFile(mCurrentProjectName, actName);
                SvnForUnity.SvnCommitFolderAndFile(mCurrentProjectName, actName);
            }

            if (GUILayout.Button("导演上传", GUILayout.Height(20)))
            {
                AssetDatabase.Refresh();
                SvnForUnity.SvnPreUpdate(mCurrentProjectName, actName, false);
                SvnForUnity.SvnAddFolderAndFile(mCurrentProjectName, actName);
                SvnForUnity.SvnCommitFolderAndFile(mCurrentProjectName, actName, false);
            }

            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            //GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
    }

    private static GUIStyle TextFieldRoundEdge;
    private static GUIStyle TextFieldRoundEdgeCancelButton;
    private static GUIStyle TextFieldRoundEdgeCancelButtonEmpty;
    private static GUIStyle TransparentTextField;
    /// <summary>
    /// 绘制输入框，放在OnGUI函数里
    /// </summary>
    public static void DrawInputTextField(ref string searchString)
    {
        if (TextFieldRoundEdge == null)
        {
            TextFieldRoundEdge = new GUIStyle("SearchTextField");
            TextFieldRoundEdgeCancelButton = new GUIStyle("SearchCancelButton");
            TextFieldRoundEdgeCancelButtonEmpty = new GUIStyle("SearchCancelButtonEmpty");
            TransparentTextField = new GUIStyle(EditorStyles.whiteLabel);
            TransparentTextField.normal.textColor = EditorStyles.textField.normal.textColor;
        }

        //获取当前输入框的Rect(位置大小)
        Rect position = EditorGUILayout.GetControlRect();
        //设置圆角style的GUIStyle
        GUIStyle textFieldRoundEdge = TextFieldRoundEdge;
        //设置输入框的GUIStyle为透明，所以看到的“输入框”是TextFieldRoundEdge的风格
        GUIStyle transparentTextField = TransparentTextField;
        //选择取消按钮(x)的GUIStyle
        GUIStyle gUIStyle = (searchString != "") ? TextFieldRoundEdgeCancelButton : TextFieldRoundEdgeCancelButtonEmpty;

        //输入框的水平位置向左移动取消按钮宽度的距离
        position.width -= gUIStyle.fixedWidth;

        //如果面板重绘
        if (Event.current.type == EventType.Repaint)
        {
            //根据是否是专业版来选取颜色
            GUI.contentColor = (EditorGUIUtility.isProSkin ? Color.grey : new Color(0f, 0f, 0f, 0.5f));
            //当没有输入的时候提示“请输入”
            if (string.IsNullOrEmpty(searchString))
            {
                textFieldRoundEdge.Draw(position, new GUIContent("请输入"), 0);
            }
            else
            {
                textFieldRoundEdge.Draw(position, new GUIContent(""), 0);
            }
            //因为是“全局变量”，用完要重置回来
            GUI.contentColor = Color.white;
        }
        Rect rect = position;
        //为了空出左边那个放大镜的位置
        float num = textFieldRoundEdge.CalcSize(new GUIContent("")).x - 2f;
        rect.width -= num;
        rect.x += num;
        rect.y += 1f;//为了和后面的style对其

        searchString = EditorGUI.TextField(rect, searchString, transparentTextField);
        //绘制取消按钮，位置要在输入框右边
        position.x += position.width;
        position.width = gUIStyle.fixedWidth;
        position.height = gUIStyle.fixedHeight;
        if (GUI.Button(position, GUIContent.none, gUIStyle) && searchString != "")
        {
            searchString = "";
            //用户是否做了输入
            GUI.changed = true;
            //把焦点移开输入框
            GUIUtility.keyboardControl = 0;
        }
    }


    public static void CutsceneGroupDesc(Rect rect, string text, string desc, Color backgroundColor, Color textColor)
    {
        GUI.color = backgroundColor;
        GUI.Box(rect, "");
        GUI.color = Color.white;

        GUI.color = textColor;
        Rect textRect = new Rect(rect.x + 4, rect.y + 4, rect.width - 8, rect.height - 8);
        GUI.enabled = false;
        GUI.TextArea(textRect, desc);
        GUI.enabled = true;
        GUI.color = Color.white;
    }


    public static string SearchField(Rect rect, string text)
    {
        text = GUI.TextField(rect, text, "ToolbarSeachTextField");

        Rect cancelRect = new Rect(rect.x + rect.width, rect.y, 20, rect.height);
        if (GUI.Button(cancelRect, "", "ToolbarSeachCancelButton"))
        {
            text = string.Empty;
        }
        return text;
    }
}
