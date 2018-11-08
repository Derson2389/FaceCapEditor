using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class FBXScreenEventProcessor : IScreenEventProcessor
{
    public override bool ProcessEvent()
    {
        EventType eventType = Event.current.type;
        if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
        {
            if(DragAndDrop.paths.Length == 1 && Path.GetExtension(DragAndDrop.paths[0]).ToLower() == ".fbx")
            {
                if (eventType == EventType.DragUpdated)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    Event.current.Use();
                }
                else if (eventType == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    Event.current.Use();
                    return true;
                }
            }
        }

        return false;
    }
}
