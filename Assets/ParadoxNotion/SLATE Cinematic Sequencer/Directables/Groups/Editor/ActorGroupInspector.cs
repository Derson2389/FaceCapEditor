#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace Slate{

	[CustomEditor(typeof(ActorGroup))]
	public class ActorGroupInspector : CutsceneGroupInspector {

		private ActorGroup group{
			get {return (ActorGroup)target;}
		}

		public override void OnInspectorGUI(){

			base.OnInspectorGUI();

			GUILayout.BeginVertical("Box");
			group.referenceMode = (CutsceneGroup.ActorReferenceMode)EditorGUILayout.EnumPopup("Reference Mode", group.referenceMode);
			GUILayout.EndVertical();
			GUILayout.BeginVertical("Box");
			group.initialTransformation = (CutsceneGroup.ActorInitialTransformation)EditorGUILayout.EnumPopup("Initial Coordinates", group.initialTransformation);
			if (group.initialTransformation == CutsceneGroup.ActorInitialTransformation.UseLocal){
				group.initialLocalPosition = EditorGUILayout.Vector3Field("Initial Local Position", group.initialLocalPosition);
				group.initialLocalRotation = EditorGUILayout.Vector3Field("Initial Local Rotation", group.initialLocalRotation);
				group.displayVirtualMeshGizmo = EditorGUILayout.ToggleLeft("Always Display Virtual Mesh Gizmo", group.displayVirtualMeshGizmo);
			}
			GUILayout.EndVertical();

            // @modify slate sequencer
            // @rongxia
            GUILayout.BeginVertical("Box");
            if (GUILayout.Button("Auto Set GroupName"))
            {
                group.name = group.actor.name;
            }
            GUILayout.EndVertical();
            // @end
        }
    }
}

#endif