#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace Slate{

	[CustomEditor(typeof(BlendShapeCtrlClip))]
	public class BlendShapeCtrlClipInspector : ActionClipInspector<BlendShapeCtrlClip> {

		public override void OnInspectorGUI(){

			base.OnInspectorGUI();

			GUILayout.Space(10);

			if (GUILayout.Button("Load Face Ctrl Config"))
            {
				
			}
		}
	}
}

#endif