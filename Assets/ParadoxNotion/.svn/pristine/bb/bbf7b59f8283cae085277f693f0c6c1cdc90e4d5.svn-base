using UnityEngine;
using System.Collections;

namespace Slate.ActionClips{

	[Category("Paths")]
	[Description("Animate the actor's position and look at target position on a Path. For example, a 'PositionOnPath' value of 0 means start of path, while a value of 1 means end of path.")]
	public class AnimateOnPath : ActorActionClip {

		[SerializeField] [HideInInspector]
		private float _length = 5f;
		[SerializeField] [HideInInspector]
		private float _blendIn = 0f;

		[Required]
		public Path path;
		[AnimatableParameter(0, 1)]
		public float positionOnPath;
		[AnimatableParameter][PositionHandle][ShowTrajectory]
		public Vector3 lookAtTargetPosition;
		public EaseType blendInterpolation = EaseType.QuadraticInOut;

		private Vector3 lastPos;
		private Quaternion lastRot;

		public override string info{
			get {return string.Format("Animate On Path '{0}'", path != null? path.name : "NONE");}
		}

		public override float length{
			get {return _length;}
			set {_length = value;}
		}

		public override float blendIn{
			get {return _blendIn;}
			set {_blendIn = value;}
		}

		public override bool isValid{
			get {return path != null;}
		}

		public override TransformSpace defaultTransformSpace{
			get {return TransformSpace.CutsceneSpace;}
		}

		protected override void OnEnter(){
			path.Compute();
			lastPos = actor.transform.position;
			lastRot = actor.transform.rotation;
		}

		protected override void OnUpdate(float deltaTime){

			if (length == 0){
				actor.transform.position = path.GetPointAt(positionOnPath);
				return;
			}
			
			var newPos = path.GetPointAt(positionOnPath);
			actor.transform.position = Easing.Ease(blendInterpolation, lastPos, newPos, GetClipWeight(deltaTime));

			var lookPos = TransformPoint(lookAtTargetPosition, defaultTransformSpace);
			var dir = lookPos - actor.transform.position;
			if (dir.magnitude > 0.001f){
				var lookRot = Quaternion.LookRotation(dir);
				actor.transform.rotation = Easing.Ease(blendInterpolation, lastRot, lookRot, GetClipWeight(deltaTime));
			}
		}

		protected override void OnReverse(){
			actor.transform.position = lastPos;
			actor.transform.rotation = lastRot;
		}


		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		protected override void OnDrawGizmosSelected(){
			var pos = TransformPoint(lookAtTargetPosition, defaultTransformSpace);
			Gizmos.color = new Color(1,1,1, GetClipWeight());
			Gizmos.DrawLine(actor.transform.position, pos);
			Gizmos.color = Color.white;
		}

		#endif
	}
}