using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

namespace Slate
{
#if UNITY_EDITOR
    public class DGEditorShortcut
    {
        public delegate void DGEditorShortcutActionDelegate();

        public int action;
        public KeyCode key;
        public EventModifiers modifiers;

        public static void Serialize(string prefix, DGEditorShortcut[] shortcuts)
        {
            if (shortcuts.Length == 0) { Debug.LogError("Shortcuts list was empty."); return; }

            string info = shortcuts.Length.ToString() + "_";
            for (int a = 0; a < shortcuts.Length; a++)
            {
                info += (int)shortcuts[a].modifiers + "_" + (int)shortcuts[a].key + "_" + shortcuts[a].action + "_";
            }

            EditorPrefs.SetString(prefix + "_KeyboardShortcuts", info);
        }

        public static DGEditorShortcut[] Deserialize(string prefix, List<Action> actions)
        {
            return Deserialize(prefix, actions, null);
        }

        public static DGEditorShortcut[] Deserialize(string prefix, List<Action> actions, DGEditorShortcut[] defaults)
        {
            if (!EditorPrefs.HasKey(prefix + "_KeyboardShortcuts")) return defaults;

            string[] info = EditorPrefs.GetString(prefix + "_KeyboardShortcuts").Split('_');
            int count = int.Parse(info[0]);

            if (count < 3) return defaults;

            DGEditorShortcut[] shortcuts = new DGEditorShortcut[count];

            int infoCount = 1;
            for (int a = 0; a < count; a++)
            {
                DGEditorShortcut shortcut = new DGEditorShortcut();
                try
                {
                    shortcut.modifiers = (EventModifiers)int.Parse(info[infoCount]);
                    shortcut.key = (KeyCode)int.Parse(info[infoCount + 1]);
                    shortcut.action = int.Parse(info[infoCount + 2]);
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                }

                infoCount += 3;

                shortcuts[a] = shortcut;
            }

            return shortcuts;
        }

        public DGEditorShortcut()
        {
        }

        public DGEditorShortcut(int action, KeyCode key, EventModifiers modifier)
        {
            this.action = action;
            this.key = key;
            this.modifiers = modifier;
        }

        public struct Action
        {
            public string name;
            public DGEditorShortcutActionDelegate action;

            public Action(string name, DGEditorShortcutActionDelegate action)
            {
                this.name = name;
                this.action = action;
            }

            public static implicit operator string(Action action)
            {
                return action.name;
            }
        }
    }
#endif
}