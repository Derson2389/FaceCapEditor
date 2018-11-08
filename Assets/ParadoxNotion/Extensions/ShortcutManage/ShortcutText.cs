using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;

namespace Slate
{
    public class ShortcutText
    {
        public const string ClipCut = "Clip/Cut Clip";
        public const string ClipSelectBehind = "Clip/Select Behind";
        public const string ClipDelete = "Clip/Delete Clip";
        public const string ClipSelectRevert = "Clip/Select Revert";
        public const string EditKeyFrame = "Edit/Key Frame";
        public const string EditPreFrame = "Edit/Pre Frame";
        public const string EditNextFrame = "Edit/Next Frame";
        public const string EditPreClip = "Edit/Pre Clip";
        public const string EditNextClip = "Edit/Next Clip";
        public const string EditStopOrPlay = "Edit/Stop Or Play";
        public const string ClipLeftCut = "Clip/Left Cut";
        public const string ClipRightCut = "Clip/Right Cut";
        public const string DopeSheetSelectBehind = "DophSheet/Select Behind";
        public const string EditPause = "Edit/Pause";

        public const string PropertyFramePre = "Edit/Property Pre Frame";
        public const string PropertyFrameNext = "Edit/Property Next Frame";
        public const string PropertKeyFrame = "Edit/Property Key Frame";

        public const string MoveStartTimeToCurrent = "Edit/StartTime Quick Move";
        public const string MoveEndTimeToCurrent = "Edit/EndTime Quick Move";
    }

}
