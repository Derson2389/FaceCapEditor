using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Slate;

public class WindowShortcut : EditorWindow
{

    public  DGEditorShortcut[] keyboardShortcuts;
    private bool shortcutsChanged;
    private Vector2 scrollPosition;

    public static void OpenWindow()
    {
        var window = EditorWindow.GetWindow(typeof(WindowShortcut), false, "快捷键设置") as WindowShortcut;
        window.Show();
    }

     WindowShortcut()
    {

    }

    private void OnEnable()
    {      
       // Get Shortcuts
       keyboardShortcuts = ShortcutManager.GetKeyboardShortcuts("StoryEngine",  keyboardShortcuts);
        if (ShortcutManager.shortcutActions != null)
        {
            keyboardShortcuts = ShortcutManager.GetKeyboardShortcuts("StoryEngine",  keyboardShortcuts);
            if (keyboardShortcuts == null)
            {
                SetDefaultShortcuts();
            }         
        }
    }

    private void OnDisable()
    {
        keyboardShortcuts = null;
        shortcutsChanged = false;
    }

  
    void OnGUI()
    {
        DoRenderConfigPanel();
    }

    private void DoRenderConfigPanel()
    {
        scrollPosition =  GUILayout.BeginScrollView(scrollPosition);
        GUILayout.Box("Keyboard Shortcut Setting", EditorStyles.boldLabel);
        GUILayout.Space(7);

        if (keyboardShortcuts == null)
            return;

        if (shortcutsChanged)
            EditorGUILayout.HelpBox("You have made changes to the keyboard shortcuts. Press Save Shortcuts to avoid losing them.", MessageType.Warning);

        GUILayout.Space(7);

        if (keyboardShortcuts.Length == 0)
            GUILayout.Box("No Keyboard Shortcuts!", EditorStyles.centeredGreyMiniLabel);

        for (int i = 0; i < keyboardShortcuts.Length; i++)
        {
            Rect lineRect = EditorGUILayout.BeginHorizontal(GUILayout.Height(25));
            if (i % 2 == 0)
            {
                GUI.Box(lineRect, "", (GUIStyle)"hostview");
            }
            GUILayout.Space(10);
            GUILayout.Label("Action");
            EditorGUI.BeginChangeCheck();
            keyboardShortcuts[i].action = EditorGUILayout.Popup(keyboardShortcuts[i].action, Array.ConvertAll(ShortcutManager.shortcutActions.ToArray(), item => (string)item));
            //GUILayout.FlexibleSpace();
            GUILayout.Label("Shortcut");
            //todo: Clean this up, it feels hacky.
            keyboardShortcuts[i].modifiers = (EventModifiers)((int)(EventModifiers)EditorGUILayout.EnumMaskField((EventModifiers)((int)keyboardShortcuts[i].modifiers << 1)) >> 1);
            GUILayout.Space(5);
            keyboardShortcuts[i].key = (KeyCode)EditorGUILayout.EnumPopup(keyboardShortcuts[i].key);
            if (EditorGUI.EndChangeCheck())
            {
                shortcutsChanged = true;
            }
            GUILayout.Space(5);
            if (GUILayout.Button("X"))
            {
                DGEditorShortcut[] newArray = new DGEditorShortcut[keyboardShortcuts.Length - 1];
                int b = 0;
                for (int a = 0; a < keyboardShortcuts.Length; a++)
                {
                    if (a != i)
                    {
                        newArray[b] = keyboardShortcuts[a];
                        b++;
                    }
                }
                shortcutsChanged = true;
                keyboardShortcuts = newArray;
                break;
            }
            GUILayout.Space(10);
            EditorGUILayout.EndHorizontal();
        }
        GUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();
        //GUILayout.Space(53);
        if (GUILayout.Button("Add Shortcut"))
        {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < ShortcutManager.shortcutActions.Count; i++)
            {
                menu.AddItem(new GUIContent(ShortcutManager.shortcutActions[i].name), false, (object choice) =>
                {
                    DGEditorShortcut[] newArray = new DGEditorShortcut[keyboardShortcuts.Length + 1];
                    for (int a = 0; a < keyboardShortcuts.Length; a++)
                    {
                        newArray[a] = keyboardShortcuts[a];
                    }
                    newArray[newArray.Length - 1] = new DGEditorShortcut((int)choice, KeyCode.None, EventModifiers.None);
                    shortcutsChanged = true;
                    keyboardShortcuts = newArray;
                }, i);
            }
            menu.ShowAsContext();
        }

        if (GUILayout.Button("Save Shortcuts"))
        {
            shortcutsChanged = false;
            ShortcutManager.SetKeyboardShortcuts("StoryEngine", keyboardShortcuts);
        }

        if (GUILayout.Button("Revert to Saved"))
        {
            shortcutsChanged = false;
            SetDefaultShortcuts();
           // keyboardShortcuts = ShortcutManager.GetKeyboardShortcuts("StoryEngine", keyboardShortcuts);
            ShortcutManager.SetKeyboardShortcuts("StoryEngine", keyboardShortcuts);
        }
        GUILayout.Space(10);
        EditorGUILayout.EndHorizontal();
        if (shortcutsChanged) EditorGUILayout.HelpBox("You have made changes to the keyboard shortcuts. Press Save Shortcuts to avoid losing them.", MessageType.Warning);
        GUILayout.EndScrollView();
    }

    private void SetDefaultShortcuts()
    {
        keyboardShortcuts = new DGEditorShortcut[] {
               new DGEditorShortcut(1, KeyCode.A ,EventModifiers.Control),
                new DGEditorShortcut(2, KeyCode.Delete, EventModifiers.FunctionKey),
                new DGEditorShortcut(3, KeyCode.R, EventModifiers.Control),
                new DGEditorShortcut(4, KeyCode.S, EventModifiers.None),
                new DGEditorShortcut(5, KeyCode.Comma, EventModifiers.None),
                new DGEditorShortcut(6, KeyCode.Period, EventModifiers.None),
                new DGEditorShortcut(7, KeyCode.LeftArrow, EventModifiers.FunctionKey),
                new DGEditorShortcut(8, KeyCode.RightArrow, EventModifiers.FunctionKey),
                new DGEditorShortcut(9, KeyCode.Space, EventModifiers.None),
                new DGEditorShortcut(10, KeyCode.LeftBracket, EventModifiers.None),
                new DGEditorShortcut(11, KeyCode.RightBracket, EventModifiers.None),
                new DGEditorShortcut(12, KeyCode.A, EventModifiers.Control),
                new DGEditorShortcut(13, KeyCode.P, EventModifiers.None),
                new DGEditorShortcut(14, KeyCode.F1, EventModifiers.FunctionKey),
                new DGEditorShortcut(15, KeyCode.F2, EventModifiers.FunctionKey),
                new DGEditorShortcut(16, KeyCode.F3, EventModifiers.FunctionKey)
            };
    }

}
