using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Slate
{
#if UNITY_EDITOR
    public class ShortcutManager
    {
        /// <summary>
        /// the Shortcut Manager
        /// </summary>
        ///
        static ShortcutManager _instance = null;
        #region param
        public static List<DGEditorShortcut.Action> shortcutActions;
        private static DGEditorShortcut[] DefaultKeyboardShortcuts;
        #endregion
        public static ShortcutManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ShortcutManager();
                }
                return _instance;
            }
        }


        public void Init(List<DGEditorShortcut.Action> _shortcutActions)
        {
            shortcutActions = _shortcutActions;
        }


        public static DGEditorShortcut[] GetKeyboardShortcuts(string prefix, DGEditorShortcut[] defaults)
        {
            return DGEditorShortcut.Deserialize(prefix, shortcutActions, defaults == null ? DefaultKeyboardShortcuts : defaults);
        }

        public static void SetKeyboardShortcuts(string prefix, DGEditorShortcut[] defaults)
        {
            DGEditorShortcut.Serialize(prefix, defaults);
        }

        public static void SetDefaultKeyboardShortcuts(DGEditorShortcut[] _DefaultKeyboardShortcuts)
        {
            DefaultKeyboardShortcuts = _DefaultKeyboardShortcuts;
        }
        public void OnDestroy()
        {
            _instance = null;
        }

        void OnNewClick()
        {

        }

    }
#endif
}


