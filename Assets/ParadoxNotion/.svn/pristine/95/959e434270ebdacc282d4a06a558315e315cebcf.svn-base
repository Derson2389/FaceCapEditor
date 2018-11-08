using UnityEngine;
using System.Collections;

namespace Slate.ActionClips{

	[Category("Transform")]
	[Description("Instantely set the transforms of the actor.")]
	public class SetTransformValues : ActorActionClip {

		public bool setPosition = true;
		public Vector3 position;
		public MiniTransformSpace space;
		public bool setRotation;
		public Vector3 rotation;
		public bool setScale;
		public Vector3 scale = Vector3.one;

		private TransformSnapshot undo;

		protected override void OnEnter(){
			undo = new TransformSnapshot(actor, TransformSnapshot.StoreMode.RootOnly);

			if (setPosition){
				actor.transform.position = TransformPoint(position, (TransformSpace)space);
			}

			if (setRotation){
				actor.transform.localEulerAngles = rotation;
			}

			if (setScale){
				actor.transform.localScale = scale;
			}
		}

		protected override void OnReverse(){
			undo.Restore();
		}


		#if UNITY_EDITOR
		protected override void OnSceneGUI(){
			if (setPosition){
				DoVectorPositionHandle( (TransformSpace)space, ref position);
			}
		}		
		#endif

	}
}